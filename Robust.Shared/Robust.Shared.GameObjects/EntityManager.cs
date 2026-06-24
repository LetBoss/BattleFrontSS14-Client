using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Prometheus;
using Robust.Shared.Analyzers;
using Robust.Shared.Collections;
using Robust.Shared.Console;
using Robust.Shared.Containers;
using Robust.Shared.Exceptions;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Profiling;
using Robust.Shared.Prototypes;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Robust.Shared.GameObjects;

[Virtual]
public abstract class EntityManager : IEntityManager
{
	public readonly struct CompInitializeHandle<T>(IEntityManager entityManager, EntityUid owner, T comp, CompIdx compType) : IDisposable where T : IComponent
	{
		private readonly IEntityManager _entMan = entityManager;

		private readonly EntityUid _owner = owner;

		public readonly CompIdx CompType = compType;

		public readonly T Comp = comp;

		public void Dispose()
		{
			MetaDataComponent component = _entMan.GetComponent<MetaDataComponent>(_owner);
			if (component.EntityInitialized || component.EntityInitializing)
			{
				if (!Comp.Initialized)
				{
					((EntityManager)_entMan).LifeInitialize(_owner, Comp, CompType);
				}
				if (component.EntityInitialized && !Comp.Running)
				{
					((EntityManager)_entMan).LifeStartup(_owner, Comp, CompType);
				}
			}
		}

		public static implicit operator T(CompInitializeHandle<T> handle)
		{
			return handle.Comp;
		}
	}

	public delegate void TerminatingEventHandler(ref EntityTerminatingEvent ev);

	[Robust.Shared.IoC.Dependency]
	private readonly IComponentFactory _componentFactory;

	[Robust.Shared.IoC.Dependency]
	private readonly IRuntimeLog _runtimeLog;

	private const int TypeCapacity = 32;

	private const int ComponentCollectionCapacity = 1024;

	private const int EntityCapacity = 1024;

	private const int NetComponentCapacity = 8;

	private FrozenDictionary<Type, Dictionary<EntityUid, IComponent>> _entTraitDict = FrozenDictionary<Type, Dictionary<EntityUid, IComponent>>.Empty;

	private Dictionary<EntityUid, IComponent>[] _entTraitArray = Array.Empty<Dictionary<EntityUid, IComponent>>();

	private readonly HashSet<IComponent> _deleteSet = new HashSet<IComponent>(32);

	private UniqueIndexHkm<EntityUid, IComponent> _entCompIndex = new UniqueIndexHkm<EntityUid, IComponent>(1024);

	[Robust.Shared.IoC.Dependency]
	protected readonly IPrototypeManager PrototypeManager;

	[Robust.Shared.IoC.Dependency]
	protected readonly ILogManager LogManager;

	[Robust.Shared.IoC.Dependency]
	private readonly IEntitySystemManager _entitySystemManager;

	[Robust.Shared.IoC.Dependency]
	private readonly IMapManager _mapManager;

	[Robust.Shared.IoC.Dependency]
	private readonly IGameTiming _gameTiming;

	[Robust.Shared.IoC.Dependency]
	private readonly ISerializationManager _serManager;

	[Robust.Shared.IoC.Dependency]
	private readonly ProfManager _prof;

	[Robust.Shared.IoC.Dependency]
	private readonly INetManager _netMan;

	[Robust.Shared.IoC.Dependency]
	private readonly IReflectionManager _reflection;

	[Robust.Shared.IoC.Dependency]
	private readonly EntityConsoleHost _entityConsoleHost;

	protected SharedTransformSystem _xforms;

	private SharedContainerSystem _containers;

	public EntityQuery<MetaDataComponent> MetaQuery;

	public EntityQuery<TransformComponent> TransformQuery;

	private EntityQuery<PhysicsComponent> _physicsQuery;

	private EntityQuery<ActorComponent> _actorQuery;

	public static readonly MapInitEvent MapInitEventInstance = new MapInitEvent();

	protected readonly Queue<EntityUid> QueuedDeletions = new Queue<EntityUid>();

	protected readonly HashSet<EntityUid> QueuedDeletionsSet = new HashSet<EntityUid>();

	private EntityDiffContext _context = new EntityDiffContext();

	protected readonly HashSet<EntityUid> Entities = new HashSet<EntityUid>();

	internal EntityEventBus EventBusInternal;

	protected int NextEntityUid = (int)EntityUid.FirstUid;

	protected int NextNetworkId = (int)NetEntity.First;

	private string _xformName = string.Empty;

	private ComponentRegistration _metaReg;

	private ComponentRegistration _xformReg;

	private SharedMapSystem _mapSystem;

	private ISawmill _sawmill;

	internal ISawmill ResolveSawmill;

	private static readonly ComponentAdd CompAddInstance = default(ComponentAdd);

	private static readonly ComponentInit CompInitInstance = new ComponentInit();

	private static readonly ComponentStartup CompStartupInstance = new ComponentStartup();

	private static readonly ComponentShutdown CompShutdownInstance = new ComponentShutdown();

	private static readonly ComponentRemove CompRemoveInstance = new ComponentRemove();

	protected readonly Dictionary<NetEntity, (EntityUid, MetaDataComponent)> NetEntityLookup = new Dictionary<NetEntity, (EntityUid, MetaDataComponent)>(1024);

	public IComponentFactory ComponentFactory => _componentFactory;

	public GameTick CurrentTick => _gameTiming.CurTick;

	IComponentFactory IEntityManager.ComponentFactory => ComponentFactory;

	public IEntitySystemManager EntitySysManager => _entitySystemManager;

	public abstract IEntityNetworkManager EntityNetManager { get; }

	public IEventBus EventBus => EventBusInternal;

	public bool Started { get; protected set; }

	public bool ShuttingDown { get; protected set; }

	public bool Initialized { get; protected set; }

	public int EntityCount => Entities.Count;

	public event Action<AddedComponentEventArgs>? ComponentAdded;

	public event Action<RemovedComponentEventArgs>? ComponentRemoved;

	public event Action<Entity<MetaDataComponent>>? EntityAdded;

	public event Action<Entity<MetaDataComponent>>? EntityInitialized;

	public event Action<Entity<MetaDataComponent>>? EntityDeleted;

	internal event TerminatingEventHandler? BeforeEntityTerminating;

	public event Action? BeforeEntityFlush;

	public event Action? AfterEntityFlush;

	public event Action<EntityUid>? EntityQueueDeleted;

	public event Action<Entity<MetaDataComponent>>? EntityDirtied;

	public uint GetModifiedFields(IComponentDelta delta, GameTick fromTick)
	{
		uint num = 0u;
		for (int i = 0; i < delta.LastModifiedFields.Length; i++)
		{
			if (!(delta.LastModifiedFields[i] < fromTick))
			{
				num |= (uint)(1 << i);
			}
		}
		return num;
	}

	public void DirtyField(EntityUid uid, IComponentDelta comp, string fieldName, MetaDataComponent? metadata = null)
	{
		if (!ComponentFactory.GetRegistration(comp).NetworkedFieldLookup.TryGetValue(fieldName, out var value))
		{
			_sawmill.Error($"Tried to dirty delta field {fieldName} on {ToPrettyString(uid)} that isn't implemented.");
		}
		else
		{
			GameTick gameTick = (comp.LastFieldUpdate = _gameTiming.CurTick);
			comp.LastModifiedFields[value] = gameTick;
			Dirty(uid, comp, metadata);
		}
	}

	public virtual void DirtyField<T>(EntityUid uid, T comp, [ValidateMember] string fieldName, MetaDataComponent? metadata = null) where T : IComponentDelta
	{
		if (!ComponentFactory.GetRegistration(CompIdx.Index<T>()).NetworkedFieldLookup.TryGetValue(fieldName, out var value))
		{
			_sawmill.Error($"Tried to dirty delta field {fieldName} on {ToPrettyString(uid)} that isn't implemented.");
		}
		else
		{
			GameTick curTick = _gameTiming.CurTick;
			comp.LastFieldUpdate = curTick;
			comp.LastModifiedFields[value] = curTick;
			Dirty(uid, comp, metadata);
		}
	}

	public virtual void DirtyFields<T>(EntityUid uid, T comp, MetaDataComponent? meta, params string[] fields) where T : IComponentDelta
	{
		ComponentRegistration registration = ComponentFactory.GetRegistration(CompIdx.Index<T>());
		GameTick curTick = _gameTiming.CurTick;
		foreach (string text in fields)
		{
			if (!registration.NetworkedFieldLookup.TryGetValue(text, out var value))
			{
				_sawmill.Error($"Tried to dirty delta field {text} on {ToPrettyString(uid)} that isn't implemented.");
			}
			else
			{
				comp.LastModifiedFields[value] = curTick;
			}
		}
		comp.LastFieldUpdate = curTick;
		Dirty(uid, comp, meta);
	}

	public void InitializeComponents()
	{
		if (Initialized)
		{
			throw new InvalidOperationException("Already initialized.");
		}
		FillComponentDict();
		_componentFactory.ComponentsAdded += OnComponentsAdded;
	}

	public void ClearComponents()
	{
		_entCompIndex.Clear();
		_deleteSet.Clear();
		ImmutableArray<Dictionary<EntityUid, IComponent>>.Enumerator enumerator = _entTraitDict.Values.GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Clear();
		}
	}

	private void RegisterComponents(IEnumerable<ComponentRegistration> components)
	{
		Dictionary<Type, Dictionary<EntityUid, IComponent>> dictionary = _entTraitDict.ToDictionary();
		foreach (ComponentRegistration component in components)
		{
			Dictionary<EntityUid, IComponent> value = new Dictionary<EntityUid, IComponent>();
			dictionary.Add(component.Type, value);
			CompIdx.AssignArray(ref _entTraitArray, component.Idx, value);
		}
		_entTraitDict = dictionary.ToFrozenDictionary();
	}

	private void OnComponentsAdded(ComponentRegistration[] components)
	{
		RegisterComponents(components);
	}

	public int Count<T>() where T : IComponent
	{
		return _entTraitDict[typeof(T)].Count;
	}

	public int Count(Type component)
	{
		return _entTraitDict[component].Count;
	}

	[Obsolete("Use InitializeEntity")]
	public void InitializeComponents(EntityUid uid, MetaDataComponent? metadata = null)
	{
		if (metadata == null)
		{
			metadata = MetaQuery.GetComponent(uid);
		}
		SetLifeStage(metadata, EntityLifeStage.Initializing);
		Span<IComponent> comps = default(FixedArray32<IComponent>).AsSpan;
		CopyComponentsInto(ref comps, uid);
		Span<IComponent> span = comps;
		for (int i = 0; i < span.Length; i++)
		{
			IComponent component = span[i];
			if (component != null && component.LifeStage == ComponentLifeStage.Added)
			{
				LifeInitialize(uid, component, _componentFactory.GetIndex(component.GetType()));
			}
		}
		SetLifeStage(metadata, EntityLifeStage.Initialized);
	}

	[Obsolete("Use StartEntity")]
	public void StartComponents(EntityUid uid)
	{
		Span<IComponent> comps = default(FixedArray32<IComponent>).AsSpan;
		CopyComponentsInto(ref comps, uid);
		TransformComponent component = TransformQuery.GetComponent(uid);
		if (component.LifeStage == ComponentLifeStage.Initialized)
		{
			LifeStartup(uid, component, CompIdx.Index<TransformComponent>());
		}
		if (_physicsQuery.TryComp(uid, out PhysicsComponent component2) && component2.LifeStage == ComponentLifeStage.Initialized)
		{
			LifeStartup(uid, component2, CompIdx.Index<PhysicsComponent>());
		}
		Span<IComponent> span = comps;
		for (int i = 0; i < span.Length; i++)
		{
			IComponent component3 = span[i];
			if (component3 != null && component3.LifeStage == ComponentLifeStage.Initialized)
			{
				LifeStartup(uid, component3, _componentFactory.GetIndex(component3.GetType()));
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddComponents(EntityUid target, EntityPrototype prototype, bool removeExisting = true)
	{
		AddComponents(target, prototype.Components, removeExisting);
	}

	public void AddComponents(EntityUid target, ComponentRegistry registry, bool removeExisting = true)
	{
		if (registry.Count == 0)
		{
			return;
		}
		MetaDataComponent component = MetaQuery.GetComponent(target);
		foreach (KeyValuePair<string, EntityPrototype.ComponentRegistryEntry> item in registry)
		{
			item.Deconstruct(out var key, out var value);
			string componentName = key;
			EntityPrototype.ComponentRegistryEntry componentRegistryEntry = value;
			ComponentRegistration registration = _componentFactory.GetRegistration(componentName);
			if (removeExisting)
			{
				IComponent target2 = _componentFactory.GetComponent(registration);
				_serManager.CopyTo(componentRegistryEntry.Component, ref target2, null, skipHook: false, notNullableOverride: true);
				AddComponentInternal(target, target2, registration, overwrite: true, component);
			}
			else if (!HasComponent(target, registration))
			{
				IComponent target3 = _componentFactory.GetComponent(registration);
				_serManager.CopyTo(componentRegistryEntry.Component, ref target3, null, skipHook: false, notNullableOverride: true);
				AddComponentInternal(target, target3, registration, overwrite: false, component);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RemoveComponents(EntityUid target, EntityPrototype prototype)
	{
		RemoveComponents(target, prototype.Components);
	}

	public void RemoveComponents(EntityUid target, ComponentRegistry registry)
	{
		if (registry.Count == 0)
		{
			return;
		}
		MetaDataComponent component = MetaQuery.GetComponent(target);
		foreach (EntityPrototype.ComponentRegistryEntry value in registry.Values)
		{
			RemoveComponent(target, value.Component.GetType(), component);
		}
	}

	public IComponent AddComponent(EntityUid uid, ushort netId, MetaDataComponent? meta = null)
	{
		IComponent component = _componentFactory.GetComponent(netId);
		AddComponent(uid, component, overwrite: false, meta);
		return component;
	}

	public T AddComponent<T>(EntityUid uid) where T : IComponent, new()
	{
		T component = _componentFactory.GetComponent<T>();
		AddComponent(uid, component);
		return component;
	}

	public void AddComponent(EntityUid uid, EntityPrototype.ComponentRegistryEntry entry, bool overwrite = false, MetaDataComponent? metadata = null)
	{
		IComponent component = _componentFactory.GetComponent(entry);
		AddComponent(uid, component, overwrite, metadata);
	}

	public void AddComponent<T>(EntityUid uid, T component, bool overwrite = false, MetaDataComponent? metadata = null) where T : IComponent
	{
		if (!MetaQuery.Resolve(uid, ref metadata, logMissing: false))
		{
			throw new ArgumentException($"Entity {uid} is not valid.", "uid");
		}
		if (component == null)
		{
			throw new ArgumentNullException("component");
		}
		if (component.Owner == default(EntityUid))
		{
			component.Owner = uid;
		}
		else if (component.Owner != uid)
		{
			throw new InvalidOperationException("Component is not owned by entity.");
		}
		AddComponentInternal(uid, component, overwrite, skipInit: false, metadata);
	}

	private void AddComponentInternal<T>(EntityUid uid, T component, ComponentRegistration compReg, bool overwrite = false, MetaDataComponent? metadata = null) where T : IComponent
	{
		if (!MetaQuery.Resolve(uid, ref metadata, logMissing: false))
		{
			throw new ArgumentException($"Entity {uid} is not valid.", "uid");
		}
		component.Owner = uid;
		AddComponentInternal(uid, component, compReg, overwrite, skipInit: false, metadata);
	}

	private void AddComponentInternal<T>(EntityUid uid, T component, bool overwrite, bool skipInit, MetaDataComponent? metadata) where T : IComponent
	{
		if (!MetaQuery.ResolveInternal(uid, ref metadata, logMissing: false))
		{
			throw new ArgumentException($"Entity {uid} is not valid.", "uid");
		}
		ComponentRegistration registration = _componentFactory.GetRegistration(component);
		AddComponentInternal(uid, component, registration, overwrite, skipInit, metadata);
	}

	private void AddComponentInternal<T>(EntityUid uid, T component, ComponentRegistration reg, bool overwrite, bool skipInit, MetaDataComponent metadata) where T : IComponent
	{
		CompIdx idx = reg.Idx;
		Dictionary<EntityUid, IComponent> dictionary = _entTraitArray[idx.Value];
		bool exists;
		ref IComponent valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, uid, out exists);
		if (exists)
		{
			if (!overwrite && !valueRefOrAddDefault.Deleted)
			{
				throw new InvalidOperationException($"Component reference type {reg.Name} already occupied by {valueRefOrAddDefault}");
			}
			RemoveComponentImmediate(uid, valueRefOrAddDefault, idx, terminating: false, metadata);
			dictionary.Add(uid, component);
		}
		else
		{
			valueRefOrAddDefault = component;
		}
		_entCompIndex.Add(uid, component);
		if (reg.NetID.HasValue && component.NetSyncEnabled)
		{
			ushort value = reg.NetID.Value;
			if (metadata == null)
			{
				metadata = MetaQuery.GetComponentInternal(uid);
			}
			metadata.NetComponents.Add(value, component);
		}
		if (component is IComponentDelta componentDelta)
		{
			GameTick curTick = _gameTiming.CurTick;
			componentDelta.LastModifiedFields = new GameTick[reg.NetworkedFields.Length];
			Array.Fill(componentDelta.LastModifiedFields, curTick);
		}
		component.Networked = reg.NetID.HasValue;
		AddedComponentEventArgs e = new AddedComponentEventArgs(new ComponentEventArgs(component, uid), reg);
		this.ComponentAdded?.Invoke(e);
		EventBusInternal.OnComponentAdded(in e);
		LifeAddToEntity(uid, component, reg.Idx);
		if (skipInit)
		{
			return;
		}
		if (metadata == null)
		{
			metadata = MetaQuery.GetComponentInternal(uid);
		}
		if (metadata.EntityInitialized || metadata.EntityInitializing)
		{
			if (component.Networked)
			{
				DirtyEntity(uid, metadata);
			}
			LifeInitialize(uid, component, reg.Idx);
			if (metadata.EntityInitialized)
			{
				LifeStartup(uid, component, reg.Idx);
			}
			if ((int)metadata.EntityLifeStage >= 3)
			{
				EventBusInternal.RaiseComponentEvent(uid, component, reg.Idx, MapInitEventInstance);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool RemoveComponent<T>(EntityUid uid, MetaDataComponent? meta = null) where T : IComponent
	{
		if (!TryGetComponent<T>(uid, out T component))
		{
			return false;
		}
		RemoveComponentImmediate(uid, component, CompIdx.Index<T>(), terminating: false, meta);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool RemoveComponent(EntityUid uid, Type type, MetaDataComponent? meta = null)
	{
		if (!TryGetComponent(uid, type, out IComponent component))
		{
			return false;
		}
		RemoveComponentImmediate(uid, component, _componentFactory.GetIndex(type), terminating: false, meta);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool RemoveComponent(EntityUid uid, ushort netId, MetaDataComponent? meta = null)
	{
		if (!MetaQuery.Resolve(uid, ref meta))
		{
			return false;
		}
		if (!TryGetComponent(uid, netId, out IComponent component, meta))
		{
			return false;
		}
		CompIdx index = _componentFactory.GetIndex(component.GetType());
		RemoveComponentImmediate(uid, component, index, terminating: false, meta);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RemoveComponent(EntityUid uid, IComponent component, MetaDataComponent? meta = null)
	{
		CompIdx index = _componentFactory.GetIndex(component.GetType());
		RemoveComponentImmediate(uid, component, index, terminating: false, meta);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool RemoveComponentDeferred<T>(EntityUid uid)
	{
		return RemoveComponentDeferred(uid, typeof(T));
	}

	public bool RemoveComponentDeferred(EntityUid uid, Type type)
	{
		if (!TryGetComponent(uid, type, out IComponent component))
		{
			return false;
		}
		RemoveComponentDeferred(component, uid, terminating: false);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool RemoveComponentDeferred(EntityUid uid, ushort netId, MetaDataComponent? meta = null)
	{
		if (!MetaQuery.Resolve(uid, ref meta))
		{
			return false;
		}
		if (!TryGetComponent(uid, netId, out IComponent component, meta))
		{
			return false;
		}
		RemoveComponentDeferred(component, uid, terminating: false);
		return true;
	}

	public void RemoveComponentDeferred(EntityUid owner, IComponent component)
	{
		RemoveComponentDeferred(component, owner, terminating: false);
	}

	public void RemoveComponentDeferred(EntityUid owner, Component component)
	{
		RemoveComponentDeferred(component, owner, terminating: false);
	}

	private static IEnumerable<IComponent> InSafeOrder(IEnumerable<IComponent> comps, bool forCreation = false)
	{
		if (!forCreation)
		{
			return comps.OrderByDescending(Sequence);
		}
		return comps.OrderBy(Sequence);
		static int Sequence(IComponent x)
		{
			if (x is MetaDataComponent)
			{
				return 0;
			}
			if (x is TransformComponent)
			{
				return 1;
			}
			if (x is PhysicsComponent)
			{
				return 2;
			}
			return int.MaxValue;
		}
	}

	public void RemoveComponents(EntityUid uid, MetaDataComponent? meta = null)
	{
		if (!MetaQuery.Resolve(uid, ref meta))
		{
			return;
		}
		foreach (IComponent item in InSafeOrder(_entCompIndex[uid]))
		{
			CompIdx index = _componentFactory.GetIndex(item.GetType());
			RemoveComponentImmediate(uid, item, index, terminating: false, meta);
		}
	}

	public void DisposeComponents(EntityUid uid, MetaDataComponent? meta = null)
	{
		if (!MetaQuery.Resolve(uid, ref meta))
		{
			return;
		}
		foreach (IComponent item in InSafeOrder(_entCompIndex[uid]))
		{
			try
			{
				CompIdx index = _componentFactory.GetIndex(item.GetType());
				RemoveComponentImmediate(uid, item, index, terminating: true, meta);
			}
			catch (Exception)
			{
				_sawmill.Error($"Caught exception while trying to remove component {_componentFactory.GetComponentName(item.GetType())} from entity '{ToPrettyString(uid)}'");
			}
		}
		_entCompIndex.Remove(uid);
	}

	private void RemoveComponentDeferred(IComponent component, EntityUid uid, bool terminating)
	{
		if (component == null)
		{
			throw new ArgumentNullException("component");
		}
		if (component.Owner != uid)
		{
			throw new InvalidOperationException("Component is not owned by entity.");
		}
		if (component.Deleted)
		{
			return;
		}
		try
		{
			bool flag = !terminating;
			if (flag)
			{
				bool flag2 = ((component is TransformComponent || component is MetaDataComponent) ? true : false);
				flag = flag2;
			}
			if (!flag && _deleteSet.Add(component))
			{
				ComponentLifeStage lifeStage = component.LifeStage;
				if ((int)lifeStage >= 4 && (int)lifeStage < 7)
				{
					LifeShutdown(uid, component, _componentFactory.GetIndex(component.GetType()));
				}
				else if (component.LifeStage == ComponentLifeStage.Added)
				{
					component.LifeStage = ComponentLifeStage.Stopped;
				}
			}
		}
		catch (Exception exception)
		{
			_sawmill.Error($"Caught exception while queuing deferred component removal. Entity={ToPrettyString(component.Owner)}, type={component.GetType()}");
			_runtimeLog.LogException(exception, "RemoveComponentDeferred");
		}
	}

	private void RemoveComponentImmediate(EntityUid uid, IComponent component, CompIdx idx, bool terminating, MetaDataComponent? meta)
	{
		if (component.Deleted)
		{
			_sawmill.Warning($"Deleting an already deleted component. Entity: {ToPrettyString(uid)}, Component: {_componentFactory.GetComponentName(component.GetType())}.");
			return;
		}
		try
		{
			bool flag = !terminating;
			if (flag)
			{
				bool flag2 = ((component is TransformComponent || component is MetaDataComponent) ? true : false);
				flag = flag2;
			}
			if (flag)
			{
				return;
			}
			if (component.Running)
			{
				LifeShutdown(uid, component, idx);
			}
			if (component.LifeStage != ComponentLifeStage.PreAdd)
			{
				LifeRemoveFromEntity(uid, component, idx);
			}
		}
		catch (Exception exception)
		{
			_sawmill.Error($"Caught exception during immediate component removal. Entity={ToPrettyString(component.Owner)}, type={component.GetType()}");
			_runtimeLog.LogException(exception, "RemoveComponentImmediate");
		}
		DeleteComponent(uid, component, idx, terminating, meta);
	}

	public void CullRemovedComponents()
	{
		foreach (IComponent item in InSafeOrder(_deleteSet))
		{
			if (item.Deleted)
			{
				continue;
			}
			EntityUid owner = item.Owner;
			CompIdx index = _componentFactory.GetIndex(item.GetType());
			try
			{
				if (item.Running)
				{
					_sawmill.Warning($"Found a running component while culling deferred deletions, owner={ToPrettyString(owner)}, type={item.GetType()}");
					LifeShutdown(owner, item, index);
				}
				if (item.LifeStage != ComponentLifeStage.PreAdd)
				{
					LifeRemoveFromEntity(owner, item, index);
				}
			}
			catch (Exception exception)
			{
				_sawmill.Error($"Caught exception  while processing deferred component removal. Entity={ToPrettyString(item.Owner)}, type={item.GetType()}");
				_runtimeLog.LogException(exception, "CullRemovedComponents");
			}
			MetaDataComponent component = MetaQuery.GetComponent(owner);
			DeleteComponent(owner, item, index, terminating: false, component);
		}
		_deleteSet.Clear();
	}

	private void DeleteComponent(EntityUid entityUid, IComponent component, CompIdx idx, bool terminating, MetaDataComponent? metadata)
	{
		if (!MetaQuery.ResolveInternal(entityUid, ref metadata))
		{
			return;
		}
		RemovedComponentEventArgs e = new RemovedComponentEventArgs(new ComponentEventArgs(component, entityUid), terminating: false, metadata, idx);
		this.ComponentRemoved?.Invoke(e);
		EventBusInternal.OnComponentRemoved(in e);
		if (!terminating)
		{
			ComponentRegistration registration = _componentFactory.GetRegistration(component);
			if (registration.NetID.HasValue)
			{
				if (!metadata.NetComponents.Remove(registration.NetID.Value))
				{
					_sawmill.Error($"Entity {ToPrettyString(entityUid, metadata)} did not have {component.GetType().Name} in its networked component dictionary during component deletion.");
				}
				if (component.NetSyncEnabled)
				{
					DirtyEntity(entityUid, metadata);
					metadata.LastComponentRemoved = _gameTiming.CurTick;
				}
			}
		}
		_entTraitArray[idx.Value].Remove(entityUid);
		_entCompIndex.Remove(entityUid, component);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasComponent<T>(EntityUid uid) where T : IComponent
	{
		if (_entTraitArray[CompIdx.ArrayIndex<T>()].TryGetValue(uid, out IComponent value))
		{
			return !value.Deleted;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasComponent<T>([NotNullWhen(true)] EntityUid? uid) where T : IComponent
	{
		if (uid.HasValue)
		{
			return HasComponent<T>(uid.Value);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasComponent(EntityUid uid, ComponentRegistration reg)
	{
		if (_entTraitArray[reg.Idx.Value].TryGetValue(uid, out IComponent value))
		{
			return !value.Deleted;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasComponent(EntityUid uid, Type type)
	{
		if (_entTraitDict[type].TryGetValue(uid, out IComponent value))
		{
			return !value.Deleted;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasComponent([NotNullWhen(true)] EntityUid? uid, Type type)
	{
		if (!uid.HasValue)
		{
			return false;
		}
		if (_entTraitDict[type].TryGetValue(uid.Value, out IComponent value))
		{
			return !value.Deleted;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasComponent(EntityUid uid, ushort netId, MetaDataComponent? meta = null)
	{
		if (!MetaQuery.Resolve(uid, ref meta))
		{
			return false;
		}
		return meta.NetComponents.ContainsKey(netId);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasComponent([NotNullWhen(true)] EntityUid? uid, ushort netId, MetaDataComponent? meta = null)
	{
		if (!uid.HasValue)
		{
			return false;
		}
		return HasComponent(uid.Value, netId, meta);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T EnsureComponent<T>(EntityUid uid) where T : IComponent, new()
	{
		if (TryGetComponent<T>(uid, out T component))
		{
			if ((int)component.LifeStage <= 6)
			{
				return component;
			}
			RemoveComponent(uid, component);
		}
		return AddComponent<T>(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool EnsureComponent<T>(ref Entity<T?> entity) where T : IComponent, new()
	{
		if (entity.Comp != null)
		{
			if ((int)entity.Comp.LifeStage <= 6)
			{
				return true;
			}
			RemoveComponent(entity, entity.Comp);
		}
		entity.Comp = AddComponent<T>(entity);
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool EnsureComponent<T>(EntityUid entity, out T component) where T : IComponent, new()
	{
		if (TryGetComponent<T>(entity, out T component2))
		{
			if ((int)component2.LifeStage <= 6)
			{
				component = component2;
				return true;
			}
			RemoveComponent(entity, component2);
		}
		component = AddComponent<T>(entity);
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T GetComponent<T>(EntityUid uid) where T : IComponent
	{
		if (_entTraitArray[CompIdx.ArrayIndex<T>()].TryGetValue(uid, out IComponent value) && !value.Deleted)
		{
			return (T)value;
		}
		throw new KeyNotFoundException($"Entity {uid} does not have a component of type {typeof(T)}");
	}

	public IComponent GetComponent(EntityUid uid, CompIdx type)
	{
		if (_entTraitArray[type.Value].TryGetValue(uid, out IComponent value) && !value.Deleted)
		{
			return value;
		}
		throw new KeyNotFoundException($"Entity {uid} does not have a component of type {_componentFactory.IdxToType(type)}");
	}

	public IComponent GetComponent(EntityUid uid, Type type)
	{
		if (_entTraitDict[type].TryGetValue(uid, out IComponent value) && !value.Deleted)
		{
			return value;
		}
		throw new KeyNotFoundException($"Entity {uid} does not have a component of type {type}");
	}

	public IComponent GetComponent(EntityUid uid, ushort netId, MetaDataComponent? meta = null)
	{
		return (meta ?? MetaQuery.GetComponentInternal(uid)).NetComponents[netId];
	}

	public IComponent GetComponentInternal(EntityUid uid, CompIdx type)
	{
		if (_entTraitArray[type.Value].TryGetValue(uid, out IComponent value))
		{
			return value;
		}
		throw new KeyNotFoundException($"Entity {uid} does not have a component of type {type}");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryGetComponent<T>(EntityUid uid, [NotNullWhen(true)] out T? component) where T : IComponent?
	{
		if (_entTraitArray[CompIdx.ArrayIndex<T>()].TryGetValue(uid, out IComponent value) && !value.Deleted)
		{
			component = (T)value;
			return true;
		}
		component = default(T);
		return false;
	}

	public bool TryGetComponent<T>([NotNullWhen(true)] EntityUid? uid, [NotNullWhen(true)] out T? component) where T : IComponent?
	{
		if (!uid.HasValue)
		{
			component = default(T);
			return false;
		}
		if (TryGetComponent(uid.Value, typeof(T), out IComponent component2) && !component2.Deleted)
		{
			component = (T)component2;
			return true;
		}
		component = default(T);
		return false;
	}

	public bool TryGetComponent(EntityUid uid, ComponentRegistration reg, [NotNullWhen(true)] out IComponent? component)
	{
		if (_entTraitArray[reg.Idx.Value].TryGetValue(uid, out IComponent value) && !value.Deleted)
		{
			component = value;
			return true;
		}
		component = null;
		return false;
	}

	public bool TryGetComponent(EntityUid uid, Type type, [NotNullWhen(true)] out IComponent? component)
	{
		if (_entTraitDict[type].TryGetValue(uid, out IComponent value) && !value.Deleted)
		{
			component = value;
			return true;
		}
		component = null;
		return false;
	}

	public bool TryGetComponent(EntityUid uid, CompIdx type, [NotNullWhen(true)] out IComponent? component)
	{
		if (_entTraitArray[type.Value].TryGetValue(uid, out IComponent value) && !value.Deleted)
		{
			component = value;
			return true;
		}
		component = null;
		return false;
	}

	public bool TryGetComponent([NotNullWhen(true)] EntityUid? uid, Type type, [NotNullWhen(true)] out IComponent? component)
	{
		if (!uid.HasValue)
		{
			component = null;
			return false;
		}
		if (_entTraitDict[type].TryGetValue(uid.Value, out IComponent value) && !value.Deleted)
		{
			component = value;
			return true;
		}
		component = null;
		return false;
	}

	public bool TryGetComponent(EntityUid uid, ushort netId, [MaybeNullWhen(false)] out IComponent component, MetaDataComponent? meta = null)
	{
		if (MetaQuery.TryGetComponentInternal(uid, out MetaDataComponent component2) && component2.NetComponents.TryGetValue(netId, out IComponent value))
		{
			component = value;
			return true;
		}
		component = null;
		return false;
	}

	public bool TryGetComponent([NotNullWhen(true)] EntityUid? uid, ushort netId, [MaybeNullWhen(false)] out IComponent component, MetaDataComponent? meta = null)
	{
		if (!uid.HasValue)
		{
			component = null;
			return false;
		}
		return TryGetComponent(uid.Value, netId, out component, meta);
	}

	public bool TryCopyComponent<T>(EntityUid source, EntityUid target, ref T? sourceComponent, [NotNullWhen(true)] out T? targetComp, MetaDataComponent? meta = null) where T : IComponent
	{
		if (!MetaQuery.Resolve(target, ref meta))
		{
			targetComp = default(T);
			return false;
		}
		if (sourceComponent == null && !TryGetComponent<T>(source, out sourceComponent))
		{
			targetComp = default(T);
			return false;
		}
		targetComp = CopyComponentInternal(source, target, sourceComponent, meta);
		return true;
	}

	public bool TryCopyComponents(EntityUid source, EntityUid target, MetaDataComponent? meta = null, params Type[] sourceComponents)
	{
		if (!MetaQuery.TryGetComponent(target, out meta))
		{
			return false;
		}
		bool result = true;
		foreach (Type type in sourceComponents)
		{
			if (!TryGetComponent(source, type, out IComponent component))
			{
				result = false;
			}
			else
			{
				CopyComponent(source, target, component, meta);
			}
		}
		return result;
	}

	public IComponent CopyComponent(EntityUid source, EntityUid target, IComponent sourceComponent, MetaDataComponent? meta = null)
	{
		if (!MetaQuery.Resolve(target, ref meta))
		{
			throw new InvalidOperationException();
		}
		return CopyComponentInternal(source, target, sourceComponent, meta);
	}

	public T CopyComponent<T>(EntityUid source, EntityUid target, T sourceComponent, MetaDataComponent? meta = null) where T : IComponent
	{
		if (!MetaQuery.Resolve(target, ref meta))
		{
			throw new InvalidOperationException();
		}
		return CopyComponentInternal(source, target, sourceComponent, meta);
	}

	public void CopyComponents(EntityUid source, EntityUid target, MetaDataComponent? meta = null, params IComponent[] sourceComponents)
	{
		if (MetaQuery.Resolve(target, ref meta))
		{
			foreach (IComponent sourceComponent in sourceComponents)
			{
				CopyComponentInternal(source, target, sourceComponent, meta);
			}
		}
	}

	private T CopyComponentInternal<T>(EntityUid source, EntityUid target, T sourceComponent, MetaDataComponent meta) where T : IComponent
	{
		ComponentRegistration registration = ComponentFactory.GetRegistration(sourceComponent.GetType());
		T target2 = (T)ComponentFactory.GetComponent(registration);
		_serManager.CopyTo(sourceComponent, ref target2, null, skipHook: false, notNullableOverride: true);
		target2.Owner = target;
		AddComponentInternal(target, target2, registration, overwrite: true, skipInit: false, meta);
		return target2;
	}

	public EntityQuery<TComp1> GetEntityQuery<TComp1>() where TComp1 : IComponent
	{
		Dictionary<EntityUid, IComponent> traitDict = _entTraitArray[CompIdx.ArrayIndex<TComp1>()];
		return new EntityQuery<TComp1>(this, traitDict);
	}

	public EntityQuery<IComponent> GetEntityQuery(Type type)
	{
		Dictionary<EntityUid, IComponent> traitDict = _entTraitDict[type];
		return new EntityQuery<IComponent>(this, traitDict);
	}

	public IEnumerable<IComponent> GetComponents(EntityUid uid)
	{
		IComponent[] array = _entCompIndex[uid].ToArray();
		foreach (IComponent component in array)
		{
			if (!component.Deleted)
			{
				yield return component;
			}
		}
	}

	internal IReadOnlyCollection<IComponent> GetComponentsInternal(EntityUid uid)
	{
		return _entCompIndex[uid];
	}

	public int ComponentCount(EntityUid uid)
	{
		return _entCompIndex[uid].Count;
	}

	private void CopyComponentsInto(ref Span<IComponent?> comps, EntityUid uid)
	{
		HashSet<IComponent> hashSet = _entCompIndex[uid];
		if (hashSet.Count > comps.Length)
		{
			comps = new IComponent[hashSet.Count];
		}
		int num = 0;
		foreach (IComponent item in hashSet)
		{
			comps[num++] = item;
		}
	}

	public IEnumerable<T> GetComponents<T>(EntityUid uid)
	{
		IComponent[] array = _entCompIndex[uid].ToArray();
		IComponent[] array2 = array;
		foreach (IComponent component in array2)
		{
			if (!component.Deleted && component is T)
			{
				yield return (T)component;
			}
		}
	}

	public NetComponentEnumerable GetNetComponents(EntityUid uid, MetaDataComponent? meta = null)
	{
		if (meta == null)
		{
			meta = MetaQuery.GetComponentInternal(uid);
		}
		return new NetComponentEnumerable(meta.NetComponents);
	}

	public NetComponentEnumerable? GetNetComponentsOrNull(EntityUid uid, MetaDataComponent? meta = null)
	{
		if (!MetaQuery.Resolve(uid, ref meta))
		{
			return null;
		}
		return new NetComponentEnumerable(meta.NetComponents);
	}

	public (EntityUid Uid, T Component)[] AllComponents<T>() where T : IComponent
	{
		AllEntityQueryEnumerator<T> allEntityQueryEnumerator = AllEntityQueryEnumerator<T>();
		(EntityUid, T)[] array = new(EntityUid, T)[Count<T>()];
		int num = 0;
		EntityUid uid;
		T comp;
		while (allEntityQueryEnumerator.MoveNext(out uid, out comp))
		{
			array[num] = (uid, comp);
			num++;
		}
		Array.Resize(ref array, num);
		return array;
	}

	public Entity<T>[] AllEntities<T>() where T : IComponent
	{
		AllEntityQueryEnumerator<T> allEntityQueryEnumerator = AllEntityQueryEnumerator<T>();
		Entity<T>[] array = new Entity<T>[Count<T>()];
		int newSize = 0;
		EntityUid uid;
		T comp;
		while (allEntityQueryEnumerator.MoveNext(out uid, out comp))
		{
			array[newSize++] = (Owner: uid, Comp: comp);
		}
		Array.Resize(ref array, newSize);
		return array;
	}

	public Entity<IComponent>[] AllEntities(Type tComp)
	{
		AllEntityQueryEnumerator<IComponent> allEntityQueryEnumerator = AllEntityQueryEnumerator(tComp);
		Entity<IComponent>[] array = new Entity<IComponent>[Count(tComp)];
		int newSize = 0;
		EntityUid uid;
		IComponent comp;
		while (allEntityQueryEnumerator.MoveNext(out uid, out comp))
		{
			array[newSize++] = (Owner: uid, Comp: comp);
		}
		Array.Resize(ref array, newSize);
		return array;
	}

	public EntityUid[] AllEntityUids<T>() where T : IComponent
	{
		AllEntityQueryEnumerator<T> allEntityQueryEnumerator = AllEntityQueryEnumerator<T>();
		EntityUid[] array = new EntityUid[Count<T>()];
		int newSize = 0;
		EntityUid uid;
		T comp;
		while (allEntityQueryEnumerator.MoveNext(out uid, out comp))
		{
			array[newSize++] = uid;
		}
		Array.Resize(ref array, newSize);
		return array;
	}

	public EntityUid[] AllEntityUids(Type tComp)
	{
		AllEntityQueryEnumerator<IComponent> allEntityQueryEnumerator = AllEntityQueryEnumerator(tComp);
		EntityUid[] array = new EntityUid[Count(tComp)];
		int newSize = 0;
		EntityUid uid;
		IComponent comp;
		while (allEntityQueryEnumerator.MoveNext(out uid, out comp))
		{
			array[newSize++] = uid;
		}
		Array.Resize(ref array, newSize);
		return array;
	}

	public List<(EntityUid Uid, T Component)> AllComponentsList<T>() where T : IComponent
	{
		AllEntityQueryEnumerator<T> allEntityQueryEnumerator = AllEntityQueryEnumerator<T>();
		List<(EntityUid, T)> list = new List<(EntityUid, T)>(Count<T>());
		EntityUid uid;
		T comp;
		while (allEntityQueryEnumerator.MoveNext(out uid, out comp))
		{
			list.Add((uid, comp));
		}
		return list;
	}

	public ComponentQueryEnumerator ComponentQueryEnumerator(ComponentRegistry registry)
	{
		if (registry.Count == 0)
		{
			return new ComponentQueryEnumerator(new Dictionary<EntityUid, IComponent>());
		}
		EntityPrototype.ComponentRegistryEntry value = registry.First().Value;
		return new ComponentQueryEnumerator(_entTraitArray[_componentFactory.GetArrayIndex(value.Component.GetType())]);
	}

	public CompRegistryEntityEnumerator CompRegistryQueryEnumerator(ComponentRegistry registry)
	{
		if (registry.Count == 0)
		{
			return new CompRegistryEntityEnumerator(this, new Dictionary<EntityUid, IComponent>(), registry);
		}
		EntityPrototype.ComponentRegistryEntry value = registry.First().Value;
		Dictionary<EntityUid, IComponent> traitDict = _entTraitArray[_componentFactory.GetArrayIndex(value.Component.GetType())];
		return new CompRegistryEntityEnumerator(this, traitDict, registry);
	}

	public AllEntityQueryEnumerator<IComponent> AllEntityQueryEnumerator(Type comp)
	{
		return new AllEntityQueryEnumerator<IComponent>(_entTraitArray[_componentFactory.GetIndex(comp).Value]);
	}

	public AllEntityQueryEnumerator<TComp1> AllEntityQueryEnumerator<TComp1>() where TComp1 : IComponent
	{
		return new AllEntityQueryEnumerator<TComp1>(_entTraitArray[CompIdx.ArrayIndex<TComp1>()]);
	}

	public AllEntityQueryEnumerator<TComp1, TComp2> AllEntityQueryEnumerator<TComp1, TComp2>() where TComp1 : IComponent where TComp2 : IComponent
	{
		Dictionary<EntityUid, IComponent> traitDict = _entTraitArray[CompIdx.ArrayIndex<TComp1>()];
		Dictionary<EntityUid, IComponent> traitDict2 = _entTraitArray[CompIdx.ArrayIndex<TComp2>()];
		return new AllEntityQueryEnumerator<TComp1, TComp2>(traitDict, traitDict2);
	}

	public AllEntityQueryEnumerator<TComp1, TComp2, TComp3> AllEntityQueryEnumerator<TComp1, TComp2, TComp3>() where TComp1 : IComponent where TComp2 : IComponent where TComp3 : IComponent
	{
		Dictionary<EntityUid, IComponent> traitDict = _entTraitArray[CompIdx.ArrayIndex<TComp1>()];
		Dictionary<EntityUid, IComponent> traitDict2 = _entTraitArray[CompIdx.ArrayIndex<TComp2>()];
		Dictionary<EntityUid, IComponent> traitDict3 = _entTraitArray[CompIdx.ArrayIndex<TComp3>()];
		return new AllEntityQueryEnumerator<TComp1, TComp2, TComp3>(traitDict, traitDict2, traitDict3);
	}

	public AllEntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4> AllEntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4>() where TComp1 : IComponent where TComp2 : IComponent where TComp3 : IComponent where TComp4 : IComponent
	{
		Dictionary<EntityUid, IComponent> traitDict = _entTraitArray[CompIdx.ArrayIndex<TComp1>()];
		Dictionary<EntityUid, IComponent> traitDict2 = _entTraitArray[CompIdx.ArrayIndex<TComp2>()];
		Dictionary<EntityUid, IComponent> traitDict3 = _entTraitArray[CompIdx.ArrayIndex<TComp3>()];
		Dictionary<EntityUid, IComponent> traitDict4 = _entTraitArray[CompIdx.ArrayIndex<TComp4>()];
		return new AllEntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4>(traitDict, traitDict2, traitDict3, traitDict4);
	}

	public EntityQueryEnumerator<TComp1> EntityQueryEnumerator<TComp1>() where TComp1 : IComponent
	{
		return new EntityQueryEnumerator<TComp1>(_entTraitArray[CompIdx.ArrayIndex<TComp1>()], MetaQuery);
	}

	public EntityQueryEnumerator<TComp1, TComp2> EntityQueryEnumerator<TComp1, TComp2>() where TComp1 : IComponent where TComp2 : IComponent
	{
		Dictionary<EntityUid, IComponent> traitDict = _entTraitArray[CompIdx.ArrayIndex<TComp1>()];
		Dictionary<EntityUid, IComponent> traitDict2 = _entTraitArray[CompIdx.ArrayIndex<TComp2>()];
		return new EntityQueryEnumerator<TComp1, TComp2>(traitDict, traitDict2, MetaQuery);
	}

	public EntityQueryEnumerator<TComp1, TComp2, TComp3> EntityQueryEnumerator<TComp1, TComp2, TComp3>() where TComp1 : IComponent where TComp2 : IComponent where TComp3 : IComponent
	{
		Dictionary<EntityUid, IComponent> traitDict = _entTraitArray[CompIdx.ArrayIndex<TComp1>()];
		Dictionary<EntityUid, IComponent> traitDict2 = _entTraitArray[CompIdx.ArrayIndex<TComp2>()];
		Dictionary<EntityUid, IComponent> traitDict3 = _entTraitArray[CompIdx.ArrayIndex<TComp3>()];
		return new EntityQueryEnumerator<TComp1, TComp2, TComp3>(traitDict, traitDict2, traitDict3, MetaQuery);
	}

	public EntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4> EntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4>() where TComp1 : IComponent where TComp2 : IComponent where TComp3 : IComponent where TComp4 : IComponent
	{
		Dictionary<EntityUid, IComponent> traitDict = _entTraitArray[CompIdx.ArrayIndex<TComp1>()];
		Dictionary<EntityUid, IComponent> traitDict2 = _entTraitArray[CompIdx.ArrayIndex<TComp2>()];
		Dictionary<EntityUid, IComponent> traitDict3 = _entTraitArray[CompIdx.ArrayIndex<TComp3>()];
		Dictionary<EntityUid, IComponent> traitDict4 = _entTraitArray[CompIdx.ArrayIndex<TComp4>()];
		return new EntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4>(traitDict, traitDict2, traitDict3, traitDict4, MetaQuery);
	}

	public IEnumerable<T> EntityQuery<T>(bool includePaused = false) where T : IComponent
	{
		Dictionary<EntityUid, IComponent> dictionary = _entTraitArray[CompIdx.ArrayIndex<T>()];
		if (includePaused)
		{
			foreach (IComponent value in dictionary.Values)
			{
				if (!value.Deleted)
				{
					yield return (T)value;
				}
			}
			yield break;
		}
		foreach (var (uid, component2) in dictionary)
		{
			if (!component2.Deleted && MetaQuery.TryGetComponentInternal(uid, out MetaDataComponent component3) && !component3.EntityPaused)
			{
				yield return (T)component2;
			}
		}
	}

	public IEnumerable<(TComp1, TComp2)> EntityQuery<TComp1, TComp2>(bool includePaused = false) where TComp1 : IComponent where TComp2 : IComponent
	{
		Dictionary<EntityUid, IComponent> dictionary = _entTraitArray[CompIdx.ArrayIndex<TComp1>()];
		Dictionary<EntityUid, IComponent> trait2 = _entTraitArray[CompIdx.ArrayIndex<TComp2>()];
		EntityUid key;
		IComponent value;
		if (includePaused)
		{
			foreach (KeyValuePair<EntityUid, IComponent> item in dictionary)
			{
				item.Deconstruct(out key, out value);
				EntityUid key2 = key;
				IComponent component = value;
				if (trait2.TryGetValue(key2, out IComponent value2) && !value2.Deleted)
				{
					yield return ((TComp1)component, (TComp2)value2);
				}
			}
			yield break;
		}
		Dictionary<EntityUid, IComponent> metaComps = _entTraitArray[CompIdx.ArrayIndex<MetaDataComponent>()];
		foreach (KeyValuePair<EntityUid, IComponent> item2 in dictionary)
		{
			item2.Deconstruct(out key, out value);
			EntityUid key3 = key;
			IComponent component2 = value;
			if (trait2.TryGetValue(key3, out IComponent value3) && !value3.Deleted && !component2.Deleted && metaComps.TryGetValue(key3, out IComponent value4) && !((MetaDataComponent)value4).EntityPaused)
			{
				yield return ((TComp1)component2, (TComp2)value3);
			}
		}
	}

	public IEnumerable<(TComp1, TComp2, TComp3)> EntityQuery<TComp1, TComp2, TComp3>(bool includePaused = false) where TComp1 : IComponent where TComp2 : IComponent where TComp3 : IComponent
	{
		Dictionary<EntityUid, IComponent> dictionary = _entTraitArray[CompIdx.ArrayIndex<TComp1>()];
		Dictionary<EntityUid, IComponent> trait2 = _entTraitArray[CompIdx.ArrayIndex<TComp2>()];
		Dictionary<EntityUid, IComponent> trait3 = _entTraitArray[CompIdx.ArrayIndex<TComp3>()];
		EntityUid key;
		IComponent value;
		if (includePaused)
		{
			foreach (KeyValuePair<EntityUid, IComponent> item in dictionary)
			{
				item.Deconstruct(out key, out value);
				EntityUid key2 = key;
				IComponent component = value;
				if (trait2.TryGetValue(key2, out IComponent value2) && !value2.Deleted && trait3.TryGetValue(key2, out IComponent value3) && !value3.Deleted)
				{
					yield return ((TComp1)component, (TComp2)value2, (TComp3)value3);
				}
			}
			yield break;
		}
		Dictionary<EntityUid, IComponent> metaComps = _entTraitArray[CompIdx.ArrayIndex<MetaDataComponent>()];
		foreach (KeyValuePair<EntityUid, IComponent> item2 in dictionary)
		{
			item2.Deconstruct(out key, out value);
			EntityUid key3 = key;
			IComponent component2 = value;
			if (trait2.TryGetValue(key3, out IComponent value4) && !value4.Deleted && trait3.TryGetValue(key3, out IComponent value5) && !value5.Deleted && !component2.Deleted && metaComps.TryGetValue(key3, out IComponent value6) && !((MetaDataComponent)value6).EntityPaused)
			{
				yield return ((TComp1)component2, (TComp2)value4, (TComp3)value5);
			}
		}
	}

	public IEnumerable<(TComp1, TComp2, TComp3, TComp4)> EntityQuery<TComp1, TComp2, TComp3, TComp4>(bool includePaused = false) where TComp1 : IComponent where TComp2 : IComponent where TComp3 : IComponent where TComp4 : IComponent
	{
		Dictionary<EntityUid, IComponent> dictionary = _entTraitArray[CompIdx.ArrayIndex<TComp1>()];
		Dictionary<EntityUid, IComponent> trait2 = _entTraitArray[CompIdx.ArrayIndex<TComp2>()];
		Dictionary<EntityUid, IComponent> trait3 = _entTraitArray[CompIdx.ArrayIndex<TComp3>()];
		Dictionary<EntityUid, IComponent> trait4 = _entTraitArray[CompIdx.ArrayIndex<TComp4>()];
		EntityUid key;
		IComponent value;
		if (includePaused)
		{
			foreach (KeyValuePair<EntityUid, IComponent> item in dictionary)
			{
				item.Deconstruct(out key, out value);
				EntityUid key2 = key;
				IComponent component = value;
				if (trait2.TryGetValue(key2, out IComponent value2) && !value2.Deleted && trait3.TryGetValue(key2, out IComponent value3) && !value3.Deleted && trait4.TryGetValue(key2, out IComponent value4) && !value4.Deleted)
				{
					yield return ((TComp1)component, (TComp2)value2, (TComp3)value3, (TComp4)value4);
				}
			}
			yield break;
		}
		Dictionary<EntityUid, IComponent> metaComps = _entTraitArray[CompIdx.ArrayIndex<MetaDataComponent>()];
		foreach (KeyValuePair<EntityUid, IComponent> item2 in dictionary)
		{
			item2.Deconstruct(out key, out value);
			EntityUid key3 = key;
			IComponent component2 = value;
			if (trait2.TryGetValue(key3, out IComponent value5) && !value5.Deleted && trait3.TryGetValue(key3, out IComponent value6) && !value6.Deleted && trait4.TryGetValue(key3, out IComponent value7) && !value7.Deleted && !component2.Deleted && metaComps.TryGetValue(key3, out IComponent value8) && !((MetaDataComponent)value8).EntityPaused)
			{
				yield return ((TComp1)component2, (TComp2)value5, (TComp3)value6, (TComp4)value7);
			}
		}
	}

	public IEnumerable<(EntityUid Uid, IComponent Component)> GetAllComponents(Type type, bool includePaused = false)
	{
		Dictionary<EntityUid, IComponent> dictionary = _entTraitDict[type];
		EntityUid key;
		IComponent value;
		if (includePaused)
		{
			foreach (KeyValuePair<EntityUid, IComponent> item2 in dictionary)
			{
				item2.Deconstruct(out key, out value);
				EntityUid item = key;
				IComponent component = value;
				if (!component.Deleted)
				{
					yield return (Uid: item, Component: component);
				}
			}
			yield break;
		}
		foreach (KeyValuePair<EntityUid, IComponent> item3 in dictionary)
		{
			item3.Deconstruct(out key, out value);
			EntityUid entityUid = key;
			IComponent component2 = value;
			if (!component2.Deleted && MetaQuery.TryGetComponent(entityUid, out MetaDataComponent component3) && !component3.EntityPaused)
			{
				yield return (Uid: entityUid, Component: component2);
			}
		}
	}

	public IComponentState? GetComponentState(IEventBus eventBus, IComponent component, ICommonSession? session, GameTick fromTick)
	{
		ComponentGetState args = new ComponentGetState(session, fromTick);
		eventBus.RaiseComponentEvent(component.Owner, component, ref args);
		return args.State;
	}

	public bool CanGetComponentState(IEventBus eventBus, IComponent component, ICommonSession player)
	{
		return CanGetComponentState(component, player);
	}

	public bool CanGetComponentState(IComponent component, ICommonSession player)
	{
		ComponentGetStateAttemptEvent args = new ComponentGetStateAttemptEvent(player);
		EventBusInternal.RaiseComponentEvent(component.Owner, component, ref args);
		return !args.Cancelled;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void FillComponentDict()
	{
		_entTraitDict = FrozenDictionary<Type, Dictionary<EntityUid, IComponent>>.Empty;
		Array.Fill(_entTraitArray, null);
		RegisterComponents(_componentFactory.GetAllRegistrations());
	}

	public EntityManager()
	{
	}

	public virtual void Initialize()
	{
		if (Initialized)
		{
			throw new InvalidOperationException("Initialize() called multiple times");
		}
		EventBusInternal = new EntityEventBus(this, _reflection);
		InitializeComponents();
		_metaReg = _componentFactory.GetRegistration(typeof(MetaDataComponent));
		_xformReg = _componentFactory.GetRegistration(typeof(TransformComponent));
		_xformName = _xformReg.Name;
		_sawmill = LogManager.GetSawmill("entity");
		ResolveSawmill = LogManager.GetSawmill("resolve");
		Initialized = true;
	}

	public bool IsDefault(EntityUid uid, ICollection<string>? ignoredComps = null)
	{
		if (!MetaQuery.TryGetComponent(uid, out MetaDataComponent component) || component.EntityPrototype == null)
		{
			return false;
		}
		EntityPrototype entityPrototype = component.EntityPrototype;
		if (component.EntityName != entityPrototype.Name || component.EntityDescription != entityPrototype.Description)
		{
			return false;
		}
		IReadOnlyDictionary<string, MappingDataNode> prototypeData = PrototypeManager.GetPrototypeData(entityPrototype);
		HashSet<IComponent> hashSet = _entCompIndex[uid];
		if (prototypeData.Count + 2 != hashSet.Count)
		{
			return false;
		}
		foreach (IComponent item in hashSet)
		{
			if (item.Deleted)
			{
				return false;
			}
			Type type = item.GetType();
			if (type == typeof(TransformComponent) || type == typeof(MetaDataComponent))
			{
				continue;
			}
			string componentName = _componentFactory.GetComponentName(type);
			if (ignoredComps == null || !ignoredComps.Contains(componentName))
			{
				if (!prototypeData.TryGetValue(componentName, out var value))
				{
					return false;
				}
				MappingDataNode mappingDataNode;
				try
				{
					mappingDataNode = _serManager.WriteValueAs<MappingDataNode>(type, item, alwaysWrite: true, _context);
				}
				catch (Exception ex)
				{
					_sawmill.Error($"Failed to serialize {componentName} component of entity prototype {entityPrototype.ID}. Exception: {ex.Message}");
					return false;
				}
				if (mappingDataNode.AnyExcept(value))
				{
					return false;
				}
			}
		}
		return true;
	}

	public virtual void Startup()
	{
		if (!Initialized)
		{
			throw new InvalidOperationException("Startup() called without Initialized");
		}
		if (Started)
		{
			throw new InvalidOperationException("Startup() called multiple times");
		}
		_entitySystemManager.Initialize();
		Started = true;
		EventBusInternal.LockSubscriptions();
		_mapSystem = System<SharedMapSystem>();
		_xforms = System<SharedTransformSystem>();
		_containers = System<SharedContainerSystem>();
		MetaQuery = GetEntityQuery<MetaDataComponent>();
		TransformQuery = GetEntityQuery<TransformComponent>();
		_physicsQuery = GetEntityQuery<PhysicsComponent>();
		_actorQuery = GetEntityQuery<ActorComponent>();
		_entityConsoleHost.Startup();
	}

	public virtual void Shutdown()
	{
		ShuttingDown = true;
		FlushEntities();
		EventBusInternal.ClearSubscriptions();
		_entitySystemManager.Shutdown();
		ClearComponents();
		ShuttingDown = false;
		Started = false;
		_entityConsoleHost.Shutdown();
	}

	public virtual void Cleanup()
	{
		_componentFactory.ComponentsAdded -= OnComponentsAdded;
		ShuttingDown = true;
		FlushEntities();
		_entitySystemManager.Clear();
		EventBusInternal.Dispose();
		EventBusInternal = null;
		ClearComponents();
		ShuttingDown = false;
		Initialized = false;
		Started = false;
	}

	public virtual void TickUpdate(float frameTime, bool noPredictions, Histogram? histogram)
	{
		ITimer val = ((histogram != null) ? TimerExtensions.NewTimer((IObserver)(object)((Collector<Child>)(object)histogram).WithLabels(new string[1] { "EntitySystems" })) : null);
		try
		{
			using (_prof.Group("Systems"))
			{
				_entitySystemManager.TickUpdate(frameTime, noPredictions);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = ((histogram != null) ? TimerExtensions.NewTimer((IObserver)(object)((Collector<Child>)(object)histogram).WithLabels(new string[1] { "EntityEventBus" })) : null);
		try
		{
			using (_prof.Group("Events"))
			{
				EventBusInternal.ProcessEventQueue();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = ((histogram != null) ? TimerExtensions.NewTimer((IObserver)(object)((Collector<Child>)(object)histogram).WithLabels(new string[1] { "QueuedDeletion" })) : null);
		try
		{
			using (_prof.Group("QueueDel"))
			{
				ProcessQueueudDeletions();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = ((histogram != null) ? TimerExtensions.NewTimer((IObserver)(object)((Collector<Child>)(object)histogram).WithLabels(new string[1] { "ComponentCull" })) : null);
		try
		{
			using (_prof.Group("ComponentCull"))
			{
				CullRemovedComponents();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	internal virtual void ProcessQueueudDeletions()
	{
		EntityUid result;
		while (QueuedDeletions.TryDequeue(out result))
		{
			DeleteEntity(result);
		}
		QueuedDeletionsSet.Clear();
	}

	public virtual void FrameUpdate(float frameTime)
	{
		_entitySystemManager.FrameUpdate(frameTime);
	}

	public EntityUid CreateEntityUninitialized(string? prototypeName, EntityUid euid, ComponentRegistry? overrides = null)
	{
		MetaDataComponent metadata;
		return CreateEntity(prototypeName, out metadata, overrides);
	}

	public EntityUid CreateEntityUninitialized(string? prototypeName, ComponentRegistry? overrides = null)
	{
		MetaDataComponent metadata;
		return CreateEntity(prototypeName, out metadata, overrides);
	}

	public EntityUid CreateEntityUninitialized(string? prototypeName, out MetaDataComponent meta, ComponentRegistry? overrides = null)
	{
		return CreateEntity(prototypeName, out meta, overrides);
	}

	public virtual EntityUid CreateEntityUninitialized(string? prototypeName, EntityCoordinates coordinates, ComponentRegistry? overrides = null, Angle rotation = default(Angle))
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		MetaDataComponent metadata;
		EntityUid entityUid = CreateEntity(prototypeName, out metadata, overrides);
		TransformComponent component = TransformQuery.GetComponent(entityUid);
		_xforms.SetCoordinates(entityUid, component, coordinates, rotation, unanchor: false);
		return entityUid;
	}

	public virtual EntityUid CreateEntityUninitialized(string? prototypeName, MapCoordinates coordinates, ComponentRegistry? overrides = null, Angle rotation = default(Angle))
	{
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		MetaDataComponent metadata;
		EntityUid entityUid = CreateEntity(prototypeName, out metadata, overrides);
		TransformComponent component = TransformQuery.GetComponent(entityUid);
		if (coordinates.MapId == MapId.Nullspace)
		{
			component._parent = EntityUid.Invalid;
			component.Anchored = false;
			return entityUid;
		}
		EntityUid map = _mapSystem.GetMap(coordinates.MapId);
		if (!TryGetComponent<TransformComponent>(map, out TransformComponent component2))
		{
			throw new ArgumentException($"Attempted to spawn entity on an invalid map. Coordinates: {coordinates}");
		}
		if (_mapManager.TryFindGridAt(coordinates, out EntityUid uid, out MapGridComponent grid) && MetaQuery.TryGetComponentInternal(uid, out MetaDataComponent component3) && (int)component3.EntityLifeStage < 4)
		{
			EntityCoordinates value = new EntityCoordinates(uid, _mapSystem.WorldToLocal(uid, grid, coordinates.Position));
			Angle value2 = rotation - _xforms.GetWorldRotation(uid);
			_xforms.SetCoordinates(entityUid, component, value, value2, unanchor: false);
		}
		else
		{
			EntityCoordinates value = new EntityCoordinates(map, coordinates.Position);
			_xforms.SetCoordinates(entityUid, component, value, rotation, unanchor: true, component2);
		}
		return entityUid;
	}

	public IEnumerable<EntityUid> GetEntities()
	{
		return Entities;
	}

	public virtual void DirtyEntity(EntityUid uid, MetaDataComponent? metadata = null)
	{
		if (MetaQuery.ResolveInternal(uid, ref metadata) && !(metadata.EntityLastModifiedTick == _gameTiming.CurTick))
		{
			metadata.EntityLastModifiedTick = _gameTiming.CurTick;
			if ((int)metadata.EntityLifeStage > 1)
			{
				this.EntityDirtied?.Invoke((Owner: uid, Comp: metadata));
			}
		}
	}

	[Obsolete("use override with an EntityUid or Entity<T>")]
	public void Dirty(IComponent component, MetaDataComponent? meta = null)
	{
		Dirty(component.Owner, component, meta);
	}

	public virtual void Dirty(EntityUid uid, IComponent component, MetaDataComponent? meta = null)
	{
		if ((int)component.LifeStage < 9 && component.NetSyncEnabled && !(component.LastModifiedTick == CurrentTick))
		{
			DirtyEntity(uid, meta);
			component.LastModifiedTick = CurrentTick;
		}
	}

	public virtual void Dirty<T>(Entity<T> ent, MetaDataComponent? meta = null) where T : IComponent
	{
		if ((int)ent.Comp.LifeStage < 9 && ent.Comp.NetSyncEnabled && !(ent.Comp.LastModifiedTick == CurrentTick))
		{
			DirtyEntity(ent, meta);
			ent.Comp.LastModifiedTick = CurrentTick;
		}
	}

	public virtual void Dirty<T1, T2>(Entity<T1, T2> ent, MetaDataComponent? meta = null) where T1 : IComponent where T2 : IComponent
	{
		DirtyEntity(ent, meta);
		ent.Comp1.LastModifiedTick = CurrentTick;
		ent.Comp2.LastModifiedTick = CurrentTick;
	}

	public virtual void Dirty<T1, T2, T3>(Entity<T1, T2, T3> ent, MetaDataComponent? meta = null) where T1 : IComponent where T2 : IComponent where T3 : IComponent
	{
		DirtyEntity(ent, meta);
		ent.Comp1.LastModifiedTick = CurrentTick;
		ent.Comp2.LastModifiedTick = CurrentTick;
		ent.Comp3.LastModifiedTick = CurrentTick;
	}

	public virtual void Dirty<T1, T2, T3, T4>(Entity<T1, T2, T3, T4> ent, MetaDataComponent? meta = null) where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
	{
		DirtyEntity(ent, meta);
		ent.Comp1.LastModifiedTick = CurrentTick;
		ent.Comp2.LastModifiedTick = CurrentTick;
		ent.Comp3.LastModifiedTick = CurrentTick;
		ent.Comp4.LastModifiedTick = CurrentTick;
	}

	public bool TryQueueDeleteEntity(EntityUid? uid)
	{
		if (!uid.HasValue)
		{
			return false;
		}
		if (Deleted(uid.Value))
		{
			return false;
		}
		if (QueuedDeletionsSet.Contains(uid.Value))
		{
			return false;
		}
		QueueDeleteEntity(uid);
		return true;
	}

	public virtual void DeleteEntity(EntityUid? uid)
	{
		if (uid.HasValue && Started && MetaQuery.TryGetComponent(uid.Value, out MetaDataComponent component))
		{
			DeleteEntity(uid.Value, component, TransformQuery.GetComponent(uid.Value));
		}
	}

	public void DeleteEntity(EntityUid e, MetaDataComponent meta, TransformComponent xform)
	{
		if (!Started || (int)meta.EntityLifeStage >= 5)
		{
			return;
		}
		if (meta.EntityLifeStage == EntityLifeStage.Terminating)
		{
			string text = $"Called Delete on an entity already being deleted. Entity: {ToPrettyString(e)}";
			_sawmill.Error(text + ". Trace: " + Environment.StackTrace);
		}
		RecursiveFlagEntityTermination(e, meta, xform);
		TransformComponent component = null;
		if (xform.ParentUid.IsValid())
		{
			if ((int)xform.LifeStage < 4)
			{
				if (TransformQuery.TryComp(xform.ParentUid, out component))
				{
					component._children.Remove(e);
				}
				component = null;
				xform._parent = EntityUid.Invalid;
				xform._anchored = false;
			}
			else
			{
				TransformQuery.Resolve(xform.ParentUid, ref component);
			}
		}
		RecursiveDeleteEntity(e, meta, xform, component);
	}

	private void RecursiveFlagEntityTermination(EntityUid uid, MetaDataComponent metadata, TransformComponent xform)
	{
		SetLifeStage(metadata, EntityLifeStage.Terminating);
		try
		{
			EntityTerminatingEvent ev = new EntityTerminatingEvent((Owner: uid, Comp: metadata));
			this.BeforeEntityTerminating?.Invoke(ref ev);
			EventBusInternal.RaiseLocalEvent(uid, ref ev, broadcast: true);
		}
		catch (Exception value)
		{
			_sawmill.Error($"Caught exception while raising event {"EntityTerminatingEvent"} on entity {ToPrettyString(uid, metadata)}\n{value}");
		}
		foreach (EntityUid child in xform._children)
		{
			if (!MetaQuery.TryGetComponent(child, out MetaDataComponent component) || component.EntityDeleted)
			{
				_sawmill.Error($"A deleted entity was still the transform child of another entity. Parent: {ToPrettyString(uid, metadata)}.");
				xform._children.Remove(child);
			}
			else
			{
				RecursiveFlagEntityTermination(child, component, TransformQuery.GetComponent(child));
			}
		}
	}

	private void RecursiveDeleteEntity(EntityUid uid, MetaDataComponent metadata, TransformComponent transform, TransformComponent? parentXform)
	{
		_xforms.DetachEntity(uid, transform, metadata, parentXform, terminating: true);
		foreach (EntityUid child in transform._children)
		{
			try
			{
				MetaDataComponent component = MetaQuery.GetComponent(child);
				TransformComponent component2 = TransformQuery.GetComponent(child);
				RecursiveDeleteEntity(child, component, component2, transform);
			}
			catch (Exception value)
			{
				_sawmill.Error($"Caught exception while trying to recursively delete child entity '{ToPrettyString(child)}' of '{ToPrettyString(uid, metadata)}'\n{value}");
			}
		}
		if (transform._children.Count != 0)
		{
			_sawmill.Error($"Failed to delete all children of entity: {ToPrettyString(uid)}");
		}
		foreach (IComponent item in InSafeOrder(_entCompIndex[uid]))
		{
			if (item.Running)
			{
				try
				{
					LifeShutdown(uid, item, _componentFactory.GetIndex(item.GetType()));
				}
				catch (Exception value2)
				{
					_sawmill.Error($"Caught exception while trying to call shutdown on component of entity '{ToPrettyString(uid, metadata)}'\n{value2}");
				}
			}
		}
		DisposeComponents(uid, metadata);
		SetLifeStage(metadata, EntityLifeStage.Deleted);
		try
		{
			this.EntityDeleted?.Invoke((Owner: uid, Comp: metadata));
		}
		catch (Exception value3)
		{
			_sawmill.Error($"Caught exception while invoking event {"EntityDeleted"} on '{ToPrettyString(uid, metadata)}'\n{value3}");
		}
		EventBusInternal.OnEntityDeleted(uid);
		Entities.Remove(uid);
		NetEntityLookup.Remove(metadata.NetEntity);
	}

	public virtual void QueueDeleteEntity(EntityUid? uid)
	{
		if (uid.HasValue && QueuedDeletionsSet.Add(uid.Value))
		{
			QueuedDeletions.Enqueue(uid.Value);
			this.EntityQueueDeleted?.Invoke(uid.Value);
		}
	}

	public virtual bool IsQueuedForDeletion(EntityUid uid)
	{
		return QueuedDeletionsSet.Contains(uid);
	}

	public virtual void PredictedDeleteEntity(Entity<MetaDataComponent?, TransformComponent?> ent)
	{
		DeleteEntity(ent.Owner);
	}

	public void PredictedDeleteEntity(Entity<MetaDataComponent?, TransformComponent?>? ent)
	{
		if (ent.HasValue)
		{
			PredictedDeleteEntity(ent.Value);
		}
	}

	public virtual void PredictedQueueDeleteEntity(Entity<MetaDataComponent?> ent)
	{
		QueueDeleteEntity(ent);
	}

	public void PredictedQueueDeleteEntity(Entity<MetaDataComponent?>? ent)
	{
		if (ent.HasValue)
		{
			PredictedQueueDeleteEntity(ent.Value);
		}
	}

	[Obsolete("use variant without TransformComponent")]
	public void PredictedQueueDeleteEntity(Entity<MetaDataComponent?, TransformComponent?> ent)
	{
		PredictedQueueDeleteEntity(new Entity<MetaDataComponent>(ent.Owner, ent.Comp1));
	}

	[Obsolete("use variant without TransformComponent")]
	public void PredictedQueueDeleteEntity(Entity<MetaDataComponent?, TransformComponent?>? ent)
	{
		if (ent.HasValue)
		{
			PredictedQueueDeleteEntity(new Entity<MetaDataComponent>(ent.Value.Owner, ent.Value.Comp1));
		}
	}

	public void PredictedQueueDeleteEntity(EntityUid uid)
	{
		PredictedQueueDeleteEntity(new Entity<MetaDataComponent>(uid, null));
	}

	public void PredictedQueueDeleteEntity(EntityUid? uid)
	{
		if (uid.HasValue)
		{
			PredictedQueueDeleteEntity(new Entity<MetaDataComponent>(uid.Value, null));
		}
	}

	public bool EntityExists(EntityUid uid)
	{
		return MetaQuery.HasComponentInternal(uid);
	}

	public bool EntityExists(EntityUid? uid)
	{
		if (uid.HasValue)
		{
			return EntityExists(uid.Value);
		}
		return false;
	}

	public bool IsPaused(EntityUid? uid, MetaDataComponent? metadata = null)
	{
		if (!uid.HasValue)
		{
			return false;
		}
		if (MetaQuery.Resolve(uid.Value, ref metadata))
		{
			return metadata.EntityPaused;
		}
		return false;
	}

	public bool Deleted(EntityUid uid)
	{
		if (MetaQuery.TryGetComponentInternal(uid, out MetaDataComponent component))
		{
			return component.EntityDeleted;
		}
		return true;
	}

	public bool Deleted([NotNullWhen(false)] EntityUid? uid)
	{
		if (uid.HasValue && MetaQuery.TryGetComponentInternal(uid.Value, out MetaDataComponent component))
		{
			return component.EntityDeleted;
		}
		return true;
	}

	public virtual void FlushEntities()
	{
		_sawmill.Info($"Flushing entities. Entity count: {Entities.Count}");
		this.BeforeEntityFlush?.Invoke();
		FlushEntitiesInternal();
		if (Entities.Count != 0)
		{
			_sawmill.Error($"Failed to flush all entities. Entity count: {Entities.Count}");
			if (Entities.Count < 512)
			{
				foreach (EntityUid entity in Entities)
				{
					_sawmill.Error($"Entity exists after flush: {ToPrettyString(entity)}");
				}
			}
		}
		FlushEntitiesInternal();
		if (Entities.Count != 0)
		{
			throw new Exception($"Failed to flush all entities. Entity count: {Entities.Count}");
		}
		this.AfterEntityFlush?.Invoke();
	}

	private void FlushEntitiesInternal()
	{
		QueuedDeletions.Clear();
		QueuedDeletionsSet.Clear();
		EntityUid[] array = _entTraitDict[typeof(MapComponent)].Keys.ToArray();
		foreach (EntityUid entityUid in array)
		{
			try
			{
				DeleteEntity(entityUid);
			}
			catch (Exception exception)
			{
				_sawmill.Log(LogLevel.Error, exception, $"Caught exception while trying to delete map entity {ToPrettyString(entityUid)}, this might corrupt the game state...");
			}
		}
		KeyValuePair<EntityUid, IComponent>[] array2 = _entTraitDict[typeof(MetaDataComponent)].ToArray();
		foreach (KeyValuePair<EntityUid, IComponent> keyValuePair in array2)
		{
			keyValuePair.Deconstruct(out var key, out var value);
			EntityUid entityUid2 = key;
			MetaDataComponent metaDataComponent = (MetaDataComponent)value;
			if ((int)metaDataComponent.EntityLifeStage < 4)
			{
				try
				{
					DeleteEntity(entityUid2, metaDataComponent, TransformQuery.GetComponent(entityUid2));
				}
				catch (Exception exception2)
				{
					_sawmill.Log(LogLevel.Error, exception2, $"Caught exception while trying to delete entity {ToPrettyString(entityUid2, metaDataComponent)}, this might corrupt the game state...");
				}
			}
		}
	}

	protected internal EntityUid AllocEntity(EntityPrototype? prototype, out MetaDataComponent metadata)
	{
		EntityUid entityUid = AllocEntity(out metadata);
		metadata._entityPrototype = prototype;
		Dirty(entityUid, metadata, metadata);
		return entityUid;
	}

	internal EntityUid AllocEntity(EntityPrototype? prototype)
	{
		MetaDataComponent metadata;
		return AllocEntity(prototype, out metadata);
	}

	private EntityUid AllocEntity(out MetaDataComponent metadata)
	{
		EntityUid entityUid = GenerateEntityUid();
		metadata = new MetaDataComponent
		{
			Owner = entityUid,
			EntityLastModifiedTick = _gameTiming.CurTick
		};
		NetEntity netEntity = GenerateNetEntity();
		SetNetEntity(entityUid, netEntity, metadata);
		this.EntityAdded?.Invoke((Owner: entityUid, Comp: metadata));
		EventBusInternal.OnEntityAdded(entityUid);
		Entities.Add(entityUid);
		AddComponentInternal(entityUid, metadata, _metaReg, overwrite: false, skipInit: true, metadata);
		TransformComponent transformComponent = Unsafe.As<TransformComponent>(_componentFactory.GetComponent(_xformReg));
		transformComponent.Owner = entityUid;
		AddComponentInternal(entityUid, transformComponent, overwrite: false, skipInit: true, metadata);
		return entityUid;
	}

	internal virtual EntityUid CreateEntity(string? prototypeName, out MetaDataComponent metadata, IEntityLoadContext? context = null)
	{
		if (prototypeName == null)
		{
			return AllocEntity(out metadata);
		}
		if (!PrototypeManager.TryIndex(prototypeName, out EntityPrototype prototype))
		{
			throw new EntityCreationException("Attempted to spawn an entity with an invalid prototype: " + prototypeName);
		}
		return CreateEntity(prototype, out metadata, context);
	}

	private protected EntityUid CreateEntity(EntityPrototype prototype, out MetaDataComponent metadata, IEntityLoadContext? context = null)
	{
		EntityUid entityUid = AllocEntity(prototype, out metadata);
		try
		{
			EntityPrototype.LoadEntity((Owner: entityUid, Comp: metadata), ComponentFactory, this, _serManager, context);
			return entityUid;
		}
		catch (Exception inner)
		{
			DeleteEntity(entityUid);
			throw new EntityCreationException("Exception inside CreateEntity with prototype " + prototype.ID, inner);
		}
	}

	public void InitializeAndStartEntity(EntityUid entity, MapId? mapId = null)
	{
		bool doMapInit = _mapSystem.IsInitialized(mapId ?? TransformQuery.GetComponent(entity).MapID);
		InitializeAndStartEntity(entity, doMapInit);
	}

	public void InitializeAndStartEntity(Entity<MetaDataComponent?> entity, bool doMapInit)
	{
		if (!MetaQuery.Resolve(entity.Owner, ref entity.Comp))
		{
			return;
		}
		try
		{
			InitializeEntity(entity.Owner, entity.Comp);
			StartEntity(entity.Owner);
			if (doMapInit)
			{
				RunMapInit(entity.Owner, entity.Comp);
			}
		}
		catch (Exception inner)
		{
			DeleteEntity(entity);
			throw new EntityCreationException("Exception inside InitializeAndStartEntity", inner);
		}
	}

	public void InitializeEntity(EntityUid entity, MetaDataComponent? meta = null)
	{
		if (meta == null)
		{
			meta = GetComponent<MetaDataComponent>(entity);
		}
		InitializeComponents(entity, meta);
		this.EntityInitialized?.Invoke((Owner: entity, Comp: meta));
	}

	public void StartEntity(EntityUid entity)
	{
		StartComponents(entity);
	}

	public void RunMapInit(EntityUid entity, MetaDataComponent meta)
	{
		if (meta.EntityLifeStage != EntityLifeStage.MapInitialized)
		{
			SetLifeStage(meta, EntityLifeStage.MapInitialized);
			EventBusInternal.RaiseLocalEvent(entity, MapInitEventInstance);
		}
	}

	[return: NotNullIfNotNull("uid")]
	public EntityStringRepresentation? ToPrettyString(EntityUid? uid, MetaDataComponent? metadata = null)
	{
		if (uid.HasValue)
		{
			return ToPrettyString(uid.Value, metadata);
		}
		return null;
	}

	public EntityStringRepresentation ToPrettyString(EntityUid uid, MetaDataComponent? metadata)
	{
		return ToPrettyString((Owner: uid, Comp: metadata));
	}

	public EntityStringRepresentation ToPrettyString(Entity<MetaDataComponent?> entity)
	{
		if (entity.Comp == null && !MetaQuery.Resolve(entity.Owner, ref entity.Comp, logMissing: false))
		{
			return new EntityStringRepresentation(entity.Owner, default(NetEntity), Deleted: true);
		}
		return new EntityStringRepresentation(entity.Owner, entity.Comp, _actorQuery.CompOrNull(entity));
	}

	[return: NotNullIfNotNull("netEntity")]
	public EntityStringRepresentation? ToPrettyString(NetEntity? netEntity)
	{
		if (netEntity.HasValue)
		{
			return ToPrettyString(netEntity.Value);
		}
		return null;
	}

	public EntityStringRepresentation ToPrettyString(NetEntity netEntity)
	{
		if (!TryGetEntityData(netEntity, out EntityUid? entity, out MetaDataComponent meta))
		{
			return new EntityStringRepresentation(EntityUid.Invalid, netEntity, Deleted: true);
		}
		return ToPrettyString(entity.Value, meta);
	}

	public virtual void RaisePredictiveEvent<T>(T msg) where T : EntityEventArgs
	{
	}

	public abstract void RaiseSharedEvent<T>(T message, EntityUid? user = null) where T : EntityEventArgs;

	public abstract void RaiseSharedEvent<T>(T message, ICommonSession? user = null) where T : EntityEventArgs;

	internal EntityUid GenerateEntityUid()
	{
		return new EntityUid(NextEntityUid++);
	}

	protected virtual NetEntity GenerateNetEntity()
	{
		return new NetEntity(NextNetworkId++);
	}

	[Conditional("DEBUG")]
	protected void ThreadCheck()
	{
	}

	internal void LifeAddToEntity<T>(EntityUid uid, T component, CompIdx idx) where T : IComponent
	{
		component.LifeStage = ComponentLifeStage.Adding;
		ref T reference = ref component;
		T val = default(T);
		if (val == null)
		{
			val = reference;
			reference = ref val;
		}
		GameTick currentTick = CurrentTick;
		reference.CreationTick = currentTick;
		ref T reference2 = ref component;
		val = default(T);
		if (val == null)
		{
			val = reference2;
			reference2 = ref val;
		}
		GameTick currentTick2 = CurrentTick;
		reference2.LastModifiedTick = currentTick2;
		EventBusInternal.RaiseComponentEvent(uid, component, idx, CompAddInstance);
		component.LifeStage = ComponentLifeStage.Added;
	}

	internal void LifeInitialize<T>(EntityUid uid, T component, CompIdx idx) where T : IComponent
	{
		component.LifeStage = ComponentLifeStage.Initializing;
		EventBusInternal.RaiseComponentEvent(uid, component, idx, CompInitInstance);
		component.LifeStage = ComponentLifeStage.Initialized;
	}

	internal void LifeStartup<T>(EntityUid uid, T component, CompIdx idx) where T : IComponent
	{
		component.LifeStage = ComponentLifeStage.Starting;
		EventBusInternal.RaiseComponentEvent(uid, component, idx, CompStartupInstance);
		component.LifeStage = ComponentLifeStage.Running;
	}

	internal void LifeShutdown<T>(EntityUid uid, T component, CompIdx idx) where T : IComponent
	{
		if ((int)component.LifeStage <= 4)
		{
			component.LifeStage = ComponentLifeStage.Stopped;
			return;
		}
		component.LifeStage = ComponentLifeStage.Stopping;
		EventBusInternal.RaiseComponentEvent(uid, component, idx, CompShutdownInstance);
		component.LifeStage = ComponentLifeStage.Stopped;
	}

	internal void LifeRemoveFromEntity<T>(EntityUid uid, T component, CompIdx idx) where T : IComponent
	{
		component.LifeStage = ComponentLifeStage.Removing;
		EventBusInternal.RaiseComponentEvent(uid, component, idx, CompRemoveInstance);
		component.LifeStage = ComponentLifeStage.Deleted;
	}

	internal virtual void SetLifeStage(MetaDataComponent meta, EntityLifeStage stage)
	{
		meta.EntityLifeStage = stage;
	}

	internal void ClearNetEntity(NetEntity netEntity)
	{
		NetEntityLookup.Remove(netEntity);
	}

	internal void SetNetEntity(EntityUid uid, NetEntity netEntity, MetaDataComponent component)
	{
		NetEntityLookup[netEntity] = (uid, component);
		component.NetEntity = netEntity;
	}

	public virtual bool IsClientSide(EntityUid uid, MetaDataComponent? metadata = null)
	{
		return false;
	}

	public bool TryParseNetEntity(string arg, [NotNullWhen(true)] out EntityUid? entity)
	{
		if (!NetEntity.TryParse(arg.AsSpan(), out var entity2) || !TryGetEntity(entity2, out entity))
		{
			entity = null;
			return false;
		}
		return true;
	}

	public bool TryGetEntity(NetEntity nEntity, [NotNullWhen(true)] out EntityUid? entity)
	{
		if (NetEntityLookup.TryGetValue(nEntity, out (EntityUid, MetaDataComponent) value))
		{
			entity = value.Item1;
			return true;
		}
		entity = null;
		return false;
	}

	public bool TryGetEntityData(NetEntity nEntity, [NotNullWhen(true)] out EntityUid? entity, [NotNullWhen(true)] out MetaDataComponent? meta)
	{
		if (NetEntityLookup.TryGetValue(nEntity, out (EntityUid, MetaDataComponent) value))
		{
			entity = value.Item1;
			meta = value.Item2;
			return true;
		}
		entity = null;
		meta = null;
		return false;
	}

	public bool TryGetEntity(NetEntity? nEntity, [NotNullWhen(true)] out EntityUid? entity)
	{
		if (!nEntity.HasValue)
		{
			entity = null;
			return false;
		}
		return TryGetEntity(nEntity.Value, out entity);
	}

	public bool TryGetNetEntity(EntityUid uid, [NotNullWhen(true)] out NetEntity? netEntity, MetaDataComponent? metadata = null)
	{
		if (uid == EntityUid.Invalid)
		{
			netEntity = null;
			return false;
		}
		if (MetaQuery.Resolve(uid, ref metadata, logMissing: false))
		{
			netEntity = metadata.NetEntity;
			return true;
		}
		netEntity = NetEntity.Invalid;
		return false;
	}

	public bool TryGetNetEntity(EntityUid? uid, [NotNullWhen(true)] out NetEntity? netEntity, MetaDataComponent? metadata = null)
	{
		if (!uid.HasValue)
		{
			netEntity = null;
			return false;
		}
		return TryGetNetEntity(uid.Value, out netEntity, metadata);
	}

	public virtual EntityUid EnsureEntity<T>(NetEntity nEntity, EntityUid callerEntity)
	{
		return GetEntity(nEntity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public EntityUid? EnsureEntity<T>(NetEntity? nEntity, EntityUid callerEntity)
	{
		if (!nEntity.HasValue)
		{
			return null;
		}
		return EnsureEntity<T>(nEntity.Value, callerEntity);
	}

	public EntityUid GetEntity(NetEntity nEntity)
	{
		if (nEntity == NetEntity.Invalid)
		{
			return EntityUid.Invalid;
		}
		if (!NetEntityLookup.TryGetValue(nEntity, out (EntityUid, MetaDataComponent) value))
		{
			return EntityUid.Invalid;
		}
		return value.Item1;
	}

	public (EntityUid, MetaDataComponent) GetEntityData(NetEntity nEntity)
	{
		return NetEntityLookup[nEntity];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public EntityUid? GetEntity(NetEntity? nEntity)
	{
		if (!nEntity.HasValue)
		{
			return null;
		}
		return GetEntity(nEntity.Value);
	}

	public NetEntity GetNetEntity(EntityUid uid, MetaDataComponent? metadata = null)
	{
		if (uid == EntityUid.Invalid)
		{
			return NetEntity.Invalid;
		}
		if (!MetaQuery.Resolve(uid, ref metadata))
		{
			return NetEntity.Invalid;
		}
		return metadata.NetEntity;
	}

	public NetEntity? GetNetEntity(EntityUid? uid, MetaDataComponent? metadata = null)
	{
		if (!uid.HasValue)
		{
			return null;
		}
		return GetNetEntity(uid.Value, metadata);
	}

	public NetCoordinates GetNetCoordinates(EntityCoordinates coordinates, MetaDataComponent? metadata = null)
	{
		return new NetCoordinates(GetNetEntity(coordinates.EntityId, metadata), coordinates.Position);
	}

	public NetCoordinates? GetNetCoordinates(EntityCoordinates? coordinates, MetaDataComponent? metadata = null)
	{
		if (!coordinates.HasValue)
		{
			return null;
		}
		return new NetCoordinates(GetNetEntity(coordinates.Value.EntityId, metadata), coordinates.Value.Position);
	}

	public EntityCoordinates GetCoordinates(NetCoordinates coordinates)
	{
		return new EntityCoordinates(GetEntity(coordinates.NetEntity), coordinates.Position);
	}

	public EntityCoordinates? GetCoordinates(NetCoordinates? coordinates)
	{
		if (!coordinates.HasValue)
		{
			return null;
		}
		return new EntityCoordinates(GetEntity(coordinates.Value.NetEntity), coordinates.Value.Position);
	}

	public virtual EntityCoordinates EnsureCoordinates<T>(NetCoordinates netCoordinates, EntityUid callerEntity)
	{
		return GetCoordinates(netCoordinates);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public EntityCoordinates? EnsureCoordinates<T>(NetCoordinates? netCoordinates, EntityUid callerEntity)
	{
		if (!netCoordinates.HasValue)
		{
			return null;
		}
		return EnsureCoordinates<T>(netCoordinates.Value, callerEntity);
	}

	public HashSet<EntityUid> GetEntitySet(HashSet<NetEntity> netEntities)
	{
		HashSet<EntityUid> hashSet = new HashSet<EntityUid>(netEntities.Count);
		foreach (NetEntity netEntity in netEntities)
		{
			hashSet.Add(GetEntity(netEntity));
		}
		return hashSet;
	}

	public List<EntityUid> GetEntityList(List<NetEntity> netEntities)
	{
		List<EntityUid> list = new List<EntityUid>(netEntities.Count);
		foreach (NetEntity netEntity in netEntities)
		{
			list.Add(GetEntity(netEntity));
		}
		return list;
	}

	public Dictionary<EntityUid, T> GetEntityDictionary<T>(Dictionary<NetEntity, T> netEntities)
	{
		Dictionary<EntityUid, T> dictionary = new Dictionary<EntityUid, T>(netEntities.Count);
		foreach (KeyValuePair<NetEntity, T> netEntity in netEntities)
		{
			dictionary.Add(GetEntity(netEntity.Key), netEntity.Value);
		}
		return dictionary;
	}

	public Dictionary<T, EntityUid> GetEntityDictionary<T>(Dictionary<T, NetEntity> netEntities) where T : notnull
	{
		Dictionary<T, EntityUid> dictionary = new Dictionary<T, EntityUid>(netEntities.Count);
		foreach (KeyValuePair<T, NetEntity> netEntity in netEntities)
		{
			dictionary.Add(netEntity.Key, GetEntity(netEntity.Value));
		}
		return dictionary;
	}

	public Dictionary<T, EntityUid?> GetEntityDictionary<T>(Dictionary<T, NetEntity?> netEntities) where T : notnull
	{
		Dictionary<T, EntityUid?> dictionary = new Dictionary<T, EntityUid?>(netEntities.Count);
		foreach (KeyValuePair<T, NetEntity?> netEntity in netEntities)
		{
			dictionary.Add(netEntity.Key, GetEntity(netEntity.Value));
		}
		return dictionary;
	}

	public Dictionary<EntityUid, EntityUid> GetEntityDictionary(Dictionary<NetEntity, NetEntity> netEntities)
	{
		Dictionary<EntityUid, EntityUid> dictionary = new Dictionary<EntityUid, EntityUid>(netEntities.Count);
		foreach (KeyValuePair<NetEntity, NetEntity> netEntity in netEntities)
		{
			dictionary.Add(GetEntity(netEntity.Key), GetEntity(netEntity.Value));
		}
		return dictionary;
	}

	public Dictionary<EntityUid, EntityUid?> GetEntityDictionary(Dictionary<NetEntity, NetEntity?> netEntities)
	{
		Dictionary<EntityUid, EntityUid?> dictionary = new Dictionary<EntityUid, EntityUid?>(netEntities.Count);
		foreach (KeyValuePair<NetEntity, NetEntity?> netEntity in netEntities)
		{
			dictionary.Add(GetEntity(netEntity.Key), GetEntity(netEntity.Value));
		}
		return dictionary;
	}

	public HashSet<EntityUid> EnsureEntitySet<T>(HashSet<NetEntity> netEntities, EntityUid callerEntity)
	{
		HashSet<EntityUid> hashSet = new HashSet<EntityUid>(netEntities.Count);
		foreach (NetEntity netEntity in netEntities)
		{
			hashSet.Add(EnsureEntity<T>(netEntity, callerEntity));
		}
		return hashSet;
	}

	public void EnsureEntitySet<T>(HashSet<NetEntity> netEntities, EntityUid callerEntity, HashSet<EntityUid> entities)
	{
		entities.Clear();
		entities.EnsureCapacity(netEntities.Count);
		foreach (NetEntity netEntity in netEntities)
		{
			entities.Add(EnsureEntity<T>(netEntity, callerEntity));
		}
	}

	public List<EntityUid> EnsureEntityList<T>(List<NetEntity> netEntities, EntityUid callerEntity)
	{
		List<EntityUid> list = new List<EntityUid>(netEntities.Count);
		foreach (NetEntity netEntity in netEntities)
		{
			list.Add(EnsureEntity<T>(netEntity, callerEntity));
		}
		return list;
	}

	public void EnsureEntityList<T>(List<NetEntity> netEntities, EntityUid callerEntity, List<EntityUid> entities)
	{
		entities.Clear();
		entities.EnsureCapacity(netEntities.Count);
		foreach (NetEntity netEntity in netEntities)
		{
			entities.Add(EnsureEntity<T>(netEntity, callerEntity));
		}
	}

	public void EnsureEntityDictionary<TComp, TValue>(Dictionary<NetEntity, TValue> netEntities, EntityUid callerEntity, Dictionary<EntityUid, TValue> entities)
	{
		entities.Clear();
		entities.EnsureCapacity(netEntities.Count);
		foreach (KeyValuePair<NetEntity, TValue> netEntity in netEntities)
		{
			entities.TryAdd(EnsureEntity<TComp>(netEntity.Key, callerEntity), netEntity.Value);
		}
	}

	public void EnsureEntityDictionaryNullableValue<TComp, TValue>(Dictionary<NetEntity, TValue?> netEntities, EntityUid callerEntity, Dictionary<EntityUid, TValue?> entities)
	{
		entities.Clear();
		entities.EnsureCapacity(netEntities.Count);
		foreach (KeyValuePair<NetEntity, TValue> netEntity in netEntities)
		{
			entities.TryAdd(EnsureEntity<TComp>(netEntity.Key, callerEntity), netEntity.Value);
		}
	}

	public void EnsureEntityDictionary<TComp, TKey>(Dictionary<TKey, NetEntity> netEntities, EntityUid callerEntity, Dictionary<TKey, EntityUid> entities) where TKey : notnull
	{
		entities.Clear();
		entities.EnsureCapacity(netEntities.Count);
		foreach (KeyValuePair<TKey, NetEntity> netEntity in netEntities)
		{
			entities.TryAdd(netEntity.Key, EnsureEntity<TComp>(netEntity.Value, callerEntity));
		}
	}

	public void EnsureEntityDictionary<TComp, TKey>(Dictionary<TKey, NetEntity?> netEntities, EntityUid callerEntity, Dictionary<TKey, EntityUid?> entities) where TKey : notnull
	{
		entities.Clear();
		entities.EnsureCapacity(netEntities.Count);
		foreach (KeyValuePair<TKey, NetEntity?> netEntity in netEntities)
		{
			entities.TryAdd(netEntity.Key, EnsureEntity<TComp>(netEntity.Value, callerEntity));
		}
	}

	public void EnsureEntityDictionary<TComp>(Dictionary<NetEntity, NetEntity> netEntities, EntityUid callerEntity, Dictionary<EntityUid, EntityUid> entities)
	{
		entities.Clear();
		entities.EnsureCapacity(netEntities.Count);
		foreach (KeyValuePair<NetEntity, NetEntity> netEntity in netEntities)
		{
			entities.TryAdd(EnsureEntity<TComp>(netEntity.Key, callerEntity), EnsureEntity<TComp>(netEntity.Value, callerEntity));
		}
	}

	public void EnsureEntityDictionary<TComp>(Dictionary<NetEntity, NetEntity?> netEntities, EntityUid callerEntity, Dictionary<EntityUid, EntityUid?> entities)
	{
		entities.Clear();
		entities.EnsureCapacity(netEntities.Count);
		foreach (KeyValuePair<NetEntity, NetEntity?> netEntity in netEntities)
		{
			entities.TryAdd(EnsureEntity<TComp>(netEntity.Key, callerEntity), EnsureEntity<TComp>(netEntity.Value, callerEntity));
		}
	}

	public List<EntityUid> GetEntityList(ICollection<NetEntity> netEntities)
	{
		List<EntityUid> list = new List<EntityUid>(netEntities.Count);
		foreach (NetEntity netEntity in netEntities)
		{
			list.Add(GetEntity(netEntity));
		}
		return list;
	}

	public List<EntityUid?> GetEntityList(List<NetEntity?> netEntities)
	{
		List<EntityUid?> list = new List<EntityUid?>(netEntities.Count);
		foreach (NetEntity? netEntity in netEntities)
		{
			list.Add(GetEntity(netEntity));
		}
		return list;
	}

	public EntityUid[] GetEntityArray(NetEntity[] netEntities)
	{
		EntityUid[] array = new EntityUid[netEntities.Length];
		for (int i = 0; i < netEntities.Length; i++)
		{
			array[i] = GetEntity(netEntities[i]);
		}
		return array;
	}

	public EntityUid?[] GetEntityArray(NetEntity?[] netEntities)
	{
		EntityUid?[] array = new EntityUid?[netEntities.Length];
		for (int i = 0; i < netEntities.Length; i++)
		{
			array[i] = GetEntity(netEntities[i]);
		}
		return array;
	}

	public HashSet<NetEntity> GetNetEntitySet(HashSet<EntityUid> entities)
	{
		HashSet<NetEntity> hashSet = new HashSet<NetEntity>(entities.Count);
		foreach (EntityUid entity in entities)
		{
			MetaQuery.TryGetComponent(entity, out MetaDataComponent component);
			hashSet.Add(GetNetEntity(entity, component));
		}
		return hashSet;
	}

	public List<NetEntity> GetNetEntityList(List<EntityUid> entities)
	{
		List<NetEntity> list = new List<NetEntity>(entities.Count);
		foreach (EntityUid entity in entities)
		{
			list.Add(GetNetEntity(entity));
		}
		return list;
	}

	public List<NetEntity> GetNetEntityList(IReadOnlyList<EntityUid> entities)
	{
		List<NetEntity> list = new List<NetEntity>(entities.Count);
		foreach (EntityUid entity in entities)
		{
			list.Add(GetNetEntity(entity));
		}
		return list;
	}

	public List<NetEntity> GetNetEntityList(ICollection<EntityUid> entities)
	{
		List<NetEntity> list = new List<NetEntity>(entities.Count);
		foreach (EntityUid entity in entities)
		{
			list.Add(GetNetEntity(entity));
		}
		return list;
	}

	public List<NetEntity?> GetNetEntityList(List<EntityUid?> entities)
	{
		List<NetEntity?> list = new List<NetEntity?>(entities.Count);
		foreach (EntityUid? entity in entities)
		{
			list.Add(GetNetEntity(entity));
		}
		return list;
	}

	public NetEntity[] GetNetEntityArray(EntityUid[] entities)
	{
		NetEntity[] array = new NetEntity[entities.Length];
		for (int i = 0; i < entities.Length; i++)
		{
			array[i] = GetNetEntity(entities[i]);
		}
		return array;
	}

	public NetEntity?[] GetNetEntityArray(EntityUid?[] entities)
	{
		NetEntity?[] array = new NetEntity?[entities.Length];
		for (int i = 0; i < entities.Length; i++)
		{
			array[i] = GetNetEntity(entities[i]);
		}
		return array;
	}

	public Dictionary<NetEntity, T> GetNetEntityDictionary<T>(Dictionary<EntityUid, T> entities)
	{
		Dictionary<NetEntity, T> dictionary = new Dictionary<NetEntity, T>(entities.Count);
		foreach (KeyValuePair<EntityUid, T> entity in entities)
		{
			dictionary.Add(GetNetEntity(entity.Key), entity.Value);
		}
		return dictionary;
	}

	public Dictionary<T, NetEntity> GetNetEntityDictionary<T>(Dictionary<T, EntityUid> entities) where T : notnull
	{
		Dictionary<T, NetEntity> dictionary = new Dictionary<T, NetEntity>(entities.Count);
		foreach (KeyValuePair<T, EntityUid> entity in entities)
		{
			dictionary.Add(entity.Key, GetNetEntity(entity.Value));
		}
		return dictionary;
	}

	public Dictionary<T, NetEntity?> GetNetEntityDictionary<T>(Dictionary<T, EntityUid?> entities) where T : notnull
	{
		Dictionary<T, NetEntity?> dictionary = new Dictionary<T, NetEntity?>(entities.Count);
		foreach (KeyValuePair<T, EntityUid?> entity in entities)
		{
			dictionary.Add(entity.Key, GetNetEntity(entity.Value));
		}
		return dictionary;
	}

	public Dictionary<NetEntity, NetEntity> GetNetEntityDictionary(Dictionary<EntityUid, EntityUid> entities)
	{
		Dictionary<NetEntity, NetEntity> dictionary = new Dictionary<NetEntity, NetEntity>(entities.Count);
		foreach (KeyValuePair<EntityUid, EntityUid> entity in entities)
		{
			dictionary.Add(GetNetEntity(entity.Key), GetNetEntity(entity.Value));
		}
		return dictionary;
	}

	public Dictionary<NetEntity, NetEntity?> GetNetEntityDictionary(Dictionary<EntityUid, EntityUid?> entities)
	{
		Dictionary<NetEntity, NetEntity?> dictionary = new Dictionary<NetEntity, NetEntity?>(entities.Count);
		foreach (KeyValuePair<EntityUid, EntityUid?> entity in entities)
		{
			dictionary.Add(GetNetEntity(entity.Key), GetNetEntity(entity.Value));
		}
		return dictionary;
	}

	public HashSet<EntityCoordinates> GetEntitySet(HashSet<NetCoordinates> netEntities)
	{
		HashSet<EntityCoordinates> hashSet = new HashSet<EntityCoordinates>(netEntities.Count);
		foreach (NetCoordinates netEntity in netEntities)
		{
			hashSet.Add(GetCoordinates(netEntity));
		}
		return hashSet;
	}

	public List<EntityCoordinates> GetEntityList(List<NetCoordinates> netEntities)
	{
		List<EntityCoordinates> list = new List<EntityCoordinates>(netEntities.Count);
		foreach (NetCoordinates netEntity in netEntities)
		{
			list.Add(GetCoordinates(netEntity));
		}
		return list;
	}

	public List<EntityCoordinates> GetEntityList(ICollection<NetCoordinates> netEntities)
	{
		List<EntityCoordinates> list = new List<EntityCoordinates>(netEntities.Count);
		foreach (NetCoordinates netEntity in netEntities)
		{
			list.Add(GetCoordinates(netEntity));
		}
		return list;
	}

	public List<EntityCoordinates?> GetEntityList(List<NetCoordinates?> netEntities)
	{
		List<EntityCoordinates?> list = new List<EntityCoordinates?>(netEntities.Count);
		foreach (NetCoordinates? netEntity in netEntities)
		{
			list.Add(GetCoordinates(netEntity));
		}
		return list;
	}

	public EntityCoordinates[] GetEntityArray(NetCoordinates[] netEntities)
	{
		EntityCoordinates[] array = new EntityCoordinates[netEntities.Length];
		for (int i = 0; i < netEntities.Length; i++)
		{
			array[i] = GetCoordinates(netEntities[i]);
		}
		return array;
	}

	public EntityCoordinates?[] GetEntityArray(NetCoordinates?[] netEntities)
	{
		EntityCoordinates?[] array = new EntityCoordinates?[netEntities.Length];
		for (int i = 0; i < netEntities.Length; i++)
		{
			array[i] = GetCoordinates(netEntities[i]);
		}
		return array;
	}

	public HashSet<NetCoordinates> GetNetCoordinatesSet(HashSet<EntityCoordinates> entities)
	{
		HashSet<NetCoordinates> hashSet = new HashSet<NetCoordinates>(entities.Count);
		foreach (EntityCoordinates entity in entities)
		{
			hashSet.Add(GetNetCoordinates(entity));
		}
		return hashSet;
	}

	public List<NetCoordinates> GetNetCoordinatesList(List<EntityCoordinates> entities)
	{
		List<NetCoordinates> list = new List<NetCoordinates>(entities.Count);
		foreach (EntityCoordinates entity in entities)
		{
			list.Add(GetNetCoordinates(entity));
		}
		return list;
	}

	public List<NetCoordinates> GetNetCoordinatesList(ICollection<EntityCoordinates> entities)
	{
		List<NetCoordinates> list = new List<NetCoordinates>(entities.Count);
		foreach (EntityCoordinates entity in entities)
		{
			list.Add(GetNetCoordinates(entity));
		}
		return list;
	}

	public List<NetCoordinates?> GetNetCoordinatesList(List<EntityCoordinates?> entities)
	{
		List<NetCoordinates?> list = new List<NetCoordinates?>(entities.Count);
		foreach (EntityCoordinates? entity in entities)
		{
			list.Add(GetNetCoordinates(entity));
		}
		return list;
	}

	public NetCoordinates[] GetNetCoordinatesArray(EntityCoordinates[] entities)
	{
		NetCoordinates[] array = new NetCoordinates[entities.Length];
		for (int i = 0; i < entities.Length; i++)
		{
			array[i] = GetNetCoordinates(entities[i]);
		}
		return array;
	}

	public NetCoordinates?[] GetNetCoordinatesArray(EntityCoordinates?[] entities)
	{
		NetCoordinates?[] array = new NetCoordinates?[entities.Length];
		for (int i = 0; i < entities.Length; i++)
		{
			array[i] = GetNetCoordinates(entities[i]);
		}
		return array;
	}

	public EntityUid SpawnEntity(string? protoName, EntityCoordinates coordinates, ComponentRegistry? overrides = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return SpawnAttachedTo(protoName, coordinates, overrides);
	}

	public EntityUid SpawnEntity(string? protoName, MapCoordinates coordinates, ComponentRegistry? overrides = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Spawn(protoName, coordinates, overrides);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public EntityUid[] SpawnEntitiesAttachedTo(EntityCoordinates coordinates, params string?[] protoNames)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid[] array = new EntityUid[protoNames.Length];
		for (int i = 0; i < protoNames.Length; i++)
		{
			array[i] = SpawnAttachedTo(protoNames[i], coordinates);
		}
		return array;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public EntityUid[] SpawnEntities(MapCoordinates coordinates, params string?[] protoNames)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid[] array = new EntityUid[protoNames.Length];
		for (int i = 0; i < protoNames.Length; i++)
		{
			array[i] = Spawn(protoNames[i], coordinates);
		}
		return array;
	}

	public EntityUid[] SpawnEntities(MapCoordinates coordinates, string? prototype, int count)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		EntityUid[] array = new EntityUid[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = Spawn(prototype, coordinates);
		}
		return array;
	}

	public EntityUid[] SpawnEntitiesAttachedTo(EntityCoordinates coordinates, params EntProtoId[] protoNames)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityUid[] array = new EntityUid[protoNames.Length];
		for (int i = 0; i < protoNames.Length; i++)
		{
			array[i] = SpawnAttachedTo(protoNames[i], coordinates);
		}
		return array;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public EntityUid[] SpawnEntitiesAttachedTo(EntityCoordinates coordinates, List<string?> protoNames)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		EntityUid[] array = new EntityUid[protoNames.Count];
		for (int i = 0; i < protoNames.Count; i++)
		{
			array[i] = SpawnAttachedTo(protoNames[i], coordinates);
		}
		return array;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public EntityUid[] SpawnEntities(MapCoordinates coordinates, List<string?> protoNames)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		EntityUid[] array = new EntityUid[protoNames.Count];
		for (int i = 0; i < protoNames.Count; i++)
		{
			array[i] = Spawn(protoNames[i], coordinates);
		}
		return array;
	}

	public EntityUid[] SpawnEntitiesAttachedTo(EntityCoordinates coordinates, IEnumerable<EntProtoId> protoNames)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		ValueList<EntityUid> valueList = default(ValueList<EntityUid>);
		foreach (EntProtoId protoName in protoNames)
		{
			EntityUid item = SpawnAttachedTo(protoName, coordinates);
			valueList.Add(item);
		}
		return valueList.ToArray();
	}

	public virtual EntityUid SpawnAttachedTo(string? protoName, EntityCoordinates coordinates, ComponentRegistry? overrides = null, Angle rotation = default(Angle))
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!coordinates.IsValid(this))
		{
			throw new InvalidOperationException($"Tried to spawn entity {protoName} on invalid coordinates {coordinates}.");
		}
		EntityUid entityUid = CreateEntityUninitialized(protoName, coordinates, overrides, rotation);
		InitializeAndStartEntity(entityUid, _xforms.GetMapId(coordinates));
		return entityUid;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public EntityUid Spawn(string? protoName = null, ComponentRegistry? overrides = null, bool doMapInit = true)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entityUid = CreateEntityUninitialized(protoName, MapCoordinates.Nullspace, overrides);
		InitializeAndStartEntity(entityUid, doMapInit);
		return entityUid;
	}

	public virtual EntityUid Spawn(string? protoName, MapCoordinates coordinates, ComponentRegistry? overrides = null, Angle rotation = default(Angle))
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entityUid = CreateEntityUninitialized(protoName, coordinates, overrides, rotation);
		InitializeAndStartEntity(entityUid, coordinates.MapId);
		return entityUid;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public EntityUid SpawnAtPosition(string? protoName, EntityCoordinates coordinates, ComponentRegistry? overrides = null)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return Spawn(protoName, _xforms.ToMapCoordinates(coordinates), overrides);
	}

	public bool TrySpawnNextTo(string? protoName, EntityUid target, [NotNullWhen(true)] out EntityUid? uid, TransformComponent? xform = null, ComponentRegistry? overrides = null)
	{
		uid = null;
		if (!TransformQuery.Resolve(target, ref xform))
		{
			return false;
		}
		if (!xform.ParentUid.IsValid())
		{
			return false;
		}
		if (!_containers.TryGetContainingContainer(target, out BaseContainer container))
		{
			uid = SpawnNextToOrDrop(protoName, target, xform, overrides);
			return true;
		}
		bool doMapInit = _mapSystem.IsInitialized(xform.MapUid);
		uid = Spawn(protoName, overrides, doMapInit);
		if (_containers.Insert(uid.Value, container))
		{
			return true;
		}
		DeleteEntity(uid.Value);
		uid = null;
		return false;
	}

	public bool TrySpawnInContainer(string? protoName, EntityUid containerUid, string containerId, [NotNullWhen(true)] out EntityUid? uid, ContainerManagerComponent? containerComp = null, ComponentRegistry? overrides = null)
	{
		uid = null;
		if (containerComp == null && !TryGetComponent<ContainerManagerComponent>(containerUid, out containerComp))
		{
			return false;
		}
		if (!containerComp.Containers.TryGetValue(containerId, out BaseContainer value))
		{
			return false;
		}
		bool doMapInit = _mapSystem.IsInitialized(TransformQuery.GetComponent(containerUid).MapUid);
		uid = Spawn(protoName, overrides, doMapInit);
		if (_containers.Insert(uid.Value, value))
		{
			return true;
		}
		DeleteEntity(uid.Value);
		uid = null;
		return false;
	}

	public EntityUid SpawnNextToOrDrop(string? protoName, EntityUid target, TransformComponent? xform = null, ComponentRegistry? overrides = null)
	{
		if (xform == null)
		{
			xform = TransformQuery.GetComponent(target);
		}
		if (!xform.ParentUid.IsValid())
		{
			return Spawn(protoName);
		}
		bool doMapInit = _mapSystem.IsInitialized(xform.MapUid);
		EntityUid entityUid = Spawn(protoName, overrides, doMapInit);
		_xforms.DropNextTo(entityUid, target);
		return entityUid;
	}

	public EntityUid SpawnInContainerOrDrop(string? protoName, EntityUid containerUid, string containerId, TransformComponent? xform = null, ContainerManagerComponent? containerComp = null, ComponentRegistry? overrides = null)
	{
		bool inserted;
		return SpawnInContainerOrDrop(protoName, containerUid, containerId, out inserted, xform, containerComp, overrides);
	}

	public EntityUid SpawnInContainerOrDrop(string? protoName, EntityUid containerUid, string containerId, out bool inserted, TransformComponent? xform = null, ContainerManagerComponent? containerComp = null, ComponentRegistry? overrides = null)
	{
		inserted = true;
		if (xform == null)
		{
			xform = TransformQuery.GetComponent(containerUid);
		}
		bool doMapInit = _mapSystem.IsInitialized(xform.MapUid);
		EntityUid entityUid = Spawn(protoName, overrides, doMapInit);
		if ((containerComp == null && !TryGetComponent<ContainerManagerComponent>(containerUid, out containerComp)) || !containerComp.Containers.TryGetValue(containerId, out BaseContainer value) || !_containers.Insert(entityUid, value))
		{
			inserted = false;
			if (xform.ParentUid.IsValid())
			{
				_xforms.DropNextTo(entityUid, (Owner: containerUid, Comp: xform));
			}
		}
		return entityUid;
	}

	public virtual EntityUid PredictedSpawnAttachedTo(string? protoName, EntityCoordinates coordinates, ComponentRegistry? overrides = null, Angle rotation = default(Angle))
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return SpawnAttachedTo(protoName, coordinates, overrides, rotation);
	}

	public virtual EntityUid PredictedSpawn(string? protoName = null, ComponentRegistry? overrides = null, bool doMapInit = true)
	{
		return Spawn(protoName, overrides, doMapInit);
	}

	public virtual EntityUid PredictedSpawn(string? protoName, MapCoordinates coordinates, ComponentRegistry? overrides = null, Angle rotation = default(Angle))
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return Spawn(protoName, coordinates, overrides, rotation);
	}

	public virtual EntityUid PredictedSpawnAtPosition(string? protoName, EntityCoordinates coordinates, ComponentRegistry? overrides = null)
	{
		return SpawnAtPosition(protoName, coordinates, overrides);
	}

	public virtual bool PredictedTrySpawnNextTo(string? protoName, EntityUid target, [NotNullWhen(true)] out EntityUid? uid, TransformComponent? xform = null, ComponentRegistry? overrides = null)
	{
		return TrySpawnNextTo(protoName, target, out uid, xform, overrides);
	}

	public virtual bool PredictedTrySpawnInContainer(string? protoName, EntityUid containerUid, string containerId, [NotNullWhen(true)] out EntityUid? uid, ContainerManagerComponent? containerComp = null, ComponentRegistry? overrides = null)
	{
		return TrySpawnInContainer(protoName, containerUid, containerId, out uid, containerComp, overrides);
	}

	public virtual EntityUid PredictedSpawnNextToOrDrop(string? protoName, EntityUid target, TransformComponent? xform = null, ComponentRegistry? overrides = null)
	{
		return SpawnNextToOrDrop(protoName, target, xform, overrides);
	}

	public virtual EntityUid PredictedSpawnInContainerOrDrop(string? protoName, EntityUid containerUid, string containerId, TransformComponent? xform = null, ContainerManagerComponent? containerComp = null, ComponentRegistry? overrides = null)
	{
		return SpawnInContainerOrDrop(protoName, containerUid, containerId, xform, containerComp, overrides);
	}

	public virtual EntityUid PredictedSpawnInContainerOrDrop(string? protoName, EntityUid containerUid, string containerId, out bool inserted, TransformComponent? xform = null, ContainerManagerComponent? containerComp = null, ComponentRegistry? overrides = null)
	{
		return SpawnInContainerOrDrop(protoName, containerUid, containerId, out inserted, xform, containerComp, overrides);
	}

	public virtual void FlagPredicted(Entity<MetaDataComponent?> ent)
	{
	}

	public T System<T>() where T : IEntitySystem
	{
		return _entitySystemManager.GetEntitySystem<T>();
	}

	public T? SystemOrNull<T>() where T : IEntitySystem
	{
		return _entitySystemManager.GetEntitySystemOrNull<T>();
	}

	public bool TrySystem<T>([NotNullWhen(true)] out T? entitySystem) where T : IEntitySystem
	{
		return _entitySystemManager.TryGetEntitySystem<T>(out entitySystem);
	}
}
