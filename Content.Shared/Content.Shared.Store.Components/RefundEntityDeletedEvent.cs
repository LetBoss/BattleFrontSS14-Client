using Robust.Shared.GameObjects;

namespace Content.Shared.Store.Components;

[ByRefEvent]
public readonly struct RefundEntityDeletedEvent
{
	public EntityUid Uid { get; }

	public RefundEntityDeletedEvent(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Uid = uid;
	}
}
