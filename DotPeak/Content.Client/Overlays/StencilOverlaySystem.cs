// Decompiled with JetBrains decompiler
// Type: Content.Client.Overlays.StencilOverlaySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Parallax;
using Content.Client.Weather;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.Overlays;

public sealed class StencilOverlaySystem : EntitySystem
{
  [Dependency]
  private IOverlayManager _overlay;
  [Dependency]
  private ParallaxSystem _parallax;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private SpriteSystem _sprite;
  [Dependency]
  private WeatherSystem _weather;

  public virtual void Initialize()
  {
    base.Initialize();
    this._overlay.AddOverlay((Overlay) new StencilOverlay(this._parallax, this._transform, this._map, this._sprite, this._weather));
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._overlay.RemoveOverlay<StencilOverlay>();
  }
}
