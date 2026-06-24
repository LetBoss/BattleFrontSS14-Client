using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Storage;

[Serializable]
[NetSerializable]
public sealed class StorageInsertItemIntoLocationEvent : EntityEventArgs
{
	public readonly NetEntity ItemEnt;

	public readonly NetEntity StorageEnt;

	public readonly ItemStorageLocation Location;

	public StorageInsertItemIntoLocationEvent(NetEntity itemEnt, NetEntity storageEnt, ItemStorageLocation location)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		ItemEnt = itemEnt;
		StorageEnt = storageEnt;
		Location = location;
	}
}
