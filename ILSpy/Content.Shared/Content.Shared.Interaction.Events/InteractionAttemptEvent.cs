using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events;

[ByRefEvent]
public struct InteractionAttemptEvent(EntityUid uid, EntityUid? target)
{
	public bool Cancelled = false;

	public readonly EntityUid Uid = uid;

	public readonly EntityUid? Target = target;
}
