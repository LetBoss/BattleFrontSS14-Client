using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Inventory.Events;

[Serializable]
[NetSerializable]
public sealed class InventoryEquipActEvent : EntityEventArgs
{
	public readonly NetEntity Uid;

	public readonly NetEntity ItemUid;

	public readonly string Slot;

	public readonly bool Silent;

	public readonly bool Force;

	public InventoryEquipActEvent(NetEntity uid, NetEntity itemUid, string slot, bool silent = false, bool force = false)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Uid = uid;
		ItemUid = itemUid;
		Slot = slot;
		Silent = silent;
		Force = force;
	}
}
