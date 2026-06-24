using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Singularity.Components;

[Serializable]
[NetSerializable]
public sealed class ParticleAcceleratorSetPowerStateMessage : BoundUserInterfaceMessage
{
	public readonly ParticleAcceleratorPowerState State;

	public ParticleAcceleratorSetPowerStateMessage(ParticleAcceleratorPowerState state)
	{
		State = state;
	}
}
