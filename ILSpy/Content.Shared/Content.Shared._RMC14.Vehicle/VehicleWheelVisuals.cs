using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle;

[Serializable]
[NetSerializable]
public enum VehicleWheelVisuals : byte
{
	HasAllWheels,
	WheelCount,
	WheelFunctionalCount,
	WheelIntegrityFraction
}
