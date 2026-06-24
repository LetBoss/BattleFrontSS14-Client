using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Zombies;

public sealed class ZombificationResistanceQueryEvent : EntityEventArgs, IInventoryRelayEvent
{
	public float TotalCoefficient = 1f;

	public SlotFlags TargetSlots { get; }

	public ZombificationResistanceQueryEvent(SlotFlags slots)
	{
		TargetSlots = slots;
	}
}
