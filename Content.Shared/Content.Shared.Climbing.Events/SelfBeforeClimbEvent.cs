using Content.Shared.Climbing.Components;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Climbing.Events;

public sealed class SelfBeforeClimbEvent : BeforeClimbEvent, IInventoryRelayEvent
{
	public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;

	public SelfBeforeClimbEvent(EntityUid gettingPutOntable, EntityUid puttingOnTable, Entity<ClimbableComponent> beingClimbedOn)
		: base(gettingPutOntable, puttingOnTable, beingClimbedOn)
	{
	}//IL_000c: Unknown result type (might be due to invalid IL or missing references)
	//IL_000d: Unknown result type (might be due to invalid IL or missing references)
	//IL_000e: Unknown result type (might be due to invalid IL or missing references)

}
