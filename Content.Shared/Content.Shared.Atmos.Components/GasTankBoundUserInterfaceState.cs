using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Components;

[Serializable]
[NetSerializable]
public sealed class GasTankBoundUserInterfaceState : BoundUserInterfaceState
{
	public float TankPressure;
}
