using Robust.Shared.GameObjects;

namespace Content.Shared.Anomaly.Components;

[ByRefEvent]
public readonly record struct AnomalyPulseEvent(EntityUid Anomaly, float Stability, float Severity, float PowerModifier);
