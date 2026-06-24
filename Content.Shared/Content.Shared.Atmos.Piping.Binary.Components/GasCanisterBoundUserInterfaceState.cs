using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Binary.Components;

[Serializable]
[NetSerializable]
public sealed class GasCanisterBoundUserInterfaceState : BoundUserInterfaceState
{
	public float CanisterPressure { get; }

	public bool PortStatus { get; }

	public float TankPressure { get; }

	public GasCanisterBoundUserInterfaceState(float canisterPressure, bool portStatus, float tankPressure)
	{
		CanisterPressure = canisterPressure;
		PortStatus = portStatus;
		TankPressure = tankPressure;
	}
}
