using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameArenaInfo
{
	public string FileName { get; set; }

	public string DisplayName { get; set; }

	public DateTime DateCreated { get; set; }

	public int Team1SpawnCount { get; set; }

	public int Team2SpawnCount { get; set; }

	public MiniGameArenaInfo(string fileName, string displayName, DateTime dateCreated, int team1SpawnCount, int team2SpawnCount)
	{
		FileName = fileName;
		DisplayName = displayName;
		DateCreated = dateCreated;
		Team1SpawnCount = team1SpawnCount;
		Team2SpawnCount = team2SpawnCount;
	}
}
