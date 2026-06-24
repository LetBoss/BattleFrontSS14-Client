using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry;

[Serializable]
[NetSerializable]
public sealed class TransferAmountSetValueMessage : BoundUserInterfaceMessage
{
	public FixedPoint2 Value;

	public TransferAmountSetValueMessage(FixedPoint2 value)
	{
		Value = value;
	}
}
