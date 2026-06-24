using Robust.Shared.GameObjects;

namespace Content.Shared.Actions.Events;

[ByRefEvent]
public record struct AttemptRelayActionComponentChangeEvent
{
	public EntityUid? Target;
}
