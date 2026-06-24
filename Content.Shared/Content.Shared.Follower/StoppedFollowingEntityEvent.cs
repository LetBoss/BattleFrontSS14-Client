using Robust.Shared.GameObjects;

namespace Content.Shared.Follower;

public sealed class StoppedFollowingEntityEvent : FollowEvent
{
	public StoppedFollowingEntityEvent(EntityUid following, EntityUid follower)
		: base(following, follower)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0002: Unknown result type (might be due to invalid IL or missing references)

}
