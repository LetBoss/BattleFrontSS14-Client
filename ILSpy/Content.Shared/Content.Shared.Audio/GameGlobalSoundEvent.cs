using System;
using Robust.Shared.Audio;
using Robust.Shared.Serialization;

namespace Content.Shared.Audio;

[Serializable]
[NetSerializable]
public sealed class GameGlobalSoundEvent : GlobalSoundEvent
{
	public GameGlobalSoundEvent(ResolvedSoundSpecifier specifier, AudioParams? audioParams = null)
		: base(specifier, audioParams)
	{
	}
}
