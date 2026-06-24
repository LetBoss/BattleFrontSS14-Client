using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Power;

[Serializable]
[NetSerializable]
public enum ChargeState : byte
{
	Still,
	Charging,
	Discharging
}
