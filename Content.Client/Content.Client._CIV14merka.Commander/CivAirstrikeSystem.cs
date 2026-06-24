using System;
using System.Numerics;
using Content.Client._CIV14merka.Commander.UI;
using Content.Shared._CIV14merka.Commander;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

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

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<CivAirstrikeResponseEvent>((EntityEventHandler<CivAirstrikeResponseEvent>)OnResponseReceived, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<CivAirstrikeEtaResponseEvent>((EntityEventHandler<CivAirstrikeEtaResponseEvent>)OnEtaReceived, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<CivArtilleryResponseEvent>((EntityEventHandler<CivArtilleryResponseEvent>)OnArtilleryResponseReceived, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<CivBallisticMissileResponseEvent>((EntityEventHandler<CivBallisticMissileResponseEvent>)OnBallisticMissileResponseReceived, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<CivSmokeSupportResponseEvent>((EntityEventHandler<CivSmokeSupportResponseEvent>)OnSmokeSupportResponseReceived, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<CivReconDroneResponseEvent>((EntityEventHandler<CivReconDroneResponseEvent>)OnReconDroneResponseReceived, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<CivFireSupportRequestsResponseEvent>((EntityEventHandler<CivFireSupportRequestsResponseEvent>)OnRequestsReceived, (Type[])null, (Type[])null);
	}

	private void OnRequestsReceived(CivFireSupportRequestsResponseEvent ev)
	{
		_window?.SetRequests(ev.Requests);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CivAirstrikeWindow? window = _window;
		if (window != null)
		{
			((Control)window).Dispose();
		}
		_window = null;
	}

	private void OnResponseReceived(CivAirstrikeResponseEvent ev)
	{
		this.OnResponse?.Invoke(ev);
		if (!ev.Success && _window != null && !string.IsNullOrEmpty(ev.Error))
		{
			_window.ShowError(ev.Error);
		}
	}

	private void OnArtilleryResponseReceived(CivArtilleryResponseEvent ev)
	{
		if (!ev.Success && _window != null && !string.IsNullOrEmpty(ev.Error))
		{
			_window.ShowError(ev.Error);
		}
	}

	private void OnBallisticMissileResponseReceived(CivBallisticMissileResponseEvent ev)
	{
		if (!ev.Success && _window != null && !string.IsNullOrEmpty(ev.Error))
		{
			_window.ShowError(ev.Error);
		}
	}

	public void UpdateCooldown(float seconds)
	{
		_window?.SetCooldown(seconds);
	}

	public void UpdateArtilleryCooldown(float seconds)
	{
		_window?.SetArtilleryCooldown(seconds);
	}

	public void UpdateSmokeCooldown(float seconds)
	{
		_window?.SetSmokeCooldown(seconds);
	}

	public void RequestAirstrike(Vector2 target, CivAirstrikeVector vector)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivAirstrikeRequestEvent(target, vector));
	}

	public void RequestArtillery(Vector2 target)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivArtilleryRequestEvent(target));
	}

	public void RequestBallisticMissile(Vector2 target)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivBallisticMissileRequestEvent(target));
	}

	public void RequestSmokeSupport(Vector2 target, CivAirstrikeVector vector)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivSmokeSupportRequestEvent(target, vector));
	}

	private void OnSmokeSupportResponseReceived(CivSmokeSupportResponseEvent ev)
	{
		if (!ev.Success && _window != null && !string.IsNullOrEmpty(ev.Error))
		{
			_window.ShowError(ev.Error);
		}
	}

	public void RequestReconDrone(Vector2 target)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivReconDroneRequestEvent(target));
	}

	private void OnReconDroneResponseReceived(CivReconDroneResponseEvent ev)
	{
		if (!ev.Success && _window != null && !string.IsNullOrEmpty(ev.Error))
		{
			_window.ShowError(ev.Error);
		}
	}

	public void AcceptRequest(int requestId)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivFireSupportAcceptRequestEvent(requestId));
	}

	public void RejectRequest(int requestId)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivFireSupportRejectRequestEvent(requestId));
	}

	public void RequestTeleport(Vector2 target)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivFireSupportTeleportEvent(target));
	}

	public Vector2? GetMyPosition()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = _player.LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			if (((EntitySystem)this).Exists(valueOrDefault))
			{
				return _transform.GetWorldPosition(valueOrDefault);
			}
		}
		return null;
	}

	public void RequestEta(Vector2 target, CivAirstrikeVector vector)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivAirstrikeEtaRequestEvent(target, vector));
	}

	private void OnEtaReceived(CivAirstrikeEtaResponseEvent ev)
	{
		this.OnEtaResponse?.Invoke(ev.Seconds);
	}

	public void OpenWindow(float airstrikeCooldown, float artilleryCooldown, float smokeCooldown = 0f, Vector2? initialCoords = null)
	{
		if (_window == null)
		{
			_window = new CivAirstrikeWindow(this);
			((BaseWindow)_window).OnClose += delegate
			{
				_window = null;
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivFireSupportRequestsCloseEvent());
			};
		}
		_window.SetCooldown(airstrikeCooldown);
		_window.SetArtilleryCooldown(artilleryCooldown);
		_window.SetSmokeCooldown(smokeCooldown);
		if (initialCoords.HasValue)
		{
			_window.SetCoordinates(initialCoords.Value);
		}
		if (!((BaseWindow)_window).IsOpen)
		{
			((BaseWindow)_window).OpenCentered();
		}
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivFireSupportRequestsRequestEvent());
	}

	public void CloseWindow()
	{
		CivAirstrikeWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
	}
}
