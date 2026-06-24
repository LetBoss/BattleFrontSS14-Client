using Robust.Shared.GameObjects;

namespace Content.Shared.Beam.Components;

public sealed class BeamFiredEvent : EntityEventArgs
{
	public readonly EntityUid CreatedBeam;

	public BeamFiredEvent(EntityUid createdBeam)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		CreatedBeam = createdBeam;
	}
}
