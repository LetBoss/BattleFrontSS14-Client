using Robust.Shared.GameObjects;

namespace Content.Shared.Paper;

[ByRefEvent]
public record struct PaperWriteEvent(EntityUid User, EntityUid Paper);
