// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Airdrop.PubgAirdropOverlaySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Airdrop;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._PUBG.Airdrop;

public sealed class PubgAirdropOverlaySystem : EntitySystem
{
  [Dependency]
  private IOverlayManager _overlayManager;
  [Dependency]
  private IResourceCache _cache;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private SharedTransformSystem _transform;
  private PubgAirdropOverlay? _overlay;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgAirdropStateEvent>(new EntitySessionEventHandler<PubgAirdropStateEvent>(this.OnAirdropState), (Type[]) null, (Type[]) null);
    this._overlay = new PubgAirdropOverlay(this._cache, this._player, this._transform);
  }

  private void OnAirdropState(PubgAirdropStateEvent ev, EntitySessionEventArgs args)
  {
    if (this._overlay == null)
      return;
    this._overlay.Active = ev.Active;
    this._overlay.Target = ev.Position;
    this._overlay.RemainingSeconds = ev.RemainingSeconds;
    this._overlay.MapId = ev.MapId;
    if (ev.Active && !this._overlayManager.HasOverlay<PubgAirdropOverlay>())
    {
      this._overlayManager.AddOverlay((Overlay) this._overlay);
    }
    else
    {
      if (ev.Active || !this._overlayManager.HasOverlay<PubgAirdropOverlay>())
        return;
      this._overlayManager.RemoveOverlay<PubgAirdropOverlay>();
    }
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    if (!this._overlayManager.HasOverlay<PubgAirdropOverlay>())
      return;
    this._overlayManager.RemoveOverlay<PubgAirdropOverlay>();
  }
}
