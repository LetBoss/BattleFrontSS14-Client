using Robust.Shared.GameObjects;

namespace Content.Shared.Camera;

[ByRefEvent]
public record struct GetEyeOffsetAttemptEvent(bool Cancelled);
