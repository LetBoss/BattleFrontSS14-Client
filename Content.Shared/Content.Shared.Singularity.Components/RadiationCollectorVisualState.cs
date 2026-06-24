using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Singularity.Components;

[Serializable]
[NetSerializable]
public enum RadiationCollectorVisualState
{
	Active = 1,
	Activating = 3,
	Deactivating = 2,
	Deactive = 0
}
