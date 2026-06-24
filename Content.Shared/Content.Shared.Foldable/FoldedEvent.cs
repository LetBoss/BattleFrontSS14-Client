using Robust.Shared.GameObjects;

namespace Content.Shared.Foldable;

[ByRefEvent]
public readonly record struct FoldedEvent(bool IsFolded);
