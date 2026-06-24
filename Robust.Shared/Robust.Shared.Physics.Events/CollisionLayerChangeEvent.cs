using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Components;

namespace Robust.Shared.Physics.Events;

[ByRefEvent]
public readonly struct CollisionLayerChangeEvent(Entity<PhysicsComponent> body)
{
	public readonly Entity<PhysicsComponent> Body = body;
}
