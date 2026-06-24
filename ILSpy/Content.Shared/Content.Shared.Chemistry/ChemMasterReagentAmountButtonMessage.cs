using System;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry;

[Serializable]
[NetSerializable]
public sealed class ChemMasterReagentAmountButtonMessage : BoundUserInterfaceMessage
{
	public readonly ReagentId ReagentId;

	public readonly ChemMasterReagentAmount Amount;

	public readonly bool FromBuffer;

	public ChemMasterReagentAmountButtonMessage(ReagentId reagentId, ChemMasterReagentAmount amount, bool fromBuffer)
	{
		ReagentId = reagentId;
		Amount = amount;
		FromBuffer = fromBuffer;
	}
}
