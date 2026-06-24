using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Mech;

[ByRefEvent]
public readonly record struct MechEquipmentRemovedEvent(EntityUid Mech)
{
	public readonly EntityUid Mech = Mech;

	[CompilerGenerated]
	public void Deconstruct(out EntityUid Mech)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Mech = this.Mech;
	}
}
