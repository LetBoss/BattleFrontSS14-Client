using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Pulling.Events;

public sealed class PullAttemptEvent : PullMessage
{
	public bool Cancelled { get; set; }

	public PullAttemptEvent(EntityUid pullerUid, EntityUid pullableUid)
		: base(pullerUid, pullableUid)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0002: Unknown result type (might be due to invalid IL or missing references)

}
