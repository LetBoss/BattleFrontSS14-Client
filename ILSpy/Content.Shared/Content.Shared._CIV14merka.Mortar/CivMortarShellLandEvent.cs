using Robust.Shared.Map;

namespace Content.Shared._CIV14merka.Mortar;

public struct CivMortarShellLandEvent(EntityCoordinates coordinates)
{
	public EntityCoordinates Coordinates = coordinates;

	public bool PiercesRoof = false;
}
