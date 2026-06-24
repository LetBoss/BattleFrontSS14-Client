using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle.Supply;

[Serializable]
[NetSerializable]
public sealed class VehicleSupplyPreviewState
{
	public string VehicleId;

	public int CopyIndex;

	public List<VehicleHardpointLayerState> Layers;

	public List<VehicleSupplyPreviewOverlay> Overlays;

	public VehicleSupplyPreviewState(string vehicleId, int copyIndex, List<VehicleHardpointLayerState> layers, List<VehicleSupplyPreviewOverlay> overlays)
	{
		VehicleId = vehicleId;
		CopyIndex = copyIndex;
		Layers = layers;
		Overlays = overlays;
	}
}
