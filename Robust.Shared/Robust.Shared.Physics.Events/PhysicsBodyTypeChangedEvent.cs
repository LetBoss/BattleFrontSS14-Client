using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Components;

namespace Robust.Shared.Physics.Events;

[ByRefEvent]
public readonly struct PhysicsBodyTypeChangedEvent(EntityUid entity, BodyType newType, BodyType oldType, PhysicsComponent component)
{
	public readonly EntityUid Entity = entity;

	public readonly BodyType New = newType;

	public readonly BodyType Old = oldType;

	public readonly PhysicsComponent Component = component;
}
