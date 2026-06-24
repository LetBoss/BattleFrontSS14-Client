// Decompiled with JetBrains decompiler
// Type: Content.Shared.Kitchen.SharedKitchenSpikeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DragDrop;
using Content.Shared.Kitchen.Components;
using Content.Shared.Nutrition.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Kitchen;

public abstract class SharedKitchenSpikeSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<KitchenSpikeComponent, CanDropTargetEvent>(new ComponentEventRefHandler<KitchenSpikeComponent, CanDropTargetEvent>(this.OnCanDrop));
  }

  private void OnCanDrop(
    EntityUid uid,
    KitchenSpikeComponent component,
    ref CanDropTargetEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    if (!this.HasComp<ButcherableComponent>(args.Dragged))
      args.CanDrop = false;
    else
      args.CanDrop = true;
  }
}
