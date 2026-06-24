// Decompiled with JetBrains decompiler
// Type: Content.Client.SurveillanceCamera.SurveillanceCameraVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.SurveillanceCamera;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.SurveillanceCamera;

public sealed class SurveillanceCameraVisualsSystem : EntitySystem
{
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SurveillanceCameraVisualsComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<SurveillanceCameraVisualsComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnAppearanceChange(
    EntityUid uid,
    SurveillanceCameraVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    object obj;
    int num;
    string str;
    if (!args.AppearanceData.TryGetValue((Enum) SurveillanceCameraVisualsKey.Key, out obj) || !(obj is SurveillanceCameraVisuals key) || args.Sprite == null || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) SurveillanceCameraVisualsKey.Layer, ref num, false) || !component.CameraSprites.TryGetValue(key, out str))
      return;
    this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, RSI.StateId.op_Implicit(str));
  }
}
