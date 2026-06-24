using Robust.Shared.GameObjects;

namespace Content.Shared.Paper;

[ByRefEvent]
public record struct PaperWriteAttemptEvent(EntityUid Paper, string? FailReason = null, bool Cancelled = false);
