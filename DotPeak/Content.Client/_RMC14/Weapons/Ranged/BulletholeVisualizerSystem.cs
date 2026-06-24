// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Weapons.Ranged.BulletholeVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Weapons.Ranged;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client._RMC14.Weapons.Ranged;

public sealed class BulletholeVisualizerSystem : VisualizerSystem<BulletholeComponent>
{
  private const string BulletholeRsiPath = "/Textures/_RMC14/Effects/bulletholes.rsi";

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    BulletholeComponent component,
    ref AppearanceChangeEvent args)
  {
    SpriteComponent sprite = args.Sprite;
    string str;
    if (sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<string>(uid, (Enum) BulletholeVisuals.State, ref str, args.Component))
      return;
    int num;
    if (!this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) BulletholeVisualsLayers.Bullethole, ref num, false))
      num = this.SpriteSystem.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) BulletholeVisualsLayers.Bullethole);
    bool flag = !string.IsNullOrWhiteSpace(str);
    this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) BulletholeVisualsLayers.Bullethole, flag);
    if (!flag)
      return;
    this.SpriteSystem.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) BulletholeVisualsLayers.Bullethole, new ResPath("/Textures/_RMC14/Effects/bulletholes.rsi"), new RSI.StateId?());
    this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) BulletholeVisualsLayers.Bullethole, RSI.StateId.op_Implicit(str));
  }
}
