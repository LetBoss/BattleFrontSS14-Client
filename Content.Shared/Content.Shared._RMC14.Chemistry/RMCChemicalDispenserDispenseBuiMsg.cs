using System;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Chemistry;

[Serializable]
[NetSerializable]
public sealed class RMCChemicalDispenserDispenseBuiMsg(ProtoId<ReagentPrototype> reagent) : BoundUserInterfaceMessage
{
	public readonly ProtoId<ReagentPrototype> Reagent = reagent;
}
