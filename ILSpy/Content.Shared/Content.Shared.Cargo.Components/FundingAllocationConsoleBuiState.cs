using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo.Components;

[Serializable]
[NetSerializable]
public sealed class FundingAllocationConsoleBuiState : BoundUserInterfaceState
{
	public NetEntity Station;

	public FundingAllocationConsoleBuiState(NetEntity station)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Station = station;
	}
}
