// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Construction.EggmorpherVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.Construction.EggMorpher;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Construction;

public sealed class EggmorpherVisualizerSystem : VisualizerSystem<EggMorpherComponent>
{
  [Dependency]
  private SpriteSystem _sprite;

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    EggMorpherComponent component,
    ref AppearanceChangeEvent args)
  {
    SpriteComponent sprite = args.Sprite;
    int num1;
    int num2;
    int num3;
    if (sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<int>(uid, (Enum) EggmorpherOverlayVisuals.Number, ref num1, (AppearanceComponent) null) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) EggmorpherOverlayLayers.Overlay, ref num2, false) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) EggmorpherOverlayLayers.Base, ref num3, false))
      return;
    int num4 = (int) Math.Min(Math.Ceiling((double) num1 / (double) component.MaxParasites * (double) component.OverlayCount), (double) component.OverlayCount);
    if (num4 == 0)
    {
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, false);
    }
    else
    {
      bool flag = true;
      if (!sprite[num2].Visible)
      {
        this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, true);
        flag = false;
      }
      string str = $"{component.OverlayPrefix}_{(num4 - 1).ToString()}";
      if (!(str != this._sprite.LayerGetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2).Name) && flag)
        return;
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, RSI.StateId.op_Implicit(str));
      RSI.StateId rsiState = this._sprite.LayerGetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num3);
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num3, RSI.StateId.op_Implicit(str));
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num3, rsiState);
    }
  }
}
