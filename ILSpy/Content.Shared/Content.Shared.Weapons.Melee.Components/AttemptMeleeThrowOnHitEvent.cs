using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Melee.Components;

[ByRefEvent]
public record struct AttemptMeleeThrowOnHitEvent(EntityUid Target, EntityUid? User, bool Cancelled = false, bool Handled = false);
