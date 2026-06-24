using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Damage.Events;

[ByRefEvent]
public record struct BeforeStaminaDamageEvent(float Value, bool Cancelled = false) : IInventoryRelayEvent
{
	SlotFlags IInventoryRelayEvent.TargetSlots => SlotFlags.WITHOUT_POCKET;
}
