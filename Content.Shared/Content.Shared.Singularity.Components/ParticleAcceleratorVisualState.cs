using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Singularity.Components;

[Serializable]
[NetSerializable]
public enum ParticleAcceleratorVisualState
{
	Unpowered,
	Powered,
	Level0,
	Level1,
	Level2,
	Level3
}
