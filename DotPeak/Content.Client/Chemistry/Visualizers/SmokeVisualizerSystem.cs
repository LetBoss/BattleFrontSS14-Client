// Decompiled with JetBrains decompiler
// Type: Content.Client.Chemistry.Visualizers.SmokeVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Smoking;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Chemistry.Visualizers;

public sealed class SmokeVisualizerSystem : VisualizerSystem<SmokeVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    SmokeVisualsComponent comp,
    ref AppearanceChangeEvent args)
  {
    Color color;
    if (args.Sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<Color>(uid, (Enum) SmokeVisuals.Color, ref color, (AppearanceComponent) null))
      return;
    this.SpriteSystem.SetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), color);
  }
}
