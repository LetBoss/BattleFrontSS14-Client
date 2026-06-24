// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Minimap.PubgPartyPingOverlaySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Vehicle;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Minimap;

public sealed class PubgPartyPingOverlaySystem : EntitySystem
{
  [Dependency]
  private IOverlayManager _overlayManager;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private VehicleSystem _vehicles;
  private PubgPartyPingOverlay? _overlay;

  public virtual void Initialize()
  {
    base.Initialize();
    this._overlay = new PubgPartyPingOverlay(this._player, this._transform, this.EntityManager.System<PubgPartyPingClientSystem>(), this._vehicles);
    this._overlayManager.AddOverlay((Overlay) this._overlay);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    if (this._overlay == null)
      return;
    this._overlayManager.RemoveOverlay((Overlay) this._overlay);
    this._overlay = (PubgPartyPingOverlay) null;
  }
}
