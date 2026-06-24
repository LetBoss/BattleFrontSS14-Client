using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo.Components;

[Serializable]
[NetSerializable]
public sealed class BountySkipMessage : BoundUserInterfaceMessage
{
	public string BountyId;

	public BountySkipMessage(string bountyId)
	{
		BountyId = bountyId;
	}
}
