using Robust.Shared.GameObjects;

namespace Content.Shared.Clothing.EntitySystems;

[ByRefEvent]
public record struct StoppedPilotingClothingEvent(EntityUid Clothing, EntityUid Wearer);
