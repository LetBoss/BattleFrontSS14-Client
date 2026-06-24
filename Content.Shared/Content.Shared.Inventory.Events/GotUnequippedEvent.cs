using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory.Events;

public sealed class GotUnequippedEvent : UnequippedEventBase
{
	public GotUnequippedEvent(EntityUid equipee, EntityUid equipment, SlotDefinition slotDefinition)
		: base(equipee, equipment, slotDefinition)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0002: Unknown result type (might be due to invalid IL or missing references)

}
