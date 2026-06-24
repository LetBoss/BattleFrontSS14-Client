using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared.Interaction;

public sealed class AfterInteractEvent : InteractEvent
{
	public AfterInteractEvent(EntityUid user, EntityUid used, EntityUid? target, EntityCoordinates clickLocation, bool canReach)
		: base(user, used, target, clickLocation, canReach)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0002: Unknown result type (might be due to invalid IL or missing references)
	//IL_0004: Unknown result type (might be due to invalid IL or missing references)

}
