// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.RedZoneOverlaySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._PUBG;

public sealed class RedZoneOverlaySystem : EntitySystem
{
  [Dependency]
  private IOverlayManager _overlayManager;
  private RedZoneOverlay? _overlay;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<RedZoneStateEvent>(new EntitySessionEventHandler<RedZoneStateEvent>(this.OnRedZoneStateUpdate), (Type[]) null, (Type[]) null);
    this._overlay = new RedZoneOverlay();
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    if (this._overlay == null)
      return;
    this._overlayManager.RemoveOverlay((Overlay) this._overlay);
    this._overlay = (RedZoneOverlay) null;
  }

  private void OnRedZoneStateUpdate(RedZoneStateEvent ev, EntitySessionEventArgs args)
  {
    if (this._overlay == null)
      return;
    this._overlay.BombActive = ev.HasActiveBomb;
    this._overlay.BombCenter = ev.BombCenter;
    this._overlay.BombRadius = ev.BombRadius;
    if (ev.ZoneActive && ev.HasActiveBomb && (double) ev.BombRadius > 0.0 && !this._overlayManager.HasOverlay<RedZoneOverlay>())
    {
      this._overlayManager.AddOverlay((Overlay) this._overlay);
    }
    else
    {
      if (ev.ZoneActive && ev.HasActiveBomb && (double) ev.BombRadius > 0.0 || !this._overlayManager.HasOverlay<RedZoneOverlay>())
        return;
      this._overlayManager.RemoveOverlay((Overlay) this._overlay);
    }
  }
}
