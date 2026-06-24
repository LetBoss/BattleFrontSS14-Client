using System;
using Robust.Shared.Audio;
using Robust.Shared.Serialization;

namespace Content.Shared.Audio;

[Serializable]
[NetSerializable]
public sealed class StationEventMusicEvent : GlobalSoundEvent
{
	public StationEventMusicType Type;

	public StationEventMusicEvent(ResolvedSoundSpecifier specifier, StationEventMusicType type, AudioParams? audioParams = null)
		: base(specifier, audioParams)
	{
		Type = type;
	}
}
