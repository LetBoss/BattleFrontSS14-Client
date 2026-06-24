using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG;

[Serializable]
[NetSerializable]
public sealed class PubgKillsChangedEvent : EntityEventArgs
{
	public int Kills { get; }

	public PubgKillsChangedEvent(int kills)
	{
		Kills = kills;
	}
}
