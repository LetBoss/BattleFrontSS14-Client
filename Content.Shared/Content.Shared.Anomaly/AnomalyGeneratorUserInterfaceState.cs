using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Anomaly;

[Serializable]
[NetSerializable]
public sealed class AnomalyGeneratorUserInterfaceState : BoundUserInterfaceState
{
	public TimeSpan CooldownEndTime;

	public int FuelAmount;

	public int FuelCost;

	public AnomalyGeneratorUserInterfaceState(TimeSpan cooldownEndTime, int fuelAmount, int fuelCost)
	{
		CooldownEndTime = cooldownEndTime;
		FuelAmount = fuelAmount;
		FuelCost = fuelCost;
	}
}
