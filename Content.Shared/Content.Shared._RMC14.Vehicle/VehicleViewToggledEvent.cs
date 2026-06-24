using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleViewToggledEvent : EntityEventArgs
{
	public readonly bool IsOutside;

	public VehicleViewToggledEvent(bool isOutside)
	{
		IsOutside = isOutside;
	}
}
