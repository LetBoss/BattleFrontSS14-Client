// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.Visualizers.PortableScrubberSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Power;
using Content.Shared.Atmos.Visuals;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Atmos.Visualizers;

public sealed class PortableScrubberSystem : VisualizerSystem<PortableScrubberVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    PortableScrubberVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    bool flag1;
    bool flag2;
    if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) PortableScrubberVisuals.IsFull, ref flag1, args.Component) && ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) PortableScrubberVisuals.IsRunning, ref flag2, args.Component))
    {
      string str1 = flag2 ? component.RunningState : component.IdleState;
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PortableScrubberVisualLayers.IsRunning, RSI.StateId.op_Implicit(str1));
      string str2 = flag1 ? component.FullState : component.ReadyState;
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PowerDeviceVisualLayers.Powered, RSI.StateId.op_Implicit(str2));
    }
    bool flag3;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) PortableScrubberVisuals.IsDraining, ref flag3, args.Component))
      return;
    this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PortableScrubberVisualLayers.IsDraining, flag3);
  }
}
