using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Eye.Blinding.Systems;

public sealed class CanSeeAttemptEvent : CancellableEntityEventArgs, IInventoryRelayEvent
{
	public bool Blind => ((CancellableEntityEventArgs)this).Cancelled;

	public SlotFlags TargetSlots => SlotFlags.HEAD | SlotFlags.EYES | SlotFlags.MASK;
}
