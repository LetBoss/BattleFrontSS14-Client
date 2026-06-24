using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Holopad;

[Serializable]
[NetSerializable]
public sealed class HolopadStartNewCallMessage : BoundUserInterfaceMessage
{
	public readonly NetEntity Receiver;

	public HolopadStartNewCallMessage(NetEntity receiver)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Receiver = receiver;
	}
}
