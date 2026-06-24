// Decompiled with JetBrains decompiler
// Type: Content.Client.PowerCell.PowerChargerVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Power;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.PowerCell;

public sealed class PowerChargerVisualizerSystem : VisualizerSystem<PowerChargerVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    PowerChargerVisualsComponent comp,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    bool flag;
    if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) CellVisual.Occupied, ref flag, args.Component) & flag)
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PowerChargerVisualLayers.Base, RSI.StateId.op_Implicit(comp.OccupiedState));
    else
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PowerChargerVisualLayers.Base, RSI.StateId.op_Implicit(comp.EmptyState));
    CellChargerStatus key;
    string str;
    if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<CellChargerStatus>(uid, (Enum) CellVisual.Light, ref key, args.Component) && comp.LightStates.TryGetValue(key, out str))
    {
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PowerChargerVisualLayers.Light, RSI.StateId.op_Implicit(str));
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PowerChargerVisualLayers.Light, true);
    }
    else
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PowerChargerVisualLayers.Light, false);
  }
}
