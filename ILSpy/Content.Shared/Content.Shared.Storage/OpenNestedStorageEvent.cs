using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Storage;

[Serializable]
[NetSerializable]
public sealed class OpenNestedStorageEvent : EntityEventArgs
{
	public readonly NetEntity InteractedItemUid;

	public readonly NetEntity StorageUid;

	public OpenNestedStorageEvent(NetEntity interactedItemUid, NetEntity storageUid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		InteractedItemUid = interactedItemUid;
		StorageUid = storageUid;
	}
}
