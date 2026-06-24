using Robust.Shared.GameObjects;

namespace Content.Shared.UserInterface;

public sealed class ActivatableUIOpenAttemptEvent : CancellableEntityEventArgs
{
	public EntityUid User { get; }

	public ActivatableUIOpenAttemptEvent(EntityUid who)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		User = who;
	}
}
