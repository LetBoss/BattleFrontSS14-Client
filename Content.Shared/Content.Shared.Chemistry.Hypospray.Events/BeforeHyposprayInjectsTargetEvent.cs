using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry.Hypospray.Events;

public abstract class BeforeHyposprayInjectsTargetEvent : CancellableEntityEventArgs, IInventoryRelayEvent
{
	public EntityUid EntityUsingHypospray;

	public readonly EntityUid Hypospray;

	public EntityUid TargetGettingInjected;

	public string? InjectMessageOverride;

	public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;

	public BeforeHyposprayInjectsTargetEvent(EntityUid user, EntityUid hypospray, EntityUid target)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		EntityUsingHypospray = user;
		Hypospray = hypospray;
		TargetGettingInjected = target;
		InjectMessageOverride = null;
	}
}
