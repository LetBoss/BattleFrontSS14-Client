using Robust.Shared.GameObjects;

namespace Content.Shared.Clothing.EntitySystems;

[ByRefEvent]
public record struct StartedPilotingClothingEvent(EntityUid Clothing, EntityUid Wearer);
