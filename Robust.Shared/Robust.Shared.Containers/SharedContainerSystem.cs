using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Robust.Shared.Containers;

public abstract class SharedContainerSystem : EntitySystem
{
	[Dependency]
	private readonly IDynamicTypeFactoryInternal _dynFactory;

	[Dependency]
	private readonly INetManager _net;

	[Dependency]
	private readonly SharedPhysicsSystem _physics;

	[Dependency]
	private readonly EntityLookupSystem _lookup;

	[Dependency]
	private readonly SharedTransformSystem _transform;

	[Dependency]
	private readonly SharedJointSystem _joint;

	[Dependency]
	private readonly IGameTiming _timing;

	private EntityQuery<ContainerManagerComponent> _managerQuery;

	private EntityQuery<MapGridComponent> _gridQuery;

	private EntityQuery<MapComponent> _mapQuery;

	protected EntityQuery<MetaDataComponent> MetaQuery;

	protected EntityQuery<PhysicsComponent> PhysicsQuery;

	protected EntityQuery<JointComponent> JointQuery;

	protected EntityQuery<TransformComponent> TransformQuery;

	public override void Initialize()
	{
		base.Initialize();
		SubscribeLocalEvent<EntParentChangedMessage>(OnParentChanged);
		SubscribeLocalEvent<ContainerManagerComponent, ComponentInit>(OnInit);
		SubscribeLocalEvent<ContainerManagerComponent, ComponentStartup>(OnStartupValidation);
		SubscribeLocalEvent<ContainerManagerComponent, ComponentGetState>(OnContainerGetState);
		SubscribeLocalEvent<ContainerManagerComponent, ComponentRemove>(OnContainerManagerRemove);
		_managerQuery = GetEntityQuery<ContainerManagerComponent>();
		_gridQuery = GetEntityQuery<MapGridComponent>();
		_mapQuery = GetEntityQuery<MapComponent>();
		MetaQuery = GetEntityQuery<MetaDataComponent>();
		PhysicsQuery = GetEntityQuery<PhysicsComponent>();
		JointQuery = GetEntityQuery<JointComponent>();
		TransformQuery = GetEntityQuery<TransformComponent>();
	}

	private void OnInit(Entity<ContainerManagerComponent> ent, ref ComponentInit args)
	{
		foreach (var (id, baseContainer2) in ent.Comp.Containers)
		{
			baseContainer2.Init(this, id, ent);
		}
	}

	private void OnContainerGetState(EntityUid uid, ContainerManagerComponent component, ref ComponentGetState args)
	{
		Dictionary<string, ContainerManagerComponent.ContainerManagerComponentState.ContainerData> dictionary = new Dictionary<string, ContainerManagerComponent.ContainerManagerComponentState.ContainerData>(component.Containers.Count);
		foreach (BaseContainer value in component.Containers.Values)
		{
			NetEntity[] array = new NetEntity[value.ContainedEntities.Count];
			for (int i = 0; i < value.ContainedEntities.Count; i++)
			{
				array[i] = GetNetEntity(value.ContainedEntities[i]);
			}
			dictionary.Add(value: new ContainerManagerComponent.ContainerManagerComponentState.ContainerData(value.GetType().Name, value.ShowContents, value.OccludesLight, array), key: value.ID);
		}
		args.State = new ContainerManagerComponent.ContainerManagerComponentState(dictionary);
	}

	private void OnContainerManagerRemove(EntityUid uid, ContainerManagerComponent component, ComponentRemove args)
	{
		foreach (BaseContainer value in component.Containers.Values)
		{
			ShutdownContainer(value);
		}
		component.Containers.Clear();
	}

	public T MakeContainer<T>(EntityUid uid, string id, ContainerManagerComponent? containerManager = null) where T : BaseContainer
	{
		if (!Resolve(uid, ref containerManager, logMissing: false))
		{
			containerManager = AddComp<ContainerManagerComponent>(uid);
		}
		if (HasContainer(uid, id, containerManager))
		{
			throw new ArgumentException("Container with specified ID already exists: '" + id + "'");
		}
		T val = _dynFactory.CreateInstanceUnchecked<T>(typeof(T), oneOff: false, inject: false);
		val.Init(this, id, (Owner: uid, Comp: containerManager));
		containerManager.Containers[id] = val;
		Dirty(uid, containerManager);
		return val;
	}

	public virtual void ShutdownContainer(BaseContainer container)
	{
		container.InternalShutdown(EntityManager, this, _net.IsClient);
		container.Manager.Containers.Remove(container.ID);
		container.ExpectedEntities.Clear();
	}

	public T EnsureContainer<T>(EntityUid uid, string id, out bool alreadyExisted, ContainerManagerComponent? containerManager = null) where T : BaseContainer
	{
		if (!Resolve(uid, ref containerManager, logMissing: false))
		{
			containerManager = AddComp<ContainerManagerComponent>(uid);
		}
		if (TryGetContainer(uid, id, out BaseContainer container, containerManager))
		{
			alreadyExisted = true;
			if (container is T result)
			{
				return result;
			}
			throw new InvalidOperationException($"The container exists but is of a different type: {container.GetType()}");
		}
		alreadyExisted = false;
		return MakeContainer<T>(uid, id, containerManager);
	}

	public T EnsureContainer<T>(EntityUid uid, string id, ContainerManagerComponent? containerManager = null) where T : BaseContainer
	{
		bool alreadyExisted;
		return EnsureContainer<T>(uid, id, out alreadyExisted, containerManager);
	}

	public BaseContainer GetContainer(EntityUid uid, string id, ContainerManagerComponent? containerManager = null)
	{
		if (!Resolve(uid, ref containerManager))
		{
			throw new ArgumentException("Entity does not have a ContainerManagerComponent!", "uid");
		}
		return containerManager.Containers[id];
	}

	public bool HasContainer(EntityUid uid, string id, ContainerManagerComponent? containerManager)
	{
		if (!Resolve(uid, ref containerManager, logMissing: false))
		{
			return false;
		}
		return containerManager.Containers.ContainsKey(id);
	}

	public bool TryGetContainer(EntityUid uid, string id, [NotNullWhen(true)] out BaseContainer? container, ContainerManagerComponent? containerManager = null)
	{
		if (!Resolve(uid, ref containerManager, logMissing: false))
		{
			container = null;
			return false;
		}
		if (!containerManager.Containers.TryGetValue(id, out container))
		{
			return false;
		}
		return true;
	}

	public bool TryGetContainingContainer(EntityUid uid, EntityUid containedUid, [NotNullWhen(true)] out BaseContainer? container, ContainerManagerComponent? containerManager = null)
	{
		if (!Resolve(uid, ref containerManager, logMissing: false))
		{
			container = null;
			return false;
		}
		foreach (BaseContainer value in containerManager.Containers.Values)
		{
			if (value.Contains(containedUid))
			{
				container = value;
				return true;
			}
		}
		container = null;
		return false;
	}

	public bool ContainsEntity(EntityUid uid, EntityUid containedUid, ContainerManagerComponent? containerManager = null)
	{
		if (!Resolve(uid, ref containerManager, logMissing: false))
		{
			return false;
		}
		foreach (BaseContainer value in containerManager.Containers.Values)
		{
			if (value.Contains(containedUid))
			{
				return true;
			}
		}
		return false;
	}

	public bool RemoveEntity(EntityUid uid, EntityUid toremove, ContainerManagerComponent? containerManager = null, TransformComponent? containedXform = null, MetaDataComponent? containedMeta = null, bool reparent = true, bool force = false, EntityCoordinates? destination = null, Angle? localRotation = null)
	{
		if (!Resolve(uid, ref containerManager) || !Resolve(toremove, ref containedMeta, ref containedXform))
		{
			return false;
		}
		foreach (BaseContainer value in containerManager.Containers.Values)
		{
			if (value.Contains(toremove))
			{
				return Remove((Owner: toremove, Comp1: containedXform, Comp2: containedMeta), value, reparent, force, destination, localRotation);
			}
		}
		return true;
	}

	public ContainerManagerComponent.AllContainersEnumerable GetAllContainers(EntityUid uid, ContainerManagerComponent? containerManager = null)
	{
		if (!Resolve(uid, ref containerManager))
		{
			return default(ContainerManagerComponent.AllContainersEnumerable);
		}
		return new ContainerManagerComponent.AllContainersEnumerable(containerManager);
	}

	public bool TryGetContainingContainer(Entity<TransformComponent?, MetaDataComponent?> ent, [NotNullWhen(true)] out BaseContainer? container)
	{
		container = null;
		if (!Resolve(ent, ref ent.Comp2, logMissing: false))
		{
			return false;
		}
		if ((ent.Comp2.Flags & MetaDataFlags.InContainer) == 0)
		{
			return false;
		}
		if (!Resolve(ent, ref ent.Comp1, logMissing: false))
		{
			return false;
		}
		return TryGetContainingContainer(ent.Comp1.ParentUid, ent, out container);
	}

	public bool IsEntityInContainer(EntityUid uid, MetaDataComponent? meta = null)
	{
		if (!Resolve(uid, ref meta, logMissing: false))
		{
			return false;
		}
		return (meta.Flags & MetaDataFlags.InContainer) == MetaDataFlags.InContainer;
	}

	public bool IsEntityOrParentInContainer(EntityUid uid, MetaDataComponent? meta = null, TransformComponent? xform = null)
	{
		if (!MetaQuery.Resolve(uid, ref meta))
		{
			return false;
		}
		if ((meta.Flags & MetaDataFlags.InContainer) == MetaDataFlags.InContainer)
		{
			return true;
		}
		if (!TransformQuery.Resolve(uid, ref xform))
		{
			return false;
		}
		if (!xform.ParentUid.Valid)
		{
			return false;
		}
		return IsEntityOrParentInContainer(xform.ParentUid);
	}

	public bool TryFindComponentOnEntityContainerOrParent<T>(EntityUid uid, EntityQuery<T> entityQuery, [NotNullWhen(true)] ref T? foundComponent, MetaDataComponent? meta = null, TransformComponent? xform = null) where T : IComponent
	{
		if (!MetaQuery.Resolve(uid, ref meta))
		{
			return false;
		}
		if ((meta.Flags & MetaDataFlags.InContainer) != MetaDataFlags.InContainer)
		{
			return false;
		}
		if (!TransformQuery.Resolve(uid, ref xform))
		{
			return false;
		}
		if (!xform.ParentUid.Valid)
		{
			return false;
		}
		if (entityQuery.TryComp(xform.ParentUid, out foundComponent))
		{
			return true;
		}
		return TryFindComponentOnEntityContainerOrParent(xform.ParentUid, entityQuery, ref foundComponent);
	}

	public bool TryFindComponentsOnEntityContainerOrParent<T>(EntityUid uid, EntityQuery<T> entityQuery, List<T> foundComponents, MetaDataComponent? meta = null, TransformComponent? xform = null) where T : IComponent
	{
		if (!MetaQuery.Resolve(uid, ref meta))
		{
			return foundComponents.Any();
		}
		if ((meta.Flags & MetaDataFlags.InContainer) != MetaDataFlags.InContainer)
		{
			return foundComponents.Any();
		}
		if (!TransformQuery.Resolve(uid, ref xform))
		{
			return foundComponents.Any();
		}
		if (!xform.ParentUid.Valid)
		{
			return foundComponents.Any();
		}
		if (TryComp(xform.ParentUid, out T comp))
		{
			foundComponents.Add(comp);
		}
		return TryFindComponentsOnEntityContainerOrParent(xform.ParentUid, entityQuery, foundComponents);
	}

	public bool IsInSameOrNoContainer(Entity<TransformComponent?, MetaDataComponent?> user, Entity<TransformComponent?, MetaDataComponent?> other)
	{
		BaseContainer container;
		bool flag = TryGetContainingContainer(user, out container);
		BaseContainer container2;
		bool flag2 = TryGetContainingContainer(other, out container2);
		if (!flag && !flag2)
		{
			return true;
		}
		if (flag != flag2)
		{
			return false;
		}
		return container == container2;
	}

	public bool IsInSameOrParentContainer(Entity<TransformComponent?, MetaDataComponent?> user, Entity<TransformComponent?, MetaDataComponent?> other)
	{
		BaseContainer userContainer;
		BaseContainer otherContainer;
		return IsInSameOrParentContainer(user, other, out userContainer, out otherContainer);
	}

	public bool IsInSameOrParentContainer(Entity<TransformComponent?, MetaDataComponent?> user, Entity<TransformComponent?, MetaDataComponent?> other, out BaseContainer? userContainer, out BaseContainer? otherContainer)
	{
		bool flag = TryGetContainingContainer(user, out userContainer);
		bool flag2 = TryGetContainingContainer(other, out otherContainer);
		if (!flag && !flag2)
		{
			return true;
		}
		if (userContainer?.Owner == other || otherContainer?.Owner == user)
		{
			return true;
		}
		if (flag != flag2)
		{
			return false;
		}
		return userContainer == otherContainer;
	}

	public bool IsInSameOrTransparentContainer(Entity<TransformComponent?, MetaDataComponent?> user, Entity<TransformComponent?, MetaDataComponent?> other, BaseContainer? userContainer = null, BaseContainer? otherContainer = null, bool userSeeInsideSelf = false)
	{
		if (userContainer == null)
		{
			TryGetContainingContainer(user, out userContainer);
		}
		if (otherContainer == null)
		{
			TryGetContainingContainer(other, out otherContainer);
		}
		if (userContainer == otherContainer)
		{
			return true;
		}
		if (userContainer?.Owner == other)
		{
			return true;
		}
		if (userSeeInsideSelf && otherContainer?.Owner == user)
		{
			return true;
		}
		if (userContainer != null && userContainer.ShowContents)
		{
			return IsInSameOrTransparentContainer((Owner: userContainer.Owner, Comp1: (TransformComponent)null, Comp2: (MetaDataComponent)null), other, null, otherContainer);
		}
		if (otherContainer != null && otherContainer.ShowContents)
		{
			return IsInSameOrTransparentContainer(user, (Owner: otherContainer.Owner, Comp1: (TransformComponent)null, Comp2: (MetaDataComponent)null), userContainer, null, userSeeInsideSelf);
		}
		return false;
	}

	public IEnumerable<BaseContainer> GetContainingContainers(Entity<TransformComponent?> ent)
	{
		if (!ent.Owner.IsValid() || !Resolve(ent, ref ent.Comp))
		{
			yield break;
		}
		EntityUid entityUid = ent.Owner;
		EntityUid parent = ent.Comp.ParentUid;
		while (parent.IsValid())
		{
			if ((MetaQuery.GetComponent(entityUid).Flags & MetaDataFlags.InContainer) == MetaDataFlags.InContainer && _managerQuery.TryGetComponent(parent, out ContainerManagerComponent component) && TryGetContainingContainer(parent, entityUid, out BaseContainer container, component))
			{
				yield return container;
			}
			TransformComponent component2 = TransformQuery.GetComponent(parent);
			entityUid = parent;
			parent = component2.ParentUid;
		}
	}

	public bool TryGetOuterContainer(EntityUid uid, TransformComponent xform, [NotNullWhen(true)] out BaseContainer? container)
	{
		return TryGetOuterContainer(uid, xform, out container, TransformQuery);
	}

	public bool TryGetOuterContainer(EntityUid uid, TransformComponent xform, [NotNullWhen(true)] out BaseContainer? container, EntityQuery<TransformComponent> xformQuery)
	{
		container = null;
		if (!uid.IsValid())
		{
			return false;
		}
		EntityUid entityUid = uid;
		EntityUid parentUid = xform.ParentUid;
		while (parentUid.IsValid())
		{
			if ((MetaQuery.GetComponent(entityUid).Flags & MetaDataFlags.InContainer) == MetaDataFlags.InContainer && _managerQuery.TryGetComponent(parentUid, out ContainerManagerComponent component) && TryGetContainingContainer(parentUid, entityUid, out BaseContainer container2, component))
			{
				container = container2;
			}
			TransformComponent component2 = xformQuery.GetComponent(parentUid);
			entityUid = parentUid;
			parentUid = component2.ParentUid;
		}
		return container != null;
	}

	public bool TryRemoveFromContainer(Entity<TransformComponent?, MetaDataComponent?> entity, bool force, out bool wasInContainer)
	{
		if (TryGetContainingContainer(entity, out BaseContainer container))
		{
			wasInContainer = true;
			if (!force)
			{
				return Remove(entity, container);
			}
			Remove(entity, container, reparent: true, force: true);
			return true;
		}
		wasInContainer = false;
		return false;
	}

	public bool TryRemoveFromContainer(Entity<TransformComponent?, MetaDataComponent?> entity, bool force = false)
	{
		bool wasInContainer;
		return TryRemoveFromContainer(entity, force, out wasInContainer);
	}

	public List<EntityUid> EmptyContainer(BaseContainer container, bool force = false, EntityCoordinates? destination = null, bool reparent = true)
	{
		List<EntityUid> list = new List<EntityUid>(container.ContainedEntities);
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (!Remove(list[num], container, reparent, force, destination))
			{
				list.RemoveSwap(num);
			}
		}
		return list;
	}

	public void CleanContainer(BaseContainer container)
	{
		EntityUid[] array = container.ContainedEntities.ToArray();
		foreach (EntityUid entityUid in array)
		{
			if (!Deleted(entityUid))
			{
				Remove(entityUid, container, reparent: true, force: true);
				PredictedDel(entityUid);
			}
		}
	}

	public void AttachParentToContainerOrGrid(Entity<TransformComponent> transform)
	{
		if (!transform.Comp.ParentUid.IsValid() || !TryGetContainingContainer((Owner: transform.Comp.ParentUid, Comp1: Transform(transform.Comp.ParentUid)), out BaseContainer container) || !TryInsertIntoContainer(transform, container))
		{
			_transform.AttachToGridOrMap(transform, transform.Comp);
		}
	}

	private bool TryInsertIntoContainer(Entity<TransformComponent> transform, BaseContainer container)
	{
		if (Insert((Owner: transform.Owner, Comp1: transform.Comp, Comp2: (MetaDataComponent)null, Comp3: (PhysicsComponent)null), container))
		{
			return true;
		}
		TransformComponent transformComponent = Transform(container.Owner);
		if (transformComponent.ParentUid.IsValid() && TryGetContainingContainer((Owner: container.Owner, Comp1: transformComponent), out BaseContainer container2))
		{
			return TryInsertIntoContainer(transform, container2);
		}
		return false;
	}

	internal bool TryGetManagerComp(EntityUid entity, [NotNullWhen(true)] out ContainerManagerComponent? manager)
	{
		if (TryComp(entity, out manager))
		{
			return true;
		}
		TransformComponent transformComponent = Transform(entity);
		if (transformComponent.ParentUid.IsValid())
		{
			return TryGetManagerComp(transformComponent.ParentUid, out manager);
		}
		return false;
	}

	protected virtual void OnParentChanged(ref EntParentChangedMessage message)
	{
		MetaDataComponent metaDataComponent = MetaData(message.Entity);
		if ((metaDataComponent.Flags & MetaDataFlags.InContainer) != MetaDataFlags.None && TryComp(message.OldParent, out ContainerManagerComponent comp))
		{
			RemoveEntity(message.OldParent.Value, message.Entity, comp, message.Transform, metaDataComponent, reparent: false, force: true);
		}
	}

	[Conditional("DEBUG")]
	[Access(new Type[] { typeof(BaseContainer) })]
	public void AssertInContainer(EntityUid uid, BaseContainer container)
	{
		if (!_timing.ApplyingState)
		{
			_ = MetaData(uid).Flags;
		}
	}

	public bool Insert(Entity<TransformComponent?, MetaDataComponent?, PhysicsComponent?> toInsert, BaseContainer container, TransformComponent? containerXform = null, bool force = false)
	{
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		Entity<TransformComponent, MetaDataComponent, PhysicsComponent> entity = toInsert;
		var (entityUid2, comp, comp2, component) = (Entity<TransformComponent, MetaDataComponent, PhysicsComponent>)(ref entity);
		if (!Resolve(entityUid2, ref comp, ref comp2))
		{
			return false;
		}
		if (!TryComp(container.Owner, out MetaDataComponent comp3))
		{
			base.Log.Error($"Attempted to insert an entity {ToPrettyString(toInsert)} into a non-existent entity. Trace: {Environment.StackTrace}");
			QueueDel(toInsert);
			return false;
		}
		if ((int)comp3.EntityLifeStage >= 4)
		{
			base.Log.Error($"Attempted to insert an entity {ToPrettyString(toInsert)} into an entity that is terminating. Entity: {ToPrettyString(container.Owner)}. Trace: {Environment.StackTrace}");
			QueueDel(toInsert);
			return false;
		}
		if (!force && !CanInsert(entityUid2, container, assumeEmpty: false, containerXform))
		{
			return false;
		}
		if ((int)comp2.EntityLifeStage >= 4)
		{
			base.Log.Error($"Attempted to insert a terminating entity {ToPrettyString(entityUid2)} into a container {container.ID} in entity: {ToPrettyString(container.Owner)}. Trace: {Environment.StackTrace}");
			return false;
		}
		if ((comp2.Flags & MetaDataFlags.InContainer) != MetaDataFlags.None && TryComp(comp.ParentUid, out ContainerManagerComponent comp4) && TryGetContainingContainer(comp.ParentUid, toInsert, out BaseContainer container2, comp4) && !Remove((Owner: entityUid2, Comp1: comp, Comp2: comp2), container2, reparent: false))
		{
			return false;
		}
		comp2.Flags |= MetaDataFlags.InContainer;
		_lookup.RemoveFromEntityTree(toInsert, comp);
		BroadphaseData? broadphase = comp.Broadphase;
		comp.Broadphase = BroadphaseData.Invalid;
		_transform.Unanchor(toInsert, comp, setPhysics: false);
		PhysicsQuery.Resolve(toInsert, ref component, logMissing: false);
		RecursivelyUpdatePhysics((Owner: toInsert, Comp1: comp, Comp2: component));
		EntityUid parentUid = comp.ParentUid;
		_transform.SetCoordinates(toInsert, comp, new EntityCoordinates(container.Owner, Vector2.Zero), Angle.Zero);
		comp.Broadphase = broadphase;
		container.InternalInsert(toInsert, EntityManager);
		RecursivelyUpdateJoints((Owner: toInsert, Comp: comp));
		RaiseLocalEvent(container.Owner, new EntInsertedIntoContainerMessage(toInsert, parentUid, container), broadcast: true);
		RaiseLocalEvent(toInsert, new EntGotInsertedIntoContainerMessage(toInsert, container), broadcast: true);
		Dirty(container.Owner, container.Manager);
		return true;
	}

	public bool InsertOrDrop(Entity<TransformComponent?, MetaDataComponent?, PhysicsComponent?> toInsert, BaseContainer container, TransformComponent? containerXform = null)
	{
		if (!Resolve(toInsert.Owner, ref toInsert.Comp1) || !Resolve(container.Owner, ref containerXform))
		{
			return false;
		}
		if (Insert(toInsert, container, containerXform))
		{
			return true;
		}
		_transform.DropNextTo(toInsert, (Owner: container.Owner, Comp: containerXform));
		return false;
	}

	public bool CanInsert(EntityUid toInsert, BaseContainer container, bool assumeEmpty = false, TransformComponent? containerXform = null)
	{
		if (container.Owner == toInsert)
		{
			return false;
		}
		if (!assumeEmpty && container.Contains(toInsert))
		{
			return false;
		}
		if (!container.CanInsert(toInsert, assumeEmpty, EntityManager))
		{
			return false;
		}
		if (_mapQuery.HasComponent(toInsert) || _gridQuery.HasComponent(toInsert))
		{
			return false;
		}
		if (_transform.ContainsEntity(toInsert, (Owner: container.Owner, Comp: containerXform)))
		{
			return false;
		}
		ContainerIsInsertingAttemptEvent containerIsInsertingAttemptEvent = new ContainerIsInsertingAttemptEvent(container, toInsert, assumeEmpty);
		RaiseLocalEvent(container.Owner, containerIsInsertingAttemptEvent, broadcast: true);
		if (containerIsInsertingAttemptEvent.Cancelled)
		{
			return false;
		}
		ContainerGettingInsertedAttemptEvent containerGettingInsertedAttemptEvent = new ContainerGettingInsertedAttemptEvent(container, toInsert, assumeEmpty);
		RaiseLocalEvent(toInsert, containerGettingInsertedAttemptEvent, broadcast: true);
		return !containerGettingInsertedAttemptEvent.Cancelled;
	}

	private void RecursivelyUpdatePhysics(Entity<TransformComponent, PhysicsComponent?> entity)
	{
		PhysicsComponent comp = entity.Comp2;
		if (comp != null)
		{
			_physics.SetLinearVelocity(entity, Vector2.Zero, dirty: false, wakeBody: true, null, comp);
			_physics.SetAngularVelocity(entity, 0f, dirty: false, null, comp);
			_physics.SetCanCollide(entity, value: false, dirty: false, force: false, null, comp);
		}
		foreach (EntityUid child in entity.Comp1._children)
		{
			TransformComponent component = TransformQuery.GetComponent(child);
			PhysicsQuery.TryGetComponent(child, out PhysicsComponent component2);
			RecursivelyUpdatePhysics((Owner: child, Comp1: component, Comp2: component2));
		}
	}

	internal void RecursivelyUpdateJoints(Entity<TransformComponent> entity)
	{
		if (_timing.ApplyingState)
		{
			return;
		}
		if (JointQuery.TryGetComponent(entity, out JointComponent component))
		{
			_joint.RefreshRelay(entity, component);
		}
		foreach (EntityUid child in entity.Comp._children)
		{
			TransformComponent component2 = TransformQuery.GetComponent(child);
			RecursivelyUpdateJoints((Owner: child, Comp: component2));
		}
	}

	public bool Remove(Entity<TransformComponent?, MetaDataComponent?> toRemove, BaseContainer container, bool reparent = true, bool force = false, EntityCoordinates? destination = null, Angle? localRotation = null)
	{
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		Entity<TransformComponent, MetaDataComponent> entity = toRemove;
		var (uid, comp, comp2) = (Entity<TransformComponent, MetaDataComponent>)(ref entity);
		if (!Resolve(uid, ref comp, ref comp2))
		{
			return false;
		}
		if (!force && !CanRemove(toRemove, container))
		{
			return false;
		}
		if (force && !container.Contains(toRemove))
		{
			return false;
		}
		if ((int)comp2.EntityLifeStage >= 4 && (!force || reparent))
		{
			base.Log.Error($"Attempting to remove an entity from a container while it is terminating. Entity: {ToPrettyString(toRemove, comp2)}. Container: {ToPrettyString(container.Owner)}. Trace: {Environment.StackTrace}");
			return false;
		}
		comp2.Flags &= ~MetaDataFlags.InContainer;
		container.InternalRemove(toRemove, EntityManager);
		EntityUid parentUid = comp.ParentUid;
		if (destination.HasValue)
		{
			_transform.SetCoordinates(toRemove, comp, destination.Value, localRotation);
		}
		else if (reparent)
		{
			AttachParentToContainerOrGrid((Owner: toRemove, Comp: comp));
			if (localRotation.HasValue)
			{
				_transform.SetLocalRotation(uid, localRotation.Value, comp);
			}
		}
		if (comp.ParentUid == parentUid && !comp.Broadphase.HasValue)
		{
			_lookup.FindAndAddToEntityTree(toRemove, recursive: true, comp);
		}
		RecursivelyUpdateJoints((Owner: toRemove, Comp: comp));
		RaiseLocalEvent(container.Owner, new EntRemovedFromContainerMessage(toRemove, container), broadcast: true);
		RaiseLocalEvent(toRemove, new EntGotRemovedFromContainerMessage(toRemove, container));
		Dirty(container.Owner, container.Manager);
		return true;
	}

	public bool CanRemove(EntityUid toRemove, BaseContainer container)
	{
		if (!container.Contains(toRemove))
		{
			return false;
		}
		ContainerIsRemovingAttemptEvent containerIsRemovingAttemptEvent = new ContainerIsRemovingAttemptEvent(container, toRemove);
		RaiseLocalEvent(container.Owner, containerIsRemovingAttemptEvent, broadcast: true);
		if (containerIsRemovingAttemptEvent.Cancelled)
		{
			return false;
		}
		ContainerGettingRemovedAttemptEvent containerGettingRemovedAttemptEvent = new ContainerGettingRemovedAttemptEvent(container, toRemove);
		RaiseLocalEvent(toRemove, containerGettingRemovedAttemptEvent, broadcast: true);
		return !containerGettingRemovedAttemptEvent.Cancelled;
	}

	private void OnStartupValidation(EntityUid uid, ContainerManagerComponent component, ComponentStartup args)
	{
		foreach (BaseContainer value in component.Containers.Values)
		{
			foreach (EntityUid containedEntity in value.ContainedEntities)
			{
				if (!MetaQuery.TryGetComponent(containedEntity, out MetaDataComponent component2))
				{
					ValidateMissingEntity(uid, value, containedEntity);
					continue;
				}
				TransformComponent component3 = TransformQuery.GetComponent(containedEntity);
				PhysicsQuery.TryGetComponent(containedEntity, out PhysicsComponent component4);
				component2.Flags |= MetaDataFlags.InContainer;
				_lookup.RemoveFromEntityTree(containedEntity, component3);
				RecursivelyUpdatePhysics((Owner: containedEntity, Comp1: component3, Comp2: component4));
				ValidateChildren(component3, TransformQuery, PhysicsQuery);
			}
		}
	}

	protected abstract void ValidateMissingEntity(EntityUid uid, BaseContainer cont, EntityUid missing);

	private void ValidateChildren(TransformComponent xform, EntityQuery<TransformComponent> xformQuery, EntityQuery<PhysicsComponent> physicsQuery)
	{
		foreach (EntityUid child in xform._children)
		{
			if (xformQuery.TryGetComponent(child, out TransformComponent component))
			{
				ValidateChildren(component, xformQuery, physicsQuery);
			}
		}
	}
}
