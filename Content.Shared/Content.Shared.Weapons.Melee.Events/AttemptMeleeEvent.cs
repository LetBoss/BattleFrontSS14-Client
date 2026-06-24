using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Melee.Events;

[ByRefEvent]
public record struct AttemptMeleeEvent(EntityUid User, bool Cancelled = false, string? Message = null);
