using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

[Serializable]
[NetSerializable]
public sealed class BwoinkDiscordRelayUpdated : EntityEventArgs
{
	public bool DiscordRelayEnabled { get; }

	public BwoinkDiscordRelayUpdated(bool enabled)
	{
		DiscordRelayEnabled = enabled;
	}
}
