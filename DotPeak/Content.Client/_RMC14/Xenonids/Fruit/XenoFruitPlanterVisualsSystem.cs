// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Fruit.XenoFruitPlanterVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.Fruit.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Fruit;

public sealed class XenoFruitPlanterVisualsSystem : 
  VisualizerSystem<XenoFruitPlanterVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    XenoFruitPlanterVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    SpriteComponent sprite = args.Sprite;
    int num;
    if (sprite == null || !this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) XenoFruitVisualLayers.Base, ref num, false))
      return;
    Color color;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<Color>(uid, (Enum) XenoFruitPlanterVisuals.Color, ref color, (AppearanceComponent) null))
    {
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, false);
    }
    else
    {
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, true);
      this.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, color);
      bool flag1;
      if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) XenoFruitPlanterVisuals.Downed, ref flag1, (AppearanceComponent) null) & flag1)
      {
        this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, RSI.StateId.op_Implicit(component.Prefix + "_downed"));
      }
      else
      {
        bool flag2;
        if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) XenoFruitPlanterVisuals.Resting, ref flag2, (AppearanceComponent) null) & flag2)
          this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, RSI.StateId.op_Implicit(component.Prefix + "_rest"));
        else
          this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, RSI.StateId.op_Implicit(component.Prefix + "_walk"));
      }
    }
  }
}
