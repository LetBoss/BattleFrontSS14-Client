using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Audio;

[Serializable]
[NetSerializable]
public sealed class AmbientSoundComponentState : ComponentState
{
	public bool Enabled { get; init; }

	public float Range { get; init; }

	public float Volume { get; init; }

	public SoundSpecifier Sound { get; init; }
}
