// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.PubgZoneSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG;

public sealed class PubgZoneSystem : EntitySystem
{
  [Dependency]
  private IOverlayManager _overlayManager;
  [Dependency]
  private IResourceCache _resourceCache;
  private PubgZoneOverlay? _overlay;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgZoneStateEvent>(new EntitySessionEventHandler<PubgZoneStateEvent>(this.OnZoneStateUpdate), (Type[]) null, (Type[]) null);
    this._overlay = new PubgZoneOverlay(this._resourceCache);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    if (this._overlay == null)
      return;
    this._overlayManager.RemoveOverlay((Overlay) this._overlay);
    this._overlay = (PubgZoneOverlay) null;
  }

  private void OnZoneStateUpdate(PubgZoneStateEvent ev, EntitySessionEventArgs args)
  {
    if (this._overlay == null)
      return;
    EntityUid entity = this.GetEntity(ev.ZoneMapEntity);
    MapId mapId = MapId.Nullspace;
    MapComponent mapComponent;
    if (this.TryComp<MapComponent>(entity, ref mapComponent))
      mapId = mapComponent.MapId;
    this._overlay.CurrentCenter = new Vector2?(ev.CurrentCenter);
    this._overlay.CurrentRadius = ev.CurrentRadius;
    this._overlay.NextCenter = new Vector2?(ev.NextCenter);
    this._overlay.NextRadius = ev.NextRadius;
    this._overlay.State = ev.State;
    this._overlay.Active = ev.Active;
    this._overlay.Visible = ev.Visible;
    this._overlay.ZoneMapId = mapId;
    if (ev.Active && !this._overlayManager.HasOverlay<PubgZoneOverlay>())
    {
      this._overlayManager.AddOverlay((Overlay) this._overlay);
    }
    else
    {
      if (ev.Active || !this._overlayManager.HasOverlay<PubgZoneOverlay>())
        return;
      this._overlayManager.RemoveOverlay((Overlay) this._overlay);
    }
  }
}
