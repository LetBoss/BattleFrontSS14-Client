using System;
using System.Collections.Generic;
using Content.Shared.Climbing.Events;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

namespace Content.Shared._RMC14.Movement;

public sealed class RMCMovementSystem : EntitySystem
{
	[Dependency]
	private FixtureSystem _fixture;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private SharedPopupSystem _popup;

	private HashSet<EntityUid> _intersectedEntities = new HashSet<EntityUid>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCMobCollisionComponent, MapInitEvent>((EntityEventRefHandler<RMCMobCollisionComponent, MapInitEvent>)OnMobCollisionMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCMobCollisionComponent, MobStateChangedEvent>((EntityEventRefHandler<RMCMobCollisionComponent, MobStateChangedEvent>)OnMobCollisionMobStateChanged, (Type[])null, (Type[])null);
	}

	private void OnMobCollisionMapInit(Entity<RMCMobCollisionComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		MobCollisionComponent collision = default(MobCollisionComponent);
		if (((EntitySystem)this).TryComp<MobCollisionComponent>(Entity<RMCMobCollisionComponent>.op_Implicit(ent), ref collision))
		{
			collision.FixtureId = ent.Comp.FixtureId;
			((EntitySystem)this).DirtyField<MobCollisionComponent>(Entity<RMCMobCollisionComponent>.op_Implicit(ent), collision, "FixtureId", (MetaDataComponent)null);
		}
		PhysicsComponent body = default(PhysicsComponent);
		if (!_mobState.IsDead(Entity<RMCMobCollisionComponent>.op_Implicit(ent)) && ((EntitySystem)this).TryComp<PhysicsComponent>(Entity<RMCMobCollisionComponent>.op_Implicit(ent), ref body))
		{
			CreateMobCollisionFixture(Entity<RMCMobCollisionComponent, PhysicsComponent>.op_Implicit((Entity<RMCMobCollisionComponent>.op_Implicit(ent), Entity<RMCMobCollisionComponent>.op_Implicit(ent), body)));
		}
	}

	private void CreateMobCollisionFixture(Entity<RMCMobCollisionComponent, PhysicsComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<PhysicsComponent>(Entity<RMCMobCollisionComponent, PhysicsComponent>.op_Implicit(ent), ref ent.Comp2, false))
		{
			_fixture.TryCreateFixture(Entity<RMCMobCollisionComponent, PhysicsComponent>.op_Implicit(ent), ent.Comp1.FixtureShape, ent.Comp1.FixtureId, 1f, false, (int)ent.Comp1.FixtureLayer, (int)ent.Comp1.FixtureLayer, 0.4f, 0f, true, (FixturesComponent)null, Entity<RMCMobCollisionComponent, PhysicsComponent>.op_Implicit(ent), (TransformComponent)null);
		}
	}

	private void OnMobCollisionMobStateChanged(Entity<RMCMobCollisionComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		switch (args.NewMobState)
		{
		case MobState.Alive:
			CreateMobCollisionFixture(Entity<RMCMobCollisionComponent, PhysicsComponent>.op_Implicit(ent));
			break;
		case MobState.Dead:
			_fixture.DestroyFixture(Entity<RMCMobCollisionComponent>.op_Implicit(ent), ent.Comp.FixtureId, true, (PhysicsComponent)null, (FixturesComponent)null, (TransformComponent)null);
			break;
		}
	}

	public bool CanClimbOver(EntityUid? user, EntityUid movingEntity, EntityUid target, bool includeTarget = true, bool popup = true)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		if (!user.HasValue)
		{
			user = movingEntity;
		}
		MapCoordinates mapCoordinates = _transform.GetMapCoordinates(movingEntity, (TransformComponent)null);
		MapCoordinates targetMapCoords = _transform.GetMapCoordinates(target, (TransformComponent)null);
		Transform transform = default(Transform);
		((Transform)(ref transform))._002Ector(0f);
		EdgeShape line = new EdgeShape(mapCoordinates.Position, targetMapCoords.Position);
		_intersectedEntities.Clear();
		_lookup.GetEntitiesIntersecting<EdgeShape>(_transform.GetMapId(Entity<TransformComponent>.op_Implicit(movingEntity)), line, transform, _intersectedEntities, (LookupFlags)110);
		if (includeTarget)
		{
			_intersectedEntities.Add(target);
		}
		else
		{
			_intersectedEntities.Remove(target);
		}
		foreach (EntityUid entity in _intersectedEntities)
		{
			AttemptClimbEvent ev = new AttemptClimbEvent(user.Value, movingEntity, entity);
			((EntitySystem)this).RaiseLocalEvent<AttemptClimbEvent>(entity, ref ev, false);
			if (ev.Cancelled)
			{
				if (popup)
				{
					_popup.PopupClient(base.Loc.GetString("rmc-climb-prevented-by-obstacles"), user, PopupType.MediumCaution);
				}
				return false;
			}
		}
		return true;
	}
}
