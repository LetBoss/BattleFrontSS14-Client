using Robust.Shared.GameObjects;

namespace Content.Shared.Anomaly.Components;

[ByRefEvent]
public readonly record struct AnomalySupercriticalEvent(EntityUid Anomaly, float PowerModifier);
