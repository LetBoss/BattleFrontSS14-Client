using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry;

[Serializable]
[NetSerializable]
public sealed class TransferAmountBoundInterfaceState : BoundUserInterfaceState
{
	public FixedPoint2 Max;

	public FixedPoint2 Min;

	public TransferAmountBoundInterfaceState(FixedPoint2 max, FixedPoint2 min)
	{
		Max = max;
		Min = min;
	}
}
