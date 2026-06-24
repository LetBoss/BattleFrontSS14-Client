using Robust.Shared.GameObjects;

namespace Content.Shared.Actions.Events;

[ByRefEvent]
public record struct DisarmAttemptEvent
{
	public readonly EntityUid TargetUid;

	public readonly EntityUid DisarmerUid;

	public readonly EntityUid? TargetItemInHandUid;

	public bool Cancelled;

	public DisarmAttemptEvent(EntityUid targetUid, EntityUid disarmerUid, EntityUid? targetItemInHandUid = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Cancelled = false;
		TargetUid = targetUid;
		DisarmerUid = disarmerUid;
		TargetItemInHandUid = targetItemInHandUid;
	}
}
