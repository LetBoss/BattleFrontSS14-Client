using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Targeting;

[ByRefEvent]
public record struct TargetingStartedEvent(DirectionTargetedEffects DirectionEffect, TargetedEffects TargetedEffect, EntityUid Target);
