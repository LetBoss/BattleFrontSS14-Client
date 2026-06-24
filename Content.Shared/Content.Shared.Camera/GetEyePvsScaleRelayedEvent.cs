using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Camera;

[ByRefEvent]
public sealed class GetEyePvsScaleRelayedEvent : EntityEventArgs, IInventoryRelayEvent
{
	public float Scale;

	public SlotFlags TargetSlots { get; } = SlotFlags.All;
}
