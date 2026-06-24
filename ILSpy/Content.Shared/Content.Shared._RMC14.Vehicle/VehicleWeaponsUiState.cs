using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle;

[Serializable]
[NetSerializable]
public sealed class VehicleWeaponsUiState : BoundUserInterfaceState
{
	public readonly NetEntity Vehicle;

	public readonly List<VehicleWeaponsUiEntry> Hardpoints;

	public readonly bool CanToggleStabilization;

	public readonly bool StabilizationEnabled;

	public readonly bool CanToggleAuto;

	public readonly bool AutoEnabled;

	public VehicleWeaponsUiState(NetEntity vehicle, List<VehicleWeaponsUiEntry> hardpoints, bool canToggleStabilization, bool stabilizationEnabled, bool canToggleAuto, bool autoEnabled)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Vehicle = vehicle;
		Hardpoints = hardpoints;
		CanToggleStabilization = canToggleStabilization;
		StabilizationEnabled = stabilizationEnabled;
		CanToggleAuto = canToggleAuto;
		AutoEnabled = autoEnabled;
	}
}
