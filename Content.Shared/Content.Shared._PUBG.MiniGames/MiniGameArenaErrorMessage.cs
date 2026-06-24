using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameArenaErrorMessage : EntityEventArgs
{
	public string ErrorLocKey { get; }

	public MiniGameArenaErrorMessage(string errorLocKey)
	{
		ErrorLocKey = errorLocKey;
	}
}
