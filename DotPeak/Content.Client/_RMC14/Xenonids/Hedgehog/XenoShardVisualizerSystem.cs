// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Hedgehog.XenoShardVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Hedgehog;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Hedgehog;

public sealed class XenoShardVisualizerSystem : VisualizerSystem<XenoShardComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    XenoShardComponent component,
    ref AppearanceChangeEvent args)
  {
    SpriteComponent sprite = args.Sprite;
    XenoShardLevel xenoShardLevel;
    int num;
    if (sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<XenoShardLevel>(uid, (Enum) XenoShardVisuals.Level, ref xenoShardLevel, (AppearanceComponent) null) || !this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) XenoShardVisualLayers.Base, ref num, true))
      return;
    string str = $"hedgehog_{(int) xenoShardLevel}";
    bool flag1;
    if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) RMCXenoStateVisuals.Dead, ref flag1, (AppearanceComponent) null) & flag1)
      return;
    bool flag2;
    if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) RMCXenoStateVisuals.Downed, ref flag2, (AppearanceComponent) null) & flag2)
    {
      str += "_crit";
    }
    else
    {
      bool flag3;
      if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) RMCXenoStateVisuals.Resting, ref flag3, (AppearanceComponent) null) & flag3)
        str += "_resting";
    }
    this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, RSI.StateId.op_Implicit(str));
  }
}
