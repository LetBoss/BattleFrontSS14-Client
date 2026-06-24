using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Singularity.Components;

[Serializable]
[NetSerializable]
public enum RadiationCollectorVisuals
{
	VisualState,
	TankInserted,
	PressureState
}
