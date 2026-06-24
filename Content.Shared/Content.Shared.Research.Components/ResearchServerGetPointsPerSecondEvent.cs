using Robust.Shared.GameObjects;

namespace Content.Shared.Research.Components;

[ByRefEvent]
public record struct ResearchServerGetPointsPerSecondEvent(EntityUid Server, int Points);
