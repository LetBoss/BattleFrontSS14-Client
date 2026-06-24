using Content.Shared.Anomaly.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared.Anomaly.Components;

[ByRefEvent]
public readonly record struct AnomalyBehaviorChangedEvent(EntityUid Anomaly, ProtoId<AnomalyBehaviorPrototype>? Old, ProtoId<AnomalyBehaviorPrototype>? New);
