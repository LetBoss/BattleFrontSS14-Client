using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Audio;

[Serializable]
[Virtual]
[NetSerializable]
public class GlobalSoundEvent : EntityEventArgs
{
	public ResolvedSoundSpecifier Specifier;

	public AudioParams? AudioParams;

	public GlobalSoundEvent(ResolvedSoundSpecifier specifier, AudioParams? audioParams = null)
	{
		Specifier = specifier;
		AudioParams = audioParams;
	}
}
