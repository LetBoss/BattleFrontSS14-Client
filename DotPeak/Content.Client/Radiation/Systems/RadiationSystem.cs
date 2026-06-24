// Decompiled with JetBrains decompiler
// Type: Content.Client.Radiation.Systems.RadiationSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Radiation.Overlays;
using Content.Shared.Radiation.Events;
using Content.Shared.Radiation.Systems;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Radiation.Systems;

public sealed class RadiationSystem : EntitySystem
{
  [Dependency]
  private IOverlayManager _overlayMan;
  public List<DebugRadiationRay>? Rays;
  public Dictionary<NetEntity, Dictionary<Vector2i, float>>? ResistanceGrids;

  public virtual void Initialize()
  {
    this.SubscribeNetworkEvent<OnRadiationOverlayToggledEvent>(new EntityEventHandler<OnRadiationOverlayToggledEvent>(this.OnOverlayToggled), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<OnRadiationOverlayUpdateEvent>(new EntityEventHandler<OnRadiationOverlayUpdateEvent>(this.OnOverlayUpdate), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<OnRadiationOverlayResistanceUpdateEvent>(new EntityEventHandler<OnRadiationOverlayResistanceUpdateEvent>(this.OnResistanceUpdate), (Type[]) null, (Type[]) null);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._overlayMan.RemoveOverlay<RadiationDebugOverlay>();
  }

  private void OnOverlayToggled(OnRadiationOverlayToggledEvent ev)
  {
    if (ev.IsEnabled)
      this._overlayMan.AddOverlay((Overlay) new RadiationDebugOverlay());
    else
      this._overlayMan.RemoveOverlay<RadiationDebugOverlay>();
  }

  private void OnOverlayUpdate(OnRadiationOverlayUpdateEvent ev)
  {
    RadiationDebugOverlay radiationDebugOverlay;
    if (!this._overlayMan.TryGetOverlay<RadiationDebugOverlay>(ref radiationDebugOverlay))
      return;
    this.Log.Info($"Radiation update: {ev.ElapsedTimeMs}ms with. Receivers: {ev.ReceiversCount}, Sources: {ev.SourcesCount}, Rays: {ev.Rays.Count}");
    this.Rays = ev.Rays;
  }

  private void OnResistanceUpdate(OnRadiationOverlayResistanceUpdateEvent ev)
  {
    this.ResistanceGrids = ev.Grids;
  }
}
