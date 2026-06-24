using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Containers.ItemSlots;

[Serializable]
[NetSerializable]
public sealed class ItemSlotsComponentState : ComponentState
{
	public readonly Dictionary<string, ItemSlot> Slots;

	public ItemSlotsComponentState(Dictionary<string, ItemSlot> slots)
	{
		Slots = slots;
	}
}
