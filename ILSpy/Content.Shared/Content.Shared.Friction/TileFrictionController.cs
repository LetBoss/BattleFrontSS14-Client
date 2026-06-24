using System;
using System.Numerics;
using Content.Shared.CCVar;
using Content.Shared.Gravity;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Controllers;

namespace Content.Shared.Friction;

public sealed class TileFrictionController : VirtualController
{
	[Dependency]
	private IConfigurationManager _configManager;

	[Dependency]
	private ITileDefinitionManager _tileDefinitionManager;

	[Dependency]
	private SharedGravitySystem _gravity;

	[Dependency]
	private SharedMoverController _mover;

	[Dependency]
	private SharedMapSystem _map;

	private EntityQuery<TileFrictionModifierComponent> _frictionQuery;

	private EntityQuery<TransformComponent> _xformQuery;

	private EntityQuery<PullerComponent> _pullerQuery;

	private EntityQuery<PullableComponent> _pullableQuery;

	private EntityQuery<MapGridComponent> _gridQuery;

	private float _frictionModifier;

	private float _minDamping;

	private float _airDamping;

	private float _offGridDamping;

	public override void Initialize()
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		((VirtualController)this).Initialize();
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _configManager, CCVars.TileFrictionModifier, (Action<float>)delegate(float value)
		{
			_frictionModifier = value;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _configManager, CCVars.MinFriction, (Action<float>)delegate(float value)
		{
			_minDamping = value;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _configManager, CCVars.AirFriction, (Action<float>)delegate(float value)
		{
			_airDamping = value;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _configManager, CCVars.OffgridFriction, (Action<float>)delegate(float value)
		{
			_offGridDamping = value;
		}, true);
		_frictionQuery = ((EntitySystem)this).GetEntityQuery<TileFrictionModifierComponent>();
		_xformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		_pullerQuery = ((EntitySystem)this).GetEntityQuery<PullerComponent>();
		_pullableQuery = ((EntitySystem)this).GetEntityQuery<PullableComponent>();
		_gridQuery = ((EntitySystem)this).GetEntityQuery<MapGridComponent>();
	}

	public override void UpdateBeforeSolve(bool prediction, float frameTime)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Invalid comparison between Unknown and I4
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Invalid comparison between Unknown and I4
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		((VirtualController)this).UpdateBeforeSolve(prediction, frameTime);
		TileFrictionModifierComponent frictionComp = default(TileFrictionModifierComponent);
		PullerComponent puller = default(PullerComponent);
		PullableComponent pullable = default(PullableComponent);
		foreach (Entity<PhysicsComponent, TransformComponent> ent in base.PhysicsSystem.AwakeBodies)
		{
			EntityUid uid = ent.Owner;
			PhysicsComponent body = ent.Comp1;
			if ((prediction && !body.Predict) || _mover.UseMobMovement(uid) || (body.LinearVelocity.Equals(Vector2.Zero) && body.AngularVelocity.Equals(0f)))
			{
				continue;
			}
			TransformComponent xform = ent.Comp2;
			float friction;
			if ((int)body.BodyStatus != 1 && !_gravity.IsWeightless(uid, body, xform))
			{
				EntityCoordinates coordinates = xform.Coordinates;
				if (((EntityCoordinates)(ref coordinates)).IsValid((IEntityManager)(object)((EntitySystem)this).EntityManager))
				{
					friction = _frictionModifier * GetTileFriction(uid, body, xform);
					goto IL_00ff;
				}
			}
			friction = ((!xform.GridUid.HasValue || !_gridQuery.HasComp(xform.GridUid)) ? _offGridDamping : _airDamping);
			goto IL_00ff;
			IL_00ff:
			float bodyModifier = 1f;
			if (_frictionQuery.TryGetComponent(uid, ref frictionComp))
			{
				bodyModifier = frictionComp.Modifier;
			}
			TileFrictionEvent ev = new TileFrictionEvent(bodyModifier);
			((EntitySystem)this).RaiseLocalEvent<TileFrictionEvent>(uid, ref ev, false);
			bodyModifier = ev.Modifier;
			if (_pullerQuery.TryGetComponent(uid, ref puller) && puller.Pulling.HasValue && _pullableQuery.TryGetComponent(uid, ref pullable) && pullable.BeingPulled)
			{
				bodyModifier *= 0.2f;
			}
			friction *= bodyModifier;
			friction = Math.Max(_minDamping, friction);
			base.PhysicsSystem.SetLinearDamping(uid, body, friction, true);
			base.PhysicsSystem.SetAngularDamping(uid, body, friction, true);
			if ((int)body.BodyType == 2)
			{
				Vector2 velocity = body.LinearVelocity;
				float angVelocity = body.AngularVelocity;
				_mover.Friction(0f, frameTime, friction, ref velocity);
				_mover.Friction(0f, frameTime, friction, ref angVelocity);
				base.PhysicsSystem.SetLinearVelocity(uid, velocity, true, true, (FixturesComponent)null, body);
				base.PhysicsSystem.SetAngularVelocity(uid, angVelocity, true, (FixturesComponent)null, body);
			}
		}
	}

	private float GetTileFriction(EntityUid uid, PhysicsComponent body, TransformComponent xform)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		float tileModifier = 1f;
		MapGridComponent grid = default(MapGridComponent);
		if (!_gridQuery.TryGetComponent(xform.GridUid, ref grid))
		{
			TileFrictionModifierComponent friction = default(TileFrictionModifierComponent);
			if (!_frictionQuery.TryGetComponent(xform.MapUid, ref friction))
			{
				return tileModifier;
			}
			return friction.Modifier;
		}
		TileRef tile = _map.GetTileRef(xform.GridUid.Value, grid, xform.Coordinates);
		GravityComponent gravity = default(GravityComponent);
		if (((Tile)(ref tile.Tile)).IsEmpty && ((EntitySystem)this).HasComp<MapComponent>(xform.GridUid) && (!((EntitySystem)this).TryComp<GravityComponent>(xform.GridUid, ref gravity) || gravity.Enabled))
		{
			return tileModifier;
		}
		AnchoredEntitiesEnumerator anc = _map.GetAnchoredEntitiesEnumerator(xform.GridUid.Value, grid, tile.GridIndices);
		EntityUid? tileEnt = default(EntityUid?);
		TileFrictionModifierComponent friction2 = default(TileFrictionModifierComponent);
		while (((AnchoredEntitiesEnumerator)(ref anc)).MoveNext(ref tileEnt))
		{
			if (_frictionQuery.TryGetComponent(tileEnt, ref friction2))
			{
				tileModifier *= friction2.Modifier;
			}
		}
		return _tileDefinitionManager[tile.Tile.TypeId].Friction * tileModifier;
	}

	public void SetModifier(EntityUid entityUid, float value, TileFrictionModifierComponent? friction = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<TileFrictionModifierComponent>(entityUid, ref friction, true) && !value.Equals(friction.Modifier))
		{
			friction.Modifier = value;
			((EntitySystem)this).Dirty(entityUid, (IComponent)(object)friction, (MetaDataComponent)null);
		}
	}
}
