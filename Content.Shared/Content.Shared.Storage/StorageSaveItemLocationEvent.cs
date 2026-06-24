using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Storage;

[Serializable]
[NetSerializable]
public sealed class StorageSaveItemLocationEvent : EntityEventArgs
{
	public readonly NetEntity Item;

	public readonly NetEntity Storage;

	public StorageSaveItemLocationEvent(NetEntity item, NetEntity storage)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Item = item;
		Storage = storage;
	}
}
