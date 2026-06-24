using Robust.Shared.GameObjects;

namespace Content.Shared.Climbing.Events;

[ByRefEvent]
public readonly record struct ClimbedOnEvent(EntityUid Climber, EntityUid Instigator);
