using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle.Supply;

[Serializable]
[NetSerializable]
public sealed class VehicleSupplyBuiState : BoundUserInterfaceState
{
	public VehicleSupplyLiftMode? LiftMode;

	public bool Busy;

	public string? ActiveVehicleId;

	public string? SelectedVehicleId;

	public int SelectedCopyIndex;

	public VehicleSupplyPreviewState? Preview;

	public List<VehicleSupplyEntryState> Available;

	public VehicleSupplyBuiState(VehicleSupplyLiftMode? liftMode, bool busy, string? activeVehicleId, string? selectedVehicleId, int selectedCopyIndex, VehicleSupplyPreviewState? preview, List<VehicleSupplyEntryState> available)
	{
		LiftMode = liftMode;
		Busy = busy;
		ActiveVehicleId = activeVehicleId;
		SelectedVehicleId = selectedVehicleId;
		SelectedCopyIndex = selectedCopyIndex;
		Preview = preview;
		Available = available;
	}
}
