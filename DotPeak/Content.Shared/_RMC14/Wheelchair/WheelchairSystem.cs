// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Wheelchair.WheelchairSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Buckle.Components;
using Content.Shared.Mobs;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Stunnable;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Events;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Wheelchair;

public sealed class WheelchairSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Dependency]
  private SharedMoverController _mover;
  private readonly HashSet<EntityUid> _processingUnbuckle = new HashSet<EntityUid>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<WheelchairComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<WheelchairComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshSpeed));
    this.SubscribeLocalEvent<WheelchairComponent, StrappedEvent>(new EntityEventRefHandler<WheelchairComponent, StrappedEvent>(this.OnStrapped));
    this.SubscribeLocalEvent<WheelchairComponent, UnstrappedEvent>(new EntityEventRefHandler<WheelchairComponent, UnstrappedEvent>(this.OnUnstrapped));
    this.SubscribeLocalEvent<ActiveWheelchairPilotComponent, RingBellActionEvent>(new EntityEventRefHandler<ActiveWheelchairPilotComponent, RingBellActionEvent>(this.OnRingBell));
    this.SubscribeLocalEvent<ActiveWheelchairPilotComponent, PreventCollideEvent>(new EntityEventRefHandler<ActiveWheelchairPilotComponent, PreventCollideEvent>(this.OnActivePilotPreventCollide));
    this.SubscribeLocalEvent<ActiveWheelchairPilotComponent, KnockedDownEvent>(new EntityEventRefHandler<ActiveWheelchairPilotComponent, KnockedDownEvent>(this.OnActivePilotStunned<KnockedDownEvent>));
    this.SubscribeLocalEvent<ActiveWheelchairPilotComponent, StunnedEvent>(new EntityEventRefHandler<ActiveWheelchairPilotComponent, StunnedEvent>(this.OnActivePilotStunned<StunnedEvent>));
    this.SubscribeLocalEvent<ActiveWheelchairPilotComponent, MobStateChangedEvent>(new EntityEventRefHandler<ActiveWheelchairPilotComponent, MobStateChangedEvent>(this.OnActivePilotMobStateChanged));
  }

  private void OnRefreshSpeed(
    Entity<WheelchairComponent> ent,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    StrapComponent comp;
    if (!this.TryComp<StrapComponent>((EntityUid) ent, out comp) || comp.BuckledEntities.Count == 0)
      return;
    float speedMultiplier = ent.Comp.SpeedMultiplier;
    args.ModifySpeed(speedMultiplier, speedMultiplier);
  }

  private void OnStrapped(Entity<WheelchairComponent> ent, ref StrappedEvent args)
  {
    Entity<BuckleComponent> buckle = args.Buckle;
    ActiveWheelchairPilotComponent wheelchairPilotComponent1 = this.EnsureComp<ActiveWheelchairPilotComponent>((EntityUid) buckle);
    this._mover.SetRelay((EntityUid) buckle, (EntityUid) ent);
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) ent);
    if (!ent.Comp.BellAction.HasValue)
      return;
    ActiveWheelchairPilotComponent wheelchairPilotComponent2 = wheelchairPilotComponent1;
    SharedActionsSystem actions = this._actions;
    EntityUid performer = (EntityUid) buckle;
    EntProtoId? bellAction = ent.Comp.BellAction;
    string valueOrDefault = bellAction.HasValue ? (string) bellAction.GetValueOrDefault() : (string) null;
    EntityUid container = new EntityUid();
    EntityUid? nullable = actions.AddAction(performer, valueOrDefault, container);
    wheelchairPilotComponent2.BellActionEntity = nullable;
  }

  private void OnUnstrapped(Entity<WheelchairComponent> ent, ref UnstrappedEvent args)
  {
    Entity<BuckleComponent> buckle = args.Buckle;
    if (this._processingUnbuckle.Contains(buckle.Owner))
      return;
    this._processingUnbuckle.Add(buckle.Owner);
    try
    {
      ActiveWheelchairPilotComponent comp;
      if (this.TryComp<ActiveWheelchairPilotComponent>((EntityUid) buckle, out comp) && comp.BellActionEntity.HasValue)
        this._actions.RemoveAction((Entity<ActionsComponent>) buckle.Owner, new Entity<ActionComponent>?((Entity<ActionComponent>) comp.BellActionEntity.Value));
      this.RemCompDeferred<RelayInputMoverComponent>((EntityUid) buckle);
      this.RemCompDeferred<ActiveWheelchairPilotComponent>((EntityUid) buckle);
      this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) ent);
    }
    finally
    {
      this._processingUnbuckle.Remove(buckle.Owner);
    }
  }

  private void OnActivePilotPreventCollide(
    Entity<ActiveWheelchairPilotComponent> ent,
    ref PreventCollideEvent args)
  {
    args.Cancelled = true;
  }

  private void OnActivePilotStunned<T>(Entity<ActiveWheelchairPilotComponent> ent, ref T args)
  {
    this.RemovePilot(ent);
  }

  private void OnActivePilotMobStateChanged(
    Entity<ActiveWheelchairPilotComponent> ent,
    ref MobStateChangedEvent args)
  {
    if (args.NewMobState != MobState.Critical && args.NewMobState != MobState.Dead)
      return;
    this.OnActivePilotStunned<MobStateChangedEvent>(ent, ref args);
  }

  private void RemovePilot(Entity<ActiveWheelchairPilotComponent> active)
  {
    if (this._processingUnbuckle.Contains(active.Owner))
      return;
    this.RemCompDeferred<ActiveWheelchairPilotComponent>((EntityUid) active);
  }

  public override void Update(float frameTime)
  {
    List<Entity<ActiveWheelchairPilotComponent>> entityList = new List<Entity<ActiveWheelchairPilotComponent>>();
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActiveWheelchairPilotComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActiveWheelchairPilotComponent>();
    EntityUid uid1;
    ActiveWheelchairPilotComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid1, out comp1))
    {
      BuckleComponent comp;
      if (!this.TryComp<BuckleComponent>(uid1, out comp) || !comp.BuckledTo.HasValue || !this.HasComp<WheelchairComponent>(comp.BuckledTo))
        entityList.Add((Entity<ActiveWheelchairPilotComponent>) (uid1, comp1));
    }
    foreach (Entity<ActiveWheelchairPilotComponent> uid2 in entityList)
      this.RemCompDeferred<ActiveWheelchairPilotComponent>((EntityUid) uid2);
  }

  private void OnRingBell(Entity<ActiveWheelchairPilotComponent> ent, ref RingBellActionEvent args)
  {
    BuckleComponent comp1;
    WheelchairComponent comp2;
    if (args.Handled || !this.TryComp<BuckleComponent>((EntityUid) ent, out comp1) || !this.TryComp<WheelchairComponent>(comp1.BuckledTo, out comp2))
      return;
    args.Handled = true;
    this._audio.PlayPredicted(comp2.BellSound, args.Performer, new EntityUid?(args.Performer));
  }
}
