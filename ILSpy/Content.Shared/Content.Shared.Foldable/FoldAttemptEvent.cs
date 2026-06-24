using Robust.Shared.GameObjects;

namespace Content.Shared.Foldable;

[ByRefEvent]
public record struct FoldAttemptEvent(FoldableComponent Comp, bool Cancelled = false);
