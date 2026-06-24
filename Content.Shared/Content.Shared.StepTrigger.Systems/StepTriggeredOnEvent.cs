using Robust.Shared.GameObjects;

namespace Content.Shared.StepTrigger.Systems;

[ByRefEvent]
public readonly record struct StepTriggeredOnEvent(EntityUid Source, EntityUid Tripper);
