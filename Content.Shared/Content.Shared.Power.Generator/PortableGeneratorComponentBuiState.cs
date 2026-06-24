using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Power.Generator;

[Serializable]
[NetSerializable]
public sealed class PortableGeneratorComponentBuiState : BoundUserInterfaceState
{
	public float RemainingFuel;

	public bool Clogged;

	public (float Load, float Supply)? NetworkStats;

	public float TargetPower;

	public float MaximumPower;

	public float OptimalPower;

	public bool On;

	public PortableGeneratorComponentBuiState(FuelGeneratorComponent component, float remainingFuel, bool clogged, (float Demand, float Supply)? networkStats)
	{
		RemainingFuel = remainingFuel;
		Clogged = clogged;
		TargetPower = component.TargetPower;
		MaximumPower = component.MaxTargetPower;
		OptimalPower = component.OptimalPower;
		On = component.On;
		NetworkStats = networkStats;
	}
}
