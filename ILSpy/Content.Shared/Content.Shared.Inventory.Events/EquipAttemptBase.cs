using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory.Events;

public abstract class EquipAttemptBase(EntityUid equipee, EntityUid equipTarget, EntityUid equipment, SlotDefinition slotDefinition) : CancellableEntityEventArgs, IInventoryRelayEvent
{
	public readonly EntityUid Equipee = equipee;

	public readonly EntityUid EquipTarget = equipTarget;

	public readonly EntityUid Equipment = equipment;

	public readonly SlotFlags SlotFlags = slotDefinition.SlotFlags;

	public readonly string Slot = slotDefinition.Name;

	public string? Reason;

	public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;
}
