using Robust.Shared.GameObjects;

namespace Content.Shared.CombatMode.Pacification;

[ByRefEvent]
public record struct AttemptPacifiedAttackEvent(EntityUid User, bool Cancelled = false, string Reason = "pacified-cannot-harm-directly");
