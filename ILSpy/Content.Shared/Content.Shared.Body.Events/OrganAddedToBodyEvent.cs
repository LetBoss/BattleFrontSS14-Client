using Robust.Shared.GameObjects;

namespace Content.Shared.Body.Events;

[ByRefEvent]
public readonly record struct OrganAddedToBodyEvent(EntityUid Body, EntityUid Part);
