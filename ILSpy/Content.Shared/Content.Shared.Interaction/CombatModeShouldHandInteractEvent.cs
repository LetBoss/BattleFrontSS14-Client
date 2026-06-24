using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction;

[ByRefEvent]
public record struct CombatModeShouldHandInteractEvent(EntityUid User, bool Cancelled = false);
