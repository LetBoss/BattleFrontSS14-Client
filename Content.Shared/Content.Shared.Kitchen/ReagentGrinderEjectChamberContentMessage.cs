using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen;

[Serializable]
[NetSerializable]
public sealed class ReagentGrinderEjectChamberContentMessage : BoundUserInterfaceMessage
{
	public NetEntity EntityId;

	public ReagentGrinderEjectChamberContentMessage(NetEntity entityId)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		EntityId = entityId;
	}
}
