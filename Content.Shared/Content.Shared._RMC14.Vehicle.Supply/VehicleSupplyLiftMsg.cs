using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle.Supply;

[Serializable]
[NetSerializable]
public sealed class VehicleSupplyLiftMsg : BoundUserInterfaceMessage
{
	public bool Raise;

	public VehicleSupplyLiftMsg(bool raise)
	{
		Raise = raise;
	}
}
