// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Hide.XenoHideSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Hide;

public sealed class XenoHideSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoHideComponent, XenoHideActionEvent>(new EntityEventRefHandler<XenoHideComponent, XenoHideActionEvent>(this.OnXenoHideAction));
  }

  private void OnXenoHideAction(Entity<XenoHideComponent> xeno, ref XenoHideActionEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    xeno.Comp.Hiding = !xeno.Comp.Hiding;
    this.Dirty<XenoHideComponent>(xeno);
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoHideActionEvent>((EntityUid) xeno))
      this._actions.SetToggled(new Entity<ActionComponent>?(entity.AsNullable()), xeno.Comp.Hiding);
    this._appearance.SetData((EntityUid) xeno, (Enum) XenoVisualLayers.Hide, (object) xeno.Comp.Hiding);
  }
}
