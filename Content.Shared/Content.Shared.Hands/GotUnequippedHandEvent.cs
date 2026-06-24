using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Hands;

public sealed class GotUnequippedHandEvent : UnequippedHandEvent
{
	public GotUnequippedHandEvent(EntityUid user, EntityUid unequipped, Hand hand)
		: base(user, unequipped, hand)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0002: Unknown result type (might be due to invalid IL or missing references)

}
