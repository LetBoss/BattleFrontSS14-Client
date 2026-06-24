using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events;

[ByRefEvent]
public readonly record struct InteractionFailureEvent(EntityUid User);
