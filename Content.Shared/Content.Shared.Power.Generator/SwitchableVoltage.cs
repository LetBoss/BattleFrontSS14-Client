using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Power.Generator;

[Serializable]
[NetSerializable]
public enum SwitchableVoltage : byte
{
	HV,
	MV,
	LV
}
