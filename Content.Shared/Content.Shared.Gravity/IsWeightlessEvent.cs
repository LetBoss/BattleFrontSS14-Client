using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Gravity;

[ByRefEvent]
public record struct IsWeightlessEvent(EntityUid Entity, bool IsWeightless = false, bool Handled = false) : IInventoryRelayEvent
{
	SlotFlags IInventoryRelayEvent.TargetSlots => SlotFlags.WITHOUT_POCKET;
}
