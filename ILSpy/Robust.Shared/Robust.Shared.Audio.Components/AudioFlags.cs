using System;

namespace Robust.Shared.Audio.Components;

[Flags]
public enum AudioFlags : byte
{
	None = 0,
	GridAudio = 1,
	NoOcclusion = 2
}
