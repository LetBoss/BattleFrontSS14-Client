using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameLobbyListEntry
{
	public int LobbyId { get; }

	public string Name { get; }

	public string LeaderName { get; }

	public string GameName { get; }

	public string SubmodeName { get; }

	public string MapName { get; }

	public int CurrentPlayers { get; }

	public int MaxPlayers { get; }

	public bool IsLocked { get; }

	public bool HasPassword { get; }

	public MiniGameLobbyListEntry(int lobbyId, string name, string leaderName, string gameName, string submodeName, string mapName, int currentPlayers, int maxPlayers, bool isLocked, bool hasPassword)
	{
		LobbyId = lobbyId;
		Name = name;
		LeaderName = leaderName;
		GameName = gameName;
		SubmodeName = submodeName;
		MapName = mapName;
		CurrentPlayers = currentPlayers;
		MaxPlayers = maxPlayers;
		IsLocked = isLocked;
		HasPassword = hasPassword;
	}
}
