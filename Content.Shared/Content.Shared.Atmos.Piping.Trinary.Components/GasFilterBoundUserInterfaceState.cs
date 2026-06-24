using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Trinary.Components;

[Serializable]
[NetSerializable]
public sealed class GasFilterBoundUserInterfaceState : BoundUserInterfaceState
{
	public string FilterLabel { get; }

	public float TransferRate { get; }

	public bool Enabled { get; }

	public Gas? FilteredGas { get; }

	public GasFilterBoundUserInterfaceState(string filterLabel, float transferRate, bool enabled, Gas? filteredGas)
	{
		FilterLabel = filterLabel;
		TransferRate = transferRate;
		Enabled = enabled;
		FilteredGas = filteredGas;
	}
}
