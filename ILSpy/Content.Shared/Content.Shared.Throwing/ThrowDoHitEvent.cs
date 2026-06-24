using Robust.Shared.GameObjects;

namespace Content.Shared.Throwing;

public sealed class ThrowDoHitEvent : ThrowEvent
{
	public ThrowDoHitEvent(EntityUid thrown, EntityUid target, ThrownItemComponent component)
		: base(thrown, target, component)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0002: Unknown result type (might be due to invalid IL or missing references)

}
