using Robust.Shared.GameObjects;

namespace Content.Shared.Actions.Events;

[ByRefEvent]
public record struct ActionSetEventEvent(BaseActionEvent Event, bool Handled = false);
