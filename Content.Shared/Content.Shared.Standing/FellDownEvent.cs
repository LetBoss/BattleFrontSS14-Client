using Robust.Shared.GameObjects;

namespace Content.Shared.Standing;

public sealed class FellDownEvent : EntityEventArgs
{
	public EntityUid Uid { get; }

	public FellDownEvent(EntityUid uid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Uid = uid;
	}
}
