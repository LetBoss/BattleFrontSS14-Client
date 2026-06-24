using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events;

public sealed class UseAttemptEvent(EntityUid uid, EntityUid used) : CancellableEntityEventArgs
{
	public EntityUid Used = used;

	public EntityUid Uid { get; } = uid;
}
