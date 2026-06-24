using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Portable.Components;

[Serializable]
[NetSerializable]
public sealed class SpaceHeaterChangePowerLevelMessage : BoundUserInterfaceMessage
{
	public SpaceHeaterPowerLevel PowerLevel { get; }

	public SpaceHeaterChangePowerLevelMessage(SpaceHeaterPowerLevel powerLevel)
	{
		PowerLevel = powerLevel;
	}
}
