using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory.Events;

public sealed class IsEquippingAttemptEvent : EquipAttemptBase
{
	public IsEquippingAttemptEvent(EntityUid equipee, EntityUid equipTarget, EntityUid equipment, SlotDefinition slotDefinition)
		: base(equipee, equipTarget, equipment, slotDefinition)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0002: Unknown result type (might be due to invalid IL or missing references)
	//IL_0003: Unknown result type (might be due to invalid IL or missing references)

}
