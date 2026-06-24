using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry;

public sealed class SolutionScanEvent : EntityEventArgs, IInventoryRelayEvent
{
	public bool CanScan;

	public SlotFlags TargetSlots { get; } = SlotFlags.EYES;
}
