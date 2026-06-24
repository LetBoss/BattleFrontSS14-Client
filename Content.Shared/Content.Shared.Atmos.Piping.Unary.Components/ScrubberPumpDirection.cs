using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Unary.Components;

[Serializable]
[NetSerializable]
public enum ScrubberPumpDirection : sbyte
{
	Siphoning,
	Scrubbing
}
