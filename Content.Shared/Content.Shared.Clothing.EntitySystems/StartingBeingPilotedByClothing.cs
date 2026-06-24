using Robust.Shared.GameObjects;

namespace Content.Shared.Clothing.EntitySystems;

[ByRefEvent]
public record struct StartingBeingPilotedByClothing(EntityUid Clothing, EntityUid Pilot);
