using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Vehicle.Components;

[Serializable]
[NetSerializable]
public enum VehicleVisuals : byte
{
	HasOperator,
	CanRun
}
