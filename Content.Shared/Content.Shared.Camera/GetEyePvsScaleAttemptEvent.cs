using Robust.Shared.GameObjects;

namespace Content.Shared.Camera;

[ByRefEvent]
public record struct GetEyePvsScaleAttemptEvent(bool Cancelled);
