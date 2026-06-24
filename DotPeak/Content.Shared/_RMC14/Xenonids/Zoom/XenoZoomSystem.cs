// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Zoom.XenoZoomSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Camera;
using Content.Shared.DoAfter;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Zoom;

public sealed class XenoZoomSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedContentEyeSystem _contentEye;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoZoomComponent, XenoZoomActionEvent>(new EntityEventRefHandler<XenoZoomComponent, XenoZoomActionEvent>(this.OnXenoZoomAction));
    this.SubscribeLocalEvent<XenoZoomComponent, XenoZoomDoAfterEvent>(new EntityEventRefHandler<XenoZoomComponent, XenoZoomDoAfterEvent>(this.OnXenoZoomDoAfter));
    this.SubscribeLocalEvent<XenoZoomComponent, GetEyeOffsetEvent>(new EntityEventRefHandler<XenoZoomComponent, GetEyeOffsetEvent>(this.OnXenoZoomGetEyeOffset));
    this.SubscribeLocalEvent<XenoZoomComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<XenoZoomComponent, RefreshMovementSpeedModifiersEvent>(this.OnXenoZoomRefreshSpeed));
    this.SubscribeLocalEvent<XenoZoomComponent, XenoLeapAttemptEvent>(new EntityEventRefHandler<XenoZoomComponent, XenoLeapAttemptEvent>(this.OnLeapAttempt));
    this.SubscribeLocalEvent<XenoZoomComponent, XenoRestEvent>(new EntityEventRefHandler<XenoZoomComponent, XenoRestEvent>(this.OnRest));
  }

  private void OnXenoZoomAction(Entity<XenoZoomComponent> xeno, ref XenoZoomActionEvent args)
  {
    XenoZoomDoAfterEvent @event = new XenoZoomDoAfterEvent();
    TimeSpan delay = xeno.Comp.Enabled ? TimeSpan.Zero : xeno.Comp.DoAfter;
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) xeno, delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) xeno))
    {
      BreakOnMove = true
    });
  }

  private void OnXenoZoomDoAfter(Entity<XenoZoomComponent> xeno, ref XenoZoomDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    xeno.Comp.Enabled = !xeno.Comp.Enabled;
    if (xeno.Comp.Enabled)
    {
      this._contentEye.SetMaxZoom((EntityUid) xeno, xeno.Comp.Zoom);
      this._contentEye.SetZoom((EntityUid) xeno, xeno.Comp.Zoom);
      XenoZoomComponent comp = xeno.Comp;
      Angle localRotation = this.Transform(args.User).LocalRotation;
      Vector2 vector2 = DirectionExtensions.ToVec(((Angle) ref localRotation).GetCardinalDir()) * (float) xeno.Comp.OffsetLength;
      comp.Offset = vector2;
    }
    else
    {
      this._contentEye.ResetZoom((EntityUid) xeno);
      xeno.Comp.Offset = Vector2.Zero;
    }
    this.Dirty<XenoZoomComponent>(xeno);
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoZoomActionEvent>((EntityUid) xeno))
      this._actions.SetToggled(new Entity<ActionComponent>?((Entity<ActionComponent>) ((EntityUid) entity, (ActionComponent) entity)), xeno.Comp.Enabled);
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) xeno);
    EyeComponent comp1;
    if (!this.TryComp<EyeComponent>((EntityUid) xeno, out comp1))
      return;
    this._contentEye.UpdateEyeOffset((Entity<EyeComponent>) (xeno.Owner, comp1));
  }

  private void OnXenoZoomGetEyeOffset(Entity<XenoZoomComponent> ent, ref GetEyeOffsetEvent args)
  {
    args.Offset += ent.Comp.Offset;
  }

  private void OnXenoZoomRefreshSpeed(
    Entity<XenoZoomComponent> ent,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    if (!ent.Comp.Enabled)
      return;
    args.ModifySpeed(ent.Comp.Speed, ent.Comp.Speed);
  }

  private void OnLeapAttempt(Entity<XenoZoomComponent> ent, ref XenoLeapAttemptEvent args)
  {
    if (!ent.Comp.Enabled || !ent.Comp.BlockLeaps)
      return;
    args.Cancelled = true;
  }

  private void OnRest(Entity<XenoZoomComponent> ent, ref XenoRestEvent args)
  {
    if (!ent.Comp.Enabled)
      return;
    ent.Comp.Enabled = false;
    this._contentEye.ResetZoom((EntityUid) ent);
    ent.Comp.Offset = Vector2.Zero;
    this.Dirty<XenoZoomComponent>(ent);
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) ent);
    EyeComponent comp;
    if (this.TryComp<EyeComponent>((EntityUid) ent, out comp))
      this._contentEye.UpdateEyeOffset((Entity<EyeComponent>) (ent.Owner, comp));
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoZoomActionEvent>((EntityUid) ent))
      this._actions.SetToggled(new Entity<ActionComponent>?((Entity<ActionComponent>) ((EntityUid) entity, (ActionComponent) entity)), ent.Comp.Enabled);
  }
}
