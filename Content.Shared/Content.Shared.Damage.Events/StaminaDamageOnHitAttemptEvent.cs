using Robust.Shared.GameObjects;

namespace Content.Shared.Damage.Events;

[ByRefEvent]
public record struct StaminaDamageOnHitAttemptEvent(bool Cancelled);
