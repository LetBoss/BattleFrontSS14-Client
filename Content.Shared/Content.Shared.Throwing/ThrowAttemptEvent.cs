using Robust.Shared.GameObjects;

namespace Content.Shared.Throwing;

public sealed class ThrowAttemptEvent : CancellableEntityEventArgs
{
	public EntityUid Uid { get; }

	public EntityUid ItemUid { get; }

	public ThrowAttemptEvent(EntityUid uid, EntityUid itemUid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Uid = uid;
		ItemUid = itemUid;
	}
}
