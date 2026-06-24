using Robust.Shared.GameObjects;

namespace Content.Shared.Clothing.EntitySystems;

[ByRefEvent]
public record struct StoppedBeingPilotedByClothing(EntityUid Clothing, EntityUid Pilot);
