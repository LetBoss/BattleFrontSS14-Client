using Content.Shared.Actions.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Actions;

[ByRefEvent]
public readonly struct ActionAddedEvent(EntityUid action, ActionComponent component)
{
	public readonly EntityUid Action = action;

	public readonly ActionComponent Component = component;
}
