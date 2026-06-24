using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Clock;

[Serializable]
[NetSerializable]
public enum ClockVisualLayers : byte
{
	HourHand,
	MinuteHand
}
