using Robust.Shared.GameObjects;

namespace Content.Shared.Anomaly.Components;

[ByRefEvent]
public readonly record struct AnomalySeverityChangedEvent(EntityUid Anomaly, float Stability, float Severity);
