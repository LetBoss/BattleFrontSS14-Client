using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry.Hypospray.Events;

public sealed class SelfBeforeHyposprayInjectsEvent : BeforeHyposprayInjectsTargetEvent
{
	public SelfBeforeHyposprayInjectsEvent(EntityUid user, EntityUid hypospray, EntityUid target)
		: base(user, hypospray, target)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0002: Unknown result type (might be due to invalid IL or missing references)
	//IL_0003: Unknown result type (might be due to invalid IL or missing references)

}
