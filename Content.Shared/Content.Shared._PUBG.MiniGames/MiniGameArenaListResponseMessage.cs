using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameArenaListResponseMessage : EntityEventArgs
{
	public List<MiniGameArenaInfo> Arenas { get; }

	public int MaxArenas { get; }

	public MiniGameArenaListResponseMessage(List<MiniGameArenaInfo> arenas, int maxArenas)
	{
		Arenas = arenas;
		MaxArenas = maxArenas;
	}
}
