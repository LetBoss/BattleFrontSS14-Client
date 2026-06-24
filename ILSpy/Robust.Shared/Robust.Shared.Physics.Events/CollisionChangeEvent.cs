using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Components;

namespace Robust.Shared.Physics.Events;

[ByRefEvent]
public readonly struct CollisionChangeEvent(EntityUid bodyUid, PhysicsComponent body, bool canCollide)
{
	public readonly EntityUid BodyUid = bodyUid;

	public readonly PhysicsComponent Body = body;

	public readonly bool CanCollide = canCollide;
}
