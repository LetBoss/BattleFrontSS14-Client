using Robust.Shared.GameObjects;

namespace Content.Shared.Research.Components;

[ByRefEvent]
public readonly record struct ResearchRegistrationChangedEvent(EntityUid? Server);
