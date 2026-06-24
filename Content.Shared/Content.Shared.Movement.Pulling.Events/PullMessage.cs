using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Pulling.Events;

public abstract class PullMessage : EntityEventArgs
{
	public readonly EntityUid PullerUid;

	public readonly EntityUid PulledUid;

	protected PullMessage(EntityUid pullerUid, EntityUid pulledUid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		PullerUid = pullerUid;
		PulledUid = pulledUid;
	}
}
