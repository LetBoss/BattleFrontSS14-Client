using Robust.Shared.GameObjects;

namespace Content.Shared.Electrocution;

public sealed class ElectrocutedEvent : EntityEventArgs
{
	public readonly EntityUid TargetUid;

	public readonly EntityUid? SourceUid;

	public readonly float SiemensCoefficient;

	public ElectrocutedEvent(EntityUid targetUid, EntityUid? sourceUid, float siemensCoefficient)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		TargetUid = targetUid;
		SourceUid = sourceUid;
		SiemensCoefficient = siemensCoefficient;
	}
}
