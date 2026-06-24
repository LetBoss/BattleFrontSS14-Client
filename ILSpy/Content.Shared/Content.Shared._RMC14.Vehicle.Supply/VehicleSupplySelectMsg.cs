using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle.Supply;

[Serializable]
[NetSerializable]
public sealed class VehicleSupplySelectMsg : BoundUserInterfaceMessage
{
	public string VehicleId;

	public int CopyIndex;

	public VehicleSupplySelectMsg(string vehicleId, int copyIndex)
	{
		VehicleId = vehicleId;
		CopyIndex = copyIndex;
	}
}
