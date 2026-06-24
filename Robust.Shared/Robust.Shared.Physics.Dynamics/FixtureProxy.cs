using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Physics.Dynamics;

public sealed class FixtureProxy
{
	public EntityUid Entity;

	[ViewVariables]
	public Box2 AABB;

	[ViewVariables]
	public int ChildIndex;

	public string FixtureId;

	public Fixture Fixture;

	[ViewVariables]
	public DynamicTree.Proxy ProxyId = DynamicTree.Proxy.Free;

	public PhysicsComponent Body { get; internal set; }

	public TransformComponent Xform { get; internal set; }

	internal FixtureProxy(EntityUid uid, PhysicsComponent body, TransformComponent xform, Box2 aabb, string fixtureId, Fixture fixture, int childIndex)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Entity = uid;
		Body = body;
		Xform = xform;
		AABB = aabb;
		FixtureId = fixtureId;
		Fixture = fixture;
		ChildIndex = childIndex;
	}
}
