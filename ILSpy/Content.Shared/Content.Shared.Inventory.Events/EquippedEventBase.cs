using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory.Events;

public abstract class EquippedEventBase : EntityEventArgs
{
	public readonly EntityUid Equipee;

	public readonly EntityUid Equipment;

	public readonly string Slot;

	public readonly string SlotGroup;

	public readonly SlotFlags SlotFlags;

	public EquippedEventBase(EntityUid equipee, EntityUid equipment, SlotDefinition slotDefinition)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Equipee = equipee;
		Equipment = equipment;
		Slot = slotDefinition.Name;
		SlotGroup = slotDefinition.SlotGroup;
		SlotFlags = slotDefinition.SlotFlags;
	}
}
