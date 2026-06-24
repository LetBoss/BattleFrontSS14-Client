using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Flash;

[ByRefEvent]
public record struct FlashAttemptEvent(EntityUid Target, EntityUid? User, EntityUid? Used, bool Cancelled = false) : IInventoryRelayEvent
{
	SlotFlags IInventoryRelayEvent.TargetSlots => SlotFlags.HEAD | SlotFlags.EYES | SlotFlags.MASK;
}
