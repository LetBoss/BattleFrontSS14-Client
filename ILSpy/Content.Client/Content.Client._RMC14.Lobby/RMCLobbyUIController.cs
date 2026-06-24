using System;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.JoinXeno;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Client._RMC14.Lobby;

public sealed class RMCLobbyUIController : UIController
{
	private JoinXenoWindow? _joinXenoWindow;

	public override void Initialize()
	{
		((UIController)this).SubscribeLocalEvent<BurrowedLarvaChangedEvent>((EntityEventRefHandler<BurrowedLarvaChangedEvent>)OnBurrowedLarvaChanged, (Type[])null, (Type[])null);
	}

	private void OnBurrowedLarvaChanged(ref BurrowedLarvaChangedEvent ev)
	{
		JoinXenoWindow joinXenoWindow = _joinXenoWindow;
		if (joinXenoWindow != null && ((BaseWindow)joinXenoWindow).IsOpen)
		{
			RefreshWindow(ev.Larva);
		}
	}

	public void OpenJoinXenoWindow()
	{
		JoinXenoSystem joinXenoSystem = base.EntityManager.System<JoinXenoSystem>();
		RefreshWindow(joinXenoSystem.ClientBurrowedLarva);
	}

	private void RefreshWindow(int larva)
	{
		JoinXenoSystem system = base.EntityManager.System<JoinXenoSystem>();
		if (_joinXenoWindow == null || ((Control)_joinXenoWindow).Disposed)
		{
			_joinXenoWindow = new JoinXenoWindow();
			((BaseWindow)_joinXenoWindow).OnClose += delegate
			{
				_joinXenoWindow = null;
			};
			((BaseButton)_joinXenoWindow.LarvaButton).OnPressed += delegate
			{
				system.ClientJoinLarva();
				((BaseWindow)_joinXenoWindow).Close();
			};
			((BaseWindow)_joinXenoWindow).OpenCentered();
		}
		if (larva == 0)
		{
			_joinXenoWindow.Label.Text = Loc.GetString("rmc-lobby-no-burrowed-larva");
			((Control)_joinXenoWindow.Buttons).Visible = false;
		}
		else
		{
			_joinXenoWindow.Label.Text = Loc.GetString("rmc-lobby-burrowed-larva-available");
			((Control)_joinXenoWindow.Buttons).Visible = true;
		}
		system.RequestBurrowedLarvaStatus();
	}
}
