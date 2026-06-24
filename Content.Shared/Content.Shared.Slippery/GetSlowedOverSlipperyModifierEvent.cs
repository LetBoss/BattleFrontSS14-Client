using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Slippery;

[ByRefEvent]
public record struct GetSlowedOverSlipperyModifierEvent() : IInventoryRelayEvent
{
	SlotFlags IInventoryRelayEvent.TargetSlots => SlotFlags.WITHOUT_POCKET;

	public float SlowdownModifier = 1f;
}
