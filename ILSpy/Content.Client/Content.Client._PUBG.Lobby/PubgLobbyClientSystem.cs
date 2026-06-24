using System;
using Content.Shared._PUBG.Lobby;
using Content.Shared._PUBG.Match;
using Robust.Shared.GameObjects;

namespace Content.Client._PUBG.Lobby;

public sealed class PubgLobbyClientSystem : EntitySystem
{
	public bool InLobby { get; private set; }

	public int TotalPlayers { get; private set; }

	public int ReadyPlayers { get; private set; }

	public int TimeRemaining { get; private set; }

	public event Action? LobbyStatusUpdated;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<LobbyStatusEvent>((EntitySessionEventHandler<LobbyStatusEvent>)OnLobbyStatus, (Type[])null, (Type[])null);
	}

	public void RequestJoinMode(PubgMatchMode matchMode, bool preferFullSquad)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new LobbyJoinModeMessage(matchMode, preferFullSquad));
	}

	private void OnLobbyStatus(LobbyStatusEvent msg, EntitySessionEventArgs args)
	{
		InLobby = msg.InLobby;
		TotalPlayers = msg.TotalPlayers;
		ReadyPlayers = msg.ReadyPlayers;
		TimeRemaining = msg.TimeRemaining;
		this.LobbyStatusUpdated?.Invoke();
	}
}
