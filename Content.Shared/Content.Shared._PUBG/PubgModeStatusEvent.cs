using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG;

[Serializable]
[NetSerializable]
public sealed class PubgModeStatusEvent : EntityEventArgs
{
	public bool Enabled { get; }

	public PubgModeStatusEvent(bool enabled)
	{
		Enabled = enabled;
	}
}
