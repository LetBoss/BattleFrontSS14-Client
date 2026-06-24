using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Corvax.TTS;

[Serializable]
[NetSerializable]
public sealed class ClientOptionTTSEvent(bool enabled) : EntityEventArgs
{
	public bool Enabled { get; } = enabled;
}
