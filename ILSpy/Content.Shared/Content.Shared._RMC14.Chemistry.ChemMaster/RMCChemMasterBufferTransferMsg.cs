using System;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Chemistry.ChemMaster;

[Serializable]
[NetSerializable]
public sealed class RMCChemMasterBufferTransferMsg(ProtoId<ReagentPrototype> reagent, FixedPoint2 amount) : BoundUserInterfaceMessage
{
	public readonly ProtoId<ReagentPrototype> Reagent = reagent;

	public readonly FixedPoint2 Amount = amount;
}
