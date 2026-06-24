using Robust.Shared.GameObjects;

namespace Content.Shared.Anomaly.Components;

[ByRefEvent]
public readonly record struct AnomalyShutdownEvent(EntityUid Anomaly, bool Supercritical);
