using Robust.Shared.GameObjects;

namespace Content.Shared.Nutrition.EntitySystems;

[ByRefEvent]
public record struct ShakeEvent(EntityUid? Shaker);
