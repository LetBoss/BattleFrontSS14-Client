using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle;

[Serializable]
[NetSerializable]
public sealed class VehicleHardpointVisualsComponentState : ComponentState
{
	public readonly List<VehicleHardpointLayerState> Layers;

	public VehicleHardpointVisualsComponentState(List<VehicleHardpointLayerState> layers)
	{
		Layers = layers;
	}
}
