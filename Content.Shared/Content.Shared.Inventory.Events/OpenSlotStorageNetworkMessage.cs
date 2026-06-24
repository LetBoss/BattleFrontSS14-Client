using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Inventory.Events;

[Serializable]
[NetSerializable]
public sealed class OpenSlotStorageNetworkMessage : EntityEventArgs
{
	public readonly string Slot;

	public OpenSlotStorageNetworkMessage(string slot)
	{
		Slot = slot;
	}
}
