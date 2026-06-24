using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.PowerLoader.Events;

[Serializable]
[NetSerializable]
public sealed class GetAttachmentSlotEvent : EntityEventArgs
{
	public NetEntity User;

	public NetEntity? Used;

	public bool BeingAttached = true;

	public string SlotId = "";

	public bool CanUse = true;

	public GetAttachmentSlotEvent(NetEntity user, NetEntity? used)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		User = user;
		Used = used;
	}
}
