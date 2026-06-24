using System;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;

namespace Content.Client._PUBG.Connection;

public sealed class QueueState : State
{
	[Dependency]
	private IUserInterfaceManager _userInterfaceManager;

	private QueueGui? _gui;

	protected override Type? LinkedScreenType => typeof(QueueGui);

	protected override void Startup()
	{
		_gui = (QueueGui)(object)_userInterfaceManager.ActiveScreen;
	}

	protected override void Shutdown()
	{
		_gui = null;
	}

	public void UpdatePosition(int position, int total)
	{
		_gui?.UpdatePosition(position, total);
	}
}
