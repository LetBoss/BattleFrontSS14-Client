// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.GasToggle.XenoGasToggleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.GasToggle;

public sealed class XenoGasToggleSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoGasToggleComponent, XenoGasToggleActionEvent>(new EntityEventRefHandler<XenoGasToggleComponent, XenoGasToggleActionEvent>(this.OnToggleType));
  }

  private void OnToggleType(Entity<XenoGasToggleComponent> xeno, ref XenoGasToggleActionEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    xeno.Comp.IsNeurotoxin = !xeno.Comp.IsNeurotoxin;
    this._actions.SetToggled(new Entity<ActionComponent>?(args.Action.AsNullable()), xeno.Comp.IsNeurotoxin);
    this.Dirty<XenoGasToggleComponent>(xeno);
  }
}
