using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Magic.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;

namespace Content.Shared.Magic.Systems;

public sealed class AnimateSpellSystem : EntitySystem
{
	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedContainerSystem _container;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<AnimateComponent, MapInitEvent>((EntityEventRefHandler<AnimateComponent, MapInitEvent>)OnAnimate, (Type[])null, (Type[])null);
	}

	private void OnAnimate(Entity<AnimateComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		FixturesComponent fixtures = default(FixturesComponent);
		PhysicsComponent physics = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<FixturesComponent>(Entity<AnimateComponent>.op_Implicit(ent), ref fixtures) && ((EntitySystem)this).TryComp<PhysicsComponent>(Entity<AnimateComponent>.op_Implicit(ent), ref physics))
		{
			TransformComponent xform = ((EntitySystem)this).Transform(Entity<AnimateComponent>.op_Implicit(ent));
			KeyValuePair<string, Fixture> fixture = fixtures.Fixtures.First();
			_transform.Unanchor(Entity<AnimateComponent>.op_Implicit(ent));
			_physics.SetCanCollide(Entity<AnimateComponent>.op_Implicit(ent), true, true, false, fixtures, physics);
			_physics.SetCollisionMask(Entity<AnimateComponent>.op_Implicit(ent), fixture.Key, fixture.Value, 10, fixtures, physics);
			_physics.SetCollisionLayer(Entity<AnimateComponent>.op_Implicit(ent), fixture.Key, fixture.Value, 65, fixtures, physics);
			_physics.SetBodyType(Entity<AnimateComponent>.op_Implicit(ent), (BodyType)2, fixtures, physics, xform);
			_physics.SetBodyStatus(Entity<AnimateComponent>.op_Implicit(ent), physics, (BodyStatus)1, true);
			_physics.SetFixedRotation(Entity<AnimateComponent>.op_Implicit(ent), false, true, fixtures, physics);
			_physics.SetHard(Entity<AnimateComponent>.op_Implicit(ent), fixture.Value, true, fixtures);
			_container.AttachParentToContainerOrGrid(Entity<TransformComponent>.op_Implicit((Entity<AnimateComponent>.op_Implicit(ent), xform)));
			AnimateSpellEvent ev = default(AnimateSpellEvent);
			((EntitySystem)this).RaiseLocalEvent<AnimateSpellEvent>(Entity<AnimateComponent>.op_Implicit(ent), ref ev, false);
		}
	}
}
