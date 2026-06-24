using Robust.Shared.GameObjects;

namespace Content.Shared.StepTrigger.Systems;

[ByRefEvent]
public readonly record struct StepTriggeredOffEvent(EntityUid Source, EntityUid Tripper);
