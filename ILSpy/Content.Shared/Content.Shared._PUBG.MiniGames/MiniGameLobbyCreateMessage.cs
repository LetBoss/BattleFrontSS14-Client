using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameLobbyCreateMessage : EntityEventArgs
{
	public string Name { get; }

	public string GameId { get; }

	public string SubmodeId { get; }

	public string MapId { get; }

	public int Rounds { get; }

	public int MaxPlayers { get; }

	public bool IsLocked { get; }

	public string Password { get; }

	public MiniGameLobbyCreateMessage(string name, string gameId, string submodeId, string mapId, int rounds, int maxPlayers, bool isLocked, string password)
	{
		Name = name;
		GameId = gameId;
		SubmodeId = submodeId;
		MapId = mapId;
		Rounds = rounds;
		MaxPlayers = maxPlayers;
		IsLocked = isLocked;
		Password = password;
	}
}
