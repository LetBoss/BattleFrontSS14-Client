using Robust.Shared.GameObjects;

namespace Content.Shared.Actions.Events;

[ByRefEvent]
public record struct ActionGetEventEvent(BaseActionEvent? Event = null);
