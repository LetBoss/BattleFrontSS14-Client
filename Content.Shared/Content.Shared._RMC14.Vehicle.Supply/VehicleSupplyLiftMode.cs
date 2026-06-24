using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle.Supply;

[Serializable]
[NetSerializable]
public enum VehicleSupplyLiftMode
{
	Lowered,
	Raised,
	Lowering,
	Raising,
	Preparing
}
