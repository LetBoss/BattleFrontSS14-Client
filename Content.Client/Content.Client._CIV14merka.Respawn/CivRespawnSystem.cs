using System;
using System.Collections.Generic;
using Content.Client._CIV14merka.Respawn.UI;
using Content.Shared._CIV14merka;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;

namespace Content.Client._CIV14merka.Respawn;

public sealed class CivRespawnSystem : EntitySystem
{
	private readonly Queue<CivRespawnAvailableEvent> _pendingRespawns = new Queue<CivRespawnAvailableEvent>();

	private readonly HashSet<int> _pendingRespawnIds = new HashSet<int>();

	private CivRespawnWindow? _window;

	private CivRespawnAvailableEvent? _activeRespawn;

	private bool _closingWindow;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<CivRespawnAvailableEvent>((EntitySessionEventHandler<CivRespawnAvailableEvent>)OnRespawnAvailable, (Type[])null, (Type[])null);
	}

	private void OnRespawnAvailable(CivRespawnAvailableEvent msg, EntitySessionEventArgs args)
	{
		EnsureWindow();
		if (_pendingRespawnIds.Add(msg.RespawnId))
		{
			_pendingRespawns.Enqueue(msg);
			ShowNextRespawn();
		}
	}

	private void EnsureWindow()
	{
		if (_window == null)
		{
			_window = new CivRespawnWindow();
			_window.AcceptPressed += OnAcceptPressed;
			_window.DeclinePressed += OnDeclinePressed;
			((BaseWindow)_window).OnClose += OnWindowClosed;
		}
	}

	private void OnAcceptPressed()
	{
		Respond(accept: true);
	}

	private void OnDeclinePressed()
	{
		Respond(accept: false);
	}

	private void OnWindowClosed()
	{
		if (_closingWindow)
		{
			_closingWindow = false;
			DisposeWindow();
		}
		else
		{
			RespondToAllPending(accept: false);
			DisposeWindow();
		}
	}

	private void DisposeWindow()
	{
		if (_window != null)
		{
			_window.AcceptPressed -= OnAcceptPressed;
			_window.DeclinePressed -= OnDeclinePressed;
			((BaseWindow)_window).OnClose -= OnWindowClosed;
			_window = null;
		}
	}

	private void Respond(bool accept)
	{
		if (_activeRespawn != null)
		{
			int respawnId = _activeRespawn.RespawnId;
			_activeRespawn = null;
			_pendingRespawnIds.Remove(respawnId);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivRespawnChoiceEvent(respawnId, accept));
			ShowNextRespawn();
		}
	}

	private void RespondToAllPending(bool accept)
	{
		if (_activeRespawn != null)
		{
			int respawnId = _activeRespawn.RespawnId;
			_activeRespawn = null;
			_pendingRespawnIds.Remove(respawnId);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivRespawnChoiceEvent(respawnId, accept));
		}
		CivRespawnAvailableEvent result;
		while (_pendingRespawns.TryDequeue(out result))
		{
			_pendingRespawnIds.Remove(result.RespawnId);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivRespawnChoiceEvent(result.RespawnId, accept));
		}
	}

	private void ShowNextRespawn()
	{
		EnsureWindow();
		if (_activeRespawn == null && _pendingRespawns.TryDequeue(out CivRespawnAvailableEvent result))
		{
			_activeRespawn = result;
		}
		if (_activeRespawn == null)
		{
			CivRespawnWindow window = _window;
			if (window != null && ((BaseWindow)window).IsOpen)
			{
				_closingWindow = true;
				((BaseWindow)_window).Close();
			}
		}
		else if (!((BaseWindow)_window).IsOpen)
		{
			((BaseWindow)_window).OpenCentered();
		}
	}
}
