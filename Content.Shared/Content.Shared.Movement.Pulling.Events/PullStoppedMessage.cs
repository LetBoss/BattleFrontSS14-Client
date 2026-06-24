using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Pulling.Events;

public sealed class PullStoppedMessage : PullMessage
{
	public PullStoppedMessage(EntityUid pullerUid, EntityUid pulledUid)
		: base(pullerUid, pulledUid)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0002: Unknown result type (might be due to invalid IL or missing references)

}
