using Robust.Shared.GameObjects;

namespace Content.Shared.Tools.Components;

[ByRefEvent]
public record struct AttemptSimpleToolUseEvent(EntityUid User, bool Cancelled = false);
