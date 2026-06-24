using System.Numerics;
using Content.Shared.Inventory;
using Content.Shared.Weapons.Reflect;
using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Ranged.Events;

[ByRefEvent]
public record struct HitScanReflectAttemptEvent(EntityUid? Shooter, EntityUid SourceItem, ReflectType Reflective, Vector2 Direction, bool Reflected) : IInventoryRelayEvent
{
	SlotFlags IInventoryRelayEvent.TargetSlots => SlotFlags.WITHOUT_POCKET;
}
