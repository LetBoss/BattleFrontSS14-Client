// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Inhands.XenoInhandsVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Inhands;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Inhands;

public sealed class XenoInhandsVisualsSystem : VisualizerSystem<XenoInhandsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    XenoInhandsComponent component,
    ref AppearanceChangeEvent args)
  {
    SpriteComponent sprite = args.Sprite;
    string str1;
    string str2;
    if (sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<string>(uid, (Enum) XenoInhandVisuals.RightHand, ref str1, (AppearanceComponent) null) || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<string>(uid, (Enum) XenoInhandVisuals.LeftHand, ref str2, (AppearanceComponent) null))
      return;
    bool flag1 = false;
    bool flag2 = false;
    bool flag3 = false;
    ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) RMCXenoStateVisuals.Downed, ref flag1, (AppearanceComponent) null);
    ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) RMCXenoStateVisuals.Resting, ref flag2, (AppearanceComponent) null);
    ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) RMCXenoStateVisuals.Ovipositor, ref flag3, (AppearanceComponent) null);
    string str3 = str2;
    XenoInhandVisualLayers inhandVisualLayers = XenoInhandVisualLayers.Left;
    for (int index = 0; index < 2; ++index)
    {
      if (index == 1)
      {
        str3 = str1;
        inhandVisualLayers = XenoInhandVisualLayers.Right;
      }
      int num;
      if (this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) inhandVisualLayers, ref num, false))
      {
        if (str3 == string.Empty)
        {
          this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, false);
        }
        else
        {
          this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, true);
          string str4 = $"{component.Prefix}_{str3}_{inhandVisualLayers.ToString().ToLower()}";
          if (flag3)
            str4 = $"{str4}_{component.Ovi}";
          else if (flag1)
            str4 = $"{str4}_{component.Downed}";
          else if (flag2)
            str4 = $"{str4}_{component.Resting}";
          RSI effectiveRsi = this.SpriteSystem.LayerGetEffectiveRsi(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num);
          if (effectiveRsi != null)
          {
            RSI.State state;
            effectiveRsi.TryGetState(RSI.StateId.op_Implicit(str4), ref state);
            if (state != null)
              this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, RSI.StateId.op_Implicit(str4));
            else
              this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, false);
          }
        }
      }
    }
  }
}
