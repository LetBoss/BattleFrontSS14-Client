using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Bed.Cryostorage;

[Serializable]
[NetSerializable]
public sealed class CryostorageRemoveItemBuiMessage : BoundUserInterfaceMessage
{
	public enum RemovalType : byte
	{
		Hand,
		Inventory
	}

	public NetEntity StoredEntity;

	public string Key;

	public RemovalType Type;

	public CryostorageRemoveItemBuiMessage(NetEntity storedEntity, string key, RemovalType type)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		StoredEntity = storedEntity;
		Key = key;
		Type = type;
	}
}
