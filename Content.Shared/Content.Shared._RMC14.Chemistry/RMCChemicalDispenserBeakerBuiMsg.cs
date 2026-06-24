using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Chemistry;

[Serializable]
[NetSerializable]
public sealed class RMCChemicalDispenserBeakerBuiMsg(FixedPoint2 amount) : BoundUserInterfaceMessage
{
	public readonly FixedPoint2 Amount = amount;
}
