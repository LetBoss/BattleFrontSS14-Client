// Decompiled with JetBrains decompiler
// Type: Content.Client.Wires.Visualizers.WiresVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Wires;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Wires.Visualizers;

public sealed class WiresVisualizerSystem : VisualizerSystem<WiresVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    WiresVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    int num = this.SpriteSystem.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) WiresVisualLayers.MaintenancePanel);
    object obj;
    if (args.AppearanceData.TryGetValue((Enum) WiresVisuals.MaintenancePanelState, out obj) && obj is bool flag)
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, flag);
    else
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, false);
  }
}
