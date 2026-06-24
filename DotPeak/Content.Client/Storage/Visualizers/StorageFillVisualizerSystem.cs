// Decompiled with JetBrains decompiler
// Type: Content.Client.Storage.Visualizers.StorageFillVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Storage.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Storage.Visualizers;

public sealed class StorageFillVisualizerSystem : VisualizerSystem<StorageFillVisualizerComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    StorageFillVisualizerComponent component,
    ref AppearanceChangeEvent args)
  {
    int num;
    if (args.Sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<int>(uid, (Enum) StorageFillVisuals.FillLevel, ref num, args.Component))
      return;
    string str = $"{component.FillBaseName}-{num}";
    this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) StorageFillLayers.Fill, RSI.StateId.op_Implicit(str));
  }
}
