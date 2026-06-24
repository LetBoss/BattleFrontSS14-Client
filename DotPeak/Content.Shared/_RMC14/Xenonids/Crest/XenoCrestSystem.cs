// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Crest.XenoCrestSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Fortify;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared._RMC14.Xenonids.Sweep;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.StatusEffectNew;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Crest;

public sealed class XenoCrestSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private CMArmorSystem _armor;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoCrestComponent, XenoToggleCrestActionEvent>(new EntityEventRefHandler<XenoCrestComponent, XenoToggleCrestActionEvent>(this.OnXenoCrestAction));
    this.SubscribeLocalEvent<XenoCrestComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<XenoCrestComponent, RefreshMovementSpeedModifiersEvent>(this.OnXenoCrestRefreshMovementSpeed));
    this.SubscribeLocalEvent<XenoCrestComponent, CMGetArmorEvent>(new EntityEventRefHandler<XenoCrestComponent, CMGetArmorEvent>(this.OnXenoCrestGetArmor));
    this.SubscribeLocalEvent<XenoCrestComponent, BeforeStatusEffectAddedEvent>(new EntityEventRefHandler<XenoCrestComponent, BeforeStatusEffectAddedEvent>(this.OnXenoCrestBeforeStatusAdded));
    this.SubscribeLocalEvent<XenoCrestComponent, XenoFortifyAttemptEvent>(new EntityEventRefHandler<XenoCrestComponent, XenoFortifyAttemptEvent>(this.OnXenoCrestFortifyAttempt));
    this.SubscribeLocalEvent<XenoCrestComponent, XenoTailSweepAttemptEvent>(new EntityEventRefHandler<XenoCrestComponent, XenoTailSweepAttemptEvent>(this.OnXenoCrestTailSweepAttempt));
    this.SubscribeLocalEvent<XenoCrestComponent, XenoRestAttemptEvent>(new EntityEventRefHandler<XenoCrestComponent, XenoRestAttemptEvent>(this.OnXenoCrestRestAttempt));
  }

  private void OnXenoCrestAction(
    Entity<XenoCrestComponent> xeno,
    ref XenoToggleCrestActionEvent args)
  {
    if (args.Handled)
      return;
    XenoToggleCrestAttemptEvent args1 = new XenoToggleCrestAttemptEvent();
    this.RaiseLocalEvent<XenoToggleCrestAttemptEvent>((EntityUid) xeno, ref args1);
    if (args1.Cancelled)
      return;
    args.Handled = true;
    RMCSizeComponent comp;
    if (this.TryComp<RMCSizeComponent>((EntityUid) xeno, out comp))
    {
      if (!xeno.Comp.Lowered)
      {
        xeno.Comp.OriginalSize = new RMCSizes?(comp.Size);
        comp.Size = xeno.Comp.CrestSize;
      }
      else
        comp.Size = xeno.Comp.OriginalSize ?? RMCSizes.Xeno;
      this.Dirty(xeno.Owner, (IComponent) comp);
    }
    xeno.Comp.Lowered = !xeno.Comp.Lowered;
    this.Dirty<XenoCrestComponent>(xeno);
    this._armor.UpdateArmorValue((Entity<CMArmorComponent>) ((EntityUid) xeno, (CMArmorComponent) null));
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) xeno);
    this._appearance.SetData((EntityUid) xeno, (Enum) XenoVisualLayers.Crest, (object) xeno.Comp.Lowered);
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoToggleCrestActionEvent>((EntityUid) xeno))
      this._actions.SetToggled(new Entity<ActionComponent>?(entity.AsNullable()), xeno.Comp.Lowered);
  }

  private void OnXenoCrestRefreshMovementSpeed(
    Entity<XenoCrestComponent> xeno,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    if (!xeno.Comp.Lowered)
      return;
    args.ModifySpeed(xeno.Comp.SpeedMultiplier, xeno.Comp.SpeedMultiplier);
  }

  private void OnXenoCrestGetArmor(Entity<XenoCrestComponent> xeno, ref CMGetArmorEvent args)
  {
    if (!xeno.Comp.Lowered)
      return;
    args.XenoArmor += xeno.Comp.Armor;
  }

  private void OnXenoCrestBeforeStatusAdded(
    Entity<XenoCrestComponent> xeno,
    ref BeforeStatusEffectAddedEvent args)
  {
    if (!xeno.Comp.Lowered || !((IEnumerable<string>) xeno.Comp.ImmuneToStatuses).Contains<string>(args.Effect.Id))
      return;
    args.Cancelled = true;
  }

  private void OnXenoCrestFortifyAttempt(
    Entity<XenoCrestComponent> xeno,
    ref XenoFortifyAttemptEvent args)
  {
    if (!xeno.Comp.Lowered)
      return;
    this._popup.PopupClient(this.Loc.GetString("cm-xeno-toggle-crest-cant-fortify"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    args.Cancelled = true;
  }

  private void OnXenoCrestTailSweepAttempt(
    Entity<XenoCrestComponent> xeno,
    ref XenoTailSweepAttemptEvent args)
  {
    if (!xeno.Comp.Lowered)
      return;
    this._popup.PopupClient(this.Loc.GetString("cm-xeno-toggle-crest-cant-tail-sweep"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    args.Cancelled = true;
  }

  private void OnXenoCrestRestAttempt(
    Entity<XenoCrestComponent> xeno,
    ref XenoRestAttemptEvent args)
  {
    if (!xeno.Comp.Lowered)
      return;
    this._popup.PopupClient(this.Loc.GetString("cm-xeno-toggle-crest-cant-rest"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    args.Cancelled = true;
  }
}
