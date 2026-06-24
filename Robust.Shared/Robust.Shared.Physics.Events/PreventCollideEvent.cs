using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;

namespace Robust.Shared.Physics.Events;

[ByRefEvent]
public struct PreventCollideEvent(EntityUid ourEntity, EntityUid otherEntity, PhysicsComponent ourBody, PhysicsComponent otherBody, Fixture ourFixture, Fixture otherFixture)
{
	public readonly EntityUid OurEntity = ourEntity;

	public readonly EntityUid OtherEntity = otherEntity;

	public readonly PhysicsComponent OurBody = ourBody;

	public readonly PhysicsComponent OtherBody = otherBody;

	public readonly Fixture OurFixture = ourFixture;

	public readonly Fixture OtherFixture = otherFixture;

	public bool Cancelled = false;
}
