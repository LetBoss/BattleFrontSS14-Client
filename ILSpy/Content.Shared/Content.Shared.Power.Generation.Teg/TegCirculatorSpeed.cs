using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Power.Generation.Teg;

[Serializable]
[NetSerializable]
public enum TegCirculatorSpeed
{
	SpeedStill,
	SpeedSlow,
	SpeedFast
}
