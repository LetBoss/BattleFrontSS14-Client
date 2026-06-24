using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameArenaSpawnVisibilityMessage : EntityEventArgs
{
	public bool ShowSpawns { get; }

	public MiniGameArenaSpawnVisibilityMessage(bool showSpawns)
	{
		ShowSpawns = showSpawns;
	}
}
