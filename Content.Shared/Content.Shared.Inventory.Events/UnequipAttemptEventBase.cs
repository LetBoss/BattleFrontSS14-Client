using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory.Events;

public abstract class UnequipAttemptEventBase(EntityUid unequipee, EntityUid unEquipTarget, EntityUid equipment, SlotDefinition slotDefinition) : CancellableEntityEventArgs, IInventoryRelayEvent
{
	public readonly EntityUid Unequipee = unequipee;

	public readonly EntityUid UnEquipTarget = unEquipTarget;

	public readonly EntityUid Equipment = equipment;

	public readonly SlotFlags SlotFlags = slotDefinition.SlotFlags;

	public readonly string Slot = slotDefinition.Name;

	public string? Reason;

	public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;
}
