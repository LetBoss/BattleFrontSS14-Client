using Robust.Shared.GameObjects;

namespace Content.Shared.Medical;

public sealed class SelfBeforeDefibrillatorZapsEvent : BeforeDefibrillatorZapsEvent
{
	public SelfBeforeDefibrillatorZapsEvent(EntityUid entityUsingDefib, EntityUid defib, EntityUid defibtarget)
		: base(entityUsingDefib, defib, defibtarget)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0002: Unknown result type (might be due to invalid IL or missing references)
	//IL_0003: Unknown result type (might be due to invalid IL or missing references)

}
