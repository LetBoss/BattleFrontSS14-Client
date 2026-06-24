using Robust.Shared.GameObjects;

namespace Content.Shared.Item;

public sealed class GettingPickedUpAttemptEvent : BasePickupAttemptEvent
{
	public GettingPickedUpAttemptEvent(EntityUid user, EntityUid item)
		: base(user, item)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0002: Unknown result type (might be due to invalid IL or missing references)

}
