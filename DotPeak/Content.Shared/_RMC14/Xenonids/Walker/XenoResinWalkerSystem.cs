// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Walker.XenoResinWalkerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Walker;

public sealed class XenoResinWalkerSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private XenoPlasmaSystem _xenoPlasma;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoResinWalkerComponent, XenoResinWalkerActionEvent>(new EntityEventRefHandler<XenoResinWalkerComponent, XenoResinWalkerActionEvent>(this.OnXenoResinWalkerAction));
    this.SubscribeLocalEvent<XenoResinWalkerComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<XenoResinWalkerComponent, RefreshMovementSpeedModifiersEvent>(this.OnXenoResinWalkerRefreshMovementSpeed));
    this.UpdatesAfter.Add(typeof (SharedPhysicsSystem));
  }

  private void OnXenoResinWalkerAction(
    Entity<XenoResinWalkerComponent> xeno,
    ref XenoResinWalkerActionEvent args)
  {
    if (args.Handled || !xeno.Comp.Active && !this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, xeno.Comp.PlasmaCost))
      return;
    args.Handled = true;
    xeno.Comp.Active = !xeno.Comp.Active;
    xeno.Comp.NextPlasmaUse = this._timing.CurTime + xeno.Comp.PlasmaUseDelay;
    this.Dirty<XenoResinWalkerComponent>(xeno);
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) xeno);
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoResinWalkerActionEvent>((EntityUid) xeno))
      this._actions.SetToggled(new Entity<ActionComponent>?((Entity<ActionComponent>) ((EntityUid) entity, (ActionComponent) entity)), xeno.Comp.Active);
  }

  private void OnXenoResinWalkerRefreshMovementSpeed(
    Entity<XenoResinWalkerComponent> ent,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    AffectableByWeedsComponent comp;
    if (!ent.Comp.Active || !this.TryComp<AffectableByWeedsComponent>((EntityUid) ent, out comp) || !comp.OnXenoWeeds)
      return;
    args.ModifySpeed(ent.Comp.SpeedMultiplier, ent.Comp.SpeedMultiplier);
  }

  public override void Update(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoResinWalkerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoResinWalkerComponent>();
    EntityUid uid;
    XenoResinWalkerComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.Active && !(this._timing.CurTime < comp1.NextPlasmaUse))
      {
        comp1.NextPlasmaUse = this._timing.CurTime + comp1.PlasmaUseDelay;
        if (!this._xenoPlasma.TryRemovePlasma((Entity<XenoPlasmaComponent>) uid, comp1.PlasmaUpkeep))
        {
          comp1.Active = false;
          this.Dirty(uid, (IComponent) comp1);
          this._movementSpeed.RefreshMovementSpeedModifiers(uid);
        }
      }
    }
  }
}
