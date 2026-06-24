using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameArenaDeleteMessage : EntityEventArgs
{
	public string ArenaName { get; }

	public MiniGameArenaDeleteMessage(string arenaName)
	{
		ArenaName = arenaName;
	}
}
