using System;
using System.Linq;
using Content.Client._CIV14merka.Commander.UI;
using Content.Shared._CIV14merka.Commander;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;

namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderTeleportSystem : EntitySystem
{
	private CivCommanderTeleportWindow? _window;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<CivCommanderTeleportTargetsResponseEvent>((EntitySessionEventHandler<CivCommanderTeleportTargetsResponseEvent>)OnTargetsResponse, (Type[])null, (Type[])null);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CivCommanderTeleportWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
		_window = null;
	}

	public void OpenWindow()
	{
		EnsureWindow();
		((BaseWindow)_window).OpenCentered();
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivCommanderTeleportTargetsRequestEvent());
	}

	public void CloseWindow()
	{
		CivCommanderTeleportWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
	}

	private void EnsureWindow()
	{
		if (_window == null)
		{
			_window = new CivCommanderTeleportWindow();
			_window.TargetSelected += OnTargetSelected;
		}
	}

	private void OnTargetsResponse(CivCommanderTeleportTargetsResponseEvent msg, EntitySessionEventArgs args)
	{
		EnsureWindow();
		_window.UpdateTargets(msg.Targets.Select((CivCommanderTeleportTargetState target) => (Name: target.Name, Entity: target.Entity)));
	}

	private void OnTargetSelected(NetEntity target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivCommanderTeleportToTargetRequestEvent(target));
		CivCommanderTeleportWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
	}
}
