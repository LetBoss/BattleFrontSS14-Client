using Content.Shared.Climbing.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Climbing.Events;

public sealed class TargetBeforeClimbEvent : BeforeClimbEvent
{
	public TargetBeforeClimbEvent(EntityUid gettingPutOntable, EntityUid puttingOnTable, Entity<ClimbableComponent> beingClimbedOn)
		: base(gettingPutOntable, puttingOnTable, beingClimbedOn)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0002: Unknown result type (might be due to invalid IL or missing references)
	//IL_0003: Unknown result type (might be due to invalid IL or missing references)

}
