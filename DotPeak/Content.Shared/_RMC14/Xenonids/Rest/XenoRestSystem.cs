// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Rest.XenoRestSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Evasion;
using Content.Shared._RMC14.Xenonids.Charge;
using Content.Shared._RMC14.Xenonids.Construction.Events;
using Content.Shared._RMC14.Xenonids.Crest;
using Content.Shared._RMC14.Xenonids.Fling;
using Content.Shared._RMC14.Xenonids.Fortify;
using Content.Shared._RMC14.Xenonids.Gut;
using Content.Shared._RMC14.Xenonids.Headbutt;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared._RMC14.Xenonids.Lunge;
using Content.Shared._RMC14.Xenonids.Punch;
using Content.Shared._RMC14.Xenonids.Screech;
using Content.Shared._RMC14.Xenonids.Stomp;
using Content.Shared._RMC14.Xenonids.Sweep;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction.Events;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Rest;

public sealed class XenoRestSystem : EntitySystem
{
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IGameTiming _timing;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoComponent, XenoRestActionEvent>(new EntityEventRefHandler<XenoComponent, XenoRestActionEvent>(this.OnXenoRestAction));
    this.SubscribeLocalEvent<XenoRestingComponent, UpdateCanMoveEvent>(new EntityEventRefHandler<XenoRestingComponent, UpdateCanMoveEvent>(this.OnXenoRestingCanMove));
    this.SubscribeLocalEvent<XenoRestingComponent, AttackAttemptEvent>(new EntityEventRefHandler<XenoRestingComponent, AttackAttemptEvent>(this.OnXenoRestingMeleeHit));
    this.SubscribeLocalEvent<XenoRestingComponent, XenoSecreteStructureAttemptEvent>(new EntityEventRefHandler<XenoRestingComponent, XenoSecreteStructureAttemptEvent>(this.OnXenoSecreteStructureAttempt));
    this.SubscribeLocalEvent<XenoRestingComponent, XenoHeadbuttAttemptEvent>(new EntityEventRefHandler<XenoRestingComponent, XenoHeadbuttAttemptEvent>(this.OnXenoRestingHeadbuttAttempt));
    this.SubscribeLocalEvent<XenoRestingComponent, XenoFortifyAttemptEvent>(new EntityEventRefHandler<XenoRestingComponent, XenoFortifyAttemptEvent>(this.OnXenoRestingFortifyAttempt));
    this.SubscribeLocalEvent<XenoRestingComponent, XenoTailSweepAttemptEvent>(new EntityEventRefHandler<XenoRestingComponent, XenoTailSweepAttemptEvent>(this.OnXenoRestingTailSweepAttempt));
    this.SubscribeLocalEvent<XenoRestingComponent, XenoToggleCrestAttemptEvent>(new EntityEventRefHandler<XenoRestingComponent, XenoToggleCrestAttemptEvent>(this.OnXenoRestingToggleCrestAttempt));
    this.SubscribeLocalEvent<XenoRestingComponent, XenoLeapAttemptEvent>(new EntityEventRefHandler<XenoRestingComponent, XenoLeapAttemptEvent>(this.OnXenoRestingLeapAttempt));
    this.SubscribeLocalEvent<XenoRestingComponent, XenoLungeAttemptEvent>(new EntityEventRefHandler<XenoRestingComponent, XenoLungeAttemptEvent>(this.OnXenoRestingLungeAttempt));
    this.SubscribeLocalEvent<XenoRestingComponent, XenoPunchAttemptEvent>(new EntityEventRefHandler<XenoRestingComponent, XenoPunchAttemptEvent>(this.OnXenoRestingPunchAttempt));
    this.SubscribeLocalEvent<XenoRestingComponent, XenoFlingAttemptEvent>(new EntityEventRefHandler<XenoRestingComponent, XenoFlingAttemptEvent>(this.OnXenoRestingFlingAttempt));
    this.SubscribeLocalEvent<XenoRestingComponent, XenoChargeAttemptEvent>(new EntityEventRefHandler<XenoRestingComponent, XenoChargeAttemptEvent>(this.OnXenoRestingChargettempt));
    this.SubscribeLocalEvent<XenoRestingComponent, XenoStompAttemptEvent>(new EntityEventRefHandler<XenoRestingComponent, XenoStompAttemptEvent>(this.OnXenoRestingStompAttempt));
    this.SubscribeLocalEvent<XenoRestingComponent, XenoGutAttemptEvent>(new EntityEventRefHandler<XenoRestingComponent, XenoGutAttemptEvent>(this.OnXenoRestingGutAttempt));
    this.SubscribeLocalEvent<XenoRestingComponent, XenoScreechAttemptEvent>(new EntityEventRefHandler<XenoRestingComponent, XenoScreechAttemptEvent>(this.OnXenoRestingScreechAttempt));
    this.SubscribeLocalEvent<XenoRestingComponent, EvasionRefreshModifiersEvent>(new EntityEventRefHandler<XenoRestingComponent, EvasionRefreshModifiersEvent>(this.OnXenoRestingEvasionRefresh));
    this.SubscribeLocalEvent<XenoRestingComponent, AttemptMobCollideEvent>(new EntityEventRefHandler<XenoRestingComponent, AttemptMobCollideEvent>(this.OnXenoRestingMobCollide));
    this.SubscribeLocalEvent<XenoRestingComponent, AttemptMobTargetCollideEvent>(new EntityEventRefHandler<XenoRestingComponent, AttemptMobTargetCollideEvent>(this.OnXenoRestingMobTargetCollide));
    this.SubscribeLocalEvent<ActionBlockIfRestingComponent, RMCActionUseAttemptEvent>(new EntityEventRefHandler<ActionBlockIfRestingComponent, RMCActionUseAttemptEvent>(this.OnXenoRestingActionUseAttempt));
  }

  private void OnXenoRestingActionUseAttempt(
    Entity<ActionBlockIfRestingComponent> ent,
    ref RMCActionUseAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    EntityUid user = args.User;
    if (!this.HasComp<XenoRestingComponent>(user))
      return;
    args.Cancelled = true;
    this._popup.PopupClient(this.Loc.GetString((string) ent.Comp.Popup), user, new EntityUid?(user), PopupType.SmallCaution);
  }

  private void OnXenoRestingCanMove(Entity<XenoRestingComponent> xeno, ref UpdateCanMoveEvent args)
  {
    args.Cancel();
  }

  private void OnXenoRestAction(Entity<XenoComponent> xeno, ref XenoRestActionEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    XenoRestAttemptEvent args1 = new XenoRestAttemptEvent();
    this.RaiseLocalEvent<XenoRestAttemptEvent>((EntityUid) xeno, ref args1);
    if (args1.Cancelled)
      return;
    args.Handled = true;
    if (this.HasComp<XenoRestingComponent>((EntityUid) xeno))
    {
      this.RemComp<XenoRestingComponent>((EntityUid) xeno);
      this._appearance.SetData((EntityUid) xeno, (Enum) XenoVisualLayers.Base, (object) XenoRestState.NotResting);
      this._actions.SetToggled(new Entity<ActionComponent>?(args.Action.AsNullable()), false);
    }
    else
    {
      this.AddComp<XenoRestingComponent>((EntityUid) xeno);
      this._appearance.SetData((EntityUid) xeno, (Enum) XenoVisualLayers.Base, (object) XenoRestState.Resting);
      this._actions.SetToggled(new Entity<ActionComponent>?(args.Action.AsNullable()), true);
    }
    this._actionBlocker.UpdateCanMove((EntityUid) xeno);
    XenoRestEvent args2 = new XenoRestEvent(this.HasComp<XenoRestingComponent>((EntityUid) xeno));
    this.RaiseLocalEvent<XenoRestEvent>((EntityUid) xeno, ref args2);
  }

  private void OnXenoRestingMeleeHit(Entity<XenoRestingComponent> xeno, ref AttackAttemptEvent args)
  {
    args.Cancel();
  }

  private void OnXenoSecreteStructureAttempt(
    Entity<XenoRestingComponent> xeno,
    ref XenoSecreteStructureAttemptEvent args)
  {
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-rest-cant-secrete"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    args.Cancelled = true;
  }

  private void OnXenoRestingHeadbuttAttempt(
    Entity<XenoRestingComponent> xeno,
    ref XenoHeadbuttAttemptEvent args)
  {
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-rest-cant-headbutt"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    args.Cancelled = true;
  }

  private void OnXenoRestingFortifyAttempt(
    Entity<XenoRestingComponent> xeno,
    ref XenoFortifyAttemptEvent args)
  {
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-rest-cant-fortify"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    args.Cancelled = true;
  }

  private void OnXenoRestingTailSweepAttempt(
    Entity<XenoRestingComponent> xeno,
    ref XenoTailSweepAttemptEvent args)
  {
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-rest-cant-tail-sweep"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    args.Cancelled = true;
  }

  private void OnXenoRestingToggleCrestAttempt(
    Entity<XenoRestingComponent> xeno,
    ref XenoToggleCrestAttemptEvent args)
  {
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-rest-cant-toggle-crest"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    args.Cancelled = true;
  }

  private void OnXenoRestingLeapAttempt(
    Entity<XenoRestingComponent> xeno,
    ref XenoLeapAttemptEvent args)
  {
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-rest-cant-leap"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    args.Cancelled = true;
  }

  private void OnXenoRestingLungeAttempt(
    Entity<XenoRestingComponent> xeno,
    ref XenoLungeAttemptEvent args)
  {
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-rest-cant-lunge"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    args.Cancelled = true;
  }

  private void OnXenoRestingPunchAttempt(
    Entity<XenoRestingComponent> xeno,
    ref XenoPunchAttemptEvent args)
  {
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-rest-cant-punch"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    args.Cancelled = true;
  }

  private void OnXenoRestingFlingAttempt(
    Entity<XenoRestingComponent> xeno,
    ref XenoFlingAttemptEvent args)
  {
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-rest-cant-fling"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    args.Cancelled = true;
  }

  private void OnXenoRestingChargettempt(
    Entity<XenoRestingComponent> xeno,
    ref XenoChargeAttemptEvent args)
  {
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-rest-cant-charge"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    args.Cancelled = true;
  }

  private void OnXenoRestingStompAttempt(
    Entity<XenoRestingComponent> xeno,
    ref XenoStompAttemptEvent args)
  {
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-rest-cant-stomp"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    args.Cancelled = true;
  }

  private void OnXenoRestingGutAttempt(
    Entity<XenoRestingComponent> xeno,
    ref XenoGutAttemptEvent args)
  {
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-rest-cant-gut"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    args.Cancelled = true;
  }

  private void OnXenoRestingScreechAttempt(
    Entity<XenoRestingComponent> xeno,
    ref XenoScreechAttemptEvent args)
  {
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-rest-cant-screech"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    args.Cancelled = true;
  }

  private void OnXenoRestingEvasionRefresh(
    Entity<XenoRestingComponent> xeno,
    ref EvasionRefreshModifiersEvent args)
  {
    if (xeno.Owner != args.Entity.Owner)
      return;
    args.Evasion += (FixedPoint2) -15;
  }

  private void OnXenoRestingMobCollide(
    Entity<XenoRestingComponent> ent,
    ref AttemptMobCollideEvent args)
  {
    args.Cancelled = true;
  }

  private void OnXenoRestingMobTargetCollide(
    Entity<XenoRestingComponent> ent,
    ref AttemptMobTargetCollideEvent args)
  {
    args.Cancelled = true;
  }

  public bool IsResting(Entity<XenoRestingComponent?> ent)
  {
    return this.Resolve<XenoRestingComponent>((EntityUid) ent, ref ent.Comp, false);
  }
}
