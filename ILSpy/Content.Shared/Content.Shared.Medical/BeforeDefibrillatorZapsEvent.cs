using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Medical;

public abstract class BeforeDefibrillatorZapsEvent : CancellableEntityEventArgs, IInventoryRelayEvent
{
	public EntityUid EntityUsingDefib;

	public readonly EntityUid Defib;

	public EntityUid DefibTarget;

	public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;

	public BeforeDefibrillatorZapsEvent(EntityUid entityUsingDefib, EntityUid defib, EntityUid defibTarget)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		EntityUsingDefib = entityUsingDefib;
		Defib = defib;
		DefibTarget = defibTarget;
	}
}
