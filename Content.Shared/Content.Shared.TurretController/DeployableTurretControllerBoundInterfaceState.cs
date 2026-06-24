using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.TurretController;

[Serializable]
[NetSerializable]
public sealed class DeployableTurretControllerBoundInterfaceState : BoundUserInterfaceState
{
	public Dictionary<string, string> TurretStateByAddress;

	public DeployableTurretControllerBoundInterfaceState(Dictionary<string, string> turretStateByAddress)
	{
		TurretStateByAddress = turretStateByAddress;
	}
}
