// Decompiled with JetBrains decompiler
// Type: Content.Client.Tools.Visualizers.WeldableVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Tools.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Tools.Visualizers;

public sealed class WeldableVisualizerSystem : VisualizerSystem<WeldableComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    WeldableComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    bool flag;
    ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) WeldableVisuals.IsWelded, ref flag, args.Component);
    int num;
    if (!this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) WeldableLayers.BaseWelded, ref num, false))
      return;
    this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, flag);
  }
}
