// Decompiled with JetBrains decompiler
// Type: Content.Shared.StatusEffectNew.SharedStatusEffectsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.StatusEffectNew.Components;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.StatusEffectNew;

public abstract class SharedStatusEffectsSystem : EntitySystem
{
  [Dependency]
  private AlertsSystem _alerts;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private IPrototypeManager _proto;
  [Dependency]
  private IComponentFactory _compFactory;
  [Dependency]
  private INetManager _net;
  private Robust.Shared.GameObjects.EntityQuery<StatusEffectContainerComponent> _containerQuery;
  private Robust.Shared.GameObjects.EntityQuery<StatusEffectComponent> _effectQuery;

  public override void Initialize()
  {
    base.Initialize();
    this.InitializeRelay();
    this.SubscribeLocalEvent<StatusEffectComponent, StatusEffectAppliedEvent>(new EntityEventRefHandler<StatusEffectComponent, StatusEffectAppliedEvent>(this.OnStatusEffectApplied));
    this.SubscribeLocalEvent<StatusEffectComponent, StatusEffectRemovedEvent>(new EntityEventRefHandler<StatusEffectComponent, StatusEffectRemovedEvent>(this.OnStatusEffectRemoved));
    this.SubscribeLocalEvent<StatusEffectContainerComponent, ComponentGetState>(new EntityEventRefHandler<StatusEffectContainerComponent, ComponentGetState>(this.OnGetState));
    this._containerQuery = this.GetEntityQuery<StatusEffectContainerComponent>();
    this._effectQuery = this.GetEntityQuery<StatusEffectComponent>();
  }

  private void OnGetState(Entity<StatusEffectContainerComponent> ent, ref ComponentGetState args)
  {
    args.State = (IComponentState) new StatusEffectContainerComponentState(this.GetNetEntitySet(ent.Comp.ActiveStatusEffects));
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<StatusEffectComponent> entityQueryEnumerator = this.EntityQueryEnumerator<StatusEffectComponent>();
    EntityUid uid;
    StatusEffectComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.EndEffectTime.HasValue)
      {
        TimeSpan curTime = this._timing.CurTime;
        TimeSpan? endEffectTime = comp1.EndEffectTime;
        if ((endEffectTime.HasValue ? (curTime >= endEffectTime.GetValueOrDefault() ? 1 : 0) : 0) != 0 && comp1.AppliedTo.HasValue)
        {
          MetaDataComponent metaDataComponent = this.MetaData(uid);
          if (metaDataComponent.EntityPrototype != null)
            this.TryRemoveStatusEffect(comp1.AppliedTo.Value, (EntProtoId) metaDataComponent.EntityPrototype);
        }
      }
    }
  }

  private void AddStatusEffectTime(EntityUid effect, TimeSpan delta)
  {
    StatusEffectComponent component;
    if (!this._effectQuery.TryComp(effect, out component))
      return;
    StatusEffectComponent statusEffectComponent = component;
    TimeSpan? endEffectTime = statusEffectComponent.EndEffectTime;
    TimeSpan timeSpan = delta;
    statusEffectComponent.EndEffectTime = endEffectTime.HasValue ? new TimeSpan?(endEffectTime.GetValueOrDefault() + timeSpan) : new TimeSpan?();
    this.Dirty(effect, (IComponent) component);
    this.ShowAlertIfNeeded(component);
  }

  private void SetStatusEffectTime(EntityUid effect, TimeSpan? duration)
  {
    StatusEffectComponent component;
    if (!this._effectQuery.TryComp(effect, out component))
      return;
    if (!duration.HasValue)
    {
      if (!component.EndEffectTime.HasValue)
        return;
      component.EndEffectTime = new TimeSpan?();
    }
    else
    {
      StatusEffectComponent statusEffectComponent = component;
      TimeSpan curTime = this._timing.CurTime;
      TimeSpan? nullable1 = duration;
      TimeSpan? nullable2 = nullable1.HasValue ? new TimeSpan?(curTime + nullable1.GetValueOrDefault()) : new TimeSpan?();
      statusEffectComponent.EndEffectTime = nullable2;
    }
    this.Dirty(effect, (IComponent) component);
    this.ShowAlertIfNeeded(component);
  }

  private void UpdateStatusEffectTime(EntityUid effect, TimeSpan? duration)
  {
    StatusEffectComponent component;
    if (!this._effectQuery.TryComp(effect, out component) || !component.EndEffectTime.HasValue)
      return;
    if (!duration.HasValue)
    {
      component.EndEffectTime = new TimeSpan?();
    }
    else
    {
      TimeSpan curTime = this._timing.CurTime;
      TimeSpan? nullable1 = duration;
      TimeSpan? nullable2 = nullable1.HasValue ? new TimeSpan?(curTime + nullable1.GetValueOrDefault()) : new TimeSpan?();
      nullable1 = component.EndEffectTime;
      TimeSpan? nullable3 = nullable2;
      if ((nullable1.HasValue & nullable3.HasValue ? (nullable1.GetValueOrDefault() >= nullable3.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        return;
      component.EndEffectTime = nullable2;
    }
    this.Dirty(effect, (IComponent) component);
    this.ShowAlertIfNeeded(component);
  }

  private void OnStatusEffectApplied(
    Entity<StatusEffectComponent> ent,
    ref StatusEffectAppliedEvent args)
  {
    this.ShowAlertIfNeeded((StatusEffectComponent) ent);
  }

  private void OnStatusEffectRemoved(
    Entity<StatusEffectComponent> ent,
    ref StatusEffectRemovedEvent args)
  {
    if (!ent.Comp.AppliedTo.HasValue)
      return;
    StatusEffectComponent comp = ent.Comp;
    if (comp == null || !comp.AppliedTo.HasValue || !comp.Alert.HasValue)
      return;
    this._alerts.ClearAlert(ent.Comp.AppliedTo.Value, ent.Comp.Alert.Value);
  }

  private bool CanAddStatusEffect(EntityUid uid, EntProtoId effectProto)
  {
    EntityPrototype prototype;
    StatusEffectComponent component;
    if (!this._proto.TryIndex(effectProto, out prototype) || !prototype.TryGetComponent<StatusEffectComponent>(out component, this._compFactory) || !this._whitelist.CheckBoth(new EntityUid?(uid), component.Blacklist, component.Whitelist))
      return false;
    BeforeStatusEffectAddedEvent args = new BeforeStatusEffectAddedEvent(effectProto);
    this.RaiseLocalEvent<BeforeStatusEffectAddedEvent>(uid, ref args);
    return !args.Cancelled;
  }

  private bool TryAddStatusEffect(
    EntityUid target,
    EntProtoId effectProto,
    [NotNullWhen(true)] out EntityUid? statusEffect,
    TimeSpan? duration = null)
  {
    statusEffect = new EntityUid?();
    if (!this.CanAddStatusEffect(target, effectProto))
      return false;
    StatusEffectContainerComponent containerComponent = this.EnsureComp<StatusEffectContainerComponent>(target);
    EntityUid uid = this.PredictedSpawnAttachedTo((string) effectProto, this.Transform(target).Coordinates, rotation: new Angle());
    this._transform.SetParent(uid, target);
    StatusEffectComponent component;
    if (!this._effectQuery.TryComp(uid, out component))
      return false;
    statusEffect = new EntityUid?(uid);
    if (duration.HasValue)
    {
      StatusEffectComponent statusEffectComponent = component;
      TimeSpan curTime = this._timing.CurTime;
      TimeSpan? nullable1 = duration;
      TimeSpan? nullable2 = nullable1.HasValue ? new TimeSpan?(curTime + nullable1.GetValueOrDefault()) : new TimeSpan?();
      statusEffectComponent.EndEffectTime = nullable2;
    }
    containerComponent.ActiveStatusEffects.Add(uid);
    component.AppliedTo = new EntityUid?(target);
    this.Dirty(target, (IComponent) containerComponent);
    this.Dirty(uid, (IComponent) component);
    StatusEffectAppliedEvent args = new StatusEffectAppliedEvent(target);
    this.RaiseLocalEvent<StatusEffectAppliedEvent>(uid, ref args);
    return true;
  }

  private void ShowAlertIfNeeded(StatusEffectComponent effectComp)
  {
    if (effectComp == null || !effectComp.AppliedTo.HasValue || !effectComp.Alert.HasValue)
      return;
    (TimeSpan, TimeSpan)? nullable1 = !effectComp.EndEffectTime.HasValue ? new (TimeSpan, TimeSpan)?() : new (TimeSpan, TimeSpan)?((this._timing.CurTime, effectComp.EndEffectTime.Value));
    AlertsSystem alerts = this._alerts;
    EntityUid euid = effectComp.AppliedTo.Value;
    ProtoId<AlertPrototype> alertType = effectComp.Alert.Value;
    (TimeSpan, TimeSpan)? nullable2 = nullable1;
    short? severity = new short?();
    (TimeSpan, TimeSpan)? cooldown = nullable2;
    alerts.ShowAlert(euid, alertType, severity, cooldown);
  }

  public bool TryAddStatusEffectDuration(
    EntityUid target,
    EntProtoId effectProto,
    [NotNullWhen(true)] out EntityUid? statusEffect,
    TimeSpan duration)
  {
    if (!this.TryGetStatusEffect(target, effectProto, out statusEffect))
      return this.TryAddStatusEffect(target, effectProto, out statusEffect, new TimeSpan?(duration));
    this.AddStatusEffectTime(statusEffect.Value, duration);
    return true;
  }

  public bool TryAddStatusEffectDuration(
    EntityUid target,
    EntProtoId effectProto,
    TimeSpan duration)
  {
    return this.TryAddStatusEffectDuration(target, effectProto, out EntityUid? _, duration);
  }

  public bool TrySetStatusEffectDuration(
    EntityUid target,
    EntProtoId effectProto,
    [NotNullWhen(true)] out EntityUid? statusEffect,
    TimeSpan? duration = null)
  {
    if (!this.TryGetStatusEffect(target, effectProto, out statusEffect))
      return this.TryAddStatusEffect(target, effectProto, out statusEffect, duration);
    this.SetStatusEffectTime(statusEffect.Value, duration);
    return true;
  }

  public bool TrySetStatusEffectDuration(
    EntityUid target,
    EntProtoId effectProto,
    TimeSpan? duration = null)
  {
    return this.TrySetStatusEffectDuration(target, effectProto, out EntityUid? _, duration);
  }

  public bool TryUpdateStatusEffectDuration(
    EntityUid target,
    EntProtoId effectProto,
    [NotNullWhen(true)] out EntityUid? statusEffect,
    TimeSpan? duration = null)
  {
    if (!this.TryGetStatusEffect(target, effectProto, out statusEffect))
      return this.TryAddStatusEffect(target, effectProto, out statusEffect, duration);
    this.UpdateStatusEffectTime(statusEffect.Value, duration);
    return true;
  }

  public bool TryUpdateStatusEffectDuration(
    EntityUid target,
    EntProtoId effectProto,
    TimeSpan? duration = null)
  {
    return this.TryUpdateStatusEffectDuration(target, effectProto, out EntityUid? _, duration);
  }

  public bool TryRemoveStatusEffect(EntityUid target, EntProtoId effectProto)
  {
    StatusEffectContainerComponent component;
    if (this._net.IsClient || !this._containerQuery.TryComp(target, out component))
      return false;
    foreach (EntityUid activeStatusEffect in component.ActiveStatusEffects)
    {
      MetaDataComponent metaDataComponent = this.MetaData(activeStatusEffect);
      if (metaDataComponent.EntityPrototype != null && (EntProtoId) metaDataComponent.EntityPrototype == effectProto)
      {
        if (!this._effectQuery.TryComp(activeStatusEffect, out StatusEffectComponent _))
          return false;
        StatusEffectRemovedEvent args = new StatusEffectRemovedEvent(target);
        this.RaiseLocalEvent<StatusEffectRemovedEvent>(activeStatusEffect, ref args);
        this.QueueDel(new EntityUid?(activeStatusEffect));
        component.ActiveStatusEffects.Remove(activeStatusEffect);
        this.Dirty(target, (IComponent) component);
        return true;
      }
    }
    return false;
  }

  public bool HasStatusEffect(EntityUid target, EntProtoId effectProto)
  {
    StatusEffectContainerComponent component;
    if (!this._containerQuery.TryComp(target, out component))
      return false;
    foreach (EntityUid activeStatusEffect in component.ActiveStatusEffects)
    {
      MetaDataComponent metaDataComponent = this.MetaData(activeStatusEffect);
      if (metaDataComponent.EntityPrototype != null && (EntProtoId) metaDataComponent.EntityPrototype == effectProto)
        return true;
    }
    return false;
  }

  public bool TryGetStatusEffect(EntityUid target, EntProtoId effectProto, [NotNullWhen(true)] out EntityUid? effect)
  {
    effect = new EntityUid?();
    StatusEffectContainerComponent component;
    if (!this._containerQuery.TryComp(target, out component))
      return false;
    foreach (EntityUid activeStatusEffect in component.ActiveStatusEffects)
    {
      MetaDataComponent comp;
      if (this.TryComp(activeStatusEffect, out comp) && comp.EntityPrototype != null && (EntProtoId) comp.EntityPrototype == effectProto)
      {
        effect = new EntityUid?(activeStatusEffect);
        return true;
      }
    }
    return false;
  }

  public bool TryGetTime(
    EntityUid uid,
    EntProtoId effectProto,
    out (EntityUid EffectEnt, TimeSpan? EndEffectTime) time,
    StatusEffectContainerComponent? container = null)
  {
    time = ();
    if (!this.Resolve<StatusEffectContainerComponent>(uid, ref container))
      return false;
    foreach (EntityUid activeStatusEffect in container.ActiveStatusEffects)
    {
      MetaDataComponent metaDataComponent = this.MetaData(activeStatusEffect);
      if (metaDataComponent.EntityPrototype != null && (EntProtoId) metaDataComponent.EntityPrototype == effectProto)
      {
        StatusEffectComponent component;
        if (!this._effectQuery.TryComp(activeStatusEffect, out component))
          return false;
        time = (activeStatusEffect, component.EndEffectTime);
        return true;
      }
    }
    return false;
  }

  public bool TryGetMaxTime<T>(
    EntityUid uid,
    out (EntityUid EffectEnt, TimeSpan? EndEffectTime) time)
    where T : IComponent
  {
    time = ();
    HashSet<Entity<T, StatusEffectComponent>> effects;
    if (!this.TryEffectsWithComp<T>(new EntityUid?(uid), out effects))
      return false;
    time.Item2 = new TimeSpan?(TimeSpan.Zero);
    foreach (Entity<T, StatusEffectComponent> entity in effects)
    {
      TimeSpan? nullable1;
      if (!entity.Comp2.EndEffectTime.HasValue)
      {
        ref (EntityUid, TimeSpan?) local = ref time;
        EntityUid owner = entity.Owner;
        nullable1 = new TimeSpan?();
        TimeSpan? nullable2 = nullable1;
        (_, _) = (owner, nullable2);
        (EntityUid, TimeSpan?) valueTuple;
        local = valueTuple;
        return true;
      }
      nullable1 = entity.Comp2.EndEffectTime;
      TimeSpan? nullable3 = time.Item2;
      if ((nullable1.HasValue & nullable3.HasValue ? (nullable1.GetValueOrDefault() > nullable3.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        time = (entity.Owner, entity.Comp2.EndEffectTime);
    }
    return true;
  }

  public bool TryAddTime(EntityUid uid, EntProtoId effectProto, TimeSpan time)
  {
    StatusEffectContainerComponent component;
    if (!this._containerQuery.TryComp(uid, out component))
      return false;
    foreach (EntityUid activeStatusEffect in component.ActiveStatusEffects)
    {
      MetaDataComponent metaDataComponent = this.MetaData(activeStatusEffect);
      if (metaDataComponent.EntityPrototype != null && (EntProtoId) metaDataComponent.EntityPrototype == effectProto)
      {
        this.AddStatusEffectTime(activeStatusEffect, time);
        return true;
      }
    }
    return false;
  }

  public bool TrySetTime(EntityUid uid, EntProtoId effectProto, TimeSpan time)
  {
    StatusEffectContainerComponent component;
    if (!this._containerQuery.TryComp(uid, out component))
      return false;
    foreach (EntityUid activeStatusEffect in component.ActiveStatusEffects)
    {
      MetaDataComponent metaDataComponent = this.MetaData(activeStatusEffect);
      if (metaDataComponent.EntityPrototype != null && (EntProtoId) metaDataComponent.EntityPrototype == effectProto)
      {
        this.SetStatusEffectTime(activeStatusEffect, new TimeSpan?(time));
        return true;
      }
    }
    return false;
  }

  public bool HasEffectComp<T>(EntityUid? target) where T : IComponent
  {
    StatusEffectContainerComponent component;
    if (!this._containerQuery.TryComp(target, out component))
      return false;
    foreach (EntityUid activeStatusEffect in component.ActiveStatusEffects)
    {
      if (this.HasComp<T>(activeStatusEffect))
        return true;
    }
    return false;
  }

  public bool TryEffectsWithComp<T>(
    EntityUid? target,
    [NotNullWhen(true)] out HashSet<Entity<T, StatusEffectComponent>>? effects)
    where T : IComponent
  {
    effects = (HashSet<Entity<T, StatusEffectComponent>>) null;
    StatusEffectContainerComponent component1;
    if (!this._containerQuery.TryComp(target, out component1))
      return false;
    foreach (EntityUid activeStatusEffect in component1.ActiveStatusEffects)
    {
      StatusEffectComponent component2;
      T comp;
      if (this._effectQuery.TryComp(activeStatusEffect, out component2) && this.TryComp<T>(activeStatusEffect, out comp))
      {
        if (effects == null)
          effects = new HashSet<Entity<T, StatusEffectComponent>>();
        effects.Add((Entity<T, StatusEffectComponent>) (activeStatusEffect, comp, component2));
      }
    }
    return effects != null;
  }

  public bool TryGetEffectsEndTimeWithComp<T>(EntityUid? target, out TimeSpan? endTime) where T : IComponent
  {
    endTime = new TimeSpan?(this._timing.CurTime);
    StatusEffectContainerComponent component1;
    if (!this._containerQuery.TryComp(target, out component1))
      return false;
    foreach (EntityUid activeStatusEffect in component1.ActiveStatusEffects)
    {
      StatusEffectComponent component2;
      if (this.HasComp<T>(activeStatusEffect) && this._effectQuery.TryComp(activeStatusEffect, out component2))
      {
        TimeSpan? endEffectTime = component2.EndEffectTime;
        if (!endEffectTime.HasValue)
        {
          endTime = new TimeSpan?();
          return true;
        }
        endEffectTime = component2.EndEffectTime;
        TimeSpan? nullable = endTime;
        if ((endEffectTime.HasValue & nullable.HasValue ? (endEffectTime.GetValueOrDefault() > nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
          endTime = component2.EndEffectTime;
      }
    }
    return endTime.HasValue;
  }

  protected void InitializeRelay()
  {
    this.SubscribeLocalEvent<StatusEffectContainerComponent, LocalPlayerAttachedEvent>(new ComponentEventHandler<StatusEffectContainerComponent, LocalPlayerAttachedEvent>(this.RelayStatusEffectEvent<LocalPlayerAttachedEvent>));
    this.SubscribeLocalEvent<StatusEffectContainerComponent, LocalPlayerDetachedEvent>(new ComponentEventHandler<StatusEffectContainerComponent, LocalPlayerDetachedEvent>(this.RelayStatusEffectEvent<LocalPlayerDetachedEvent>));
  }

  protected void RefRelayStatusEffectEvent<T>(
    EntityUid uid,
    StatusEffectContainerComponent component,
    ref T args)
    where T : struct
  {
    this.RelayEvent<T>((Entity<StatusEffectContainerComponent>) (uid, component), ref args);
  }

  protected void RelayStatusEffectEvent<T>(
    EntityUid uid,
    StatusEffectContainerComponent component,
    T args)
    where T : class
  {
    this.RelayEvent<T>((Entity<StatusEffectContainerComponent>) (uid, component), args);
  }

  public void RelayEvent<T>(
    Entity<StatusEffectContainerComponent> statusEffect,
    ref T args)
    where T : struct
  {
    StatusEffectRelayedEvent<T> args1 = new StatusEffectRelayedEvent<T>(args);
    foreach (EntityUid activeStatusEffect in statusEffect.Comp.ActiveStatusEffects)
      this.RaiseLocalEvent<StatusEffectRelayedEvent<T>>(activeStatusEffect, ref args1);
    args = args1.Args;
  }

  public void RelayEvent<T>(
    Entity<StatusEffectContainerComponent> statusEffect,
    T args)
    where T : class
  {
    StatusEffectRelayedEvent<T> args1 = new StatusEffectRelayedEvent<T>(args);
    foreach (EntityUid activeStatusEffect in statusEffect.Comp.ActiveStatusEffects)
      this.RaiseLocalEvent<StatusEffectRelayedEvent<T>>(activeStatusEffect, ref args1);
  }
}
