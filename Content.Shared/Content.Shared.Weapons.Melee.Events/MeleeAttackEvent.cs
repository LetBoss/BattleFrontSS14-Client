using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Melee.Events;

[ByRefEvent]
public record struct MeleeAttackEvent(EntityUid Weapon);
