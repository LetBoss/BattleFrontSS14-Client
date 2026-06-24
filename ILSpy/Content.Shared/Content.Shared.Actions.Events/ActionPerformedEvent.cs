using Robust.Shared.GameObjects;

namespace Content.Shared.Actions.Events;

[ByRefEvent]
public readonly record struct ActionPerformedEvent(EntityUid Performer);
