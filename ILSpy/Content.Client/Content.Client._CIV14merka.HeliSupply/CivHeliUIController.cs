using System;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Shared._CIV14merka.HeliSupply;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Timing;

namespace Content.Client._CIV14merka.HeliSupply;

public sealed class CivHeliUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
	[UISystemDependency]
	private readonly CivHeliSupplySystem? _heli;

	private CivHeliWindow? _window;

	private CivHeliRoutePanel? _routePanel;

	private bool _suppressCancelOnClose;

	private int _lastEtaPointCount = -1;

	public void OnStateEntered(GameplayState state)
	{
		if (_heli != null)
		{
			_heli.OnOpenReceived += OnOpen;
			_heli.OnStateReceived += OnState;
			_heli.OnRouteModeEnded += OnRouteModeEnded;
		}
	}

	public void OnStateExited(GameplayState state)
	{
		if (_heli != null)
		{
			_heli.OnOpenReceived -= OnOpen;
			_heli.OnStateReceived -= OnState;
			_heli.OnRouteModeEnded -= OnRouteModeEnded;
		}
		CloseWindow(silent: true);
		DisposeRoutePanel();
	}

	public override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((UIController)this).FrameUpdate(args);
		bool flag = _heli?.IsRouteMode ?? false;
		if (flag)
		{
			EnsureRoutePanel();
			int count = _heli.RoutePoints.Count;
			_routePanel?.SetPointCount(count);
			if (count != _lastEtaPointCount)
			{
				_lastEtaPointCount = count;
				_routePanel?.SetEta(_heli.EstimateEta());
			}
		}
		else
		{
			_lastEtaPointCount = -1;
		}
		if (_routePanel != null)
		{
			((Control)_routePanel).Visible = flag;
		}
	}

	private void OnOpen()
	{
		if (_heli == null)
		{
			return;
		}
		if (_window == null)
		{
			_window = new CivHeliWindow();
			_window.OnAddItem += delegate(string protoId, int amount)
			{
				_heli.RequestAdd(protoId, amount);
			};
			_window.OnRemoveItem += delegate(string protoId)
			{
				_heli.RequestRemove(protoId);
			};
			_window.OnBuildRoute += OnBuildRoute;
			_window.OnCancelPressed += OnCancelPressed;
			((BaseWindow)_window).OnClose += OnWindowClosed;
			((BaseWindow)_window).OpenCentered();
		}
		else if (!((BaseWindow)_window).IsOpen)
		{
			((BaseWindow)_window).OpenCentered();
		}
		if (_heli.LastState != null)
		{
			_window.UpdateState(_heli.LastState);
		}
	}

	private void OnState(CivHeliStateMessage state)
	{
		_window?.UpdateState(state);
	}

	private void OnBuildRoute()
	{
		if (_heli != null)
		{
			_suppressCancelOnClose = true;
			CivHeliWindow? window = _window;
			if (window != null)
			{
				((BaseWindow)window).Close();
			}
			_heli.StartRouteMode();
		}
	}

	private void OnCancelPressed()
	{
		_heli?.RequestCancel();
		_suppressCancelOnClose = true;
		CivHeliWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
	}

	private void OnWindowClosed()
	{
		if (!_suppressCancelOnClose)
		{
			_heli?.RequestCancel();
		}
		_suppressCancelOnClose = false;
		_window = null;
	}

	private void OnRouteModeEnded(bool launched)
	{
		if (!launched)
		{
			OnOpen();
		}
	}

	private void CloseWindow(bool silent)
	{
		if (_window != null)
		{
			if (silent)
			{
				_suppressCancelOnClose = true;
			}
			((BaseWindow)_window).Close();
		}
	}

	private void EnsureRoutePanel()
	{
		UIScreen activeScreen = base.UIManager.ActiveScreen;
		if (activeScreen != null && activeScreen.GetWidget<MainViewport>() != null)
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
			if (_routePanel == null)
			{
				_routePanel = new CivHeliRoutePanel();
				_routePanel.LaunchPressed += OnRouteLaunchPressed;
				_routePanel.BackPressed += OnRouteBackPressed;
				_routePanel.SetCost(_heli?.GetConfig().LaunchCost ?? 0);
			}
			if ((object)((Control)_routePanel).Parent != val)
			{
				((Control)_routePanel).Orphan();
				((Control)val).AddChild((Control)(object)_routePanel);
				LayoutContainer.SetAnchorAndMarginPreset((Control)(object)_routePanel, (LayoutPreset)5, (LayoutPresetMode)0, 8);
				LayoutContainer.SetGrowHorizontal((Control)(object)_routePanel, (GrowDirection)2);
				LayoutContainer.SetGrowVertical((Control)(object)_routePanel, (GrowDirection)0);
			}
			((Control)_routePanel).SetPositionLast();
		}
	}

	private void OnRouteLaunchPressed()
	{
		_heli?.FinishRoute();
	}

	private void OnRouteBackPressed()
	{
		_heli?.CancelRouteMode();
	}

	private void DisposeRoutePanel()
	{
		if (_routePanel != null)
		{
			_routePanel.LaunchPressed -= OnRouteLaunchPressed;
			_routePanel.BackPressed -= OnRouteBackPressed;
			((Control)_routePanel).Orphan();
			_routePanel = null;
		}
	}
}
