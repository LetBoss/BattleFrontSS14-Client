using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Doors.Systems;

[Serializable]
[NetSerializable]
public enum FirelockVisuals : byte
{
	PressureWarning,
	TemperatureWarning
}
