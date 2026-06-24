using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameArenaEnterCustomizationMessage : EntityEventArgs
{
	public string? ArenaName { get; }

	public MiniGameArenaEnterCustomizationMessage(string? arenaName = null)
	{
		ArenaName = arenaName;
	}
}
