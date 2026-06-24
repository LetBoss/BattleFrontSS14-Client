// Decompiled with JetBrains decompiler
// Type: Content.Client.Lock.Visualizers.LockVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Lock;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Lock.Visualizers;

public sealed class LockVisualizerSystem : VisualizerSystem<LockVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    LockVisualsComponent comp,
    ref AppearanceChangeEvent args)
  {
    bool flag1;
    if (args.Sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) LockVisuals.Locked, ref flag1, args.Component))
      return;
    bool flag2;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) LockVisuals.Locked, ref flag2, args.Component))
      flag2 = true;
    RSI.State state1;
    bool? state2 = args.Sprite.BaseRSI?.TryGetState(RSI.StateId.op_Implicit(comp.StateUnlocked), ref state1);
    bool flag3;
    if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) StorageVisuals.Open, ref flag3, args.Component))
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) LockVisualLayers.Lock, !flag3);
    else if (!state2.Value)
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) LockVisualLayers.Lock, flag2);
    if (flag3 || !state2.Value)
      return;
    this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) LockVisualLayers.Lock, RSI.StateId.op_Implicit(flag2 ? comp.StateLocked : comp.StateUnlocked));
  }
}
