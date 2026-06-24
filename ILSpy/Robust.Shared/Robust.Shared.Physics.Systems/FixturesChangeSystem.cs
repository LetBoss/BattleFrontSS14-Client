using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;

namespace Robust.Shared.Physics.Systems;

public sealed class FixturesChangeSystem : EntitySystem
{
	[Dependency]
	private readonly FixtureSystem _fixtures;

	[Dependency]
	private readonly SharedPhysicsSystem _physics;

	private EntityQuery<FixturesComponent> _fixturesQuery;

	private EntityQuery<PhysicsComponent> _physicsQuery;

	public override void Initialize()
	{
		base.Initialize();
		_fixturesQuery = GetEntityQuery<FixturesComponent>();
		_physicsQuery = GetEntityQuery<PhysicsComponent>();
		SubscribeLocalEvent<FixturesChangeComponent, ComponentStartup>(OnChangeStartup);
		SubscribeLocalEvent<FixturesChangeComponent, ComponentShutdown>(OnChangeShutdown);
	}

	private void OnChangeStartup(Entity<FixturesChangeComponent> ent, ref ComponentStartup args)
	{
		if (!_physicsQuery.TryComp(ent, out PhysicsComponent component) || !_fixturesQuery.TryComp(ent, out FixturesComponent component2))
		{
			return;
		}
		foreach (var (id, fixture2) in ent.Comp.Fixtures)
		{
			_fixtures.TryCreateFixture(ent.Owner, fixture2.Shape, id, fixture2.Density, fixture2.Hard, fixture2.CollisionLayer, fixture2.CollisionMask, fixture2.Friction, fixture2.Restitution, updates: true, component2, component);
		}
		_physics.WakeBody(ent.Owner, force: false, component2, component);
	}

	private void OnChangeShutdown(Entity<FixturesChangeComponent> ent, ref ComponentShutdown args)
	{
		foreach (string key in ent.Comp.Fixtures.Keys)
		{
			_fixtures.DestroyFixture(ent.Owner, key);
		}
	}
}
