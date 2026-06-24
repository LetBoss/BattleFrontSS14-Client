// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Parasite.XenoParasitesVisualSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Projectile.Parasite;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Parasite;

public sealed class XenoParasitesVisualSystem : VisualizerSystem<XenoParasiteThrowerComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    XenoParasiteThrowerComponent component,
    ref AppearanceChangeEvent args)
  {
    SpriteComponent sprite = args.Sprite;
    bool[] flagArray;
    if (sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool[]>(uid, (Enum) ParasiteOverlayVisuals.States, ref flagArray, (AppearanceComponent) null))
      return;
    string str = "para_";
    bool flag1;
    if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) RMCXenoStateVisuals.Downed, ref flag1, (AppearanceComponent) null) & flag1)
    {
      str = "para_downed_";
    }
    else
    {
      bool flag2;
      if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) RMCXenoStateVisuals.Resting, ref flag2, (AppearanceComponent) null) & flag2)
        str = "para_rest_";
    }
    foreach (ParasiteOverlayLayers index in Enum.GetValues<ParasiteOverlayLayers>())
    {
      int num;
      if (this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) index, ref num, false))
      {
        this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) index, flagArray[(int) index]);
        this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) index, RSI.StateId.op_Implicit($"{str}{(int) index}"));
      }
    }
  }
}
