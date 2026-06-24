using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory.Events;

public sealed class IsUnequippingAttemptEvent : UnequipAttemptEventBase
{
	public IsUnequippingAttemptEvent(EntityUid unequipee, EntityUid unEquipTarget, EntityUid equipment, SlotDefinition slotDefinition)
		: base(unequipee, unEquipTarget, equipment, slotDefinition)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0002: Unknown result type (might be due to invalid IL or missing references)
	//IL_0003: Unknown result type (might be due to invalid IL or missing references)

}
