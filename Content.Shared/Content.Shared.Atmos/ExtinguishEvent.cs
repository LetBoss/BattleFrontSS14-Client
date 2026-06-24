using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Atmos;

[ByRefEvent]
public struct ExtinguishEvent : IInventoryRelayEvent
{
	public float FireStacksAdjustment;

	SlotFlags IInventoryRelayEvent.TargetSlots => SlotFlags.WITHOUT_POCKET;
}
