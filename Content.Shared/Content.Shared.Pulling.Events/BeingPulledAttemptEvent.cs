using Robust.Shared.GameObjects;

namespace Content.Shared.Pulling.Events;

public sealed class BeingPulledAttemptEvent : CancellableEntityEventArgs
{
	public EntityUid Puller { get; }

	public EntityUid Pulled { get; }

	public BeingPulledAttemptEvent(EntityUid puller, EntityUid pulled)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Puller = puller;
		Pulled = pulled;
	}
}
