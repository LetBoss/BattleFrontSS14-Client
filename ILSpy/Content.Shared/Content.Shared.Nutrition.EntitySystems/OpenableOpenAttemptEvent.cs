using Robust.Shared.GameObjects;

namespace Content.Shared.Nutrition.EntitySystems;

[ByRefEvent]
public record struct OpenableOpenAttemptEvent(EntityUid? User, bool Cancelled = false);
