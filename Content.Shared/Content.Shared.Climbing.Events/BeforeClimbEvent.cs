using Content.Shared.Climbing.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Climbing.Events;

public abstract class BeforeClimbEvent : CancellableEntityEventArgs
{
	public readonly EntityUid GettingPutOnTable;

	public readonly EntityUid PuttingOnTable;

	public readonly Entity<ClimbableComponent> BeingClimbedOn;

	public BeforeClimbEvent(EntityUid gettingPutOntable, EntityUid puttingOnTable, Entity<ClimbableComponent> beingClimbedOn)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		GettingPutOnTable = gettingPutOntable;
		PuttingOnTable = puttingOnTable;
		BeingClimbedOn = beingClimbedOn;
	}
}
