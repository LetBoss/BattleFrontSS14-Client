using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle.Supply;

[Serializable]
[NetSerializable]
public sealed class VehicleHardpointVendorBuiState : BoundUserInterfaceState
{
	public List<VehicleSupplyEntryState> Vehicles;

	public List<VehicleSupplyEntryState> Hardpoints;

	public string SelectedVehicle;

	public VehicleHardpointVendorBuiState(List<VehicleSupplyEntryState> vehicles, List<VehicleSupplyEntryState> hardpoints, string selectedVehicle)
	{
		Vehicles = vehicles;
		Hardpoints = hardpoints;
		SelectedVehicle = selectedVehicle;
	}
}
