using System;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Shared._PUBG.Gulag;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;

namespace Content.Client._PUBG.UserInterface.Systems.Gulag;

public sealed class GulagUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
	private GulagQueueHud? _queueHud;

	private GulagFightHud? _fightHud;

	private GulagSpectatorHud? _spectatorHud;

	private GulagAdminOfferWindow? _adminOfferWindow;

	private bool _inGulag;

	private bool _isFighting;

	private bool _isSpectating;

	private bool _systemSubscribed;

	private bool _queueHiddenByUser;

	public override void Initialize()
	{
		((UIController)this).Initialize();
		GameplayStateLoadController uIController = base.UIManager.GetUIController<GameplayStateLoadController>();
		uIController.OnScreenLoad = (Action)Delegate.Combine(uIController.OnScreenLoad, (Action)delegate
		{
			if (_inGulag && !_isFighting && !_queueHiddenByUser)
			{
				EnsureQueueHud();
			}
			else if (_isFighting)
			{
				EnsureFightHud();
			}
		});
	}

	public void OnStateEntered(GameplayState state)
	{
		EnsureSystemSubscribed();
		if (_inGulag && !_isFighting)
		{
			EnsureQueueHud();
		}
		else if (_isFighting)
		{
			EnsureFightHud();
		}
	}

	public void OnStateExited(GameplayState state)
	{
		UnsubscribeFromSystem();
		HideQueueHud();
		HideFightHud();
		HideSpectatorHud();
		HideAdminOfferWindow();
	}

	private void EnsureSystemSubscribed()
	{
		if (!_systemSubscribed)
		{
			GulagSystem gulagSystem = base.EntityManager.System<GulagSystem>();
			gulagSystem.OnGulagStatusReceived += OnGulagStatus;
			gulagSystem.OnQueueUpdateReceived += OnQueueUpdate;
			gulagSystem.OnFightStartReceived += OnFightStart;
			gulagSystem.OnFightUpdateReceived += OnFightUpdate;
			gulagSystem.OnSpectatorUpdateReceived += OnSpectatorUpdate;
			gulagSystem.OnAdminOfferReceived += OnAdminOffer;
			gulagSystem.OnMapInfoReceived += OnMapInfo;
			gulagSystem.OnLocalGulagMapChanged += OnLocalGulagMapChanged;
			_systemSubscribed = true;
		}
	}

	private void UnsubscribeFromSystem()
	{
		if (_systemSubscribed)
		{
			GulagSystem gulagSystem = base.EntityManager.SystemOrNull<GulagSystem>();
			if (gulagSystem != null)
			{
				gulagSystem.OnGulagStatusReceived -= OnGulagStatus;
				gulagSystem.OnQueueUpdateReceived -= OnQueueUpdate;
				gulagSystem.OnFightStartReceived -= OnFightStart;
				gulagSystem.OnFightUpdateReceived -= OnFightUpdate;
				gulagSystem.OnSpectatorUpdateReceived -= OnSpectatorUpdate;
				gulagSystem.OnAdminOfferReceived -= OnAdminOffer;
				gulagSystem.OnMapInfoReceived -= OnMapInfo;
				gulagSystem.OnLocalGulagMapChanged -= OnLocalGulagMapChanged;
			}
			_systemSubscribed = false;
		}
	}

	private void OnGulagStatus(GulagStatusEvent msg, EntitySessionEventArgs args)
	{
		_inGulag = msg.InGulag;
		if (_inGulag)
		{
			if (!_isFighting && !_queueHiddenByUser)
			{
				EnsureQueueHud();
			}
		}
		else
		{
			HideQueueHud();
			HideFightHud();
			HideSpectatorHud();
			_queueHiddenByUser = false;
		}
	}

	private void OnQueueUpdate(GulagQueueUpdateEvent msg, EntitySessionEventArgs args)
	{
		if (_queueHud != null)
		{
			_queueHud.UpdatePosition(msg.QueuePosition, msg.TotalInQueue);
		}
	}

	private void OnFightStart(GulagFightStartEvent msg, EntitySessionEventArgs args)
	{
		_isFighting = true;
		HideQueueHud();
		EnsureFightHud();
	}

	private void OnFightUpdate(GulagFightUpdateEvent msg, EntitySessionEventArgs args)
	{
		if (_fightHud != null)
		{
			_fightHud.UpdateFight(msg.OpponentUsername, msg.OpponentRank, msg.TimeRemaining);
		}
	}

	private void OnSpectatorUpdate(GulagSpectatorUpdateEvent msg, EntitySessionEventArgs args)
	{
		_isSpectating = true;
		EnsureSpectatorHud();
		if (_spectatorHud != null)
		{
			_spectatorHud.UpdateFight(msg.Fighter1Username, msg.Fighter1Rank, msg.Fighter2Username, msg.Fighter2Rank, msg.TimeRemaining, msg.QueueSize);
		}
	}

	private void OnAdminOffer(GulagAdminOfferEvent msg, EntitySessionEventArgs args)
	{
		EnsureAdminOfferWindow();
	}

	private void OnMapInfo(GulagMapInfoEvent msg, EntitySessionEventArgs args)
	{
	}

	private void OnLocalGulagMapChanged(bool onGulagMap)
	{
		if (onGulagMap)
		{
			if (_inGulag && !_isFighting && !_queueHiddenByUser)
			{
				EnsureQueueHud();
			}
			else if (_isFighting)
			{
				EnsureFightHud();
			}
			if (_isSpectating)
			{
				EnsureSpectatorHud();
			}
		}
		else
		{
			_isFighting = false;
			_isSpectating = false;
			HideQueueHud();
			HideFightHud();
			HideSpectatorHud();
			HideAdminOfferWindow();
		}
	}

	private void EnsureQueueHud()
	{
		if (_queueHud != null)
		{
			return;
		}
		UIScreen activeScreen = base.UIManager.ActiveScreen;
		if (activeScreen != null)
		{
			LayoutContainer val;
			try
			{
				val = ((Control)activeScreen).FindControl<LayoutContainer>("ViewportContainer");
			}
			catch (ArgumentException)
			{
				return;
			}
			_queueHud = new GulagQueueHud();
			_queueHud.HideRequested += OnQueueHideRequested;
			((Control)val).AddChild((Control)(object)_queueHud);
			LayoutContainer.SetAnchorAndMarginPreset((Control)(object)_queueHud, (LayoutPreset)8, (LayoutPresetMode)0, 0);
		}
	}

	private void HideQueueHud()
	{
		if (_queueHud != null)
		{
			_queueHud.HideRequested -= OnQueueHideRequested;
			if (!((Control)_queueHud).Disposed)
			{
				((Control)_queueHud).Orphan();
			}
			_queueHud = null;
		}
	}

	private void OnQueueHideRequested()
	{
		_queueHiddenByUser = true;
		HideQueueHud();
	}

	private void EnsureFightHud()
	{
		if (_fightHud != null)
		{
			return;
		}
		UIScreen activeScreen = base.UIManager.ActiveScreen;
		if (activeScreen != null)
		{
			LayoutContainer val;
			try
			{
				val = ((Control)activeScreen).FindControl<LayoutContainer>("ViewportContainer");
			}
			catch (ArgumentException)
			{
				return;
			}
			_fightHud = new GulagFightHud();
			((Control)val).AddChild((Control)(object)_fightHud);
			LayoutContainer.SetAnchorAndMarginPreset((Control)(object)_fightHud, (LayoutPreset)10, (LayoutPresetMode)0, 0);
			LayoutContainer.SetMarginTop((Control)(object)_fightHud, 20f);
			LayoutContainer.SetGrowHorizontal((Control)(object)_fightHud, (GrowDirection)2);
		}
	}

	private void HideFightHud()
	{
		if (_fightHud != null)
		{
			if (!((Control)_fightHud).Disposed)
			{
				((Control)_fightHud).Orphan();
			}
			_fightHud = null;
			_isFighting = false;
		}
	}

	private void EnsureSpectatorHud()
	{
		if (_spectatorHud != null)
		{
			return;
		}
		UIScreen activeScreen = base.UIManager.ActiveScreen;
		if (activeScreen != null)
		{
			LayoutContainer val;
			try
			{
				val = ((Control)activeScreen).FindControl<LayoutContainer>("ViewportContainer");
			}
			catch (ArgumentException)
			{
				return;
			}
			_spectatorHud = new GulagSpectatorHud();
			((Control)val).AddChild((Control)(object)_spectatorHud);
			LayoutContainer.SetAnchorAndMarginPreset((Control)(object)_spectatorHud, (LayoutPreset)10, (LayoutPresetMode)0, 0);
			LayoutContainer.SetMarginTop((Control)(object)_spectatorHud, 20f);
			LayoutContainer.SetGrowHorizontal((Control)(object)_spectatorHud, (GrowDirection)2);
		}
	}

	private void HideSpectatorHud()
	{
		if (_spectatorHud != null)
		{
			if (!((Control)_spectatorHud).Disposed)
			{
				((Control)_spectatorHud).Orphan();
			}
			_spectatorHud = null;
			_isSpectating = false;
		}
	}

	private void EnsureAdminOfferWindow()
	{
		if (_adminOfferWindow != null)
		{
			return;
		}
		UIScreen activeScreen = base.UIManager.ActiveScreen;
		if (activeScreen != null)
		{
			_adminOfferWindow = new GulagAdminOfferWindow();
			_adminOfferWindow.OnResponse += delegate(bool accepted)
			{
				base.EntityManager.RaisePredictiveEvent<GulagAdminResponseEvent>(new GulagAdminResponseEvent(accepted));
				HideAdminOfferWindow();
			};
			((Control)activeScreen).AddChild((Control)(object)_adminOfferWindow);
			LayoutContainer.SetAnchorAndMarginPreset((Control)(object)_adminOfferWindow, (LayoutPreset)8, (LayoutPresetMode)0, 0);
		}
	}

	private void HideAdminOfferWindow()
	{
		if (_adminOfferWindow != null)
		{
			if (!((Control)_adminOfferWindow).Disposed)
			{
				((Control)_adminOfferWindow).Orphan();
			}
			_adminOfferWindow = null;
		}
	}
}
