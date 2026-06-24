using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Events;

public sealed class UpdateCanMoveEvent : CancellableEntityEventArgs
{
	public EntityUid Uid { get; }

	public UpdateCanMoveEvent(EntityUid uid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Uid = uid;
	}
}
