using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Ame.Components;

[Serializable]
[NetSerializable]
public sealed class AmeControllerBoundUserInterfaceState : BoundUserInterfaceState
{
	public readonly bool HasPower;

	public readonly bool IsMaster;

	public readonly bool Injecting;

	public readonly bool HasFuelJar;

	public readonly int FuelAmount;

	public readonly int InjectionAmount;

	public readonly int CoreCount;

	public readonly float CurrentPowerSupply;

	public readonly float TargetedPowerSupply;

	public AmeControllerBoundUserInterfaceState(bool hasPower, bool isMaster, bool injecting, bool hasFuelJar, int fuelAmount, int injectionAmount, int coreCount, float currentPowerSupply, float targetedPowerSupply)
	{
		HasPower = hasPower;
		IsMaster = isMaster;
		Injecting = injecting;
		HasFuelJar = hasFuelJar;
		FuelAmount = fuelAmount;
		InjectionAmount = injectionAmount;
		CoreCount = coreCount;
		CurrentPowerSupply = currentPowerSupply;
		TargetedPowerSupply = targetedPowerSupply;
	}
}
