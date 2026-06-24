using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Containers.ItemSlots;

[Serializable]
[NetSerializable]
public sealed class ItemSlotButtonPressedEvent : BoundUserInterfaceMessage
{
	public string SlotId;

	public bool TryInsert;

	public bool TryEject;

	public ItemSlotButtonPressedEvent(string slotId, bool tryEject = true, bool tryInsert = true)
	{
		SlotId = slotId;
		TryEject = tryEject;
		TryInsert = tryInsert;
	}
}
