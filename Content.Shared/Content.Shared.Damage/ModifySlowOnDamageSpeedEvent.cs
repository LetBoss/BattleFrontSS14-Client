using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Damage;

[ByRefEvent]
public record struct ModifySlowOnDamageSpeedEvent(float Speed) : IInventoryRelayEvent
{
	public SlotFlags TargetSlots => SlotFlags.WITHOUT_POCKET;
}
