using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory.Events;

[ByRefEvent]
public record struct RefreshEquipmentHudEvent<T>(SlotFlags TargetSlots) : IInventoryRelayEvent where T : IComponent
{
	public bool Active;

	public List<T> Components;
}
