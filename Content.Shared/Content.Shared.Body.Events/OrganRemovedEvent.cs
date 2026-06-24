using Robust.Shared.GameObjects;

namespace Content.Shared.Body.Events;

[ByRefEvent]
public readonly record struct OrganRemovedEvent(EntityUid OldPart);
