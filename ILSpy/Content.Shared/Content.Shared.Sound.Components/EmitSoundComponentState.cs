using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Sound.Components;

[Serializable]
[NetSerializable]
public struct EmitSoundComponentState(SoundSpecifier? sound) : IComponentState
{
	public SoundSpecifier? Sound { get; } = sound;
}
