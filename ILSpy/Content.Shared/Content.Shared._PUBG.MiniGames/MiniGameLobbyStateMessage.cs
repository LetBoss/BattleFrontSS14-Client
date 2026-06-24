using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameLobbyStateMessage : EntityEventArgs
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

	public int RoundsTotal { get; }

	public int CurrentRound { get; }

	public int MaxRounds { get; }

	public bool InGame { get; }

	public List<MiniGameLobbyPlayerInfo> Players { get; }

	public List<MiniGameLobbyChatEntry> ChatHistory { get; }

	public MiniGameLobbyStateMessage(int lobbyId, string name, string leaderName, string gameName, string submodeName, string mapName, int currentPlayers, int maxPlayers, bool isLocked, bool hasPassword, int roundsTotal, int currentRound, int maxRounds, bool inGame, List<MiniGameLobbyPlayerInfo> players, List<MiniGameLobbyChatEntry> chatHistory)
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
		RoundsTotal = roundsTotal;
		CurrentRound = currentRound;
		MaxRounds = maxRounds;
		InGame = inGame;
		Players = players;
		ChatHistory = chatHistory;
	}
}
