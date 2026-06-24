// Decompiled with JetBrains decompiler
// Type: Content.Client.Shuttles.ThrusterSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Shuttles.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Shuttles;

public sealed class ThrusterSystem : VisualizerSystem<ThrusterComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    ThrusterComponent comp,
    ref AppearanceChangeEvent args)
  {
    bool flag1;
    if (args.Sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) ThrusterVisualState.State, ref flag1, args.Component))
      return;
    this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) ThrusterVisualLayers.ThrustOn, flag1);
    bool flag2;
    this.SetThrusting(uid, ((!flag1 ? 0 : (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) ThrusterVisualState.Thrusting, ref flag2, args.Component) ? 1 : 0)) & (flag2 ? 1 : 0)) != 0, args.Sprite);
  }

  private void SetThrusting(EntityUid uid, bool value, SpriteComponent sprite)
  {
    int num1;
    if (this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) ThrusterVisualLayers.Thrusting, ref num1, false))
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num1, value);
    int num2;
    if (!this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) ThrusterVisualLayers.ThrustingUnshaded, ref num2, false))
      return;
    this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, value);
  }
}
