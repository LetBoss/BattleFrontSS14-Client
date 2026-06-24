using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared.Anomaly.Components;
using Content.Shared.Anomaly.Effects.Components;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;

namespace Content.Shared.Anomaly.Effects;

public abstract class SharedGravityAnomalySystem : EntitySystem
{
	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private ThrowingSystem _throwing;

	[Dependency]
	private SharedTransformSystem _xform;

	[Dependency]
	private SharedMapSystem _mapSystem;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<GravityAnomalyComponent, AnomalyPulseEvent>((ComponentEventRefHandler<GravityAnomalyComponent, AnomalyPulseEvent>)OnAnomalyPulse, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GravityAnomalyComponent, AnomalySupercriticalEvent>((ComponentEventRefHandler<GravityAnomalyComponent, AnomalySupercriticalEvent>)OnSupercritical, (Type[])null, (Type[])null);
	}

	private void OnAnomalyPulse(EntityUid uid, GravityAnomalyComponent component, ref AnomalyPulseEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = ((EntitySystem)this).Transform(uid);
		float range = component.MaxThrowRange * args.Severity * args.PowerModifier;
		float strength = component.MaxThrowStrength * args.Severity * args.PowerModifier;
		HashSet<EntityUid> entitiesInRange = _lookup.GetEntitiesInRange(uid, range, (LookupFlags)10);
		EntityQuery<TransformComponent> xformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		Vector2 worldPos = _xform.GetWorldPosition(xform, xformQuery);
		EntityQuery<PhysicsComponent> physQuery = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		PhysicsComponent phys = default(PhysicsComponent);
		foreach (EntityUid ent in entitiesInRange)
		{
			if (!physQuery.TryGetComponent(ent, ref phys) || (phys.CollisionMask & 0x20) == 0)
			{
				Vector2 foo = _xform.GetWorldPosition(ent, xformQuery) - worldPos;
				_throwing.TryThrow(ent, foo * 10f, strength, uid, 0f);
			}
		}
	}

	private void OnSupercritical(EntityUid uid, GravityAnomalyComponent component, ref AnomalySupercriticalEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = ((EntitySystem)this).Transform(uid);
		MapGridComponent grid = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(xform.GridUid, ref grid))
		{
			return;
		}
		Vector2 worldPos = _xform.GetWorldPosition(xform);
		List<(Vector2i, Tile)> tiles = (from t in _mapSystem.GetTilesIntersecting(xform.GridUid.Value, grid, new Circle(worldPos, component.SpaceRange), true, (Predicate<TileRef>)null).ToArray()
			select (GridIndices: t.GridIndices, Empty: Tile.Empty)).ToList();
		_mapSystem.SetTiles(xform.GridUid.Value, grid, tiles);
		float range = component.MaxThrowRange * 2f * args.PowerModifier;
		float strength = component.MaxThrowStrength * 2f * args.PowerModifier;
		HashSet<EntityUid> entitiesInRange = _lookup.GetEntitiesInRange(uid, range, (LookupFlags)10);
		EntityQuery<TransformComponent> xformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		EntityQuery<PhysicsComponent> physQuery = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		PhysicsComponent phys = default(PhysicsComponent);
		foreach (EntityUid ent in entitiesInRange)
		{
			if (!physQuery.TryGetComponent(ent, ref phys) || (phys.CollisionMask & 0x20) == 0)
			{
				Vector2 foo = _xform.GetWorldPosition(ent, xformQuery) - worldPos;
				_throwing.TryThrow(ent, foo * 5f, strength, uid, 0f);
			}
		}
	}
}
