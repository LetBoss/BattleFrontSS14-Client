// Decompiled with JetBrains decompiler
// Type: Content.Client.ParticleAccelerator.ParticleAcceleratorPartVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Singularity.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.ParticleAccelerator;

public sealed class ParticleAcceleratorPartVisualizerSystem : 
  VisualizerSystem<ParticleAcceleratorPartVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    ParticleAcceleratorPartVisualsComponent comp,
    ref AppearanceChangeEvent args)
  {
    int num;
    if (args.Sprite == null || !this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) ParticleAcceleratorVisualLayers.Unlit, ref num, false))
      return;
    ParticleAcceleratorVisualState key;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<ParticleAcceleratorVisualState>(uid, (Enum) ParticleAcceleratorVisuals.VisualState, ref key, args.Component))
      key = ParticleAcceleratorVisualState.Unpowered;
    if (key != ParticleAcceleratorVisualState.Unpowered)
    {
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, true);
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, RSI.StateId.op_Implicit(comp.StateBase + comp.StatesSuffixes[key]));
    }
    else
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, false);
  }
}
