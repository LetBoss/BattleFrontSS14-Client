// Decompiled with JetBrains decompiler
// Type: Content.Client.Power.Visualizers.CableVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.SubFloor;
using Content.Shared.Wires;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Power.Visualizers;

public sealed class CableVisualizerSystem : EntitySystem
{
  [Dependency]
  private AppearanceSystem _appearanceSystem;
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CableVisualizerComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<CableVisualizerComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, new Type[1]
    {
      typeof (SubFloorHideSystem)
    });
  }

  private void OnAppearanceChange(
    EntityUid uid,
    CableVisualizerComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null || !args.Sprite.Visible)
      return;
    WireVisDirFlags wireVisDirFlags;
    if (!((SharedAppearanceSystem) this._appearanceSystem).TryGetData<WireVisDirFlags>(uid, (Enum) WireVisVisuals.ConnectedMask, ref wireVisDirFlags, args.Component))
      wireVisDirFlags = WireVisDirFlags.None;
    this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, RSI.StateId.op_Implicit($"{component.StatePrefix}{(int) wireVisDirFlags}"));
    if (component.ExtraLayerPrefix == null)
      return;
    this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 1, RSI.StateId.op_Implicit($"{component.ExtraLayerPrefix}{(int) wireVisDirFlags}"));
  }
}
