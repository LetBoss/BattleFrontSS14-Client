using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events;

[ByRefEvent]
public record struct GettingAttackedAttemptEvent(EntityUid Attacker, EntityUid? Weapon, bool Disarm, bool Cancelled = false);
