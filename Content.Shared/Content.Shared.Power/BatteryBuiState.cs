using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Power;

[Serializable]
[NetSerializable]
public sealed class BatteryBuiState : BoundUserInterfaceState
{
	public bool CanCharge;

	public bool CanDischarge;

	public bool SupplyingNetworkHasPower;

	public bool LoadingNetworkHasPower;

	public float CurrentReceiving;

	public float CurrentSupply;

	public float MaxChargeRate;

	public float MinMaxChargeRate;

	public float MaxMaxChargeRate;

	public float Efficiency;

	public float MaxSupply;

	public float MinMaxSupply;

	public float MaxMaxSupply;

	public float Charge;

	public float Capacity;
}
