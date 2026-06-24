using System;
using Content.Client._PUBG.Lobby.UI;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;

namespace Content.Client._PUBG.Lobby;

public sealed class PubgPreLobbyHubState : State
{
	[Dependency]
	private IUserInterfaceManager _userInterfaceManager;

	public PubgPreLobbyHubGui? Hub { get; private set; }

	protected override Type? LinkedScreenType => typeof(PubgPreLobbyHubGui);

	protected override void Startup()
	{
		Hub = (PubgPreLobbyHubGui)(object)_userInterfaceManager.ActiveScreen;
	}

	protected override void Shutdown()
	{
		Hub = null;
	}
}
