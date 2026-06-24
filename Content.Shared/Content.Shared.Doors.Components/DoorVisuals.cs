using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Doors.Components;

[Serializable]
[NetSerializable]
public enum DoorVisuals : byte
{
	State,
	BoltLights,
	EmergencyLights,
	ClosedLights,
	BaseRSI
}
