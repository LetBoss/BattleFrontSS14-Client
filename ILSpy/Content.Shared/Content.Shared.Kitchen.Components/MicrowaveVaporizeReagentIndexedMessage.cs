using System;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen.Components;

[Serializable]
[NetSerializable]
public sealed class MicrowaveVaporizeReagentIndexedMessage : BoundUserInterfaceMessage
{
	public ReagentQuantity ReagentQuantity;

	public MicrowaveVaporizeReagentIndexedMessage(ReagentQuantity reagentQuantity)
	{
		ReagentQuantity = reagentQuantity;
	}
}
