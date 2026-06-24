// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Egg.XenoEggStorageVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Egg.EggRetriever;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Egg;

public sealed class XenoEggStorageVisualizerSystem : VisualizerSystem<XenoEggStorageVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    XenoEggStorageVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    SpriteComponent sprite = args.Sprite;
    int num1;
    int num2;
    if (sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<int>(uid, (Enum) XenoEggStorageVisuals.Number, ref num1, (AppearanceComponent) null) || !this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) XenoEggStorageVisualLayers.Base, ref num2, false))
      return;
    string str = "eggsac_" + Math.Clamp((int) Math.Ceiling((double) num1 / (double) component.MaxEggs * (double) component.FullStates), 0, component.FullStates).ToString();
    bool flag1;
    if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) RMCXenoStateVisuals.Downed, ref flag1, (AppearanceComponent) null) & flag1)
    {
      str += "_downed";
    }
    else
    {
      bool flag2;
      if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) RMCXenoStateVisuals.Resting, ref flag2, (AppearanceComponent) null) & flag2)
        str += "_rest";
    }
    bool flag3;
    if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) XenoEggStorageVisuals.Active, ref flag3, (AppearanceComponent) null) & flag3)
      str += "_active";
    this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, RSI.StateId.op_Implicit(str));
    bool flag4;
    if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) RMCXenoStateVisuals.Dead, ref flag4, (AppearanceComponent) null) & flag4)
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, false);
    else
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, true);
  }
}
