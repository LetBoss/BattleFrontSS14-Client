// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.MetaDataSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Robust.Shared.GameObjects;

public abstract class MetaDataSystem : EntitySystem
{
  [Dependency]
  private readonly IGameTiming _timing;
  [Dependency]
  private readonly IPrototypeManager _proto;
  private EntityPausedEvent _pausedEvent;
  private Robust.Shared.GameObjects.EntityQuery<MetaDataComponent> _metaQuery;

  public override void Initialize()
  {
    this._metaQuery = this.GetEntityQuery<MetaDataComponent>();
    this.SubscribeLocalEvent<MetaDataComponent, ComponentHandleState>(new ComponentEventRefHandler<MetaDataComponent, ComponentHandleState>(this.OnMetaDataHandle));
    this.SubscribeLocalEvent<MetaDataComponent, ComponentGetState>(new ComponentEventRefHandler<MetaDataComponent, ComponentGetState>(this.OnMetaDataGetState));
  }

  private void OnMetaDataGetState(
    EntityUid uid,
    MetaDataComponent component,
    ref ComponentGetState args)
  {
    args.State = (IComponentState) new MetaDataComponentState(component._entityName, component._entityDescription, component._entityPrototype?.ID, component.PauseTime);
  }

  private void OnMetaDataHandle(
    EntityUid uid,
    MetaDataComponent component,
    ref ComponentHandleState args)
  {
    if (!(args.Current is MetaDataComponentState current))
      return;
    component._entityName = current.Name;
    component._entityDescription = current.Description;
    if (current.PrototypeId != null && current.PrototypeId != component._entityPrototype?.ID)
      component._entityPrototype = this._proto.Index<EntityPrototype>(current.PrototypeId);
    component.PauseTime = current.PauseTime;
  }

  public void SetEntityName(
    EntityUid uid,
    string value,
    MetaDataComponent? metadata = null,
    bool raiseEvents = true)
  {
    if (!this._metaQuery.Resolve(uid, ref metadata) || value.Equals(metadata.EntityName))
      return;
    string entityName = metadata.EntityName;
    metadata._entityName = value;
    if (raiseEvents)
    {
      EntityRenamedEvent args = new EntityRenamedEvent(uid, entityName, value);
      this.RaiseLocalEvent<EntityRenamedEvent>(uid, ref args, true);
    }
    this.Dirty(uid, (IComponent) metadata, metadata);
  }

  public void SetEntityDescription(EntityUid uid, string value, MetaDataComponent? metadata = null)
  {
    if (!this._metaQuery.Resolve(uid, ref metadata) || value.Equals(metadata.EntityDescription))
      return;
    metadata._entityDescription = value;
    this.Dirty(uid, (IComponent) metadata, metadata);
  }

  internal void SetEntityPrototype(
    EntityUid uid,
    EntityPrototype? value,
    MetaDataComponent? metadata = null)
  {
    if (!this._metaQuery.Resolve(uid, ref metadata) || value != null && value.Equals((object) metadata._entityPrototype))
      return;
    metadata._entityPrototype = value;
  }

  public bool EntityPaused(EntityUid uid, MetaDataComponent? metadata = null)
  {
    return !this._metaQuery.Resolve(uid, ref metadata) || metadata.EntityPaused;
  }

  public void SetEntityPaused(EntityUid uid, bool value, MetaDataComponent? metadata = null)
  {
    if (!this._metaQuery.Resolve(uid, ref metadata) || metadata.EntityPaused == value)
      return;
    if (value)
    {
      metadata.PauseTime = new TimeSpan?(this._timing.CurTime);
      this.RaiseLocalEvent<EntityPausedEvent>(uid, ref this._pausedEvent);
    }
    else
    {
      EntityUnpausedEvent args = new EntityUnpausedEvent(this._timing.CurTime - metadata.PauseTime.Value);
      metadata.PauseTime = new TimeSpan?();
      this.RaiseLocalEvent<EntityUnpausedEvent>(uid, ref args);
    }
    this.Dirty(uid, (IComponent) metadata, metadata);
  }

  public TimeSpan GetPauseTime(EntityUid uid, MetaDataComponent? metadata = null)
  {
    if (!this._metaQuery.Resolve(uid, ref metadata))
      return TimeSpan.Zero;
    TimeSpan curTime = this._timing.CurTime;
    TimeSpan? pauseTime = metadata.PauseTime;
    return (pauseTime.HasValue ? new TimeSpan?(curTime - pauseTime.GetValueOrDefault()) : new TimeSpan?()) ?? TimeSpan.Zero;
  }

  public void PauseOffset(EntityUid uid, ref TimeSpan time, MetaDataComponent? metadata = null)
  {
    TimeSpan pauseTime = this.GetPauseTime(uid, metadata);
    time += pauseTime;
  }

  public void SetFlag(Entity<MetaDataComponent?> entity, MetaDataFlags flags, bool enabled)
  {
    if (!this._metaQuery.Resolve((EntityUid) entity, ref entity.Comp))
      return;
    if (enabled)
      entity.Comp.Flags |= flags;
    else
      this.RemoveFlag((EntityUid) entity, flags, entity.Comp);
  }

  public void AddFlag(EntityUid uid, MetaDataFlags flags, MetaDataComponent? comp = null)
  {
    this.SetFlag((Entity<MetaDataComponent>) (uid, comp), flags, true);
  }

  public void RemoveFlag(EntityUid uid, MetaDataFlags flags, MetaDataComponent? component = null)
  {
    if (!this._metaQuery.Resolve(uid, ref component))
      return;
    MetaDataFlags toRemove = component.Flags & flags;
    if (toRemove == MetaDataFlags.None)
      return;
    MetaFlagRemoveAttemptEvent args = new MetaFlagRemoveAttemptEvent(toRemove);
    this.RaiseLocalEvent<MetaFlagRemoveAttemptEvent>(uid, ref args, true);
    component.Flags &= ~args.ToRemove;
  }
}
