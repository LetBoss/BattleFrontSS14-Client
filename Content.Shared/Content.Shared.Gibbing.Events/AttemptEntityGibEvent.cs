using Robust.Shared.GameObjects;

namespace Content.Shared.Gibbing.Events;

[ByRefEvent]
public record struct AttemptEntityGibEvent(EntityUid Target, int GibletCount, GibType GibType);
