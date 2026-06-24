using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Electrocution;

public sealed class ElectrocutionAttemptEvent : CancellableEntityEventArgs, IInventoryRelayEvent
{
	public readonly EntityUid TargetUid;

	public readonly EntityUid? SourceUid;

	public float SiemensCoefficient = 1f;

	public SlotFlags TargetSlots { get; }

	public ElectrocutionAttemptEvent(EntityUid targetUid, EntityUid? sourceUid, float siemensCoefficient, SlotFlags targetSlots)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		TargetUid = targetUid;
		TargetSlots = targetSlots;
		SourceUid = sourceUid;
		SiemensCoefficient = siemensCoefficient;
	}
}
