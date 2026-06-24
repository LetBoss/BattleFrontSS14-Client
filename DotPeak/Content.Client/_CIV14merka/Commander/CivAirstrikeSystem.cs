// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.CivAirstrikeSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.Commander.UI;
using Content.Shared._CIV14merka.Commander;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Commander;

public sealed class CivAirstrikeSystem : EntitySystem
{
  [Dependency]
  private ISharedPlayerManager _player;
  [Dependency]
  private SharedTransformSystem _transform;
  private CivAirstrikeWindow? _window;

  public event Action<CivAirstrikeResponseEvent>? OnResponse;

  public event Action<float>? OnEtaResponse;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<CivAirstrikeResponseEvent>(new EntityEventHandler<CivAirstrikeResponseEvent>(this.OnResponseReceived), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<CivAirstrikeEtaResponseEvent>(new EntityEventHandler<CivAirstrikeEtaResponseEvent>(this.OnEtaReceived), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<CivArtilleryResponseEvent>(new EntityEventHandler<CivArtilleryResponseEvent>(this.OnArtilleryResponseReceived), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<CivBallisticMissileResponseEvent>(new EntityEventHandler<CivBallisticMissileResponseEvent>(this.OnBallisticMissileResponseReceived), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<CivSmokeSupportResponseEvent>(new EntityEventHandler<CivSmokeSupportResponseEvent>(this.OnSmokeSupportResponseReceived), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<CivReconDroneResponseEvent>(new EntityEventHandler<CivReconDroneResponseEvent>(this.OnReconDroneResponseReceived), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<CivFireSupportRequestsResponseEvent>(new EntityEventHandler<CivFireSupportRequestsResponseEvent>(this.OnRequestsReceived), (Type[]) null, (Type[]) null);
  }

  private void OnRequestsReceived(CivFireSupportRequestsResponseEvent ev)
  {
    this._window?.SetRequests(ev.Requests);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    ((Control) this._window)?.Dispose();
    this._window = (CivAirstrikeWindow) null;
  }

  private void OnResponseReceived(CivAirstrikeResponseEvent ev)
  {
    Action<CivAirstrikeResponseEvent> onResponse = this.OnResponse;
    if (onResponse != null)
      onResponse(ev);
    if (ev.Success || this._window == null || string.IsNullOrEmpty(ev.Error))
      return;
    this._window.ShowError(ev.Error);
  }

  private void OnArtilleryResponseReceived(CivArtilleryResponseEvent ev)
  {
    if (ev.Success || this._window == null || string.IsNullOrEmpty(ev.Error))
      return;
    this._window.ShowError(ev.Error);
  }

  private void OnBallisticMissileResponseReceived(CivBallisticMissileResponseEvent ev)
  {
    if (ev.Success || this._window == null || string.IsNullOrEmpty(ev.Error))
      return;
    this._window.ShowError(ev.Error);
  }

  public void UpdateCooldown(float seconds) => this._window?.SetCooldown(seconds);

  public void UpdateArtilleryCooldown(float seconds) => this._window?.SetArtilleryCooldown(seconds);

  public void UpdateSmokeCooldown(float seconds) => this._window?.SetSmokeCooldown(seconds);

  public void RequestAirstrike(Vector2 target, CivAirstrikeVector vector)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivAirstrikeRequestEvent(target, vector));
  }

  public void RequestArtillery(Vector2 target)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivArtilleryRequestEvent(target));
  }

  public void RequestBallisticMissile(Vector2 target)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivBallisticMissileRequestEvent(target));
  }

  public void RequestSmokeSupport(Vector2 target, CivAirstrikeVector vector)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivSmokeSupportRequestEvent(target, vector));
  }

  private void OnSmokeSupportResponseReceived(CivSmokeSupportResponseEvent ev)
  {
    if (ev.Success || this._window == null || string.IsNullOrEmpty(ev.Error))
      return;
    this._window.ShowError(ev.Error);
  }

  public void RequestReconDrone(Vector2 target)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivReconDroneRequestEvent(target));
  }

  private void OnReconDroneResponseReceived(CivReconDroneResponseEvent ev)
  {
    if (ev.Success || this._window == null || string.IsNullOrEmpty(ev.Error))
      return;
    this._window.ShowError(ev.Error);
  }

  public void AcceptRequest(int requestId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivFireSupportAcceptRequestEvent(requestId));
  }

  public void RejectRequest(int requestId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivFireSupportRejectRequestEvent(requestId));
  }

  public void RequestTeleport(Vector2 target)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivFireSupportTeleportEvent(target));
  }

  public Vector2? GetMyPosition()
  {
    EntityUid? localEntity = this._player.LocalEntity;
    if (localEntity.HasValue)
    {
      EntityUid valueOrDefault = localEntity.GetValueOrDefault();
      if (this.Exists(valueOrDefault))
        return new Vector2?(this._transform.GetWorldPosition(valueOrDefault));
    }
    return new Vector2?();
  }

  public void RequestEta(Vector2 target, CivAirstrikeVector vector)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivAirstrikeEtaRequestEvent(target, vector));
  }

  private void OnEtaReceived(CivAirstrikeEtaResponseEvent ev)
  {
    Action<float> onEtaResponse = this.OnEtaResponse;
    if (onEtaResponse == null)
      return;
    onEtaResponse(ev.Seconds);
  }

  public void OpenWindow(
    float airstrikeCooldown,
    float artilleryCooldown,
    float smokeCooldown = 0.0f,
    Vector2? initialCoords = null)
  {
    if (this._window == null)
    {
      this._window = new CivAirstrikeWindow(this);
      ((BaseWindow) this._window).OnClose += (Action) (() =>
      {
        this._window = (CivAirstrikeWindow) null;
        this.RaiseNetworkEvent((EntityEventArgs) new CivFireSupportRequestsCloseEvent());
      });
    }
    this._window.SetCooldown(airstrikeCooldown);
    this._window.SetArtilleryCooldown(artilleryCooldown);
    this._window.SetSmokeCooldown(smokeCooldown);
    if (initialCoords.HasValue)
      this._window.SetCoordinates(initialCoords.Value);
    if (!((BaseWindow) this._window).IsOpen)
      ((BaseWindow) this._window).OpenCentered();
    this.RaiseNetworkEvent((EntityEventArgs) new CivFireSupportRequestsRequestEvent());
  }

  public void CloseWindow() => ((BaseWindow) this._window)?.Close();
}
