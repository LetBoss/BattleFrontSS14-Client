using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Interaction;

[Serializable]
[NetSerializable]
public sealed class InteractInventorySlotEvent : EntityEventArgs
{
	public NetEntity ItemUid { get; }

	public bool AltInteract { get; }

	public InteractInventorySlotEvent(NetEntity itemUid, bool altInteract = false)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		ItemUid = itemUid;
		AltInteract = altInteract;
	}
}
