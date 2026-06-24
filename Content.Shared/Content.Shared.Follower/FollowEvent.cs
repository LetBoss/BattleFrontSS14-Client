using Robust.Shared.GameObjects;

namespace Content.Shared.Follower;

public abstract class FollowEvent : EntityEventArgs
{
	public EntityUid Following;

	public EntityUid Follower;

	protected FollowEvent(EntityUid following, EntityUid follower)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Following = following;
		Follower = follower;
	}
}
