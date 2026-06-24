using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events;

[ByRefEvent]
public struct ConsciousAttemptEvent(EntityUid uid)
{
	public bool Cancelled = false;

	public readonly EntityUid Uid = uid;
}
