using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.IdentityManagement.Components;

public sealed class SeeIdentityAttemptEvent : CancellableEntityEventArgs, IInventoryRelayEvent
{
	public IdentityBlockerCoverage TotalCoverage;

	public SlotFlags TargetSlots => SlotFlags.HEAD | SlotFlags.EYES | SlotFlags.MASK | SlotFlags.OUTERCLOTHING;
}
