using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Network;

namespace Robust.Shared.Placement;

public readonly struct PlacementEntityEvent(EntityUid editedEntity, EntityCoordinates coordinates, PlacementEventAction placementEventAction, NetUserId? placerNetUserId)
{
	public readonly EntityUid EditedEntity = editedEntity;

	public readonly EntityCoordinates Coordinates = coordinates;

	public readonly PlacementEventAction PlacementEventAction = placementEventAction;

	public readonly NetUserId? PlacerNetUserId = placerNetUserId;
}
