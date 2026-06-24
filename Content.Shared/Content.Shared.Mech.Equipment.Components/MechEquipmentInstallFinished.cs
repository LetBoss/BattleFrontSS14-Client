using Robust.Shared.GameObjects;

namespace Content.Shared.Mech.Equipment.Components;

public sealed class MechEquipmentInstallFinished : EntityEventArgs
{
	public EntityUid Mech;

	public MechEquipmentInstallFinished(EntityUid mech)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Mech = mech;
	}
}
