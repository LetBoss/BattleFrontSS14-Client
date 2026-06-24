// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.EntitySystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.GameObjects;

[Reflect(false)]
public abstract class EntitySystem : IEntitySystem, IEntityEventSubscriber, IPostInjectInit
{
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
  private ValueList<EntitySystem.SubBase> _subscriptions;

  protected IComponentFactory Factory => this.EntityManager.ComponentFactory;

  public ISawmill Log { get; private set; }

  protected virtual string SawmillName
  {
    get
    {
      string source = this.GetType().Name;
      if (source.EndsWith("System"))
        source = source.Substring(0, source.Length - "System".Length);
      return "system." + (!source.All<char>(EntitySystem.\u003C\u003EO.\u003C0\u003E__IsUpper ?? (EntitySystem.\u003C\u003EO.\u003C0\u003E__IsUpper = new Func<char, bool>(char.IsUpper))) ? string.Concat(source.Select<char, string>((Func<char, string>) (x =>
      {
        if (!char.IsUpper(x))
          return x.ToString();
        return $"_{char.ToLower(x)}";
      }))).Trim('_') : source.ToLower(CultureInfo.InvariantCulture));
    }
  }

  protected internal List<Type> UpdatesAfter { get; } = new List<Type>();

  protected internal List<Type> UpdatesBefore { get; } = new List<Type>();

  public bool UpdatesOutsidePrediction { get; protected internal set; }

  IEnumerable<Type> IEntitySystem.UpdatesAfter => (IEnumerable<Type>) this.UpdatesAfter;

  IEnumerable<Type> IEntitySystem.UpdatesBefore => (IEnumerable<Type>) this.UpdatesBefore;

  protected EntitySystem() => this.Subs = new EntitySystem.Subscriptions(this);

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
  public virtual void Shutdown() => this.ShutdownSubscriptions();

  protected void RaiseLocalEvent<T>(T message) where T : notnull
  {
    this.EntityManager.EventBusInternal.RaiseEvent<T>(EventSource.Local, message);
  }

  protected void RaiseLocalEvent<T>(ref T message) where T : notnull
  {
    this.EntityManager.EventBusInternal.RaiseEvent<T>(EventSource.Local, ref message);
  }

  protected void RaiseLocalEvent(object message)
  {
    this.EntityManager.EventBusInternal.RaiseEvent(EventSource.Local, message);
  }

  protected void QueueLocalEvent(EntityEventArgs message)
  {
    this.EntityManager.EventBusInternal.QueueEvent(EventSource.Local, message);
  }

  protected void RaiseNetworkEvent(EntityEventArgs message)
  {
    this.EntityManager.EntityNetManager?.SendSystemNetworkMessage(message);
  }

  protected void RaiseNetworkEvent(EntityEventArgs message, INetChannel channel)
  {
    this.EntityManager.EntityNetManager?.SendSystemNetworkMessage(message, channel);
  }

  protected void RaiseNetworkEvent(EntityEventArgs message, ICommonSession session)
  {
    this.EntityManager.EntityNetManager?.SendSystemNetworkMessage(message, session.Channel);
  }

  protected void RaiseNetworkEvent(EntityEventArgs message, Filter filter, bool recordReplay = true)
  {
    if (recordReplay)
      this._replayMan.RecordServerMessage((object) message);
    foreach (ICommonSession recipient in filter.Recipients)
      this.EntityManager.EntityNetManager?.SendSystemNetworkMessage(message, recipient.Channel);
  }

  protected void RaiseNetworkEvent(EntityEventArgs message, EntityUid recipient)
  {
    ICommonSession session;
    if (!this._playerMan.TryGetSessionByEntity(recipient, out session))
      return;
    this.EntityManager.EntityNetManager?.SendSystemNetworkMessage(message, session.Channel);
  }

  protected void RaiseLocalEvent<TEvent>(EntityUid uid, TEvent args, bool broadcast = false) where TEvent : notnull
  {
    this.EntityManager.EventBusInternal.RaiseLocalEvent<TEvent>(uid, args, broadcast);
  }

  protected void RaiseLocalEvent(EntityUid uid, object args, bool broadcast = false)
  {
    this.EntityManager.EventBusInternal.RaiseLocalEvent(uid, args, broadcast);
  }

  protected void RaiseLocalEvent<TEvent>(EntityUid uid, ref TEvent args, bool broadcast = false) where TEvent : notnull
  {
    this.EntityManager.EventBusInternal.RaiseLocalEvent<TEvent>(uid, ref args, broadcast);
  }

  protected void RaiseLocalEvent(EntityUid uid, ref object args, bool broadcast = false)
  {
    this.EntityManager.EventBusInternal.RaiseLocalEvent(uid, ref args, broadcast);
  }

  protected void RaiseComponentEvent<TEvent, TComp>(EntityUid uid, TComp comp, ref TEvent args)
    where TEvent : notnull
    where TComp : IComponent
  {
    this.EntityManager.EventBusInternal.RaiseComponentEvent<TEvent, TComp>(uid, comp, ref args);
  }

  public void RaiseComponentEvent<TEvent>(EntityUid uid, IComponent component, ref TEvent args) where TEvent : notnull
  {
    this.EntityManager.EventBusInternal.RaiseComponentEvent<TEvent>(uid, component, ref args);
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

  void IPostInjectInit.PostInject() => this.PostInject();

  protected virtual void PostInject()
  {
    this.Log = this.LogManager.GetSawmill(this.SawmillName);
    this.Log.Level = new LogLevel?(LogLevel.Info);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool Exists(EntityUid uid) => this.EntityManager.EntityExists(uid);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool Exists([NotNullWhen(true)] EntityUid? uid) => this.EntityManager.EntityExists(uid);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool Initializing(EntityUid uid, MetaDataComponent? metaData = null)
  {
    return this.LifeStage(uid, metaData) == EntityLifeStage.Initializing;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool Initialized(EntityUid uid, MetaDataComponent? metaData = null)
  {
    EntityLifeStage entityLifeStage = this.LifeStage(uid, metaData);
    return entityLifeStage >= EntityLifeStage.Initialized && entityLifeStage < EntityLifeStage.Terminating;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool Terminating(EntityUid uid, MetaDataComponent? metaData = null)
  {
    return this.LifeStage(uid, metaData) == EntityLifeStage.Terminating;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool Deleted(EntityUid uid, MetaDataComponent? metaData = null)
  {
    return this.LifeStage(uid, metaData) >= EntityLifeStage.Deleted;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool TerminatingOrDeleted(EntityUid uid, MetaDataComponent? metaData = null)
  {
    return this.LifeStage(uid, metaData) >= EntityLifeStage.Terminating;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool TerminatingOrDeleted(EntityUid? uid, MetaDataComponent? metaData = null)
  {
    return !uid.HasValue || this.TerminatingOrDeleted(uid.Value, metaData);
  }

  [Obsolete("Use override without the EntityQuery")]
  protected bool Deleted(EntityUid uid, Robust.Shared.GameObjects.EntityQuery<MetaDataComponent> metaQuery)
  {
    return this.Deleted(uid);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool Deleted([NotNullWhen(false)] EntityUid? uid)
  {
    return !uid.HasValue || this.Deleted(uid.Value);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityLifeStage LifeStage(EntityUid uid, MetaDataComponent? metaData = null)
  {
    return !this.EntityManager.MetaQuery.Resolve(uid, ref metaData, false) ? EntityLifeStage.Deleted : metaData.EntityLifeStage;
  }

  [Obsolete("Use LifeStage()")]
  protected bool TryLifeStage(
    EntityUid uid,
    [NotNullWhen(true)] out EntityLifeStage? lifeStage,
    MetaDataComponent? metaData = null)
  {
    if (!this.EntityManager.MetaQuery.Resolve(uid, ref metaData, false))
    {
      lifeStage = new EntityLifeStage?();
      return false;
    }
    lifeStage = new EntityLifeStage?(metaData.EntityLifeStage);
    return true;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool IsPaused(EntityUid? uid, MetaDataComponent? metadata = null)
  {
    return this.EntityManager.IsPaused(uid, metadata);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void DirtyEntity(EntityUid uid, MetaDataComponent? meta = null)
  {
    this.EntityManager.DirtyEntity(uid, meta);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void Dirty(EntityUid uid, IComponent component, MetaDataComponent? meta = null)
  {
    this.EntityManager.Dirty(uid, component, meta);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void DirtyField(
    EntityUid uid,
    IComponentDelta delta,
    string fieldName,
    MetaDataComponent? meta = null)
  {
    this.EntityManager.DirtyField(uid, delta, fieldName, meta);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void DirtyField<T>(Entity<T?> entity, [ValidateMember] string fieldName, MetaDataComponent? meta = null) where T : IComponentDelta
  {
    if (!this.Resolve<T>(entity.Owner, ref entity.Comp))
      return;
    this.EntityManager.DirtyField<T>(entity.Owner, entity.Comp, fieldName, meta);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void DirtyField<T>(
    EntityUid uid,
    T component,
    [ValidateMember] string fieldName,
    MetaDataComponent? meta = null)
    where T : IComponentDelta
  {
    this.EntityManager.DirtyField<T>(uid, component, fieldName, meta);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void DirtyFields<T>(
    EntityUid uid,
    T comp,
    MetaDataComponent? meta,
    params string[] fields)
    where T : IComponentDelta
  {
    this.EntityManager.DirtyFields<T>(uid, comp, meta, fields);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void DirtyFields<T>(Entity<T?> ent, MetaDataComponent? meta, params string[] fields) where T : IComponentDelta
  {
    if (!this.Resolve<T>((EntityUid) ent, ref ent.Comp))
      return;
    this.EntityManager.DirtyFields<T>((EntityUid) ent, ent.Comp, meta, fields);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void Dirty<T>(Entity<T> ent, MetaDataComponent? meta = null) where T : IComponent?
  {
    T component = ent.Comp;
    if ((object) component == null && !this.EntityManager.TryGetComponent<T>(ent.Owner, out component))
      return;
    this.EntityManager.Dirty((EntityUid) ent, (IComponent) component, meta);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void Dirty<T1, T2>(Entity<T1, T2> ent, MetaDataComponent? meta = null)
    where T1 : IComponent
    where T2 : IComponent
  {
    this.EntityManager.Dirty<T1, T2>(ent, meta);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void Dirty<T1, T2, T3>(Entity<T1, T2, T3> ent, MetaDataComponent? meta = null)
    where T1 : IComponent
    where T2 : IComponent
    where T3 : IComponent
  {
    this.EntityManager.Dirty<T1, T2, T3>(ent, meta);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void Dirty<T1, T2, T3, T4>(Entity<T1, T2, T3, T4> ent, MetaDataComponent? meta = null)
    where T1 : IComponent
    where T2 : IComponent
    where T3 : IComponent
    where T4 : IComponent
  {
    this.EntityManager.Dirty<T1, T2, T3, T4>(ent, meta);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected string Name(EntityUid uid, MetaDataComponent? metaData = null)
  {
    if (!this.EntityManager.MetaQuery.Resolve(uid, ref metaData, false))
      throw EntitySystem.CompNotFound<MetaDataComponent>(uid);
    return metaData.EntityName;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected string Description(EntityUid uid, MetaDataComponent? metaData = null)
  {
    if (!this.EntityManager.MetaQuery.Resolve(uid, ref metaData, false))
      throw EntitySystem.CompNotFound<MetaDataComponent>(uid);
    return metaData.EntityDescription;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityPrototype? Prototype(EntityUid uid, MetaDataComponent? metaData = null)
  {
    if (!this.EntityManager.MetaQuery.Resolve(uid, ref metaData, false))
      throw EntitySystem.CompNotFound<MetaDataComponent>(uid);
    return metaData.EntityPrototype;
  }

  protected GameTick LastModifiedTick(EntityUid uid, MetaDataComponent? metaData = null)
  {
    if (!this.EntityManager.MetaQuery.Resolve(uid, ref metaData, false))
      throw EntitySystem.CompNotFound<MetaDataComponent>(uid);
    return metaData.EntityLastModifiedTick;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool Paused(EntityUid uid, MetaDataComponent? metaData = null)
  {
    if (!this.EntityManager.MetaQuery.Resolve(uid, ref metaData, false))
      throw EntitySystem.CompNotFound<MetaDataComponent>(uid);
    return metaData.EntityPaused;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void SetPaused(EntityUid uid, bool paused, MetaDataComponent? metaData = null)
  {
    if (!this.EntityManager.MetaQuery.Resolve(uid, ref metaData, false))
      throw EntitySystem.CompNotFound<MetaDataComponent>(uid);
    this.EntityManager.EntitySysManager.GetEntitySystem<MetaDataSystem>().SetEntityPaused(uid, paused, metaData);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool TryDirty(EntityUid uid, MetaDataComponent? metaData = null)
  {
    if (!this.EntityManager.MetaQuery.Resolve(uid, ref metaData, false))
      return false;
    this.DirtyEntity(uid, metaData);
    return true;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool TryName(EntityUid uid, [NotNullWhen(true)] out string? name, MetaDataComponent? metaData = null)
  {
    if (!this.EntityManager.MetaQuery.Resolve(uid, ref metaData, false))
    {
      name = (string) null;
      return false;
    }
    name = metaData.EntityName;
    return true;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool TryDescription(EntityUid uid, [NotNullWhen(true)] out string? description, MetaDataComponent? metaData = null)
  {
    if (!this.EntityManager.MetaQuery.Resolve(uid, ref metaData, false))
    {
      description = (string) null;
      return false;
    }
    description = metaData.EntityDescription;
    return true;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool TryPrototype(
    EntityUid uid,
    [NotNullWhen(true)] out EntityPrototype? prototype,
    MetaDataComponent? metaData = null)
  {
    if (!this.EntityManager.MetaQuery.Resolve(uid, ref metaData, false))
    {
      prototype = (EntityPrototype) null;
      return false;
    }
    prototype = metaData.EntityPrototype;
    return prototype != null;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool TryLastModifiedTick(
    EntityUid uid,
    [NotNullWhen(true)] out GameTick? lastModifiedTick,
    MetaDataComponent? metaData = null)
  {
    if (!this.EntityManager.MetaQuery.Resolve(uid, ref metaData, false))
    {
      lastModifiedTick = new GameTick?();
      return false;
    }
    lastModifiedTick = new GameTick?(metaData.EntityLastModifiedTick);
    return true;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool TryPaused(EntityUid uid, [NotNullWhen(true)] out bool? paused, MetaDataComponent? metaData = null)
  {
    if (!this.EntityManager.MetaQuery.Resolve(uid, ref metaData, false))
    {
      paused = new bool?();
      return false;
    }
    paused = new bool?(metaData.EntityPaused);
    return true;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  [return: NotNullIfNotNull("uid")]
  protected EntityStringRepresentation? ToPrettyString(EntityUid? uid, MetaDataComponent? metadata = null)
  {
    return this.EntityManager.ToPrettyString(uid, metadata);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  [return: NotNullIfNotNull("netEntity")]
  protected EntityStringRepresentation? ToPrettyString(NetEntity? netEntity)
  {
    return this.EntityManager.ToPrettyString(netEntity);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityStringRepresentation ToPrettyString(EntityUid uid, MetaDataComponent? metadata)
  {
    return this.EntityManager.ToPrettyString((Entity<MetaDataComponent>) (uid, metadata));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityStringRepresentation ToPrettyString(Entity<MetaDataComponent?> entity)
  {
    return this.EntityManager.ToPrettyString(entity);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityStringRepresentation ToPrettyString(NetEntity netEntity)
  {
    return this.EntityManager.ToPrettyString(netEntity);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected T Comp<T>(EntityUid uid) where T : IComponent
  {
    return this.EntityManager.GetComponent<T>(uid);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected T? CompOrNull<T>(EntityUid uid) where T : IComponent
  {
    return this.EntityManager.GetComponentOrNull<T>(uid);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected T? CompOrNull<T>(EntityUid? uid) where T : IComponent
  {
    return !uid.HasValue ? default (T) : this.EntityManager.GetComponentOrNull<T>(uid.Value);
  }

  [PreferNonGenericVariantFor(new Type[] {typeof (TransformComponent), typeof (MetaDataComponent)})]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool TryComp<T>(EntityUid uid, [NotNullWhen(true)] out T? comp) where T : IComponent
  {
    return this.EntityManager.TryGetComponent<T>(uid, out comp);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool TryComp(EntityUid uid, [NotNullWhen(true)] out TransformComponent? comp)
  {
    return this.EntityManager.TransformQuery.TryGetComponent(uid, out comp);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool TryComp(EntityUid uid, [NotNullWhen(true)] out MetaDataComponent? comp)
  {
    return this.EntityManager.MetaQuery.TryGetComponent(uid, out comp);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool TryComp<T>([NotNullWhen(true)] EntityUid? uid, [NotNullWhen(true)] out T? comp) where T : IComponent
  {
    if (uid.HasValue)
      return this.EntityManager.TryGetComponent<T>(uid.Value, out comp);
    comp = default (T);
    return false;
  }

  protected bool TryComp([NotNullWhen(true)] EntityUid? uid, [NotNullWhen(true)] out TransformComponent? comp)
  {
    if (uid.HasValue)
      return this.EntityManager.TransformQuery.TryGetComponent(uid.Value, out comp);
    comp = (TransformComponent) null;
    return false;
  }

  protected bool TryComp([NotNullWhen(true)] EntityUid? uid, [NotNullWhen(true)] out MetaDataComponent? comp)
  {
    if (uid.HasValue)
      return this.EntityManager.MetaQuery.TryGetComponent(uid.Value, out comp);
    comp = (MetaDataComponent) null;
    return false;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected IEnumerable<IComponent> AllComps(EntityUid uid)
  {
    return this.EntityManager.GetComponents(uid);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected IEnumerable<T> AllComps<T>(EntityUid uid) => this.EntityManager.GetComponents<T>(uid);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected TransformComponent Transform(EntityUid uid)
  {
    return this.EntityManager.TransformQuery.GetComponent(uid);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected MetaDataComponent MetaData(EntityUid uid)
  {
    return this.EntityManager.MetaQuery.GetComponent(uid);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected (EntityUid, MetaDataComponent) GetEntityData(NetEntity nuid)
  {
    return this.EntityManager.GetEntityData(nuid);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool TryGetEntityData(NetEntity nuid, [NotNullWhen(true)] out EntityUid? uid, [NotNullWhen(true)] out MetaDataComponent? meta)
  {
    return this.EntityManager.TryGetEntityData(nuid, out uid, out meta);
  }

  protected bool TryCopyComponent<T>(
    EntityUid source,
    EntityUid target,
    ref T? sourceComponent,
    [NotNullWhen(true)] out T? targetComp,
    MetaDataComponent? meta = null)
    where T : IComponent
  {
    return this.EntityManager.TryCopyComponent<T>(source, target, ref sourceComponent, out targetComp, meta);
  }

  protected bool TryCopyComponents(
    EntityUid source,
    EntityUid target,
    MetaDataComponent? meta = null,
    params Type[] sourceComponents)
  {
    return this.EntityManager.TryCopyComponents(source, target, meta, sourceComponents);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected IComponent CopyComp(
    EntityUid source,
    EntityUid target,
    IComponent sourceComponent,
    MetaDataComponent? meta = null)
  {
    return this.EntityManager.CopyComponent(source, target, sourceComponent, meta);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected T CopyComp<T>(
    EntityUid source,
    EntityUid target,
    T sourceComponent,
    MetaDataComponent? meta = null)
    where T : IComponent
  {
    return this.EntityManager.CopyComponent<T>(source, target, sourceComponent, meta);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void CopyComps(
    EntityUid source,
    EntityUid target,
    MetaDataComponent? meta = null,
    params IComponent[] sourceComponents)
  {
    this.EntityManager.CopyComponents(source, target, meta, sourceComponents);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool HasComp<T>(EntityUid uid) where T : IComponent
  {
    return this.EntityManager.HasComponent<T>(uid);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool HasComp(EntityUid uid, Type type) => this.EntityManager.HasComponent(uid, type);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool HasComp<T>([NotNullWhen(true)] EntityUid? uid) where T : IComponent
  {
    return this.EntityManager.HasComponent<T>(uid);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool HasComp([NotNullWhen(true)] EntityUid? uid, Type type)
  {
    return this.EntityManager.HasComponent(uid, type);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected T AddComp<T>(EntityUid uid) where T : IComponent, new()
  {
    return this.EntityManager.AddComponent<T>(uid);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void AddComp<T>(EntityUid uid, T component, bool overwrite = false) where T : IComponent
  {
    this.EntityManager.AddComponent<T>(uid, component, overwrite, (MetaDataComponent) null);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected T EnsureComp<T>(EntityUid uid) where T : IComponent, new()
  {
    return this.EntityManager.EnsureComponent<T>(uid);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool EnsureComp<T>(EntityUid uid, out T comp) where T : IComponent, new()
  {
    return this.EntityManager.EnsureComponent<T>(uid, out comp);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool EnsureComp<T>(ref Entity<T?> entity) where T : IComponent, new()
  {
    return this.EntityManager.EnsureComponent<T>(ref entity);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool RemCompDeferred<T>(EntityUid uid) where T : IComponent
  {
    return this.EntityManager.RemoveComponentDeferred<T>(uid);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool RemCompDeferred(EntityUid uid, Type type)
  {
    return this.EntityManager.RemoveComponentDeferred(uid, type);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void RemCompDeferred(EntityUid uid, IComponent component)
  {
    this.EntityManager.RemoveComponentDeferred(uid, component);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected int Count<T>() where T : IComponent => this.EntityManager.Count<T>();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected int Count(Type type) => this.EntityManager.Count(type);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool RemComp<T>(EntityUid uid) where T : IComponent
  {
    return this.EntityManager.RemoveComponent<T>(uid, (MetaDataComponent) null);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool RemComp(EntityUid uid, Type type)
  {
    return this.EntityManager.RemoveComponent(uid, type, (MetaDataComponent) null);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void RemComp(EntityUid uid, IComponent component)
  {
    this.EntityManager.RemoveComponent(uid, component, (MetaDataComponent) null);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void Del(EntityUid? uid) => this.EntityManager.DeleteEntity(uid);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void QueueDel(EntityUid? uid) => this.EntityManager.QueueDeleteEntity(uid);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void PredictedDel(Entity<MetaDataComponent?, TransformComponent?> ent)
  {
    this.EntityManager.PredictedDeleteEntity(ent);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void PredictedDel(Entity<MetaDataComponent?, TransformComponent?>? ent)
  {
    this.EntityManager.PredictedDeleteEntity(ent);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void PredictedQueueDel(Entity<MetaDataComponent?> ent)
  {
    this.EntityManager.PredictedQueueDeleteEntity(ent);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void PredictedQueueDel(Entity<MetaDataComponent?>? ent)
  {
    this.EntityManager.PredictedQueueDeleteEntity(ent);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void PredictedQueueDel(EntityUid uid)
  {
    this.EntityManager.PredictedQueueDeleteEntity(uid);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void PredictedQueueDel(EntityUid? uid)
  {
    this.EntityManager.PredictedQueueDeleteEntity(uid);
  }

  [Obsolete("use variant without TransformComponent")]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void PredictedQueueDel(Entity<MetaDataComponent?, TransformComponent?> ent)
  {
    this.EntityManager.PredictedQueueDeleteEntity(ent);
  }

  [Obsolete("use variant without TransformComponent")]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void PredictedQueueDel(Entity<MetaDataComponent?, TransformComponent?>? ent)
  {
    this.EntityManager.PredictedQueueDeleteEntity(ent);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool TryQueueDel(EntityUid? uid) => this.EntityManager.TryQueueDeleteEntity(uid);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityUid Spawn(string? prototype, EntityCoordinates coordinates)
  {
    return this.EntityManager.SpawnEntity(prototype, coordinates, (ComponentRegistry) null);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityUid Spawn(
    string? prototype,
    MapCoordinates coordinates,
    ComponentRegistry? overrides = null,
    Angle rotation = default (Angle))
  {
    return this.EntityManager.Spawn(prototype, coordinates, overrides, rotation);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityUid Spawn(string? prototype = null, ComponentRegistry? overrides = null, bool doMapInit = true)
  {
    return this.EntityManager.Spawn(prototype, overrides, doMapInit);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityUid SpawnAttachedTo(
    string? prototype,
    EntityCoordinates coordinates,
    ComponentRegistry? overrides = null,
    Angle rotation = default (Angle))
  {
    return this.EntityManager.SpawnAttachedTo(prototype, coordinates, overrides, rotation);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityUid SpawnAtPosition(
    string? prototype,
    EntityCoordinates coordinates,
    ComponentRegistry? overrides = null)
  {
    return this.EntityManager.SpawnAtPosition(prototype, coordinates, overrides);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool TrySpawnInContainer(
    string? protoName,
    EntityUid containerUid,
    string containerId,
    [NotNullWhen(true)] out EntityUid? uid,
    ContainerManagerComponent? containerComp = null,
    ComponentRegistry? overrides = null)
  {
    return this.EntityManager.TrySpawnInContainer(protoName, containerUid, containerId, out uid, containerComp, overrides);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool TrySpawnNextTo(
    string? protoName,
    EntityUid target,
    [NotNullWhen(true)] out EntityUid? uid,
    TransformComponent? xform = null,
    ComponentRegistry? overrides = null)
  {
    return this.EntityManager.TrySpawnNextTo(protoName, target, out uid, xform, overrides);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityUid SpawnNextToOrDrop(
    string? protoName,
    EntityUid target,
    TransformComponent? xform = null,
    ComponentRegistry? overrides = null)
  {
    return this.EntityManager.SpawnNextToOrDrop(protoName, target, xform, overrides);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityUid SpawnInContainerOrDrop(
    string? protoName,
    EntityUid containerUid,
    string containerId,
    TransformComponent? xform = null,
    ContainerManagerComponent? container = null,
    ComponentRegistry? overrides = null)
  {
    return this.EntityManager.SpawnInContainerOrDrop(protoName, containerUid, containerId, xform, container, overrides);
  }

  protected void FlagPredicted(Entity<MetaDataComponent?> ent)
  {
    this.EntityManager.FlagPredicted(ent);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityUid PredictedSpawnAttachedTo(
    string? prototype,
    EntityCoordinates coordinates,
    ComponentRegistry? overrides = null,
    Angle rotation = default (Angle))
  {
    return this.EntityManager.PredictedSpawnAttachedTo(prototype, coordinates, overrides, rotation);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityUid PredictedSpawnAtPosition(
    string? prototype,
    EntityCoordinates coordinates,
    ComponentRegistry? overrides = null)
  {
    return this.EntityManager.PredictedSpawnAtPosition(prototype, coordinates, overrides);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool PredictedTrySpawnInContainer(
    string? protoName,
    EntityUid containerUid,
    string containerId,
    [NotNullWhen(true)] out EntityUid? uid,
    ContainerManagerComponent? containerComp = null,
    ComponentRegistry? overrides = null)
  {
    return this.EntityManager.PredictedTrySpawnInContainer(protoName, containerUid, containerId, out uid, containerComp, overrides);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool PredictedTrySpawnNextTo(
    string? protoName,
    EntityUid target,
    [NotNullWhen(true)] out EntityUid? uid,
    TransformComponent? xform = null,
    ComponentRegistry? overrides = null)
  {
    return this.EntityManager.PredictedTrySpawnNextTo(protoName, target, out uid, xform, overrides);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityUid PredictedSpawnNextToOrDrop(
    string? protoName,
    EntityUid target,
    TransformComponent? xform = null,
    ComponentRegistry? overrides = null)
  {
    return this.EntityManager.PredictedSpawnNextToOrDrop(protoName, target, xform, overrides);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityUid PredictedSpawnInContainerOrDrop(
    string? protoName,
    EntityUid containerUid,
    string containerId,
    TransformComponent? xform = null,
    ContainerManagerComponent? container = null,
    ComponentRegistry? overrides = null)
  {
    return this.EntityManager.PredictedSpawnInContainerOrDrop(protoName, containerUid, containerId, xform, container, overrides);
  }

  private static KeyNotFoundException CompNotFound<T>(EntityUid uid)
  {
    return new KeyNotFoundException($"Entity {uid} does not have a component of type {typeof (T)}");
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected AllEntityQueryEnumerator<TComp1> AllEntityQuery<TComp1>() where TComp1 : IComponent
  {
    return this.EntityManager.AllEntityQueryEnumerator<TComp1>();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected AllEntityQueryEnumerator<TComp1, TComp2> AllEntityQuery<TComp1, TComp2>()
    where TComp1 : IComponent
    where TComp2 : IComponent
  {
    return this.EntityManager.AllEntityQueryEnumerator<TComp1, TComp2>();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected AllEntityQueryEnumerator<TComp1, TComp2, TComp3> AllEntityQuery<TComp1, TComp2, TComp3>()
    where TComp1 : IComponent
    where TComp2 : IComponent
    where TComp3 : IComponent
  {
    return this.EntityManager.AllEntityQueryEnumerator<TComp1, TComp2, TComp3>();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected AllEntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4> AllEntityQuery<TComp1, TComp2, TComp3, TComp4>()
    where TComp1 : IComponent
    where TComp2 : IComponent
    where TComp3 : IComponent
    where TComp4 : IComponent
  {
    return this.EntityManager.AllEntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4>();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected Robust.Shared.GameObjects.EntityQueryEnumerator<TComp1> EntityQueryEnumerator<TComp1>() where TComp1 : IComponent
  {
    return this.EntityManager.EntityQueryEnumerator<TComp1>();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected Robust.Shared.GameObjects.EntityQueryEnumerator<TComp1, TComp2> EntityQueryEnumerator<TComp1, TComp2>()
    where TComp1 : IComponent
    where TComp2 : IComponent
  {
    return this.EntityManager.EntityQueryEnumerator<TComp1, TComp2>();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected Robust.Shared.GameObjects.EntityQueryEnumerator<TComp1, TComp2, TComp3> EntityQueryEnumerator<TComp1, TComp2, TComp3>()
    where TComp1 : IComponent
    where TComp2 : IComponent
    where TComp3 : IComponent
  {
    return this.EntityManager.EntityQueryEnumerator<TComp1, TComp2, TComp3>();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected Robust.Shared.GameObjects.EntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4> EntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4>()
    where TComp1 : IComponent
    where TComp2 : IComponent
    where TComp3 : IComponent
    where TComp4 : IComponent
  {
    return this.EntityManager.EntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4>();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected Robust.Shared.GameObjects.EntityQuery<T> GetEntityQuery<T>() where T : IComponent
  {
    return this.EntityManager.GetEntityQuery<T>();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected IEnumerable<TComp1> EntityQuery<TComp1>(bool includePaused = false) where TComp1 : IComponent
  {
    return this.EntityManager.EntityQuery<TComp1>(includePaused);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected IEnumerable<(TComp1, TComp2)> EntityQuery<TComp1, TComp2>(bool includePaused = false)
    where TComp1 : IComponent
    where TComp2 : IComponent
  {
    return this.EntityManager.EntityQuery<TComp1, TComp2>(includePaused);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected IEnumerable<(TComp1, TComp2, TComp3)> EntityQuery<TComp1, TComp2, TComp3>(
    bool includePaused = false)
    where TComp1 : IComponent
    where TComp2 : IComponent
    where TComp3 : IComponent
  {
    return this.EntityManager.EntityQuery<TComp1, TComp2, TComp3>(includePaused);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected IEnumerable<(TComp1, TComp2, TComp3, TComp4)> EntityQuery<TComp1, TComp2, TComp3, TComp4>(
    bool includePaused = false)
    where TComp1 : IComponent
    where TComp2 : IComponent
    where TComp3 : IComponent
    where TComp4 : IComponent
  {
    return this.EntityManager.EntityQuery<TComp1, TComp2, TComp3, TComp4>(includePaused);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void RaisePredictiveEvent<T>(T msg) where T : EntityEventArgs
  {
    this.EntityManager.RaisePredictiveEvent<T>(msg);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool IsClientSide(EntityUid entity, MetaDataComponent? meta = null)
  {
    return this.EntityManager.IsClientSide(entity, meta);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool IsClientSide(Entity<MetaDataComponent> entity)
  {
    return this.EntityManager.IsClientSide((EntityUid) entity, entity.Comp);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool TryGetEntity(NetEntity nEntity, [NotNullWhen(true)] out EntityUid? entity)
  {
    return this.EntityManager.TryGetEntity(nEntity, out entity);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool TryGetEntity(NetEntity? nEntity, [NotNullWhen(true)] out EntityUid? entity)
  {
    return this.EntityManager.TryGetEntity(nEntity, out entity);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool TryGetNetEntity(EntityUid uid, [NotNullWhen(true)] out NetEntity? netEntity, MetaDataComponent? metadata = null)
  {
    return this.EntityManager.TryGetNetEntity(uid, out netEntity, (MetaDataComponent) null);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool TryGetNetEntity(EntityUid? uid, [NotNullWhen(true)] out NetEntity? netEntity, MetaDataComponent? metadata = null)
  {
    return this.EntityManager.TryGetNetEntity(uid, out netEntity, (MetaDataComponent) null);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected NetEntity GetNetEntity(EntityUid uid, MetaDataComponent? metadata = null)
  {
    return this.EntityManager.GetNetEntity(uid, metadata);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected NetEntity? GetNetEntity(EntityUid? uid, MetaDataComponent? metadata = null)
  {
    return this.EntityManager.GetNetEntity(uid, metadata);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityUid EnsureEntity<T>(NetEntity netEntity, EntityUid callerEntity)
  {
    return this.EntityManager.EnsureEntity<T>(netEntity, callerEntity);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityUid? EnsureEntity<T>(NetEntity? netEntity, EntityUid callerEntity)
  {
    return this.EntityManager.EnsureEntity<T>(netEntity, callerEntity);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityCoordinates EnsureCoordinates<T>(
    NetCoordinates netCoordinates,
    EntityUid callerEntity)
  {
    return this.EntityManager.EnsureCoordinates<T>(netCoordinates, callerEntity);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityCoordinates? EnsureCoordinates<T>(
    NetCoordinates? netCoordinates,
    EntityUid callerEntity)
  {
    return this.EntityManager.EnsureCoordinates<T>(netCoordinates, callerEntity);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected HashSet<EntityUid> EnsureEntitySet<T>(
    HashSet<NetEntity> netEntities,
    EntityUid callerEntity)
  {
    return this.EntityManager.EnsureEntitySet<T>(netEntities, callerEntity);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void EnsureEntitySet<T>(
    HashSet<NetEntity> netEntities,
    EntityUid callerEntity,
    HashSet<EntityUid> entities)
  {
    this.EntityManager.EnsureEntitySet<T>(netEntities, callerEntity, entities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected List<EntityUid> EnsureEntityList<T>(List<NetEntity> netEntities, EntityUid callerEntity)
  {
    return this.EntityManager.EnsureEntityList<T>(netEntities, callerEntity);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void EnsureEntityList<T>(
    List<NetEntity> netEntities,
    EntityUid callerEntity,
    List<EntityUid> entities)
  {
    this.EntityManager.EnsureEntityList<T>(netEntities, callerEntity, entities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void EnsureEntityDictionary<TComp, TValue>(
    Dictionary<NetEntity, TValue> netEntities,
    EntityUid callerEntity,
    Dictionary<EntityUid, TValue> entities)
  {
    this.EntityManager.EnsureEntityDictionary<TComp, TValue>(netEntities, callerEntity, entities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void EnsureEntityDictionaryNullableValue<TComp, TValue>(
    Dictionary<NetEntity, TValue?> netEntities,
    EntityUid callerEntity,
    Dictionary<EntityUid, TValue?> entities)
  {
    this.EntityManager.EnsureEntityDictionaryNullableValue<TComp, TValue>(netEntities, callerEntity, entities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void EnsureEntityDictionary<TComp, TKey>(
    Dictionary<TKey, NetEntity> netEntities,
    EntityUid callerEntity,
    Dictionary<TKey, EntityUid> entities)
    where TKey : notnull
  {
    this.EntityManager.EnsureEntityDictionary<TComp, TKey>(netEntities, callerEntity, entities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void EnsureEntityDictionary<TComp, TKey>(
    Dictionary<TKey, NetEntity?> netEntities,
    EntityUid callerEntity,
    Dictionary<TKey, EntityUid?> entities)
    where TKey : notnull
  {
    this.EntityManager.EnsureEntityDictionary<TComp, TKey>(netEntities, callerEntity, entities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void EnsureEntityDictionary<TComp>(
    Dictionary<NetEntity, NetEntity> netEntities,
    EntityUid callerEntity,
    Dictionary<EntityUid, EntityUid> entities)
  {
    this.EntityManager.EnsureEntityDictionary<TComp>(netEntities, callerEntity, entities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected void EnsureEntityDictionary<TComp>(
    Dictionary<NetEntity, NetEntity?> netEntities,
    EntityUid callerEntity,
    Dictionary<EntityUid, EntityUid?> entities)
  {
    this.EntityManager.EnsureEntityDictionary<TComp>(netEntities, callerEntity, entities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityUid GetEntity(NetEntity netEntity) => this.EntityManager.GetEntity(netEntity);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityUid? GetEntity(NetEntity? netEntity) => this.EntityManager.GetEntity(netEntity);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected HashSet<NetEntity> GetNetEntitySet(HashSet<EntityUid> uids)
  {
    return this.EntityManager.GetNetEntitySet(uids);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected HashSet<EntityUid> GetEntitySet(HashSet<NetEntity> netEntities)
  {
    return this.EntityManager.GetEntitySet(netEntities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected List<NetEntity> GetNetEntityList(ICollection<EntityUid> uids)
  {
    return this.EntityManager.GetNetEntityList(uids);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected List<NetEntity> GetNetEntityList(IReadOnlyList<EntityUid> uids)
  {
    return this.EntityManager.GetNetEntityList(uids);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected List<EntityUid> GetEntityList(ICollection<NetEntity> netEntities)
  {
    return this.EntityManager.GetEntityList(netEntities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected List<NetEntity> GetNetEntityList(List<EntityUid> uids)
  {
    return this.EntityManager.GetNetEntityList(uids);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected List<EntityUid> GetEntityList(List<NetEntity> netEntities)
  {
    return this.EntityManager.GetEntityList(netEntities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected List<NetEntity?> GetNetEntityList(List<EntityUid?> uids)
  {
    return this.EntityManager.GetNetEntityList(uids);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected List<EntityUid?> GetEntityList(List<NetEntity?> netEntities)
  {
    return this.EntityManager.GetEntityList(netEntities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected NetEntity[] GetNetEntityArray(EntityUid[] uids)
  {
    return this.EntityManager.GetNetEntityArray(uids);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityUid[] GetEntityArray(NetEntity[] netEntities)
  {
    return this.EntityManager.GetEntityArray(netEntities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected NetEntity?[] GetNetEntityArray(EntityUid?[] uids)
  {
    return this.EntityManager.GetNetEntityArray(uids);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityUid?[] GetEntityArray(NetEntity?[] netEntities)
  {
    return this.EntityManager.GetEntityArray(netEntities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected Dictionary<NetEntity, T> GetNetEntityDictionary<T>(Dictionary<EntityUid, T> uids)
  {
    return this.EntityManager.GetNetEntityDictionary<T>(uids);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected Dictionary<T, NetEntity> GetNetEntityDictionary<T>(Dictionary<T, EntityUid> uids) where T : notnull
  {
    return this.EntityManager.GetNetEntityDictionary<T>(uids);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected Dictionary<T, NetEntity?> GetNetEntityDictionary<T>(Dictionary<T, EntityUid?> uids) where T : notnull
  {
    return this.EntityManager.GetNetEntityDictionary<T>(uids);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected Dictionary<NetEntity, NetEntity> GetNetEntityDictionary(
    Dictionary<EntityUid, EntityUid> uids)
  {
    return this.EntityManager.GetNetEntityDictionary(uids);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected Dictionary<NetEntity, NetEntity?> GetNetEntityDictionary(
    Dictionary<EntityUid, EntityUid?> uids)
  {
    return this.EntityManager.GetNetEntityDictionary(uids);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected Dictionary<EntityUid, T> GetEntityDictionary<T>(Dictionary<NetEntity, T> uids)
  {
    return this.EntityManager.GetEntityDictionary<T>(uids);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected Dictionary<T, EntityUid> GetEntityDictionary<T>(Dictionary<T, NetEntity> uids) where T : notnull
  {
    return this.EntityManager.GetEntityDictionary<T>(uids);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected Dictionary<T, EntityUid?> GetEntityDictionary<T>(Dictionary<T, NetEntity?> uids) where T : notnull
  {
    return this.EntityManager.GetEntityDictionary<T>(uids);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected Dictionary<EntityUid, EntityUid> GetEntityDictionary(
    Dictionary<NetEntity, NetEntity> uids)
  {
    return this.EntityManager.GetEntityDictionary(uids);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected Dictionary<EntityUid, EntityUid?> GetEntityDictionary(
    Dictionary<NetEntity, NetEntity?> uids)
  {
    return this.EntityManager.GetEntityDictionary(uids);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected NetCoordinates GetNetCoordinates(
    EntityCoordinates coordinates,
    MetaDataComponent? metadata = null)
  {
    return this.EntityManager.GetNetCoordinates(coordinates, metadata);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected NetCoordinates? GetNetCoordinates(
    EntityCoordinates? coordinates,
    MetaDataComponent? metadata = null)
  {
    return this.EntityManager.GetNetCoordinates(coordinates, metadata);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityCoordinates GetCoordinates(NetCoordinates netEntity)
  {
    return this.EntityManager.GetCoordinates(netEntity);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityCoordinates? GetCoordinates(NetCoordinates? netEntity)
  {
    return this.EntityManager.GetCoordinates(netEntity);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected HashSet<EntityCoordinates> GetEntitySet(HashSet<NetCoordinates> netEntities)
  {
    return this.EntityManager.GetEntitySet(netEntities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected List<EntityCoordinates> GetEntityList(List<NetCoordinates> netEntities)
  {
    return this.EntityManager.GetEntityList(netEntities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected List<EntityCoordinates> GetEntityList(ICollection<NetCoordinates> netEntities)
  {
    return this.EntityManager.GetEntityList(netEntities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected List<EntityCoordinates?> GetEntityList(List<NetCoordinates?> netEntities)
  {
    return this.EntityManager.GetEntityList(netEntities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityCoordinates[] GetEntityArray(NetCoordinates[] netEntities)
  {
    return this.EntityManager.GetEntityArray(netEntities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected EntityCoordinates?[] GetEntityArray(NetCoordinates?[] netEntities)
  {
    return this.EntityManager.GetEntityArray(netEntities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected HashSet<NetCoordinates> GetNetCoordinatesSet(HashSet<EntityCoordinates> entities)
  {
    return this.EntityManager.GetNetCoordinatesSet(entities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected List<NetCoordinates> GetNetCoordinatesList(List<EntityCoordinates> entities)
  {
    return this.EntityManager.GetNetCoordinatesList(entities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected List<NetCoordinates> GetNetCoordinatesList(ICollection<EntityCoordinates> entities)
  {
    return this.EntityManager.GetNetCoordinatesList(entities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected List<NetCoordinates?> GetNetCoordinatesList(List<EntityCoordinates?> entities)
  {
    return this.EntityManager.GetNetCoordinatesList(entities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected NetCoordinates[] GetNetCoordinatesArray(EntityCoordinates[] entities)
  {
    return this.EntityManager.GetNetCoordinatesArray(entities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected NetCoordinates?[] GetNetCoordinatesArray(EntityCoordinates?[] entities)
  {
    return this.EntityManager.GetNetCoordinatesArray(entities);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool Resolve<TComp>(EntityUid uid, [NotNullWhen(true)] ref TComp? component, bool logMissing = true) where TComp : IComponent
  {
    if ((object) component != null && !component.Deleted)
      return true;
    bool component1 = this.EntityManager.TryGetComponent<TComp>(uid, out component);
    if (logMissing && !component1)
      this.Log.Error($"Can't resolve \"{typeof (TComp)}\" on entity {this.ToPrettyString((Entity<MetaDataComponent>) uid)}!\n{Environment.StackTrace}");
    return component1;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool Resolve(EntityUid uid, [NotNullWhen(true)] ref MetaDataComponent? component, bool logMissing = true)
  {
    return this.EntityManager.MetaQuery.Resolve(uid, ref component, logMissing);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool Resolve(EntityUid uid, [NotNullWhen(true)] ref TransformComponent? component, bool logMissing = true)
  {
    return this.EntityManager.TransformQuery.Resolve(uid, ref component, logMissing);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool Resolve<TComp1, TComp2>(
    EntityUid uid,
    [NotNullWhen(true)] ref TComp1? comp1,
    [NotNullWhen(true)] ref TComp2? comp2,
    bool logMissing = true)
    where TComp1 : IComponent
    where TComp2 : IComponent
  {
    return this.Resolve<TComp1>(uid, ref comp1, logMissing) & this.Resolve<TComp2>(uid, ref comp2, logMissing);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool Resolve<TComp1, TComp2, TComp3>(
    EntityUid uid,
    [NotNullWhen(true)] ref TComp1? comp1,
    [NotNullWhen(true)] ref TComp2? comp2,
    [NotNullWhen(true)] ref TComp3? comp3,
    bool logMissing = true)
    where TComp1 : IComponent
    where TComp2 : IComponent
    where TComp3 : IComponent
  {
    return this.Resolve<TComp1, TComp2>(uid, ref comp1, ref comp2, logMissing) & this.Resolve<TComp3>(uid, ref comp3, logMissing);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected bool Resolve<TComp1, TComp2, TComp3, TComp4>(
    EntityUid uid,
    [NotNullWhen(true)] ref TComp1? comp1,
    [NotNullWhen(true)] ref TComp2? comp2,
    [NotNullWhen(true)] ref TComp3? comp3,
    [NotNullWhen(true)] ref TComp4? comp4,
    bool logMissing = true)
    where TComp1 : IComponent
    where TComp2 : IComponent
    where TComp3 : IComponent
    where TComp4 : IComponent
  {
    return this.Resolve<TComp1, TComp2>(uid, ref comp1, ref comp2, logMissing) & this.Resolve<TComp3, TComp4>(uid, ref comp3, ref comp4, logMissing);
  }

  protected EntitySystem.Subscriptions Subs { get; }

  protected void SubscribeNetworkEvent<T>(
    EntityEventHandler<T> handler,
    Type[]? before = null,
    Type[]? after = null)
    where T : notnull
  {
    this.SubEvent<T>(EventSource.Network, handler, before, after);
  }

  protected void SubscribeLocalEvent<T>(EntityEventHandler<T> handler, Type[]? before = null, Type[]? after = null) where T : notnull
  {
    this.SubEvent<T>(EventSource.Local, handler, before, after);
  }

  protected void SubscribeLocalEvent<T>(
    EntityEventRefHandler<T> handler,
    Type[]? before = null,
    Type[]? after = null)
    where T : notnull
  {
    this.SubEvent<T>(EventSource.Local, handler, before, after);
  }

  protected void SubscribeAllEvent<T>(EntityEventHandler<T> handler, Type[]? before = null, Type[]? after = null) where T : notnull
  {
    this.SubEvent<T>(EventSource.All, handler, before, after);
  }

  protected void SubscribeNetworkEvent<T>(
    EntitySessionEventHandler<T> handler,
    Type[]? before = null,
    Type[]? after = null)
    where T : notnull
  {
    this.SubSessionEvent<T>(EventSource.Network, handler, before, after);
  }

  protected void SubscribeLocalEvent<T>(
    EntitySessionEventHandler<T> handler,
    Type[]? before = null,
    Type[]? after = null)
    where T : notnull
  {
    this.SubSessionEvent<T>(EventSource.Local, handler, before, after);
  }

  protected void SubscribeAllEvent<T>(
    EntitySessionEventHandler<T> handler,
    Type[]? before = null,
    Type[]? after = null)
    where T : notnull
  {
    this.SubSessionEvent<T>(EventSource.All, handler, before, after);
  }

  private void SubEvent<T>(
    EventSource src,
    EntityEventHandler<T> handler,
    Type[]? before,
    Type[]? after)
    where T : notnull
  {
    this.EntityManager.EventBus.SubscribeEvent<T>(src, (IEntityEventSubscriber) this, handler, this.GetType(), before, after);
    this._subscriptions.Add((EntitySystem.SubBase) new EntitySystem.SubBroadcast<T>(src));
  }

  private void SubEvent<T>(
    EventSource src,
    EntityEventRefHandler<T> handler,
    Type[]? before,
    Type[]? after)
    where T : notnull
  {
    this.EntityManager.EventBus.SubscribeEvent<T>(src, (IEntityEventSubscriber) this, handler, this.GetType(), before, after);
    this._subscriptions.Add((EntitySystem.SubBase) new EntitySystem.SubBroadcast<T>(src));
  }

  private void SubSessionEvent<T>(
    EventSource src,
    EntitySessionEventHandler<T> handler,
    Type[]? before,
    Type[]? after)
    where T : notnull
  {
    this.EntityManager.EventBus.SubscribeSessionEvent<T>(src, (IEntityEventSubscriber) this, handler, this.GetType(), before, after);
    this._subscriptions.Add((EntitySystem.SubBase) new EntitySystem.SubBroadcast<EntitySessionMessage<T>>(src));
  }

  protected void SubscribeLocalEvent<TComp, TEvent>(
    EntityEventRefHandler<TComp, TEvent> handler,
    Type[]? before = null,
    Type[]? after = null)
    where TComp : IComponent
    where TEvent : notnull
  {
    this.EntityManager.EventBus.SubscribeLocalEvent<TComp, TEvent>(handler, this.GetType(), before, after);
    this._subscriptions.Add((EntitySystem.SubBase) new EntitySystem.SubLocal<TComp, TEvent>());
  }

  protected void SubscribeLocalEvent<TComp, TEvent>(
    ComponentEventHandler<TComp, TEvent> handler,
    Type[]? before = null,
    Type[]? after = null)
    where TComp : IComponent
    where TEvent : notnull
  {
    this.EntityManager.EventBus.SubscribeLocalEvent<TComp, TEvent>(handler, this.GetType(), before, after);
    this._subscriptions.Add((EntitySystem.SubBase) new EntitySystem.SubLocal<TComp, TEvent>());
  }

  protected void SubscribeLocalEvent<TComp, TEvent>(
    ComponentEventRefHandler<TComp, TEvent> handler,
    Type[]? before = null,
    Type[]? after = null)
    where TComp : IComponent
    where TEvent : notnull
  {
    this.EntityManager.EventBus.SubscribeLocalEvent<TComp, TEvent>(handler, this.GetType(), before, after);
    this._subscriptions.Add((EntitySystem.SubBase) new EntitySystem.SubLocal<TComp, TEvent>());
  }

  private void ShutdownSubscriptions()
  {
    foreach (EntitySystem.SubBase subscription in this._subscriptions)
      subscription.Unsubscribe(this, (IEventBus) this.EntityManager.EventBusInternal);
    this._subscriptions = new ValueList<EntitySystem.SubBase>();
  }

  public sealed class Subscriptions
  {
    public EntitySystem System { get; }

    internal Subscriptions(EntitySystem system) => this.System = system;

    public void SubEvent<T>(
      EventSource src,
      EntityEventHandler<T> handler,
      Type[]? before = null,
      Type[]? after = null)
      where T : notnull
    {
      this.System.SubEvent<T>(src, handler, before, after);
    }

    public void SubSessionEvent<T>(
      EventSource src,
      EntitySessionEventHandler<T> handler,
      Type[]? before = null,
      Type[]? after = null)
      where T : notnull
    {
      this.System.SubSessionEvent<T>(src, handler, before, after);
    }

    public void SubscribeLocalEvent<TComp, TEvent>(
      ComponentEventHandler<TComp, TEvent> handler,
      Type[]? before = null,
      Type[]? after = null)
      where TComp : IComponent
      where TEvent : notnull
    {
      this.System.SubscribeLocalEvent<TComp, TEvent>(handler, before, after);
    }

    public void SubscribeLocalEvent<TComp, TEvent>(
      ComponentEventRefHandler<TComp, TEvent> handler,
      Type[]? before = null,
      Type[]? after = null)
      where TComp : IComponent
      where TEvent : notnull
    {
      this.System.SubscribeLocalEvent<TComp, TEvent>(handler, before, after);
    }

    public void SubscribeLocalEvent<TComp, TEvent>(
      EntityEventRefHandler<TComp, TEvent> handler,
      Type[]? before = null,
      Type[]? after = null)
      where TComp : IComponent
      where TEvent : notnull
    {
      this.System.SubscribeLocalEvent<TComp, TEvent>(handler, before, after);
    }

    public void RegisterUnsubscription(Action action)
    {
      this.System._subscriptions.Add((EntitySystem.SubBase) new EntitySystem.SubAction(action));
    }
  }

  private abstract class SubBase
  {
    public abstract void Unsubscribe(EntitySystem sys, IEventBus bus);
  }

  private sealed class SubBroadcast<T> : EntitySystem.SubBase where T : notnull
  {
    private readonly EventSource _source;

    public SubBroadcast(EventSource source) => this._source = source;

    public override void Unsubscribe(EntitySystem sys, IEventBus bus)
    {
      bus.UnsubscribeEvent<T>(this._source, (IEntityEventSubscriber) sys);
    }
  }

  private sealed class SubLocal<TComp, TBase> : EntitySystem.SubBase
    where TComp : IComponent
    where TBase : notnull
  {
    public override void Unsubscribe(EntitySystem sys, IEventBus bus)
    {
      bus.UnsubscribeLocalEvent<TComp, TBase>();
    }
  }

  private sealed class SubAction(Action action) : EntitySystem.SubBase
  {
    public override void Unsubscribe(EntitySystem sys, IEventBus bus) => action();
  }
}
