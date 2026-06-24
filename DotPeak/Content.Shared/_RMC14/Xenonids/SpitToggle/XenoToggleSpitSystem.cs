// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.SpitToggle.XenoToggleSpitSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Projectile.Spit.Standard;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.SpitToggle;

public sealed class XenoToggleSpitSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoToggleSpitComponent, XenoSpitToggleActionEvent>(new EntityEventRefHandler<XenoToggleSpitComponent, XenoSpitToggleActionEvent>(this.OnToggleSpit));
  }

  private void OnToggleSpit(
    Entity<XenoToggleSpitComponent> xeno,
    ref XenoSpitToggleActionEvent args)
  {
    XenoSpitComponent comp;
    if (args.Handled || !this.TryComp<XenoSpitComponent>((EntityUid) xeno, out comp))
      return;
    args.Handled = true;
    xeno.Comp.UseAcid = !xeno.Comp.UseAcid;
    this._actions.SetToggled(new Entity<ActionComponent>?(args.Action.AsNullable()), xeno.Comp.UseAcid);
    EntProtoId entProtoId = xeno.Comp.UseAcid ? xeno.Comp.AcidProto : xeno.Comp.NeuroProto;
    FixedPoint2 fixedPoint2 = xeno.Comp.UseAcid ? xeno.Comp.AcidCost : xeno.Comp.NeuroCost;
    comp.PlasmaCost = fixedPoint2;
    comp.ProjectileId = entProtoId;
    this.Dirty<XenoToggleSpitComponent>(xeno);
  }
}
