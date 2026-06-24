using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameArenaSaveMessage : EntityEventArgs
{
	public string DisplayName { get; }

	public bool Overwrite { get; }

	public string? ExistingArenaName { get; }

	public MiniGameArenaSaveMessage(string displayName, bool overwrite = false, string? existingArenaName = null)
	{
		DisplayName = displayName;
		Overwrite = overwrite;
		ExistingArenaName = existingArenaName;
	}
}
