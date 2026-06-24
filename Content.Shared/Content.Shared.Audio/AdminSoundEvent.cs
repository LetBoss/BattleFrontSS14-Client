using System;
using Robust.Shared.Audio;
using Robust.Shared.Serialization;

namespace Content.Shared.Audio;

[Serializable]
[NetSerializable]
public sealed class AdminSoundEvent : GlobalSoundEvent
{
	public AdminSoundEvent(ResolvedSoundSpecifier specifier, AudioParams? audioParams = null)
		: base(specifier, audioParams)
	{
	}
}
