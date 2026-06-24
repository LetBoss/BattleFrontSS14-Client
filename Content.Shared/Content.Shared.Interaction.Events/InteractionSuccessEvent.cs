using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events;

[ByRefEvent]
public readonly record struct InteractionSuccessEvent(EntityUid User);
