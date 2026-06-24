using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Silicons.Borgs;

[Serializable]
[NetSerializable]
public sealed class BorgBuiState : BoundUserInterfaceState
{
	public float ChargePercent;

	public bool HasBattery;

	public BorgBuiState(float chargePercent, bool hasBattery)
	{
		ChargePercent = chargePercent;
		HasBattery = hasBattery;
	}
}
