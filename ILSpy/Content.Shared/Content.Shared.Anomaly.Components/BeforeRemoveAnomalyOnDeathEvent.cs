using Robust.Shared.GameObjects;

namespace Content.Shared.Anomaly.Components;

[ByRefEvent]
public record struct BeforeRemoveAnomalyOnDeathEvent(bool Cancelled = false);
