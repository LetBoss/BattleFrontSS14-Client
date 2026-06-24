using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Forensics;

public sealed class TryAccessFingerprintEvent : CancellableEntityEventArgs, IInventoryRelayEvent
{
	public EntityUid? Blocker;

	SlotFlags IInventoryRelayEvent.TargetSlots => SlotFlags.WITHOUT_POCKET;
}
