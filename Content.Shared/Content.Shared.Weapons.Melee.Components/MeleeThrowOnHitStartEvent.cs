using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Melee.Components;

[ByRefEvent]
public record struct MeleeThrowOnHitStartEvent(EntityUid Weapon, EntityUid? User);
