using Robust.Shared.GameObjects;

namespace Content.Shared.Research.Components;

[ByRefEvent]
public readonly record struct ResearchServerPointsChangedEvent(EntityUid Server, int Total, int Delta);
