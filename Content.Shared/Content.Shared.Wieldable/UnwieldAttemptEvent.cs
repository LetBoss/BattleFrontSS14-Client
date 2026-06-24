using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Wieldable;

[ByRefEvent]
public record struct UnwieldAttemptEvent(EntityUid User, EntityUid Wielded, bool Cancelled = false) : IInventoryRelayEvent
{
	SlotFlags IInventoryRelayEvent.TargetSlots => SlotFlags.WITHOUT_POCKET;

	public string? Message = null;

	public void Cancel()
	{
		Cancelled = true;
	}
}
