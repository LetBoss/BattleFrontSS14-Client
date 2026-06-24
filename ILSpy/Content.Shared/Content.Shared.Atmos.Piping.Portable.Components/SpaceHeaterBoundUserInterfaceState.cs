using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Portable.Components;

[Serializable]
[NetSerializable]
public sealed class SpaceHeaterBoundUserInterfaceState : BoundUserInterfaceState
{
	public float MinTemperature { get; }

	public float MaxTemperature { get; }

	public float TargetTemperature { get; }

	public bool Enabled { get; }

	public SpaceHeaterMode Mode { get; }

	public SpaceHeaterPowerLevel PowerLevel { get; }

	public SpaceHeaterBoundUserInterfaceState(float minTemperature, float maxTemperature, float temperature, bool enabled, SpaceHeaterMode mode, SpaceHeaterPowerLevel powerLevel)
	{
		MinTemperature = minTemperature;
		MaxTemperature = maxTemperature;
		TargetTemperature = temperature;
		Enabled = enabled;
		Mode = mode;
		PowerLevel = powerLevel;
	}
}
