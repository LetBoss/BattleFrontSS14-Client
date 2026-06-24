using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle;

[Serializable]
[NetSerializable]
public sealed class RMCVehicleAmmoLoaderUiState : BoundUserInterfaceState
{
	public readonly List<RMCVehicleAmmoLoaderUiEntry> Hardpoints;

	public readonly int AmmoAmount;

	public readonly int AmmoMax;

	public RMCVehicleAmmoLoaderUiState(List<RMCVehicleAmmoLoaderUiEntry> hardpoints, int ammoAmount, int ammoMax)
	{
		Hardpoints = hardpoints;
		AmmoAmount = ammoAmount;
		AmmoMax = ammoMax;
	}
}
