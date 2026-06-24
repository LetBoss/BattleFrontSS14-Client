using System;
using Content.Shared._CIV14merka.Supply;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;

namespace Content.Client._CIV14merka.Supply;

public sealed class CivSupplyRefillSystem : EntitySystem
{
	private const float RefreshInterval = 1.5f;

	private CivSupplyRefillWindow? _window;

	private float _refreshTimer;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeNetworkEvent<CivSupplyRefillStateEvent>((EntityEventHandler<CivSupplyRefillStateEvent>)OnState, (Type[])null, (Type[])null);
	}

	public void OpenWindow()
	{
		if (_window == null)
		{
			_window = new CivSupplyRefillWindow();
			_window.OnSetPeriodic += delegate(string proto, int amount)
			{
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivSupplySetPeriodicEvent(proto, amount));
			};
			_window.OnRefillNow += delegate(string proto, int amount)
			{
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivSupplyRefillNowEvent(proto, amount));
			};
			_window.OnSetThreshold += delegate(int threshold)
			{
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivSupplySetRefillThresholdEvent(threshold));
			};
			((BaseWindow)_window).OnClose += delegate
			{
				_window = null;
			};
		}
		_refreshTimer = 0f;
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivSupplyRefillRequestEvent());
		((BaseWindow)_window).OpenCentered();
	}

	public void CloseWindow()
	{
		CivSupplyRefillWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
		_window = null;
	}

	public override void Update(float frameTime)
	{
		if (_window != null)
		{
			_refreshTimer += frameTime;
			if (!(_refreshTimer < 1.5f))
			{
				_refreshTimer = 0f;
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivSupplyRefillRequestEvent());
			}
		}
	}

	private void OnState(CivSupplyRefillStateEvent ev)
	{
		if (_window != null)
		{
			_window.Populate(ev.Entries);
			_window.SetThreshold(ev.RefillThreshold);
		}
	}
}
