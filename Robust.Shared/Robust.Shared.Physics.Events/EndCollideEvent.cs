using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;

namespace Robust.Shared.Physics.Events;

[ByRefEvent]
public readonly struct EndCollideEvent(EntityUid ourEntity, EntityUid otherEntity, string ourFixtureId, string otherFixtureId, Fixture ourFixture, Fixture otherFixture, PhysicsComponent ourBody, PhysicsComponent otherBody)
{
	public readonly EntityUid OurEntity = ourEntity;

	public readonly EntityUid OtherEntity = otherEntity;

	public readonly PhysicsComponent OurBody = ourBody;

	public readonly PhysicsComponent OtherBody = otherBody;

	public readonly string OurFixtureId = ourFixtureId;

	public readonly string OtherFixtureId = otherFixtureId;

	public readonly Fixture OurFixture = ourFixture;

	public readonly Fixture OtherFixture = otherFixture;
}
