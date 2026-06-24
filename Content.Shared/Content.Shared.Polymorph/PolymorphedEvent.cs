using Robust.Shared.GameObjects;

namespace Content.Shared.Polymorph;

[ByRefEvent]
public record struct PolymorphedEvent(EntityUid OldEntity, EntityUid NewEntity, bool IsRevert);
