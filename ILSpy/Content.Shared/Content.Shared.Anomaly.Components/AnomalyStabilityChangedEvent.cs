using Robust.Shared.GameObjects;

namespace Content.Shared.Anomaly.Components;

[ByRefEvent]
public readonly record struct AnomalyStabilityChangedEvent(EntityUid Anomaly, float Stability, float Severity);
