using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Clock;

[Serializable]
[NetSerializable]
public enum ClockType : byte
{
	TwelveHour,
	TwentyFourHour
}
