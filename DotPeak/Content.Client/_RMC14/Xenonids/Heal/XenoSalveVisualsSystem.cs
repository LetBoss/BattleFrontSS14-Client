// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Heal.XenoSalveVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Heal;
using Content.Shared._RMC14.Xenonids.Salve;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Heal;

public sealed class XenoSalveVisualsSystem : VisualizerSystem<XenoSalveVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    XenoSalveVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    SpriteComponent sprite = args.Sprite;
    bool flag1;
    int num;
    if (sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) XenoHealerVisuals.Gooped, ref flag1, (AppearanceComponent) null) || !this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) XenoHealerVisualLayers.Goop, ref num, false))
      return;
    if (!flag1)
    {
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, false);
    }
    else
    {
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, true);
      string str = "salved";
      bool flag2;
      if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) RMCXenoStateVisuals.Downed, ref flag2, (AppearanceComponent) null) & flag2)
      {
        str += "_downed";
      }
      else
      {
        bool flag3;
        if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) RMCXenoStateVisuals.Resting, ref flag3, (AppearanceComponent) null) & flag3)
          str += "_rest";
      }
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, RSI.StateId.op_Implicit(str));
    }
  }
}
