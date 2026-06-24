// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Medical.HUD.Holocard.HolocardContainerVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Medical.HUD;
using Content.Shared._RMC14.Medical.HUD.Components;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Medical.HUD.Holocard;

public sealed class HolocardContainerVisualizerSystem : VisualizerSystem<HolocardContainerComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    HolocardContainerComponent component,
    ref AppearanceChangeEvent args)
  {
    HolocardStatus holocardStatus;
    bool flag;
    if (args.Sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<HolocardStatus>(uid, (Enum) HolocardContainerVisuals.State, ref holocardStatus, args.Component) || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) StorageVisuals.Open, ref flag, args.Component))
      return;
    (EntityUid, SpriteComponent) valueTuple = (uid, args.Sprite);
    int num;
    if (!this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit(valueTuple), (Enum) HolocardContainerVisualLayers.Base, ref num, false))
      return;
    if (flag && component.HideOnOpen)
    {
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit(valueTuple), num, false);
    }
    else
    {
      string prefix = component.Prefix;
      string str;
      switch (holocardStatus)
      {
        case HolocardStatus.Urgent:
          str = prefix + "_holoorange";
          break;
        case HolocardStatus.Emergency:
          str = prefix + "_holored";
          break;
        case HolocardStatus.Xeno:
          str = prefix + "_holopurple";
          break;
        case HolocardStatus.Permadead:
          str = prefix + "_holoblack";
          break;
        default:
          this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit(valueTuple), num, false);
          return;
      }
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit(valueTuple), num, RSI.StateId.op_Implicit(str));
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit(valueTuple), num, true);
    }
  }
}
