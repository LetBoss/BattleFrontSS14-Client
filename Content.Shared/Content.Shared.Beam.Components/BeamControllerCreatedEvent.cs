using Robust.Shared.GameObjects;

namespace Content.Shared.Beam.Components;

public sealed class BeamControllerCreatedEvent : EntityEventArgs
{
	public EntityUid OriginBeam;

	public EntityUid BeamControllerEntity;

	public BeamControllerCreatedEvent(EntityUid originBeam, EntityUid beamControllerEntity)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		OriginBeam = originBeam;
		BeamControllerEntity = beamControllerEntity;
	}
}
