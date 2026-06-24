using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Movement;

[ByRefEvent]
public record struct RMCMovementSpeedRefreshedEvent(float WalkModifier, float SprintModifier);
