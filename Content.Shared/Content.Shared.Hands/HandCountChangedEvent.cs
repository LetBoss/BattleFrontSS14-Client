using Robust.Shared.GameObjects;

namespace Content.Shared.Hands;

public sealed class HandCountChangedEvent : EntityEventArgs
{
	public EntityUid Sender { get; }

	public HandCountChangedEvent(EntityUid sender)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Sender = sender;
	}
}
