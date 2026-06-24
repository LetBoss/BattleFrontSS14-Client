using System;
using System.Collections.Generic;
using Content.Shared.Cargo.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo.Components;

[Serializable]
[NetSerializable]
public sealed class SetFundingAllocationBuiMessage : BoundUserInterfaceMessage
{
	public Dictionary<ProtoId<CargoAccountPrototype>, int> Percents;

	public double PrimaryCut;

	public double LockboxCut;

	public SetFundingAllocationBuiMessage(Dictionary<ProtoId<CargoAccountPrototype>, int> percents, double primaryCut, double lockboxCut)
	{
		Percents = percents;
		PrimaryCut = primaryCut;
		LockboxCut = lockboxCut;
	}
}
