using Robust.Shared.GameObjects;

namespace Content.Shared.Hands;

public sealed class VirtualItemDeletedEvent : EntityEventArgs
{
	public EntityUid BlockingEntity;

	public EntityUid User;

	public VirtualItemDeletedEvent(EntityUid blockingEntity, EntityUid user)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		BlockingEntity = blockingEntity;
		User = user;
	}
}
