using Robust.Shared.GameObjects;

namespace Content.Shared.Actions.Events;

[ByRefEvent]
public record struct ActionSetTargetEvent(EntityUid Target, bool Handled = false);
