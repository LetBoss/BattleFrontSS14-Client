using Robust.Shared.Map;
using Robust.Shared.Network;

namespace Robust.Shared.Placement;

public readonly struct PlacementTileEvent(int tileType, EntityCoordinates coordinates, NetUserId? placerNetUserId)
{
	public readonly int TileType = tileType;

	public readonly EntityCoordinates Coordinates = coordinates;

	public readonly NetUserId? PlacerNetUserId = placerNetUserId;
}
