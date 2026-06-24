using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameArenaLoadMessage : EntityEventArgs
{
	public string ArenaName { get; }

	public MiniGameArenaLoadMessage(string arenaName)
	{
		ArenaName = arenaName;
	}
}
