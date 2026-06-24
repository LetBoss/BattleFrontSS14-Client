using Robust.Shared.GameObjects;

namespace Content.Shared.Flash;

[ByRefEvent]
public record struct AfterFlashedEvent(EntityUid Target, EntityUid? User, EntityUid? Used, bool Melee);
