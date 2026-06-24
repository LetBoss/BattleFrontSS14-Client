using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Events;

[ByRefEvent]
public readonly struct ContainerRelayMovementEntityEvent(EntityUid entity)
{
	public readonly EntityUid Entity = entity;
}
