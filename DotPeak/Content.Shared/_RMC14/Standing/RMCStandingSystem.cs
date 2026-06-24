// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Standing.RMCStandingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Evasion;
using Content.Shared._RMC14.Input;
using Content.Shared.Buckle.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory.Events;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Pulling.Events;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Standing;

public sealed class RMCStandingSystem : EntitySystem
{
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private StandingStateSystem _standing;
  [Dependency]
  private MobStateSystem _mob;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IGameTiming _timing;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<DropItemsOnRestComponent, IsEquippingAttemptEvent>(new EntityEventRefHandler<DropItemsOnRestComponent, IsEquippingAttemptEvent>(this.OnDropIsEquippingAttempt));
    this.SubscribeLocalEvent<DropItemsOnRestComponent, IsUnequippingAttemptEvent>(new EntityEventRefHandler<DropItemsOnRestComponent, IsUnequippingAttemptEvent>(this.OnDropIsUnequippingAttempt));
    this.SubscribeLocalEvent<DownOnEnterComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<DownOnEnterComponent, EntInsertedIntoContainerMessage>(this.OnEnterDown));
    this.SubscribeLocalEvent<DownOnEnterComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<DownOnEnterComponent, EntRemovedFromContainerMessage>(this.OnLeaveDown));
    this.SubscribeLocalEvent<StandingStateComponent, EvasionRefreshModifiersEvent>(new EntityEventRefHandler<StandingStateComponent, EvasionRefreshModifiersEvent>(this.OnStandingStateEvasionRefresh));
    this.SubscribeLocalEvent<RMCRestComponent, StoodEvent>(new EntityEventRefHandler<RMCRestComponent, StoodEvent>(this.OnRestStood));
    this.SubscribeLocalEvent<RMCRestComponent, StandAttemptEvent>(new EntityEventRefHandler<RMCRestComponent, StandAttemptEvent>(this.OnRestStandAttempt));
    this.SubscribeLocalEvent<RMCRestComponent, StartPullAttemptEvent>(new EntityEventRefHandler<RMCRestComponent, StartPullAttemptEvent>(this.OnRestStartPullAttempt));
    CommandBinds.Builder.Bind(CMKeyFunctions.RMCRest, InputCmdHandler.FromDelegate((StateInputCmdDelegate) (session =>
    {
      EntityUid? attachedEntity = (EntityUid?) session?.AttachedEntity;
      if (!attachedEntity.HasValue)
        return;
      EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
      RMCRestComponent comp;
      if (!this.TryComp<RMCRestComponent>(valueOrDefault, out comp))
        return;
      TimeSpan curTime = this._timing.CurTime;
      if (curTime < comp.LastToggleAt + comp.Cooldown || !comp.Resting)
        return;
      this.SetRest((Entity<RMCRestComponent>) (valueOrDefault, comp), false);
      if (this._standing.IsDown(valueOrDefault))
        this._popup.PopupClient(this.Loc.GetString("rmc-standing-stand-when-able"), valueOrDefault, new EntityUid?(valueOrDefault), PopupType.Medium);
      comp.LastToggleAt = curTime;
      this._movementSpeed.RefreshMovementSpeedModifiers(valueOrDefault);
    }), handle: false)).Register<RMCStandingSystem>();
  }

  public override void Shutdown()
  {
    base.Shutdown();
    CommandBinds.Unregister<RMCStandingSystem>();
  }

  private void OnDropBuckled(Entity<DropItemsOnRestComponent> drop, ref BuckledEvent args)
  {
    if (!this._standing.IsDown((EntityUid) drop))
      return;
    foreach (EntityUid entity in this._hands.EnumerateHeld((Entity<HandsComponent>) drop.Owner))
      this._hands.TryDrop((Entity<HandsComponent>) drop.Owner, entity);
  }

  private void CancelIfResting<T>(Entity<DropItemsOnRestComponent> drop, ref T args) where T : CancellableEntityEventArgs
  {
    this.TryCancelIfResting<T>(drop, ref args);
  }

  private void OnDropIsEquippingAttempt(
    Entity<DropItemsOnRestComponent> drop,
    ref IsEquippingAttemptEvent args)
  {
    if (args.Cancelled || !(args.Equipee == args.EquipTarget) || !this.TryCancelIfResting<IsEquippingAttemptEvent>(drop, ref args))
      return;
    args.Reason = "rmc-cant-while-resting";
  }

  private void OnDropIsUnequippingAttempt(
    Entity<DropItemsOnRestComponent> drop,
    ref IsUnequippingAttemptEvent args)
  {
    if (args.Cancelled || !(args.Unequipee == args.UnEquipTarget) || !this.TryCancelIfResting<IsUnequippingAttemptEvent>(drop, ref args))
      return;
    args.Reason = "rmc-cant-while-resting";
  }

  private bool TryCancelIfResting<T>(Entity<DropItemsOnRestComponent> drop, ref T args) where T : CancellableEntityEventArgs
  {
    if (args.Cancelled || !this._standing.IsDown((EntityUid) drop))
      return false;
    args.Cancel();
    return true;
  }

  private void OnEnterDown(
    Entity<DownOnEnterComponent> mob,
    ref EntInsertedIntoContainerMessage args)
  {
    this._standing.Down(args.Entity, false, false, true, true);
  }

  private void OnLeaveDown(
    Entity<DownOnEnterComponent> mob,
    ref EntRemovedFromContainerMessage args)
  {
    if (this.HasComp<KnockedDownComponent>(args.Entity) || this._mob.IsIncapacitated(args.Entity))
      this._standing.Down(args.Entity, false, force: true, changeCollision: true);
    else
      this._standing.Stand(args.Entity);
  }

  private void OnStandingStateEvasionRefresh(
    Entity<StandingStateComponent> entity,
    ref EvasionRefreshModifiersEvent args)
  {
    if (entity.Owner != args.Entity.Owner || !this._standing.IsDown(entity.Owner, entity.Comp))
      return;
    args.Evasion += (FixedPoint2) -15;
  }

  private void OnRestStood(Entity<RMCRestComponent> ent, ref StoodEvent args)
  {
    ent.Comp.Resting = false;
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) ent);
    this.Dirty<RMCRestComponent>(ent);
  }

  private void OnRestStandAttempt(Entity<RMCRestComponent> ent, ref StandAttemptEvent args)
  {
    if (!ent.Comp.Resting)
      return;
    args.Cancel();
  }

  private void OnRestStartPullAttempt(Entity<RMCRestComponent> ent, ref StartPullAttemptEvent args)
  {
    if (args.Cancelled || args.Puller != ent.Owner || !ent.Comp.Resting)
      return;
    args.Cancel();
  }

  public void SetRest(Entity<RMCRestComponent?> rest, bool resting)
  {
    if (!this.Resolve<RMCRestComponent>((EntityUid) rest, ref rest.Comp, false))
      return;
    rest.Comp.Resting = resting;
    this.Dirty<RMCRestComponent>(rest);
    if (resting)
      return;
    this._standing.Stand((EntityUid) rest);
  }
}
