using Robust.Shared.GameObjects;

namespace Content.Shared.Stacks;

[ByRefEvent]
public readonly record struct StackSplitEvent(EntityUid NewId);
