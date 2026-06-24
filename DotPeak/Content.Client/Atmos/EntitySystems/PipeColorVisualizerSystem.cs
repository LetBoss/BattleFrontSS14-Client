// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.EntitySystems.PipeColorVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Atmos.Components;
using Content.Shared.Atmos.Piping;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Atmos.EntitySystems;

public sealed class PipeColorVisualizerSystem : VisualizerSystem<PipeColorVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    PipeColorVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    SpriteComponent spriteComponent;
    Color color;
    if (!((EntitySystem) this).TryComp<SpriteComponent>(uid, ref spriteComponent) || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<Color>(uid, (Enum) PipeColorVisuals.Color, ref color, args.Component))
      return;
    ISpriteLayer ispriteLayer = spriteComponent[(object) PipeVisualLayers.Pipe];
    ispriteLayer.Color = ((Color) ref color).WithAlpha(ispriteLayer.Color.A);
  }
}
