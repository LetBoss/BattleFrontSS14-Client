using Robust.Shared.GameObjects;

namespace Content.Shared.Containers;

[ByRefEvent]
public record struct BeforeThrowInsertEvent(EntityUid ThrownEntity, bool Cancelled = false);
