// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.Visualizers.LightBulbSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Light.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Light.Visualizers;

public sealed class LightBulbSystem : VisualizerSystem<LightBulbComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    LightBulbComponent comp,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    LightBulbState lightBulbState;
    if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<LightBulbState>(uid, (Enum) LightBulbVisuals.State, ref lightBulbState, args.Component))
    {
      switch (lightBulbState)
      {
        case LightBulbState.Normal:
          this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) LightBulbVisualLayers.Base, RSI.StateId.op_Implicit(comp.NormalSpriteState));
          break;
        case LightBulbState.Broken:
          this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) LightBulbVisualLayers.Base, RSI.StateId.op_Implicit(comp.BrokenSpriteState));
          break;
        case LightBulbState.Burned:
          this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) LightBulbVisualLayers.Base, RSI.StateId.op_Implicit(comp.BurnedSpriteState));
          break;
      }
    }
    Color color;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<Color>(uid, (Enum) LightBulbVisuals.Color, ref color, args.Component))
      return;
    this.SpriteSystem.SetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), color);
  }
}
