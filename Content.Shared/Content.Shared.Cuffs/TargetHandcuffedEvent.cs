using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Cuffs;

[ByRefEvent]
public record struct TargetHandcuffedEvent : IInventoryRelayEvent
{
	public SlotFlags TargetSlots { get; set; }
}
