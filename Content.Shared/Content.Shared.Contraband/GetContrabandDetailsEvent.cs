using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Contraband;

[ByRefEvent]
public record struct GetContrabandDetailsEvent(bool CanShowContraband = false) : IInventoryRelayEvent
{
	SlotFlags IInventoryRelayEvent.TargetSlots => SlotFlags.EYES;
}
