using Robust.Shared.GameObjects;

namespace Content.Shared.Actions.Events;

[ByRefEvent]
public record struct ActionAttemptEvent(EntityUid User, bool Cancelled = false);
