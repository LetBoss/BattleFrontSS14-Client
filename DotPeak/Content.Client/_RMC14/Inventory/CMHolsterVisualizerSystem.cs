// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Inventory.CMHolsterVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Inventory;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Inventory;

public sealed class CMHolsterVisualizerSystem : VisualizerSystem<CMHolsterComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    CMHolsterComponent component,
    ref AppearanceChangeEvent args)
  {
    SpriteComponent sprite = args.Sprite;
    int num;
    if (sprite == null || !this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) CMHolsterLayers.Fill, ref num, false))
      return;
    if (component.Contents.Count != 0)
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, true);
    else
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, false);
  }
}
