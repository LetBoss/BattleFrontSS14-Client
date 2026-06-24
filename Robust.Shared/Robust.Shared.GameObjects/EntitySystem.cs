using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Collections;
using Robust.Shared.Containers;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Reflection;
using Robust.Shared.Replays;
using Robust.Shared.Timing;

namespace Robust.Shared.GameObjects;

[Reflect(false)]
public abstract class EntitySystem : IEntitySystem, IEntityEventSubscriber, IPostInjectInit
{
	public sealed class Subscriptions
	{
		public EntitySystem System { get; }

		internal Subscriptions(EntitySystem system)
		{
			System = system;
		}

		public void SubEvent<T>(EventSource src, EntityEventHandler<T> handler, Type[]? before = null, Type[]? after = null) where T : notnull
		{
			System.SubEvent(src, handler, before, after);
		}

		public void SubSessionEvent<T>(EventSource src, EntitySessionEventHandler<T> handler, Type[]? before = null, Type[]? after = null) where T : notnull
		{
			System.SubSessionEvent(src, handler, before, after);
		}

		public void SubscribeLocalEvent<TComp, TEvent>(ComponentEventHandler<TComp, TEvent> handler, Type[]? before = null, Type[]? after = null) where TComp : IComponent where TEvent : notnull
		{
			System.SubscribeLocalEvent(handler, before, after);
		}

		public void SubscribeLocalEvent<TComp, TEvent>(ComponentEventRefHandler<TComp, TEvent> handler, Type[]? before = null, Type[]? after = null) where TComp : IComponent where TEvent : notnull
		{
			System.SubscribeLocalEvent(handler, before, after);
		}

		public void SubscribeLocalEvent<TComp, TEvent>(EntityEventRefHandler<TComp, TEvent> handler, Type[]? before = null, Type[]? after = null) where TComp : IComponent where TEvent : notnull
		{
			System.SubscribeLocalEvent(handler, before, after);
		}

		public void RegisterUnsubscription(Action action)
		{
			System._subscriptions.Add(new SubAction(action));
		}
	}

	private abstract class SubBase
	{
		public abstract void Unsubscribe(EntitySystem sys, IEventBus bus);
	}

	private sealed class SubBroadcast<T> : SubBase where T : notnull
	{
		private readonly EventSource _source;

		public SubBroadcast(EventSource source)
		{
			_source = source;
		}

		public override void Unsubscribe(EntitySystem sys, IEventBus bus)
		{
			bus.UnsubscribeEvent<T>(_source, sys);
		}
	}

	private sealed class SubLocal<TComp, TBase> : SubBase where TComp : IComponent where TBase : notnull
	{
		public override void Unsubscribe(EntitySystem sys, IEventBus bus)
		{
			bus.UnsubscribeLocalEvent<TComp, TBase>();
		}
	}

	private sealed class SubAction(Action action) : SubBase
	{
		public override void Unsubscribe(EntitySystem sys, IEventBus bus)
		{
			action();
		}
	}

	[Robust.Shared.IoC.Dependency]
	protected readonly EntityManager EntityManager;

	[Robust.Shared.IoC.Dependency]
	protected readonly ILogManager LogManager;

	[Robust.Shared.IoC.Dependency]
	private readonly ISharedPlayerManager _playerMan;

	[Robust.Shared.IoC.Dependency]
	private readonly IReplayRecordingManager _replayMan;

	[Robust.Shared.IoC.Dependency]
	protected readonly ILocalizationManager Loc;

	private ValueList<SubBase> _subscriptions;

	protected IComponentFactory Factory => EntityManager.ComponentFactory;

	public ISawmill Log { get; private set; }

	protected virtual string SawmillName
	{
		get
		{
			string text = GetType().Name;
			if (text.EndsWith("System"))
			{
				text = text.Substring(0, text.Length - "System".Length);
			}
			if (text.All(char.IsUpper))
			{
				text = text.ToLower(CultureInfo.InvariantCulture);
			}
			else
			{
				text = string.Concat(text.Select((char x) => char.IsUpper(x) ? $"_{char.ToLower(x)}" : x.ToString()));
				text = text.Trim('_');
			}
			return "system." + text;
		}
	}

	protected internal List<Type> UpdatesAfter { get; } = new List<Type>();

	protected internal List<Type> UpdatesBefore { get; } = new List<Type>();

	public bool UpdatesOutsidePrediction { get; protected internal set; }

	IEnumerable<Type> IEntitySystem.UpdatesAfter => UpdatesAfter;

	IEnumerable<Type> IEntitySystem.UpdatesBefore => UpdatesBefore;

	protected Subscriptions Subs { get; }

	protected EntitySystem()
	{
		Subs = new Subscriptions(this);
	}

	[MustCallBase(true)]
	public virtual void Initialize()
	{
	}

	[MustCallBase(true)]
	public virtual void Update(float frameTime)
	{
	}

	[MustCallBase(true)]
	public virtual void FrameUpdate(float frameTime)
	{
	}

	[MustCallBase(true)]
	public virtual void Shutdown()
	{
		ShutdownSubscriptions();
	}

	protected void RaiseLocalEvent<T>(T message) where T : notnull
	{
		EntityManager.EventBusInternal.RaiseEvent(EventSource.Local, message);
	}

	protected void RaiseLocalEvent<T>(ref T message) where T : notnull
	{
		EntityManager.EventBusInternal.RaiseEvent(EventSource.Local, ref message);
	}

	protected void RaiseLocalEvent(object message)
	{
		EntityManager.EventBusInternal.RaiseEvent(EventSource.Local, message);
	}

	protected void QueueLocalEvent(EntityEventArgs message)
	{
		EntityManager.EventBusInternal.QueueEvent(EventSource.Local, message);
	}

	protected void RaiseNetworkEvent(EntityEventArgs message)
	{
		EntityManager.EntityNetManager?.SendSystemNetworkMessage(message);
	}

	protected void RaiseNetworkEvent(EntityEventArgs message, INetChannel channel)
	{
		EntityManager.EntityNetManager?.SendSystemNetworkMessage(message, channel);
	}

	protected void RaiseNetworkEvent(EntityEventArgs message, ICommonSession session)
	{
		EntityManager.EntityNetManager?.SendSystemNetworkMessage(message, session.Channel);
	}

	protected void RaiseNetworkEvent(EntityEventArgs message, Filter filter, bool recordReplay = true)
	{
		if (recordReplay)
		{
			_replayMan.RecordServerMessage(message);
		}
		foreach (ICommonSession recipient in filter.Recipients)
		{
			EntityManager.EntityNetManager?.SendSystemNetworkMessage(message, recipient.Channel);
		}
	}

	protected void RaiseNetworkEvent(EntityEventArgs message, EntityUid recipient)
	{
		if (_playerMan.TryGetSessionByEntity(recipient, out ICommonSession session))
		{
			EntityManager.EntityNetManager?.SendSystemNetworkMessage(message, session.Channel);
		}
	}

	protected void RaiseLocalEvent<TEvent>(EntityUid uid, TEvent args, bool broadcast = false) where TEvent : notnull
	{
		EntityManager.EventBusInternal.RaiseLocalEvent(uid, args, broadcast);
	}

	protected void RaiseLocalEvent(EntityUid uid, object args, bool broadcast = false)
	{
		EntityManager.EventBusInternal.RaiseLocalEvent(uid, args, broadcast);
	}

	protected void RaiseLocalEvent<TEvent>(EntityUid uid, ref TEvent args, bool broadcast = false) where TEvent : notnull
	{
		EntityManager.EventBusInternal.RaiseLocalEvent(uid, ref args, broadcast);
	}

	protected void RaiseLocalEvent(EntityUid uid, ref object args, bool broadcast = false)
	{
		EntityManager.EventBusInternal.RaiseLocalEvent(uid, ref args, broadcast);
	}

	protected void RaiseComponentEvent<TEvent, TComp>(EntityUid uid, TComp comp, ref TEvent args) where TEvent : notnull where TComp : IComponent
	{
		EntityManager.EventBusInternal.RaiseComponentEvent(uid, comp, ref args);
	}

	public void RaiseComponentEvent<TEvent>(EntityUid uid, IComponent component, ref TEvent args) where TEvent : notnull
	{
		EntityManager.EventBusInternal.RaiseComponentEvent(uid, component, ref args);
	}

	[Obsolete("Either use a dependency, resolve and cache IEntityManager manually, or use EntityManager.System<T>()")]
	public static T Get<T>() where T : IEntitySystem
	{
		return IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<T>();
	}

	[Obsolete("Either use a dependency, resolve and cache IEntityManager manually, or use EntityManager.System<T>()")]
	public static bool TryGet<T>([NotNullWhen(true)] out T? entitySystem) where T : IEntitySystem
	{
		return IoCManager.Resolve<IEntitySystemManager>().TryGetEntitySystem<T>(out entitySystem);
	}

	void IPostInjectInit.PostInject()
	{
		PostInject();
	}

	protected virtual void PostInject()
	{
		Log = LogManager.GetSawmill(SawmillName);
		Log.Level = LogLevel.Info;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool Exists(EntityUid uid)
	{
		return EntityManager.EntityExists(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool Exists([NotNullWhen(true)] EntityUid? uid)
	{
		return EntityManager.EntityExists(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool Initializing(EntityUid uid, MetaDataComponent? metaData = null)
	{
		return LifeStage(uid, metaData) == EntityLifeStage.Initializing;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool Initialized(EntityUid uid, MetaDataComponent? metaData = null)
	{
		EntityLifeStage entityLifeStage = LifeStage(uid, metaData);
		if ((int)entityLifeStage >= 2)
		{
			return (int)entityLifeStage < 4;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool Terminating(EntityUid uid, MetaDataComponent? metaData = null)
	{
		return LifeStage(uid, metaData) == EntityLifeStage.Terminating;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool Deleted(EntityUid uid, MetaDataComponent? metaData = null)
	{
		return (int)LifeStage(uid, metaData) >= 5;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool TerminatingOrDeleted(EntityUid uid, MetaDataComponent? metaData = null)
	{
		return (int)LifeStage(uid, metaData) >= 4;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool TerminatingOrDeleted(EntityUid? uid, MetaDataComponent? metaData = null)
	{
		if (uid.HasValue)
		{
			return TerminatingOrDeleted(uid.Value, metaData);
		}
		return true;
	}

	[Obsolete("Use override without the EntityQuery")]
	protected bool Deleted(EntityUid uid, EntityQuery<MetaDataComponent> metaQuery)
	{
		return Deleted(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool Deleted([NotNullWhen(false)] EntityUid? uid)
	{
		if (uid.HasValue)
		{
			return Deleted(uid.Value);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityLifeStage LifeStage(EntityUid uid, MetaDataComponent? metaData = null)
	{
		if (!EntityManager.MetaQuery.Resolve(uid, ref metaData, logMissing: false))
		{
			return EntityLifeStage.Deleted;
		}
		return metaData.EntityLifeStage;
	}

	[Obsolete("Use LifeStage()")]
	protected bool TryLifeStage(EntityUid uid, [NotNullWhen(true)] out EntityLifeStage? lifeStage, MetaDataComponent? metaData = null)
	{
		if (!EntityManager.MetaQuery.Resolve(uid, ref metaData, logMissing: false))
		{
			lifeStage = null;
			return false;
		}
		lifeStage = metaData.EntityLifeStage;
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool IsPaused(EntityUid? uid, MetaDataComponent? metadata = null)
	{
		return EntityManager.IsPaused(uid, metadata);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void DirtyEntity(EntityUid uid, MetaDataComponent? meta = null)
	{
		EntityManager.DirtyEntity(uid, meta);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void Dirty(EntityUid uid, IComponent component, MetaDataComponent? meta = null)
	{
		EntityManager.Dirty(uid, component, meta);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void DirtyField(EntityUid uid, IComponentDelta delta, string fieldName, MetaDataComponent? meta = null)
	{
		EntityManager.DirtyField(uid, delta, fieldName, meta);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void DirtyField<T>(Entity<T?> entity, [ValidateMember] string fieldName, MetaDataComponent? meta = null) where T : IComponentDelta
	{
		if (Resolve(entity.Owner, ref entity.Comp))
		{
			EntityManager.DirtyField(entity.Owner, entity.Comp, fieldName, meta);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void DirtyField<T>(EntityUid uid, T component, [ValidateMember] string fieldName, MetaDataComponent? meta = null) where T : IComponentDelta
	{
		EntityManager.DirtyField(uid, component, fieldName, meta);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void DirtyFields<T>(EntityUid uid, T comp, MetaDataComponent? meta, params string[] fields) where T : IComponentDelta
	{
		EntityManager.DirtyFields(uid, comp, meta, fields);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void DirtyFields<T>(Entity<T?> ent, MetaDataComponent? meta, params string[] fields) where T : IComponentDelta
	{
		if (Resolve(ent, ref ent.Comp))
		{
			EntityManager.DirtyFields(ent, ent.Comp, meta, fields);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void Dirty<T>(Entity<T> ent, MetaDataComponent? meta = null) where T : IComponent?
	{
		T component = ent.Comp;
		if (component != null || EntityManager.TryGetComponent<T>(ent.Owner, out component))
		{
			EntityManager.Dirty(ent, component, meta);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void Dirty<T1, T2>(Entity<T1, T2> ent, MetaDataComponent? meta = null) where T1 : IComponent where T2 : IComponent
	{
		EntityManager.Dirty(ent, meta);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void Dirty<T1, T2, T3>(Entity<T1, T2, T3> ent, MetaDataComponent? meta = null) where T1 : IComponent where T2 : IComponent where T3 : IComponent
	{
		EntityManager.Dirty(ent, meta);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void Dirty<T1, T2, T3, T4>(Entity<T1, T2, T3, T4> ent, MetaDataComponent? meta = null) where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
	{
		EntityManager.Dirty(ent, meta);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected string Name(EntityUid uid, MetaDataComponent? metaData = null)
	{
		if (!EntityManager.MetaQuery.Resolve(uid, ref metaData, logMissing: false))
		{
			throw CompNotFound<MetaDataComponent>(uid);
		}
		return metaData.EntityName;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected string Description(EntityUid uid, MetaDataComponent? metaData = null)
	{
		if (!EntityManager.MetaQuery.Resolve(uid, ref metaData, logMissing: false))
		{
			throw CompNotFound<MetaDataComponent>(uid);
		}
		return metaData.EntityDescription;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityPrototype? Prototype(EntityUid uid, MetaDataComponent? metaData = null)
	{
		if (!EntityManager.MetaQuery.Resolve(uid, ref metaData, logMissing: false))
		{
			throw CompNotFound<MetaDataComponent>(uid);
		}
		return metaData.EntityPrototype;
	}

	protected GameTick LastModifiedTick(EntityUid uid, MetaDataComponent? metaData = null)
	{
		if (!EntityManager.MetaQuery.Resolve(uid, ref metaData, logMissing: false))
		{
			throw CompNotFound<MetaDataComponent>(uid);
		}
		return metaData.EntityLastModifiedTick;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool Paused(EntityUid uid, MetaDataComponent? metaData = null)
	{
		if (!EntityManager.MetaQuery.Resolve(uid, ref metaData, logMissing: false))
		{
			throw CompNotFound<MetaDataComponent>(uid);
		}
		return metaData.EntityPaused;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void SetPaused(EntityUid uid, bool paused, MetaDataComponent? metaData = null)
	{
		if (!EntityManager.MetaQuery.Resolve(uid, ref metaData, logMissing: false))
		{
			throw CompNotFound<MetaDataComponent>(uid);
		}
		EntityManager.EntitySysManager.GetEntitySystem<MetaDataSystem>().SetEntityPaused(uid, paused, metaData);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool TryDirty(EntityUid uid, MetaDataComponent? metaData = null)
	{
		if (!EntityManager.MetaQuery.Resolve(uid, ref metaData, logMissing: false))
		{
			return false;
		}
		DirtyEntity(uid, metaData);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool TryName(EntityUid uid, [NotNullWhen(true)] out string? name, MetaDataComponent? metaData = null)
	{
		if (!EntityManager.MetaQuery.Resolve(uid, ref metaData, logMissing: false))
		{
			name = null;
			return false;
		}
		name = metaData.EntityName;
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool TryDescription(EntityUid uid, [NotNullWhen(true)] out string? description, MetaDataComponent? metaData = null)
	{
		if (!EntityManager.MetaQuery.Resolve(uid, ref metaData, logMissing: false))
		{
			description = null;
			return false;
		}
		description = metaData.EntityDescription;
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool TryPrototype(EntityUid uid, [NotNullWhen(true)] out EntityPrototype? prototype, MetaDataComponent? metaData = null)
	{
		if (!EntityManager.MetaQuery.Resolve(uid, ref metaData, logMissing: false))
		{
			prototype = null;
			return false;
		}
		prototype = metaData.EntityPrototype;
		return prototype != null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool TryLastModifiedTick(EntityUid uid, [NotNullWhen(true)] out GameTick? lastModifiedTick, MetaDataComponent? metaData = null)
	{
		if (!EntityManager.MetaQuery.Resolve(uid, ref metaData, logMissing: false))
		{
			lastModifiedTick = null;
			return false;
		}
		lastModifiedTick = metaData.EntityLastModifiedTick;
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool TryPaused(EntityUid uid, [NotNullWhen(true)] out bool? paused, MetaDataComponent? metaData = null)
	{
		if (!EntityManager.MetaQuery.Resolve(uid, ref metaData, logMissing: false))
		{
			paused = null;
			return false;
		}
		paused = metaData.EntityPaused;
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[return: NotNullIfNotNull("uid")]
	protected EntityStringRepresentation? ToPrettyString(EntityUid? uid, MetaDataComponent? metadata = null)
	{
		return EntityManager.ToPrettyString(uid, metadata);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[return: NotNullIfNotNull("netEntity")]
	protected EntityStringRepresentation? ToPrettyString(NetEntity? netEntity)
	{
		return EntityManager.ToPrettyString(netEntity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityStringRepresentation ToPrettyString(EntityUid uid, MetaDataComponent? metadata)
	{
		return EntityManager.ToPrettyString((Owner: uid, Comp: metadata));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityStringRepresentation ToPrettyString(Entity<MetaDataComponent?> entity)
	{
		return EntityManager.ToPrettyString(entity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityStringRepresentation ToPrettyString(NetEntity netEntity)
	{
		return EntityManager.ToPrettyString(netEntity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected T Comp<T>(EntityUid uid) where T : IComponent
	{
		return EntityManager.GetComponent<T>(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected T? CompOrNull<T>(EntityUid uid) where T : IComponent
	{
		return EntityManager.GetComponentOrNull<T>(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected T? CompOrNull<T>(EntityUid? uid) where T : IComponent
	{
		if (!uid.HasValue)
		{
			return default(T);
		}
		return EntityManager.GetComponentOrNull<T>(uid.Value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[PreferNonGenericVariantFor(new Type[]
	{
		typeof(TransformComponent),
		typeof(MetaDataComponent)
	})]
	protected bool TryComp<T>(EntityUid uid, [NotNullWhen(true)] out T? comp) where T : IComponent
	{
		return EntityManager.TryGetComponent<T>(uid, out comp);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool TryComp(EntityUid uid, [NotNullWhen(true)] out TransformComponent? comp)
	{
		return EntityManager.TransformQuery.TryGetComponent(uid, out comp);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool TryComp(EntityUid uid, [NotNullWhen(true)] out MetaDataComponent? comp)
	{
		return EntityManager.MetaQuery.TryGetComponent(uid, out comp);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool TryComp<T>([NotNullWhen(true)] EntityUid? uid, [NotNullWhen(true)] out T? comp) where T : IComponent
	{
		if (!uid.HasValue)
		{
			comp = default(T);
			return false;
		}
		return EntityManager.TryGetComponent<T>(uid.Value, out comp);
	}

	protected bool TryComp([NotNullWhen(true)] EntityUid? uid, [NotNullWhen(true)] out TransformComponent? comp)
	{
		if (!uid.HasValue)
		{
			comp = null;
			return false;
		}
		return EntityManager.TransformQuery.TryGetComponent(uid.Value, out comp);
	}

	protected bool TryComp([NotNullWhen(true)] EntityUid? uid, [NotNullWhen(true)] out MetaDataComponent? comp)
	{
		if (!uid.HasValue)
		{
			comp = null;
			return false;
		}
		return EntityManager.MetaQuery.TryGetComponent(uid.Value, out comp);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected IEnumerable<IComponent> AllComps(EntityUid uid)
	{
		return EntityManager.GetComponents(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected IEnumerable<T> AllComps<T>(EntityUid uid)
	{
		return EntityManager.GetComponents<T>(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected TransformComponent Transform(EntityUid uid)
	{
		return EntityManager.TransformQuery.GetComponent(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected MetaDataComponent MetaData(EntityUid uid)
	{
		return EntityManager.MetaQuery.GetComponent(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected (EntityUid, MetaDataComponent) GetEntityData(NetEntity nuid)
	{
		return EntityManager.GetEntityData(nuid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool TryGetEntityData(NetEntity nuid, [NotNullWhen(true)] out EntityUid? uid, [NotNullWhen(true)] out MetaDataComponent? meta)
	{
		return EntityManager.TryGetEntityData(nuid, out uid, out meta);
	}

	protected bool TryCopyComponent<T>(EntityUid source, EntityUid target, ref T? sourceComponent, [NotNullWhen(true)] out T? targetComp, MetaDataComponent? meta = null) where T : IComponent
	{
		return EntityManager.TryCopyComponent(source, target, ref sourceComponent, out targetComp, meta);
	}

	protected bool TryCopyComponents(EntityUid source, EntityUid target, MetaDataComponent? meta = null, params Type[] sourceComponents)
	{
		return EntityManager.TryCopyComponents(source, target, meta, sourceComponents);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected IComponent CopyComp(EntityUid source, EntityUid target, IComponent sourceComponent, MetaDataComponent? meta = null)
	{
		return EntityManager.CopyComponent(source, target, sourceComponent, meta);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected T CopyComp<T>(EntityUid source, EntityUid target, T sourceComponent, MetaDataComponent? meta = null) where T : IComponent
	{
		return EntityManager.CopyComponent(source, target, sourceComponent, meta);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void CopyComps(EntityUid source, EntityUid target, MetaDataComponent? meta = null, params IComponent[] sourceComponents)
	{
		EntityManager.CopyComponents(source, target, meta, sourceComponents);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool HasComp<T>(EntityUid uid) where T : IComponent
	{
		return EntityManager.HasComponent<T>(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool HasComp(EntityUid uid, Type type)
	{
		return EntityManager.HasComponent(uid, type);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool HasComp<T>([NotNullWhen(true)] EntityUid? uid) where T : IComponent
	{
		return EntityManager.HasComponent<T>(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool HasComp([NotNullWhen(true)] EntityUid? uid, Type type)
	{
		return EntityManager.HasComponent(uid, type);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected T AddComp<T>(EntityUid uid) where T : IComponent, new()
	{
		return EntityManager.AddComponent<T>(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void AddComp<T>(EntityUid uid, T component, bool overwrite = false) where T : IComponent
	{
		EntityManager.AddComponent(uid, component, overwrite);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected T EnsureComp<T>(EntityUid uid) where T : IComponent, new()
	{
		return EntityManager.EnsureComponent<T>(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool EnsureComp<T>(EntityUid uid, out T comp) where T : IComponent, new()
	{
		return EntityManager.EnsureComponent<T>(uid, out comp);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool EnsureComp<T>(ref Entity<T?> entity) where T : IComponent, new()
	{
		return EntityManager.EnsureComponent(ref entity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool RemCompDeferred<T>(EntityUid uid) where T : IComponent
	{
		return EntityManager.RemoveComponentDeferred<T>(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool RemCompDeferred(EntityUid uid, Type type)
	{
		return EntityManager.RemoveComponentDeferred(uid, type);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void RemCompDeferred(EntityUid uid, IComponent component)
	{
		EntityManager.RemoveComponentDeferred(uid, component);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected int Count<T>() where T : IComponent
	{
		return EntityManager.Count<T>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected int Count(Type type)
	{
		return EntityManager.Count(type);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool RemComp<T>(EntityUid uid) where T : IComponent
	{
		return EntityManager.RemoveComponent<T>(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool RemComp(EntityUid uid, Type type)
	{
		return EntityManager.RemoveComponent(uid, type);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void RemComp(EntityUid uid, IComponent component)
	{
		EntityManager.RemoveComponent(uid, component);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void Del(EntityUid? uid)
	{
		EntityManager.DeleteEntity(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void QueueDel(EntityUid? uid)
	{
		EntityManager.QueueDeleteEntity(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void PredictedDel(Entity<MetaDataComponent?, TransformComponent?> ent)
	{
		EntityManager.PredictedDeleteEntity(ent);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void PredictedDel(Entity<MetaDataComponent?, TransformComponent?>? ent)
	{
		EntityManager.PredictedDeleteEntity(ent);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void PredictedQueueDel(Entity<MetaDataComponent?> ent)
	{
		EntityManager.PredictedQueueDeleteEntity(ent);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void PredictedQueueDel(Entity<MetaDataComponent?>? ent)
	{
		EntityManager.PredictedQueueDeleteEntity(ent);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void PredictedQueueDel(EntityUid uid)
	{
		EntityManager.PredictedQueueDeleteEntity(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void PredictedQueueDel(EntityUid? uid)
	{
		EntityManager.PredictedQueueDeleteEntity(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete("use variant without TransformComponent")]
	protected void PredictedQueueDel(Entity<MetaDataComponent?, TransformComponent?> ent)
	{
		EntityManager.PredictedQueueDeleteEntity(ent);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete("use variant without TransformComponent")]
	protected void PredictedQueueDel(Entity<MetaDataComponent?, TransformComponent?>? ent)
	{
		EntityManager.PredictedQueueDeleteEntity(ent);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool TryQueueDel(EntityUid? uid)
	{
		return EntityManager.TryQueueDeleteEntity(uid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityUid Spawn(string? prototype, EntityCoordinates coordinates)
	{
		return ((IEntityManager)EntityManager).SpawnEntity(prototype, coordinates, (ComponentRegistry?)null);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityUid Spawn(string? prototype, MapCoordinates coordinates, ComponentRegistry? overrides = null, Angle rotation = default(Angle))
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return EntityManager.Spawn(prototype, coordinates, overrides, rotation);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityUid Spawn(string? prototype = null, ComponentRegistry? overrides = null, bool doMapInit = true)
	{
		return EntityManager.Spawn(prototype, overrides, doMapInit);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityUid SpawnAttachedTo(string? prototype, EntityCoordinates coordinates, ComponentRegistry? overrides = null, Angle rotation = default(Angle))
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return EntityManager.SpawnAttachedTo(prototype, coordinates, overrides, rotation);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityUid SpawnAtPosition(string? prototype, EntityCoordinates coordinates, ComponentRegistry? overrides = null)
	{
		return EntityManager.SpawnAtPosition(prototype, coordinates, overrides);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool TrySpawnInContainer(string? protoName, EntityUid containerUid, string containerId, [NotNullWhen(true)] out EntityUid? uid, ContainerManagerComponent? containerComp = null, ComponentRegistry? overrides = null)
	{
		return EntityManager.TrySpawnInContainer(protoName, containerUid, containerId, out uid, containerComp, overrides);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool TrySpawnNextTo(string? protoName, EntityUid target, [NotNullWhen(true)] out EntityUid? uid, TransformComponent? xform = null, ComponentRegistry? overrides = null)
	{
		return EntityManager.TrySpawnNextTo(protoName, target, out uid, xform, overrides);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityUid SpawnNextToOrDrop(string? protoName, EntityUid target, TransformComponent? xform = null, ComponentRegistry? overrides = null)
	{
		return EntityManager.SpawnNextToOrDrop(protoName, target, xform, overrides);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityUid SpawnInContainerOrDrop(string? protoName, EntityUid containerUid, string containerId, TransformComponent? xform = null, ContainerManagerComponent? container = null, ComponentRegistry? overrides = null)
	{
		return EntityManager.SpawnInContainerOrDrop(protoName, containerUid, containerId, xform, container, overrides);
	}

	protected void FlagPredicted(Entity<MetaDataComponent?> ent)
	{
		EntityManager.FlagPredicted(ent);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityUid PredictedSpawnAttachedTo(string? prototype, EntityCoordinates coordinates, ComponentRegistry? overrides = null, Angle rotation = default(Angle))
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return EntityManager.PredictedSpawnAttachedTo(prototype, coordinates, overrides, rotation);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityUid PredictedSpawnAtPosition(string? prototype, EntityCoordinates coordinates, ComponentRegistry? overrides = null)
	{
		return EntityManager.PredictedSpawnAtPosition(prototype, coordinates, overrides);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool PredictedTrySpawnInContainer(string? protoName, EntityUid containerUid, string containerId, [NotNullWhen(true)] out EntityUid? uid, ContainerManagerComponent? containerComp = null, ComponentRegistry? overrides = null)
	{
		return EntityManager.PredictedTrySpawnInContainer(protoName, containerUid, containerId, out uid, containerComp, overrides);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool PredictedTrySpawnNextTo(string? protoName, EntityUid target, [NotNullWhen(true)] out EntityUid? uid, TransformComponent? xform = null, ComponentRegistry? overrides = null)
	{
		return EntityManager.PredictedTrySpawnNextTo(protoName, target, out uid, xform, overrides);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityUid PredictedSpawnNextToOrDrop(string? protoName, EntityUid target, TransformComponent? xform = null, ComponentRegistry? overrides = null)
	{
		return EntityManager.PredictedSpawnNextToOrDrop(protoName, target, xform, overrides);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityUid PredictedSpawnInContainerOrDrop(string? protoName, EntityUid containerUid, string containerId, TransformComponent? xform = null, ContainerManagerComponent? container = null, ComponentRegistry? overrides = null)
	{
		return EntityManager.PredictedSpawnInContainerOrDrop(protoName, containerUid, containerId, xform, container, overrides);
	}

	private static KeyNotFoundException CompNotFound<T>(EntityUid uid)
	{
		return new KeyNotFoundException($"Entity {uid} does not have a component of type {typeof(T)}");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected AllEntityQueryEnumerator<TComp1> AllEntityQuery<TComp1>() where TComp1 : IComponent
	{
		return EntityManager.AllEntityQueryEnumerator<TComp1>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected AllEntityQueryEnumerator<TComp1, TComp2> AllEntityQuery<TComp1, TComp2>() where TComp1 : IComponent where TComp2 : IComponent
	{
		return EntityManager.AllEntityQueryEnumerator<TComp1, TComp2>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected AllEntityQueryEnumerator<TComp1, TComp2, TComp3> AllEntityQuery<TComp1, TComp2, TComp3>() where TComp1 : IComponent where TComp2 : IComponent where TComp3 : IComponent
	{
		return EntityManager.AllEntityQueryEnumerator<TComp1, TComp2, TComp3>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected AllEntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4> AllEntityQuery<TComp1, TComp2, TComp3, TComp4>() where TComp1 : IComponent where TComp2 : IComponent where TComp3 : IComponent where TComp4 : IComponent
	{
		return EntityManager.AllEntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityQueryEnumerator<TComp1> EntityQueryEnumerator<TComp1>() where TComp1 : IComponent
	{
		return EntityManager.EntityQueryEnumerator<TComp1>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityQueryEnumerator<TComp1, TComp2> EntityQueryEnumerator<TComp1, TComp2>() where TComp1 : IComponent where TComp2 : IComponent
	{
		return EntityManager.EntityQueryEnumerator<TComp1, TComp2>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityQueryEnumerator<TComp1, TComp2, TComp3> EntityQueryEnumerator<TComp1, TComp2, TComp3>() where TComp1 : IComponent where TComp2 : IComponent where TComp3 : IComponent
	{
		return EntityManager.EntityQueryEnumerator<TComp1, TComp2, TComp3>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4> EntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4>() where TComp1 : IComponent where TComp2 : IComponent where TComp3 : IComponent where TComp4 : IComponent
	{
		return EntityManager.EntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityQuery<T> GetEntityQuery<T>() where T : IComponent
	{
		return EntityManager.GetEntityQuery<T>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected IEnumerable<TComp1> EntityQuery<TComp1>(bool includePaused = false) where TComp1 : IComponent
	{
		return EntityManager.EntityQuery<TComp1>(includePaused);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected IEnumerable<(TComp1, TComp2)> EntityQuery<TComp1, TComp2>(bool includePaused = false) where TComp1 : IComponent where TComp2 : IComponent
	{
		return EntityManager.EntityQuery<TComp1, TComp2>(includePaused);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected IEnumerable<(TComp1, TComp2, TComp3)> EntityQuery<TComp1, TComp2, TComp3>(bool includePaused = false) where TComp1 : IComponent where TComp2 : IComponent where TComp3 : IComponent
	{
		return EntityManager.EntityQuery<TComp1, TComp2, TComp3>(includePaused);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected IEnumerable<(TComp1, TComp2, TComp3, TComp4)> EntityQuery<TComp1, TComp2, TComp3, TComp4>(bool includePaused = false) where TComp1 : IComponent where TComp2 : IComponent where TComp3 : IComponent where TComp4 : IComponent
	{
		return EntityManager.EntityQuery<TComp1, TComp2, TComp3, TComp4>(includePaused);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void RaisePredictiveEvent<T>(T msg) where T : EntityEventArgs
	{
		EntityManager.RaisePredictiveEvent(msg);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool IsClientSide(EntityUid entity, MetaDataComponent? meta = null)
	{
		return EntityManager.IsClientSide(entity, meta);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool IsClientSide(Entity<MetaDataComponent> entity)
	{
		return EntityManager.IsClientSide(entity, entity.Comp);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool TryGetEntity(NetEntity nEntity, [NotNullWhen(true)] out EntityUid? entity)
	{
		return EntityManager.TryGetEntity(nEntity, out entity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool TryGetEntity(NetEntity? nEntity, [NotNullWhen(true)] out EntityUid? entity)
	{
		return EntityManager.TryGetEntity(nEntity, out entity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryGetNetEntity(EntityUid uid, [NotNullWhen(true)] out NetEntity? netEntity, MetaDataComponent? metadata = null)
	{
		return EntityManager.TryGetNetEntity(uid, out netEntity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryGetNetEntity(EntityUid? uid, [NotNullWhen(true)] out NetEntity? netEntity, MetaDataComponent? metadata = null)
	{
		return EntityManager.TryGetNetEntity(uid, out netEntity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected NetEntity GetNetEntity(EntityUid uid, MetaDataComponent? metadata = null)
	{
		return EntityManager.GetNetEntity(uid, metadata);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected NetEntity? GetNetEntity(EntityUid? uid, MetaDataComponent? metadata = null)
	{
		return EntityManager.GetNetEntity(uid, metadata);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityUid EnsureEntity<T>(NetEntity netEntity, EntityUid callerEntity)
	{
		return EntityManager.EnsureEntity<T>(netEntity, callerEntity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityUid? EnsureEntity<T>(NetEntity? netEntity, EntityUid callerEntity)
	{
		return EntityManager.EnsureEntity<T>(netEntity, callerEntity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityCoordinates EnsureCoordinates<T>(NetCoordinates netCoordinates, EntityUid callerEntity)
	{
		return EntityManager.EnsureCoordinates<T>(netCoordinates, callerEntity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityCoordinates? EnsureCoordinates<T>(NetCoordinates? netCoordinates, EntityUid callerEntity)
	{
		return EntityManager.EnsureCoordinates<T>(netCoordinates, callerEntity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected HashSet<EntityUid> EnsureEntitySet<T>(HashSet<NetEntity> netEntities, EntityUid callerEntity)
	{
		return EntityManager.EnsureEntitySet<T>(netEntities, callerEntity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void EnsureEntitySet<T>(HashSet<NetEntity> netEntities, EntityUid callerEntity, HashSet<EntityUid> entities)
	{
		EntityManager.EnsureEntitySet<T>(netEntities, callerEntity, entities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected List<EntityUid> EnsureEntityList<T>(List<NetEntity> netEntities, EntityUid callerEntity)
	{
		return EntityManager.EnsureEntityList<T>(netEntities, callerEntity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void EnsureEntityList<T>(List<NetEntity> netEntities, EntityUid callerEntity, List<EntityUid> entities)
	{
		EntityManager.EnsureEntityList<T>(netEntities, callerEntity, entities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void EnsureEntityDictionary<TComp, TValue>(Dictionary<NetEntity, TValue> netEntities, EntityUid callerEntity, Dictionary<EntityUid, TValue> entities)
	{
		EntityManager.EnsureEntityDictionary<TComp, TValue>(netEntities, callerEntity, entities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void EnsureEntityDictionaryNullableValue<TComp, TValue>(Dictionary<NetEntity, TValue?> netEntities, EntityUid callerEntity, Dictionary<EntityUid, TValue?> entities)
	{
		EntityManager.EnsureEntityDictionaryNullableValue<TComp, TValue>(netEntities, callerEntity, entities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void EnsureEntityDictionary<TComp, TKey>(Dictionary<TKey, NetEntity> netEntities, EntityUid callerEntity, Dictionary<TKey, EntityUid> entities) where TKey : notnull
	{
		EntityManager.EnsureEntityDictionary<TComp, TKey>(netEntities, callerEntity, entities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void EnsureEntityDictionary<TComp, TKey>(Dictionary<TKey, NetEntity?> netEntities, EntityUid callerEntity, Dictionary<TKey, EntityUid?> entities) where TKey : notnull
	{
		EntityManager.EnsureEntityDictionary<TComp, TKey>(netEntities, callerEntity, entities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void EnsureEntityDictionary<TComp>(Dictionary<NetEntity, NetEntity> netEntities, EntityUid callerEntity, Dictionary<EntityUid, EntityUid> entities)
	{
		EntityManager.EnsureEntityDictionary<TComp>(netEntities, callerEntity, entities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void EnsureEntityDictionary<TComp>(Dictionary<NetEntity, NetEntity?> netEntities, EntityUid callerEntity, Dictionary<EntityUid, EntityUid?> entities)
	{
		EntityManager.EnsureEntityDictionary<TComp>(netEntities, callerEntity, entities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityUid GetEntity(NetEntity netEntity)
	{
		return EntityManager.GetEntity(netEntity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityUid? GetEntity(NetEntity? netEntity)
	{
		return EntityManager.GetEntity(netEntity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected HashSet<NetEntity> GetNetEntitySet(HashSet<EntityUid> uids)
	{
		return EntityManager.GetNetEntitySet(uids);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected HashSet<EntityUid> GetEntitySet(HashSet<NetEntity> netEntities)
	{
		return EntityManager.GetEntitySet(netEntities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected List<NetEntity> GetNetEntityList(ICollection<EntityUid> uids)
	{
		return EntityManager.GetNetEntityList(uids);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected List<NetEntity> GetNetEntityList(IReadOnlyList<EntityUid> uids)
	{
		return EntityManager.GetNetEntityList(uids);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected List<EntityUid> GetEntityList(ICollection<NetEntity> netEntities)
	{
		return EntityManager.GetEntityList(netEntities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected List<NetEntity> GetNetEntityList(List<EntityUid> uids)
	{
		return EntityManager.GetNetEntityList(uids);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected List<EntityUid> GetEntityList(List<NetEntity> netEntities)
	{
		return EntityManager.GetEntityList(netEntities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected List<NetEntity?> GetNetEntityList(List<EntityUid?> uids)
	{
		return EntityManager.GetNetEntityList(uids);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected List<EntityUid?> GetEntityList(List<NetEntity?> netEntities)
	{
		return EntityManager.GetEntityList(netEntities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected NetEntity[] GetNetEntityArray(EntityUid[] uids)
	{
		return EntityManager.GetNetEntityArray(uids);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityUid[] GetEntityArray(NetEntity[] netEntities)
	{
		return EntityManager.GetEntityArray(netEntities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected NetEntity?[] GetNetEntityArray(EntityUid?[] uids)
	{
		return EntityManager.GetNetEntityArray(uids);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityUid?[] GetEntityArray(NetEntity?[] netEntities)
	{
		return EntityManager.GetEntityArray(netEntities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected Dictionary<NetEntity, T> GetNetEntityDictionary<T>(Dictionary<EntityUid, T> uids)
	{
		return EntityManager.GetNetEntityDictionary(uids);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected Dictionary<T, NetEntity> GetNetEntityDictionary<T>(Dictionary<T, EntityUid> uids) where T : notnull
	{
		return EntityManager.GetNetEntityDictionary(uids);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected Dictionary<T, NetEntity?> GetNetEntityDictionary<T>(Dictionary<T, EntityUid?> uids) where T : notnull
	{
		return EntityManager.GetNetEntityDictionary(uids);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected Dictionary<NetEntity, NetEntity> GetNetEntityDictionary(Dictionary<EntityUid, EntityUid> uids)
	{
		return EntityManager.GetNetEntityDictionary(uids);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected Dictionary<NetEntity, NetEntity?> GetNetEntityDictionary(Dictionary<EntityUid, EntityUid?> uids)
	{
		return EntityManager.GetNetEntityDictionary(uids);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected Dictionary<EntityUid, T> GetEntityDictionary<T>(Dictionary<NetEntity, T> uids)
	{
		return EntityManager.GetEntityDictionary(uids);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected Dictionary<T, EntityUid> GetEntityDictionary<T>(Dictionary<T, NetEntity> uids) where T : notnull
	{
		return EntityManager.GetEntityDictionary(uids);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected Dictionary<T, EntityUid?> GetEntityDictionary<T>(Dictionary<T, NetEntity?> uids) where T : notnull
	{
		return EntityManager.GetEntityDictionary(uids);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected Dictionary<EntityUid, EntityUid> GetEntityDictionary(Dictionary<NetEntity, NetEntity> uids)
	{
		return EntityManager.GetEntityDictionary(uids);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected Dictionary<EntityUid, EntityUid?> GetEntityDictionary(Dictionary<NetEntity, NetEntity?> uids)
	{
		return EntityManager.GetEntityDictionary(uids);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected NetCoordinates GetNetCoordinates(EntityCoordinates coordinates, MetaDataComponent? metadata = null)
	{
		return EntityManager.GetNetCoordinates(coordinates, metadata);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected NetCoordinates? GetNetCoordinates(EntityCoordinates? coordinates, MetaDataComponent? metadata = null)
	{
		return EntityManager.GetNetCoordinates(coordinates, metadata);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityCoordinates GetCoordinates(NetCoordinates netEntity)
	{
		return EntityManager.GetCoordinates(netEntity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityCoordinates? GetCoordinates(NetCoordinates? netEntity)
	{
		return EntityManager.GetCoordinates(netEntity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected HashSet<EntityCoordinates> GetEntitySet(HashSet<NetCoordinates> netEntities)
	{
		return EntityManager.GetEntitySet(netEntities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected List<EntityCoordinates> GetEntityList(List<NetCoordinates> netEntities)
	{
		return EntityManager.GetEntityList(netEntities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected List<EntityCoordinates> GetEntityList(ICollection<NetCoordinates> netEntities)
	{
		return EntityManager.GetEntityList(netEntities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected List<EntityCoordinates?> GetEntityList(List<NetCoordinates?> netEntities)
	{
		return EntityManager.GetEntityList(netEntities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityCoordinates[] GetEntityArray(NetCoordinates[] netEntities)
	{
		return EntityManager.GetEntityArray(netEntities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected EntityCoordinates?[] GetEntityArray(NetCoordinates?[] netEntities)
	{
		return EntityManager.GetEntityArray(netEntities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected HashSet<NetCoordinates> GetNetCoordinatesSet(HashSet<EntityCoordinates> entities)
	{
		return EntityManager.GetNetCoordinatesSet(entities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected List<NetCoordinates> GetNetCoordinatesList(List<EntityCoordinates> entities)
	{
		return EntityManager.GetNetCoordinatesList(entities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected List<NetCoordinates> GetNetCoordinatesList(ICollection<EntityCoordinates> entities)
	{
		return EntityManager.GetNetCoordinatesList(entities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected List<NetCoordinates?> GetNetCoordinatesList(List<EntityCoordinates?> entities)
	{
		return EntityManager.GetNetCoordinatesList(entities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected NetCoordinates[] GetNetCoordinatesArray(EntityCoordinates[] entities)
	{
		return EntityManager.GetNetCoordinatesArray(entities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected NetCoordinates?[] GetNetCoordinatesArray(EntityCoordinates?[] entities)
	{
		return EntityManager.GetNetCoordinatesArray(entities);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool Resolve<TComp>(EntityUid uid, [NotNullWhen(true)] ref TComp? component, bool logMissing = true) where TComp : IComponent
	{
		if (component != null && !component.Deleted)
		{
			return true;
		}
		bool flag = EntityManager.TryGetComponent<TComp>(uid, out component);
		if (logMissing && !flag)
		{
			Log.Error($"Can't resolve \"{typeof(TComp)}\" on entity {ToPrettyString(uid)}!\n{Environment.StackTrace}");
		}
		return flag;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool Resolve(EntityUid uid, [NotNullWhen(true)] ref MetaDataComponent? component, bool logMissing = true)
	{
		return EntityManager.MetaQuery.Resolve(uid, ref component, logMissing);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool Resolve(EntityUid uid, [NotNullWhen(true)] ref TransformComponent? component, bool logMissing = true)
	{
		return EntityManager.TransformQuery.Resolve(uid, ref component, logMissing);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool Resolve<TComp1, TComp2>(EntityUid uid, [NotNullWhen(true)] ref TComp1? comp1, [NotNullWhen(true)] ref TComp2? comp2, bool logMissing = true) where TComp1 : IComponent where TComp2 : IComponent
	{
		return Resolve(uid, ref comp1, logMissing) & Resolve(uid, ref comp2, logMissing);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool Resolve<TComp1, TComp2, TComp3>(EntityUid uid, [NotNullWhen(true)] ref TComp1? comp1, [NotNullWhen(true)] ref TComp2? comp2, [NotNullWhen(true)] ref TComp3? comp3, bool logMissing = true) where TComp1 : IComponent where TComp2 : IComponent where TComp3 : IComponent
	{
		return Resolve(uid, ref comp1, ref comp2, logMissing) & Resolve(uid, ref comp3, logMissing);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool Resolve<TComp1, TComp2, TComp3, TComp4>(EntityUid uid, [NotNullWhen(true)] ref TComp1? comp1, [NotNullWhen(true)] ref TComp2? comp2, [NotNullWhen(true)] ref TComp3? comp3, [NotNullWhen(true)] ref TComp4? comp4, bool logMissing = true) where TComp1 : IComponent where TComp2 : IComponent where TComp3 : IComponent where TComp4 : IComponent
	{
		return Resolve(uid, ref comp1, ref comp2, logMissing) & Resolve(uid, ref comp3, ref comp4, logMissing);
	}

	protected void SubscribeNetworkEvent<T>(EntityEventHandler<T> handler, Type[]? before = null, Type[]? after = null) where T : notnull
	{
		SubEvent(EventSource.Network, handler, before, after);
	}

	protected void SubscribeLocalEvent<T>(EntityEventHandler<T> handler, Type[]? before = null, Type[]? after = null) where T : notnull
	{
		SubEvent(EventSource.Local, handler, before, after);
	}

	protected void SubscribeLocalEvent<T>(EntityEventRefHandler<T> handler, Type[]? before = null, Type[]? after = null) where T : notnull
	{
		SubEvent(EventSource.Local, handler, before, after);
	}

	protected void SubscribeAllEvent<T>(EntityEventHandler<T> handler, Type[]? before = null, Type[]? after = null) where T : notnull
	{
		SubEvent(EventSource.All, handler, before, after);
	}

	protected void SubscribeNetworkEvent<T>(EntitySessionEventHandler<T> handler, Type[]? before = null, Type[]? after = null) where T : notnull
	{
		SubSessionEvent(EventSource.Network, handler, before, after);
	}

	protected void SubscribeLocalEvent<T>(EntitySessionEventHandler<T> handler, Type[]? before = null, Type[]? after = null) where T : notnull
	{
		SubSessionEvent(EventSource.Local, handler, before, after);
	}

	protected void SubscribeAllEvent<T>(EntitySessionEventHandler<T> handler, Type[]? before = null, Type[]? after = null) where T : notnull
	{
		SubSessionEvent(EventSource.All, handler, before, after);
	}

	private void SubEvent<T>(EventSource src, EntityEventHandler<T> handler, Type[]? before, Type[]? after) where T : notnull
	{
		EntityManager.EventBus.SubscribeEvent(src, this, handler, GetType(), before, after);
		_subscriptions.Add(new SubBroadcast<T>(src));
	}

	private void SubEvent<T>(EventSource src, EntityEventRefHandler<T> handler, Type[]? before, Type[]? after) where T : notnull
	{
		EntityManager.EventBus.SubscribeEvent(src, this, handler, GetType(), before, after);
		_subscriptions.Add(new SubBroadcast<T>(src));
	}

	private void SubSessionEvent<T>(EventSource src, EntitySessionEventHandler<T> handler, Type[]? before, Type[]? after) where T : notnull
	{
		EntityManager.EventBus.SubscribeSessionEvent(src, this, handler, GetType(), before, after);
		_subscriptions.Add(new SubBroadcast<EntitySessionMessage<T>>(src));
	}

	protected void SubscribeLocalEvent<TComp, TEvent>(EntityEventRefHandler<TComp, TEvent> handler, Type[]? before = null, Type[]? after = null) where TComp : IComponent where TEvent : notnull
	{
		EntityManager.EventBus.SubscribeLocalEvent(handler, GetType(), before, after);
		_subscriptions.Add(new SubLocal<TComp, TEvent>());
	}

	protected void SubscribeLocalEvent<TComp, TEvent>(ComponentEventHandler<TComp, TEvent> handler, Type[]? before = null, Type[]? after = null) where TComp : IComponent where TEvent : notnull
	{
		EntityManager.EventBus.SubscribeLocalEvent(handler, GetType(), before, after);
		_subscriptions.Add(new SubLocal<TComp, TEvent>());
	}

	protected void SubscribeLocalEvent<TComp, TEvent>(ComponentEventRefHandler<TComp, TEvent> handler, Type[]? before = null, Type[]? after = null) where TComp : IComponent where TEvent : notnull
	{
		EntityManager.EventBus.SubscribeLocalEvent(handler, GetType(), before, after);
		_subscriptions.Add(new SubLocal<TComp, TEvent>());
	}

	private void ShutdownSubscriptions()
	{
		foreach (SubBase subscription in _subscriptions)
		{
			subscription.Unsubscribe(this, EntityManager.EventBusInternal);
		}
		_subscriptions = default(ValueList<SubBase>);
	}
}
