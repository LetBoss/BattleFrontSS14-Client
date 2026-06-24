using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Slippery;

public sealed class SlipAttemptEvent : EntityEventArgs, IInventoryRelayEvent
{
	public bool NoSlip;

	public bool SlowOverSlippery;

	public EntityUid? SlipCausingEntity;

	public SlotFlags TargetSlots { get; } = SlotFlags.FEET;

	public SlipAttemptEvent(EntityUid? slipCausingEntity)
	{
		SlipCausingEntity = slipCausingEntity;
	}
}
