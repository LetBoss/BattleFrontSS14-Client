using Robust.Shared.GameObjects;

namespace Content.Shared.Mech;

[ByRefEvent]
public record struct AttemptRemoveMechEquipmentEvent()
{
	public bool Cancelled = false;
}
