using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle.Supply;

[Serializable]
[NetSerializable]
public sealed class VehicleHardpointVendorPrintMsg : BoundUserInterfaceMessage
{
	public string HardpointId;

	public VehicleHardpointVendorPrintMsg(string hardpointId)
	{
		HardpointId = hardpointId;
	}
}
