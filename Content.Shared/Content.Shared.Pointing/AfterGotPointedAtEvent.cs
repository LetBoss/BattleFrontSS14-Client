using Robust.Shared.GameObjects;

namespace Content.Shared.Pointing;

[ByRefEvent]
public readonly record struct AfterGotPointedAtEvent(EntityUid Pointer);
