using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG;

[Serializable]
[NetSerializable]
public sealed class PubgTeamModeStatusEvent : EntityEventArgs
{
	public bool Enabled { get; }

	public int TeamSize { get; }

	public PubgTeamModeStatusEvent(bool enabled, int teamSize)
	{
		Enabled = enabled;
		TeamSize = teamSize;
	}
}
