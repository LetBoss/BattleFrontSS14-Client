// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Infected.VictimBurstVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.Parasite;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Infected;

public sealed class VictimBurstVisualizerSystem : VisualizerSystem<VictimBurstComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    VictimBurstComponent component,
    ref AppearanceChangeEvent args)
  {
    base.OnAppearanceChange(uid, component, ref args);
    VictimBurstState victimBurstState;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<VictimBurstState>(uid, (Enum) BurstVisuals.Visuals, ref victimBurstState, args.Component) || args.Sprite == null)
      return;
    ResPath rsiPath = component.RsiPath;
    string str1;
    switch (victimBurstState)
    {
      case VictimBurstState.Bursting:
        str1 = component.BurstingState;
        break;
      case VictimBurstState.Burst:
        str1 = component.BurstState;
        break;
      default:
        str1 = (string) null;
        break;
    }
    string str2 = str1;
    if (string.IsNullOrWhiteSpace(str2))
      return;
    int num;
    if (!this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) BurstLayer.Base, ref num, false))
    {
      num = this.SpriteSystem.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) BurstLayer.Base);
      this.SpriteSystem.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, rsiPath, new RSI.StateId?());
    }
    this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, RSI.StateId.op_Implicit(str2));
  }
}
