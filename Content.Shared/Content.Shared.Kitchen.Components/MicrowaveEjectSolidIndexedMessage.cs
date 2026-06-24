using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen.Components;

[Serializable]
[NetSerializable]
public sealed class MicrowaveEjectSolidIndexedMessage : BoundUserInterfaceMessage
{
	public NetEntity EntityID;

	public MicrowaveEjectSolidIndexedMessage(NetEntity entityId)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		EntityID = entityId;
	}
}
