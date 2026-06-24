using System.Numerics;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Camera;

[ByRefEvent]
public sealed class GetEyeOffsetRelayedEvent : EntityEventArgs, IInventoryRelayEvent
{
	public Vector2 Offset;

	public SlotFlags TargetSlots { get; } = SlotFlags.All;
}
