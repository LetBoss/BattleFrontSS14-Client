using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle.Supply;

[Serializable]
[NetSerializable]
public sealed class VehicleHardpointVendorSelectMsg : BoundUserInterfaceMessage
{
	public string VehicleId;

	public VehicleHardpointVendorSelectMsg(string vehicleId)
	{
		VehicleId = vehicleId;
	}
}
