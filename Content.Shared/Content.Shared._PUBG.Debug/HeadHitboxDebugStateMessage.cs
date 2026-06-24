using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Debug;

[Serializable]
[NetSerializable]
public sealed class HeadHitboxDebugStateMessage : EntityEventArgs
{
	public bool Enabled { get; }

	public HeadHitboxDebugStateMessage(bool enabled)
	{
		Enabled = enabled;
	}
}
