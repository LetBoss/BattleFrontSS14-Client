using Robust.Shared.GameObjects;

namespace Content.Shared.Anomaly.Components;

[ByRefEvent]
public readonly record struct AnomalyHealthChangedEvent(EntityUid Anomaly, float Health);
