using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo.Components;

[Serializable]
[NetSerializable]
public sealed class BountyPrintLabelMessage : BoundUserInterfaceMessage
{
	public string BountyId;

	public BountyPrintLabelMessage(string bountyId)
	{
		BountyId = bountyId;
	}
}
