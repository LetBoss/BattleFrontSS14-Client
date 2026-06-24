using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle;

[Serializable]
[NetSerializable]
public sealed class VehicleWeaponsStabilizationMessage : BoundUserInterfaceMessage
{
	public readonly bool Enabled;

	public VehicleWeaponsStabilizationMessage(bool enabled)
	{
		Enabled = enabled;
	}
}
