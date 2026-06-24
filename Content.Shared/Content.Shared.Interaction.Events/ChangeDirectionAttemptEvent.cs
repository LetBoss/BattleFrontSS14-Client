using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events;

public sealed class ChangeDirectionAttemptEvent : CancellableEntityEventArgs
{
	public EntityUid Uid { get; }

	public ChangeDirectionAttemptEvent(EntityUid uid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Uid = uid;
	}
}
