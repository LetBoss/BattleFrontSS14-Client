using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.Collections;
using Robust.Shared.Containers;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Shapes;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

namespace Robust.Shared.GameObjects;

public sealed class EntityLookupSystem : EntitySystem
{
	private record struct AnyEntityQueryState<T>(bool Found, EntityUid? Ignored, T Shape, Transform Transform, FixtureSystem Fixtures, EntityLookupSystem Lookup, SharedPhysicsSystem Physics, IManifoldManager Manifolds, EntityQuery<FixturesComponent> FixturesQuery, LookupFlags Flags) where T : IPhysShape;

	private readonly record struct EntityQueryState<T>(HashSet<EntityUid> Intersecting, T Shape, Transform Transform, FixtureSystem Fixtures, EntityLookupSystem Lookup, SharedPhysicsSystem Physics, IManifoldManager Manifolds, EntityQuery<FixturesComponent> FixturesQuery, LookupFlags Flags) where T : IPhysShape;

	private readonly record struct GridQueryState<T, TShape>(HashSet<Entity<T>> Intersecting, TShape Shape, Transform Transform, EntityLookupSystem Lookup, SharedPhysicsSystem Physics, LookupFlags Flags, EntityQuery<T> Query) where T : IComponent where TShape : IPhysShape;

	private record struct AnyQueryState<T, TShape>(bool Found, EntityUid? Ignored, TShape Shape, Transform Transform, FixtureSystem Fixtures, SharedPhysicsSystem Physics, IManifoldManager Manifolds, EntityQuery<T> Query, EntityQuery<FixturesComponent> FixturesQuery, LookupFlags Flags) where T : IComponent where TShape : IPhysShape;

	private readonly record struct QueryState<T, TShape>(HashSet<Entity<T>> Intersecting, TShape Shape, Transform Transform, FixtureSystem Fixtures, SharedPhysicsSystem Physics, IManifoldManager Manifolds, EntityQuery<T> Query, EntityQuery<FixturesComponent> FixturesQuery, bool Sensors, bool Approximate) where T : IComponent where TShape : IPhysShape;

	[Robust.Shared.IoC.Dependency]
	private readonly IManifoldManager _manifoldManager;

	[Robust.Shared.IoC.Dependency]
	private readonly IMapManager _mapManager;

	[Robust.Shared.IoC.Dependency]
	private readonly IGameTiming _timing;

	[Robust.Shared.IoC.Dependency]
	private readonly INetManager _netMan;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedContainerSystem _container;

	[Robust.Shared.IoC.Dependency]
	private readonly FixtureSystem _fixtures;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedMapSystem _map;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedPhysicsSystem _physics;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedTransformSystem _transform;

	private EntityQuery<BroadphaseComponent> _broadQuery;

	private EntityQuery<ContainerManagerComponent> _containerQuery;

	private EntityQuery<FixturesComponent> _fixturesQuery;

	private EntityQuery<MapComponent> _mapQuery;

	private EntityQuery<MapGridComponent> _gridQuery;

	private EntityQuery<MetaDataComponent> _metaQuery;

	private EntityQuery<PhysicsComponent> _physicsQuery;

	private EntityQuery<TransformComponent> _xformQuery;

	public const float TileEnlargementRadius = -0.04f;

	public const float LookupEpsilon = 1.4E-44f;

	public const LookupFlags DefaultFlags = LookupFlags.All;

	private void RecursiveAdd(EntityUid uid, ref ValueList<EntityUid> toAdd)
	{
		if (!_xformQuery.TryGetComponent(uid, out TransformComponent component))
		{
			base.Log.Error($"Encountered deleted entity {uid} while performing entity lookup.");
			return;
		}
		toAdd.Add(uid);
		foreach (EntityUid child in component._children)
		{
			RecursiveAdd(child, ref toAdd);
		}
	}

	private void AddContained(HashSet<EntityUid> intersecting, LookupFlags flags)
	{
		if ((flags & LookupFlags.Contained) == 0 || intersecting.Count == 0)
		{
			return;
		}
		ValueList<EntityUid> toAdd = default(ValueList<EntityUid>);
		foreach (EntityUid item2 in intersecting)
		{
			if (!_containerQuery.TryGetComponent(item2, out ContainerManagerComponent component))
			{
				continue;
			}
			foreach (BaseContainer allContainer in _container.GetAllContainers(item2, component))
			{
				foreach (EntityUid containedEntity in allContainer.ContainedEntities)
				{
					RecursiveAdd(containedEntity, ref toAdd);
				}
			}
		}
		Span<EntityUid> span = toAdd.Span;
		for (int i = 0; i < span.Length; i++)
		{
			EntityUid item = span[i];
			intersecting.Add(item);
		}
	}

	private void AddEntitiesIntersecting<T>(MapId mapId, HashSet<EntityUid> intersecting, T shape, Transform shapeTransform, LookupFlags flags) where T : IPhysShape
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		Box2 worldAABB = shape.ComputeAABB(shapeTransform, 0);
		EntityQueryState<T> state = new EntityQueryState<T>(intersecting, shape, shapeTransform, _fixtures, this, _physics, _manifoldManager, _fixturesQuery, flags);
		_mapManager.FindGridsIntersecting(mapId, worldAABB, ref state, delegate(EntityUid uid, MapGridComponent _, ref EntityQueryState<T> reference)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			Transform relativePhysicsTransform2 = reference.Physics.GetRelativePhysicsTransform(reference.Transform, uid);
			Box2 localAABB2 = reference.Shape.ComputeAABB(relativePhysicsTransform2, 0);
			reference.Lookup.AddEntitiesIntersecting(uid, reference.Intersecting, reference.Shape, localAABB2, relativePhysicsTransform2, reference.Flags);
			return true;
		}, approx: true, includeMap: false);
		EntityUid mapOrInvalid = _map.GetMapOrInvalid(mapId);
		Transform relativePhysicsTransform = state.Physics.GetRelativePhysicsTransform(state.Transform, mapOrInvalid);
		Box2 localAABB = state.Shape.ComputeAABB(relativePhysicsTransform, 0);
		AddEntitiesIntersecting(mapOrInvalid, intersecting, shape, localAABB, relativePhysicsTransform, flags);
		AddContained(intersecting, flags);
	}

	private void AddEntitiesIntersecting<T>(EntityUid lookupUid, HashSet<EntityUid> intersecting, T shape, Box2 localAABB, Transform localShapeTransform, LookupFlags flags, BroadphaseComponent? lookup = null) where T : IPhysShape
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		if (_broadQuery.Resolve(lookupUid, ref lookup))
		{
			EntityQueryState<T> state = new EntityQueryState<T>(intersecting, shape, localShapeTransform, _fixtures, this, _physics, _manifoldManager, _fixturesQuery, flags);
			if ((flags & LookupFlags.Dynamic) != LookupFlags.None)
			{
				lookup.DynamicTree.QueryAabb(ref state, PhysicsQuery, localAABB, approx: true);
			}
			if ((flags & LookupFlags.Static) != LookupFlags.None)
			{
				lookup.StaticTree.QueryAabb(ref state, PhysicsQuery, localAABB, approx: true);
			}
			if ((flags & LookupFlags.StaticSundries) == LookupFlags.StaticSundries)
			{
				lookup.StaticSundriesTree.QueryAabb(ref state, SundriesQuery, localAABB, approx: true);
			}
			if ((flags & LookupFlags.Sundries) != LookupFlags.None)
			{
				lookup.SundriesTree.QueryAabb(ref state, SundriesQuery, localAABB, approx: true);
			}
		}
		static bool PhysicsQuery(ref EntityQueryState<T> reference, in FixtureProxy value)
		{
			if ((reference.Flags & LookupFlags.Sensors) == 0 && !value.Fixture.Hard)
			{
				return true;
			}
			if ((reference.Flags & LookupFlags.Approximate) == 0)
			{
				Transform xfB = reference.Physics.GetLocalPhysicsTransform(value.Entity);
				if (!reference.Manifolds.TestOverlap(reference.Shape, 0, value.Fixture.Shape, value.ChildIndex, reference.Transform, in xfB))
				{
					return true;
				}
			}
			reference.Intersecting.Add(value.Entity);
			return true;
		}
		static bool SundriesQuery(ref EntityQueryState<T> reference, in EntityUid value)
		{
			if ((reference.Flags & LookupFlags.Approximate) != LookupFlags.None)
			{
				reference.Intersecting.Add(value);
				return true;
			}
			Transform xfB = reference.Physics.GetLocalPhysicsTransform(value);
			if (reference.FixturesQuery.TryGetComponent(value, out FixturesComponent component))
			{
				bool flag = (reference.Flags & LookupFlags.Sensors) != 0;
				bool flag2 = false;
				foreach (Fixture value in component.Fixtures.Values)
				{
					if (flag || value.Hard)
					{
						flag2 = true;
						for (int i = 0; i < value.Shape.ChildCount; i++)
						{
							if (reference.Manifolds.TestOverlap(reference.Shape, 0, value.Shape, i, reference.Transform, in xfB))
							{
								reference.Intersecting.Add(value);
								return true;
							}
						}
					}
				}
				if (flag2)
				{
					return true;
				}
			}
			if (reference.Fixtures.TestPoint(reference.Shape, reference.Transform, xfB.Position))
			{
				reference.Intersecting.Add(value);
			}
			return true;
		}
	}

	private bool AnyEntitiesIntersecting<T>(MapId mapId, T shape, Transform shapeTransform, LookupFlags flags, EntityUid? ignored = null) where T : IPhysShape
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		Box2 worldAABB = shape.ComputeAABB(shapeTransform, 0);
		AnyEntityQueryState<T> state = new AnyEntityQueryState<T>(Found: false, ignored, shape, shapeTransform, _fixtures, this, _physics, _manifoldManager, _fixturesQuery, flags);
		_mapManager.FindGridsIntersecting(mapId, worldAABB, ref state, delegate(EntityUid uid, MapGridComponent _, ref AnyEntityQueryState<T> reference)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			Transform relativePhysicsTransform2 = reference.Physics.GetRelativePhysicsTransform(reference.Transform, uid);
			Box2 localAABB2 = reference.Shape.ComputeAABB(relativePhysicsTransform2, 0);
			if (reference.Lookup.AnyEntitiesIntersecting(uid, reference.Shape, localAABB2, relativePhysicsTransform2, reference.Flags, reference.Ignored))
			{
				reference.Found = true;
				return false;
			}
			return true;
		}, approx: true, includeMap: false);
		if (!state.Found)
		{
			EntityUid mapOrInvalid = _map.GetMapOrInvalid(mapId);
			Transform relativePhysicsTransform = state.Physics.GetRelativePhysicsTransform(state.Transform, mapOrInvalid);
			Box2 localAABB = state.Shape.ComputeAABB(relativePhysicsTransform, 0);
			state.Found = AnyEntitiesIntersecting(mapOrInvalid, shape, localAABB, relativePhysicsTransform, flags, ignored);
		}
		return state.Found;
	}

	private bool AnyEntitiesIntersecting<T>(EntityUid lookupUid, T shape, Box2 localAABB, Transform shapeTransform, LookupFlags flags, EntityUid? ignored = null, BroadphaseComponent? lookup = null) where T : IPhysShape
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		if (!_broadQuery.Resolve(lookupUid, ref lookup))
		{
			return false;
		}
		AnyEntityQueryState<T> state = new AnyEntityQueryState<T>(Found: false, ignored, shape, shapeTransform, _fixtures, this, _physics, _manifoldManager, _fixturesQuery, flags);
		if ((flags & LookupFlags.Dynamic) != LookupFlags.None)
		{
			lookup.DynamicTree.QueryAabb(ref state, PhysicsQuery, localAABB, approx: true);
			if (state.Found)
			{
				return true;
			}
		}
		if ((flags & LookupFlags.Static) != LookupFlags.None)
		{
			lookup.StaticTree.QueryAabb(ref state, PhysicsQuery, localAABB, approx: true);
			if (state.Found)
			{
				return true;
			}
		}
		if ((flags & LookupFlags.StaticSundries) == LookupFlags.StaticSundries)
		{
			lookup.StaticSundriesTree.QueryAabb(ref state, SundriesQuery, localAABB, approx: true);
			if (state.Found)
			{
				return true;
			}
		}
		if ((flags & LookupFlags.Sundries) != LookupFlags.None)
		{
			lookup.SundriesTree.QueryAabb(ref state, SundriesQuery, localAABB, approx: true);
		}
		return state.Found;
		static bool PhysicsQuery(ref AnyEntityQueryState<T> reference, in FixtureProxy value)
		{
			if (reference.Ignored == value.Entity)
			{
				return true;
			}
			if ((reference.Flags & LookupFlags.Sensors) == 0 && !value.Fixture.Hard)
			{
				return true;
			}
			if ((reference.Flags & LookupFlags.Approximate) == 0)
			{
				Transform xfB = reference.Physics.GetLocalPhysicsTransform(value.Entity);
				if (!reference.Manifolds.TestOverlap(reference.Shape, 0, value.Fixture.Shape, value.ChildIndex, reference.Transform, in xfB))
				{
					return true;
				}
			}
			reference.Found = true;
			return false;
		}
		static bool SundriesQuery(ref AnyEntityQueryState<T> reference, in EntityUid value)
		{
			if (reference.Ignored == value)
			{
				return true;
			}
			if ((reference.Flags & LookupFlags.Approximate) != LookupFlags.None)
			{
				reference.Found = true;
				return false;
			}
			Transform xfB = reference.Physics.GetLocalPhysicsTransform(value);
			if (reference.FixturesQuery.TryGetComponent(value, out FixturesComponent component))
			{
				bool flag = (reference.Flags & LookupFlags.Sensors) != 0;
				bool flag2 = false;
				foreach (Fixture value in component.Fixtures.Values)
				{
					if (flag || value.Hard)
					{
						flag2 = true;
						for (int i = 0; i < value.Shape.ChildCount; i++)
						{
							if (reference.Manifolds.TestOverlap(reference.Shape, 0, value.Shape, i, reference.Transform, in xfB))
							{
								reference.Found = true;
								return false;
							}
						}
					}
				}
				if (flag2)
				{
					return true;
				}
			}
			if (reference.Fixtures.TestPoint(reference.Shape, reference.Transform, xfB.Position))
			{
				reference.Found = true;
				return false;
			}
			return true;
		}
	}

	private bool AnyEntitiesIntersecting(EntityUid lookupUid, Box2Rotated worldBounds, LookupFlags flags, EntityUid? ignored = null)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Box2 aabb;
		SlimPolygon shape = new SlimPolygon(in worldBounds, _transform.GetInvWorldMatrix(lookupUid), out aabb);
		return AnyEntitiesIntersecting(lookupUid, shape, aabb, Robust.Shared.Physics.Transform.Empty, flags, ignored);
	}

	public IEnumerable<EntityUid> GetEntitiesInArc(EntityCoordinates coordinates, float range, Angle direction, float arcWidth, LookupFlags flags = LookupFlags.All)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		MapCoordinates coordinates2 = _transform.ToMapCoordinates(coordinates);
		return GetEntitiesInArc(coordinates2, range, direction, arcWidth, flags);
	}

	public IEnumerable<EntityUid> GetEntitiesInArc(MapCoordinates coordinates, float range, Angle direction, float arcWidth, LookupFlags flags = LookupFlags.All)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Unsafe.SkipInit(out Angle val);
		foreach (EntityUid item in GetEntitiesInRange(coordinates, range * 2f, flags))
		{
			((Angle)(ref val))._002Ector(_transform.GetWorldPosition(item) - coordinates.Position);
			if (((Angle)(ref val)).Degrees < ((Angle)(ref direction)).Degrees + (double)(arcWidth / 2f) && ((Angle)(ref val)).Degrees > ((Angle)(ref direction)).Degrees - (double)(arcWidth / 2f))
			{
				yield return item;
			}
		}
	}

	public bool AnyEntitiesIntersecting(MapId mapId, Box2 worldAABB, LookupFlags flags = LookupFlags.All)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (mapId == MapId.Nullspace)
		{
			return false;
		}
		SlimPolygon shape = new SlimPolygon(worldAABB);
		return AnyEntitiesIntersecting(mapId, shape, Robust.Shared.Physics.Transform.Empty, flags);
	}

	public HashSet<EntityUid> GetEntitiesIntersecting(MapId mapId, Box2 worldAABB, LookupFlags flags = LookupFlags.All)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
		GetEntitiesIntersecting(mapId, worldAABB, hashSet, flags);
		return hashSet;
	}

	public void GetEntitiesIntersecting(MapId mapId, Box2 worldAABB, HashSet<EntityUid> intersecting, LookupFlags flags = LookupFlags.All)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!(mapId == MapId.Nullspace))
		{
			SlimPolygon shape = new SlimPolygon(worldAABB);
			AddEntitiesIntersecting(mapId, intersecting, shape, Robust.Shared.Physics.Transform.Empty, flags);
		}
	}

	public bool AnyEntitiesIntersecting(MapId mapId, Box2Rotated worldBounds, LookupFlags flags = LookupFlags.All)
	{
		SlimPolygon shape = new SlimPolygon(in worldBounds);
		return AnyEntitiesIntersecting(mapId, shape, Robust.Shared.Physics.Transform.Empty, flags);
	}

	public HashSet<EntityUid> GetEntitiesIntersecting(MapId mapId, Box2Rotated worldBounds, LookupFlags flags = LookupFlags.All)
	{
		HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
		SlimPolygon shape = new SlimPolygon(in worldBounds);
		AddEntitiesIntersecting(mapId, hashSet, shape, Robust.Shared.Physics.Transform.Empty, flags);
		return hashSet;
	}

	public bool AnyEntitiesIntersecting(EntityUid uid, LookupFlags flags = LookupFlags.All)
	{
		return AnyEntitiesInRange(uid, 1.4E-44f, flags);
	}

	public bool AnyEntitiesInRange(EntityUid uid, float range, LookupFlags flags = LookupFlags.All)
	{
		MapCoordinates mapCoordinates = _transform.GetMapCoordinates(uid);
		if (mapCoordinates.MapId == MapId.Nullspace)
		{
			return false;
		}
		PhysShapeCircle shape = new PhysShapeCircle(range, mapCoordinates.Position);
		return AnyEntitiesIntersecting(mapCoordinates.MapId, shape, Robust.Shared.Physics.Transform.Empty, flags, uid);
	}

	public HashSet<EntityUid> GetEntitiesInRange(EntityUid uid, float range, LookupFlags flags = LookupFlags.All)
	{
		MapCoordinates mapCoordinates = _transform.GetMapCoordinates(uid);
		if (mapCoordinates.MapId == MapId.Nullspace)
		{
			return new HashSet<EntityUid>();
		}
		HashSet<EntityUid> entitiesInRange = GetEntitiesInRange(mapCoordinates, range, flags);
		entitiesInRange.Remove(uid);
		return entitiesInRange;
	}

	public void GetEntitiesInRange(EntityUid uid, float range, HashSet<EntityUid> entities, LookupFlags flags = LookupFlags.All)
	{
		MapCoordinates mapCoordinates = _transform.GetMapCoordinates(uid);
		if (!(mapCoordinates.MapId == MapId.Nullspace))
		{
			GetEntitiesInRange(mapCoordinates.MapId, mapCoordinates.Position, range, entities, flags);
			entities.Remove(uid);
		}
	}

	public void GetEntitiesIntersecting(EntityUid uid, HashSet<EntityUid> intersecting, LookupFlags flags = LookupFlags.All)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent component = _xformQuery.GetComponent(uid);
		MapId mapID = component.MapID;
		if (!(mapID == MapId.Nullspace))
		{
			(Vector2 WorldPosition, Angle WorldRotation) worldPositionRotation = _transform.GetWorldPositionRotation(component);
			Vector2 item = worldPositionRotation.WorldPosition;
			Angle item2 = worldPositionRotation.WorldRotation;
			Box2 aABBNoContainer = GetAABBNoContainer(uid, item, item2);
			bool num = intersecting.Contains(uid);
			Transform item3 = new Transform(item, item2);
			(EntityUid, Transform, HashSet<EntityUid>, EntityQuery<FixturesComponent>, EntityLookupSystem, SharedPhysicsSystem, LookupFlags) state = (uid, item3, intersecting, _fixturesQuery, this, _physics, flags);
			_mapManager.FindGridsIntersecting<(EntityUid, Transform, HashSet<EntityUid>, EntityQuery<FixturesComponent>, EntityLookupSystem, SharedPhysicsSystem, LookupFlags)>(mapID, aABBNoContainer, ref state, delegate(EntityUid gridUid, MapGridComponent grid, ref (EntityUid entity, Transform transform, HashSet<EntityUid> intersecting, EntityQuery<FixturesComponent> fixturesQuery, EntityLookupSystem lookup, SharedPhysicsSystem physics, LookupFlags flags) reference)
			{
				EntityIntersectingQuery(gridUid, reference);
				return true;
			}, approx: true, includeMap: false);
			EntityIntersectingQuery(_map.GetMapOrInvalid(mapID), state);
			if (!num)
			{
				intersecting.Remove(uid);
			}
		}
		static void EntityIntersectingQuery(EntityUid lookupUid, (EntityUid entity, Transform shapeTransform, HashSet<EntityUid> intersecting, EntityQuery<FixturesComponent> fixturesQuery, EntityLookupSystem lookup, SharedPhysicsSystem physics, LookupFlags flags) tuple)
		{
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			Transform relativePhysicsTransform = tuple.physics.GetRelativePhysicsTransform(tuple.shapeTransform, lookupUid);
			if (tuple.fixturesQuery.TryGetComponent(tuple.entity, out FixturesComponent component2))
			{
				foreach (Fixture value in component2.Fixtures.Values)
				{
					if (value.Hard || (tuple.flags & LookupFlags.Sensors) != LookupFlags.None)
					{
						Box2 localAABB = value.Shape.ComputeAABB(relativePhysicsTransform, 0);
						tuple.lookup.AddEntitiesIntersecting(lookupUid, tuple.intersecting, value.Shape, localAABB, relativePhysicsTransform, tuple.flags);
					}
				}
				return;
			}
			PhysShapeCircle physShapeCircle = new PhysShapeCircle(1.4E-44f);
			Box2 localAABB2 = physShapeCircle.ComputeAABB(relativePhysicsTransform, 0);
			tuple.lookup.AddEntitiesIntersecting(lookupUid, tuple.intersecting, physShapeCircle, localAABB2, relativePhysicsTransform, tuple.flags);
		}
	}

	public HashSet<EntityUid> GetEntitiesIntersecting(EntityUid uid, LookupFlags flags = LookupFlags.All)
	{
		MapId mapID = _xformQuery.GetComponent(uid).MapID;
		HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
		if (mapID == MapId.Nullspace)
		{
			return hashSet;
		}
		GetEntitiesIntersecting(uid, hashSet, flags);
		return hashSet;
	}

	public bool AnyEntitiesIntersecting(EntityCoordinates coordinates, LookupFlags flags = LookupFlags.All)
	{
		if (!coordinates.IsValid(EntityManager))
		{
			return false;
		}
		MapCoordinates coordinates2 = _transform.ToMapCoordinates(coordinates);
		return AnyEntitiesIntersecting(coordinates2, flags);
	}

	public bool AnyEntitiesInRange(EntityCoordinates coordinates, float range, LookupFlags flags = LookupFlags.All)
	{
		if (!coordinates.IsValid(EntityManager))
		{
			return false;
		}
		MapCoordinates coordinates2 = _transform.ToMapCoordinates(coordinates);
		return AnyEntitiesInRange(coordinates2, range, flags);
	}

	public HashSet<EntityUid> GetEntitiesIntersecting(EntityCoordinates coordinates, LookupFlags flags = LookupFlags.All)
	{
		MapCoordinates coordinates2 = _transform.ToMapCoordinates(coordinates);
		return GetEntitiesIntersecting(coordinates2, flags);
	}

	public HashSet<EntityUid> GetEntitiesInRange(EntityCoordinates coordinates, float range, LookupFlags flags = LookupFlags.All)
	{
		HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
		GetEntitiesInRange(coordinates, range, hashSet, flags);
		return hashSet;
	}

	public void GetEntitiesInRange(EntityCoordinates coordinates, float range, HashSet<EntityUid> entities, LookupFlags flags = LookupFlags.All)
	{
		MapCoordinates mapCoordinates = _transform.ToMapCoordinates(coordinates);
		if (!(mapCoordinates.MapId == MapId.Nullspace))
		{
			GetEntitiesInRange(mapCoordinates.MapId, mapCoordinates.Position, range, entities, flags);
		}
	}

	public bool AnyEntitiesIntersecting(MapCoordinates coordinates, LookupFlags flags = LookupFlags.All)
	{
		if (coordinates.MapId == MapId.Nullspace)
		{
			return false;
		}
		return AnyEntitiesInRange(coordinates, 1.4E-44f, flags);
	}

	public bool AnyEntitiesInRange(MapCoordinates coordinates, float range, LookupFlags flags = LookupFlags.All)
	{
		if (coordinates.MapId == MapId.Nullspace)
		{
			return false;
		}
		PhysShapeCircle shape = new PhysShapeCircle(range, coordinates.Position);
		return AnyEntitiesIntersecting(coordinates.MapId, shape, Robust.Shared.Physics.Transform.Empty, flags);
	}

	public HashSet<EntityUid> GetEntitiesIntersecting(MapCoordinates coordinates, LookupFlags flags = LookupFlags.All)
	{
		if (coordinates.MapId == MapId.Nullspace)
		{
			return new HashSet<EntityUid>();
		}
		return GetEntitiesInRange(coordinates, 1.4E-44f, flags);
	}

	public HashSet<EntityUid> GetEntitiesInRange(MapCoordinates coordinates, float range, LookupFlags flags = LookupFlags.All)
	{
		return GetEntitiesInRange(coordinates.MapId, coordinates.Position, range, flags);
	}

	public HashSet<EntityUid> GetEntitiesInRange(MapId mapId, Vector2 worldPos, float range, LookupFlags flags = LookupFlags.All)
	{
		HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
		GetEntitiesInRange(mapId, worldPos, range, hashSet, flags);
		return hashSet;
	}

	public void GetEntitiesIntersecting<T>(MapId mapId, T shape, Transform transform, HashSet<EntityUid> entities, LookupFlags flags = LookupFlags.All) where T : IPhysShape
	{
		if (!(mapId == MapId.Nullspace))
		{
			AddEntitiesIntersecting(mapId, entities, shape, transform, flags);
		}
	}

	public void GetEntitiesInRange(MapId mapId, Vector2 worldPos, float range, HashSet<EntityUid> entities, LookupFlags flags = LookupFlags.All)
	{
		if (!(mapId == MapId.Nullspace))
		{
			PhysShapeCircle shape = new PhysShapeCircle(range, worldPos);
			AddEntitiesIntersecting(mapId, entities, shape, Robust.Shared.Physics.Transform.Empty, flags);
		}
	}

	public HashSet<EntityUid> GetEntitiesIntersecting(EntityUid gridId, Box2 worldAABB, LookupFlags flags = LookupFlags.All)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
		GetEntitiesIntersecting(gridId, worldAABB, hashSet, flags);
		return hashSet;
	}

	public HashSet<EntityUid> GetEntitiesIntersecting(EntityUid gridId, Box2Rotated worldBounds, LookupFlags flags = LookupFlags.All)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
		GetEntitiesIntersecting(gridId, worldBounds, hashSet, flags);
		return hashSet;
	}

	public void GetEntitiesIntersecting(EntityUid gridId, Box2 worldAABB, HashSet<EntityUid> intersecting, LookupFlags flags = LookupFlags.All)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (_broadQuery.TryGetComponent(gridId, out BroadphaseComponent component))
		{
			Box2 val = Matrix3Helpers.TransformBox(_transform.GetInvWorldMatrix(gridId), ref worldAABB);
			SlimPolygon shape = new SlimPolygon(val);
			AddEntitiesIntersecting(gridId, intersecting, shape, val, Robust.Shared.Physics.Transform.Empty, flags, component);
			AddContained(intersecting, flags);
		}
	}

	public void GetEntitiesIntersecting(EntityUid gridId, Box2Rotated worldBounds, HashSet<EntityUid> intersecting, LookupFlags flags = LookupFlags.All)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (_broadQuery.TryGetComponent(gridId, out BroadphaseComponent component))
		{
			Box2 aabb;
			SlimPolygon shape = new SlimPolygon(in worldBounds, _transform.GetInvWorldMatrix(gridId), out aabb);
			AddEntitiesIntersecting(gridId, intersecting, shape, aabb, Robust.Shared.Physics.Transform.Empty, flags, component);
			AddContained(intersecting, flags);
		}
	}

	public void FindLookupsIntersecting(MapId mapId, Box2Rotated worldBounds, ComponentQueryCallback<BroadphaseComponent> callback)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (!(mapId == MapId.Nullspace))
		{
			(ComponentQueryCallback<BroadphaseComponent>, EntityQuery<BroadphaseComponent>) state = (callback, _broadQuery);
			_mapManager.FindGridsIntersecting<(ComponentQueryCallback<BroadphaseComponent>, EntityQuery<BroadphaseComponent>)>(mapId, worldBounds, ref state, delegate(EntityUid uid, MapGridComponent grid, ref (ComponentQueryCallback<BroadphaseComponent> callback, EntityQuery<BroadphaseComponent> _broadQuery) tuple)
			{
				tuple.callback(uid, tuple._broadQuery.GetComponent(uid));
				return true;
			}, approx: true, includeMap: false);
			EntityUid mapOrInvalid = _map.GetMapOrInvalid(mapId);
			callback(mapOrInvalid, _broadQuery.GetComponent(mapOrInvalid));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Box2 GetLocalBounds(Vector2i gridIndices, ushort tileSize)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		return new Box2(Vector2i.op_Implicit(gridIndices * (int)tileSize), Vector2i.op_Implicit((gridIndices + 1) * (int)tileSize));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Box2 GetLocalBounds(TileRef tileRef, ushort tileSize)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return GetLocalBounds(tileRef.GridIndices, tileSize);
	}

	public Box2Rotated GetWorldBounds(TileRef tileRef, Matrix3x2? worldMatrix = null, Angle? angle = null)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		MapGridComponent component = _gridQuery.GetComponent(tileRef.GridUid);
		if (!worldMatrix.HasValue || !angle.HasValue)
		{
			(Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix) worldPositionRotationMatrix = _transform.GetWorldPositionRotationMatrix(tileRef.GridUid);
			Angle item = worldPositionRotationMatrix.WorldRotation;
			Matrix3x2 item2 = worldPositionRotationMatrix.WorldMatrix;
			worldMatrix = item2;
			angle = item;
		}
		Vector2 vector = new Vector2(0.5f, 0.5f);
		Vector2 vector2 = Vector2.Transform(Vector2i.op_Implicit(tileRef.GridIndices) + vector, worldMatrix.Value) * (int)component.TileSize;
		return new Box2Rotated(Box2.CenteredAround(vector2, new Vector2((int)component.TileSize, (int)component.TileSize)), -angle.Value, vector2);
	}

	private void RecursiveAdd<T>(EntityUid uid, ref ValueList<Entity<T>> toAdd, EntityQuery<T> query) where T : IComponent
	{
		foreach (EntityUid child in _xformQuery.GetComponent(uid)._children)
		{
			if (query.TryGetComponent(child, out T component))
			{
				toAdd.Add((Owner: child, Comp: component));
			}
			RecursiveAdd(child, ref toAdd, query);
		}
	}

	private void AddContained<T>(HashSet<Entity<T>> intersecting, LookupFlags flags, EntityQuery<T> query) where T : IComponent
	{
		if ((flags & LookupFlags.Contained) == 0)
		{
			return;
		}
		ValueList<Entity<T>> toAdd = default(ValueList<Entity<T>>);
		foreach (Entity<T> item2 in intersecting)
		{
			if (!_containerQuery.TryGetComponent(item2, out ContainerManagerComponent component))
			{
				continue;
			}
			foreach (BaseContainer allContainer in _container.GetAllContainers(item2, component))
			{
				foreach (EntityUid containedEntity in allContainer.ContainedEntities)
				{
					if (query.TryGetComponent(containedEntity, out T component2))
					{
						toAdd.Add((Owner: containedEntity, Comp: component2));
					}
					RecursiveAdd(containedEntity, ref toAdd, query);
				}
			}
		}
		Span<Entity<T>> span = toAdd.Span;
		for (int i = 0; i < span.Length; i++)
		{
			Entity<T> item = span[i];
			intersecting.Add(item);
		}
	}

	private bool IsIntersecting<TShape>(MapId mapId, EntityUid uid, TransformComponent xform, TShape shape, Transform shapeTransform, Box2 worldAABB, LookupFlags flags) where TShape : IPhysShape
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		var (vector, angle) = _transform.GetWorldPositionRotation(xform);
		if (xform.MapID != mapId || !((Box2)(ref worldAABB)).Contains(vector, true) || ((flags & LookupFlags.Contained) == 0 && _container.IsEntityOrParentInContainer(uid, _metaQuery.GetComponent(uid), xform)))
		{
			return false;
		}
		if (_fixturesQuery.TryGetComponent(uid, out FixturesComponent component))
		{
			Transform xfB = new Transform(vector, angle);
			bool flag = false;
			bool flag2 = (flags & LookupFlags.Sensors) != 0;
			foreach (Fixture value in component.Fixtures.Values)
			{
				if (!flag2 && !value.Hard)
				{
					continue;
				}
				flag = true;
				for (int i = 0; i < value.Shape.ChildCount; i++)
				{
					if (_manifoldManager.TestOverlap(shape, 0, value.Shape, i, in shapeTransform, in xfB))
					{
						return true;
					}
				}
			}
			if (flag)
			{
				return false;
			}
		}
		if (!_fixtures.TestPoint(shape, shapeTransform, vector))
		{
			return false;
		}
		return true;
	}

	private void AddLocalEntitiesIntersecting<T>(EntityUid lookupUid, HashSet<Entity<T>> intersecting, Box2 localAABB, LookupFlags flags, EntityQuery<T> query, BroadphaseComponent? lookup = null) where T : IComponent
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (_broadQuery.Resolve(lookupUid, ref lookup))
		{
			SlimPolygon shape = new SlimPolygon(localAABB);
			AddEntitiesIntersecting(lookupUid, intersecting, shape, localAABB, Robust.Shared.Physics.Transform.Empty, flags, query, lookup);
		}
	}

	private void AddEntitiesIntersecting<T, TShape>(EntityUid lookupUid, HashSet<Entity<T>> intersecting, TShape shape, Box2 localAABB, Transform localTransform, LookupFlags flags, EntityQuery<T> query, BroadphaseComponent? lookup = null) where T : IComponent where TShape : IPhysShape
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		if (_broadQuery.Resolve(lookupUid, ref lookup))
		{
			QueryState<T, TShape> state = new QueryState<T, TShape>(intersecting, shape, localTransform, _fixtures, _physics, _manifoldManager, query, _fixturesQuery, (flags & LookupFlags.Sensors) != 0, (flags & LookupFlags.Approximate) != 0);
			if ((flags & LookupFlags.Dynamic) != LookupFlags.None)
			{
				lookup.DynamicTree.QueryAabb(ref state, PhysicsQuery, localAABB, approx: true);
			}
			if ((flags & LookupFlags.Static) != LookupFlags.None)
			{
				lookup.StaticTree.QueryAabb(ref state, PhysicsQuery, localAABB, approx: true);
			}
			if ((flags & LookupFlags.StaticSundries) == LookupFlags.StaticSundries)
			{
				lookup.StaticSundriesTree.QueryAabb(ref state, SundriesQuery, localAABB, approx: true);
			}
			if ((flags & LookupFlags.Sundries) != LookupFlags.None)
			{
				lookup.SundriesTree.QueryAabb(ref state, SundriesQuery, localAABB, approx: true);
			}
		}
		static bool PhysicsQuery(ref QueryState<T, TShape> reference, in FixtureProxy value)
		{
			if (!reference.Sensors && !value.Fixture.Hard)
			{
				return true;
			}
			if (!reference.Query.TryGetComponent(value.Entity, out T component))
			{
				return true;
			}
			if (!reference.Approximate)
			{
				Transform xfB = reference.Physics.GetLocalPhysicsTransform(value.Entity);
				if (!reference.Manifolds.TestOverlap(reference.Shape, 0, value.Fixture.Shape, value.ChildIndex, reference.Transform, in xfB))
				{
					return true;
				}
			}
			reference.Intersecting.Add((Owner: value.Entity, Comp: component));
			return true;
		}
		static bool SundriesQuery(ref QueryState<T, TShape> reference, in EntityUid value)
		{
			if (!reference.Query.TryGetComponent(value, out T component))
			{
				return true;
			}
			if (reference.Approximate)
			{
				reference.Intersecting.Add((Owner: value, Comp: component));
				return true;
			}
			Transform xfB = reference.Physics.GetLocalPhysicsTransform(value);
			if (reference.FixturesQuery.TryGetComponent(value, out FixturesComponent component2))
			{
				bool flag = false;
				foreach (Fixture value in component2.Fixtures.Values)
				{
					if (reference.Sensors || value.Hard)
					{
						flag = true;
						for (int i = 0; i < value.Shape.ChildCount; i++)
						{
							if (reference.Manifolds.TestOverlap(reference.Shape, 0, value.Shape, i, reference.Transform, in xfB))
							{
								reference.Intersecting.Add((Owner: value, Comp: component));
								return true;
							}
						}
					}
				}
				if (flag)
				{
					return true;
				}
			}
			if (reference.Fixtures.TestPoint(reference.Shape, reference.Transform, xfB.Position))
			{
				reference.Intersecting.Add((Owner: value, Comp: component));
			}
			return true;
		}
	}

	private bool AnyLocalComponentsIntersecting<T>(EntityUid lookupUid, Box2 localAABB, LookupFlags flags, EntityQuery<T> query, EntityUid? ignored = null, BroadphaseComponent? lookup = null) where T : IComponent
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!_broadQuery.Resolve(lookupUid, ref lookup))
		{
			return false;
		}
		SlimPolygon shape = new SlimPolygon(localAABB);
		(Vector2 WorldPosition, Angle WorldRotation) worldPositionRotation = _transform.GetWorldPositionRotation(lookupUid);
		Vector2 item = worldPositionRotation.WorldPosition;
		Angle item2 = worldPositionRotation.WorldRotation;
		Transform shapeTransform = new Transform(item, item2);
		return AnyComponentsIntersecting(lookupUid, shape, localAABB, shapeTransform, flags, query, ignored, lookup);
	}

	private bool AnyComponentsIntersecting<T, TShape>(EntityUid lookupUid, TShape shape, Box2 localAABB, Transform shapeTransform, LookupFlags flags, EntityQuery<T> query, EntityUid? ignored = null, BroadphaseComponent? lookup = null) where T : IComponent where TShape : IPhysShape
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		if (!_broadQuery.Resolve(lookupUid, ref lookup))
		{
			return false;
		}
		AnyQueryState<T, TShape> state = new AnyQueryState<T, TShape>(Found: false, ignored, shape, shapeTransform, _fixtures, _physics, _manifoldManager, query, _fixturesQuery, flags);
		if ((flags & LookupFlags.Dynamic) != LookupFlags.None)
		{
			lookup.DynamicTree.QueryAabb(ref state, PhysicsQuery, localAABB, approx: true);
			if (state.Found)
			{
				return true;
			}
		}
		if ((flags & LookupFlags.Static) != LookupFlags.None)
		{
			lookup.StaticTree.QueryAabb(ref state, PhysicsQuery, localAABB, approx: true);
			if (state.Found)
			{
				return true;
			}
		}
		if ((flags & LookupFlags.StaticSundries) == LookupFlags.StaticSundries)
		{
			lookup.StaticSundriesTree.QueryAabb(ref state, SundriesQuery, localAABB, approx: true);
			if (state.Found)
			{
				return true;
			}
		}
		if ((flags & LookupFlags.Sundries) != LookupFlags.None)
		{
			lookup.SundriesTree.QueryAabb(ref state, SundriesQuery, localAABB, approx: true);
		}
		return state.Found;
		static bool PhysicsQuery(ref AnyQueryState<T, TShape> reference, in FixtureProxy value)
		{
			EntityUid entity = value.Entity;
			EntityUid? ignored2 = reference.Ignored;
			if (entity == ignored2)
			{
				return true;
			}
			if (!reference.Query.HasComponent(value.Entity))
			{
				return true;
			}
			if ((reference.Flags & LookupFlags.Approximate) == 0)
			{
				Transform xfB = reference.Physics.GetPhysicsTransform(value.Entity);
				if (!reference.Manifolds.TestOverlap(reference.Shape, 0, value.Fixture.Shape, value.ChildIndex, reference.Transform, in xfB))
				{
					return true;
				}
			}
			reference.Found = true;
			return false;
		}
		static bool SundriesQuery(ref AnyQueryState<T, TShape> reference, in EntityUid value)
		{
			if (reference.Ignored == value)
			{
				return true;
			}
			if (!reference.Query.HasComponent(value))
			{
				return true;
			}
			if ((reference.Flags & LookupFlags.Approximate) != LookupFlags.None)
			{
				reference.Found = true;
				return false;
			}
			Transform xfB = reference.Physics.GetPhysicsTransform(value);
			if (reference.FixturesQuery.TryGetComponent(value, out FixturesComponent component))
			{
				bool flag = (reference.Flags & LookupFlags.Sensors) != 0;
				bool flag2 = false;
				foreach (Fixture value in component.Fixtures.Values)
				{
					if (flag || value.Hard)
					{
						flag2 = true;
						for (int i = 0; i < value.Shape.ChildCount; i++)
						{
							if (reference.Manifolds.TestOverlap(reference.Shape, 0, value.Shape, i, reference.Transform, in xfB))
							{
								reference.Found = true;
								return false;
							}
						}
					}
				}
				if (flag2)
				{
					return true;
				}
			}
			if (reference.Fixtures.TestPoint(reference.Shape, reference.Transform, xfB.Position))
			{
				reference.Found = true;
				return false;
			}
			return true;
		}
	}

	private bool UseBoundsQuery(Type type, float area)
	{
		return (float)Count(type) > area;
	}

	private bool UseBoundsQuery<T>(float area) where T : IComponent
	{
		return (float)Count<T>() > area;
	}

	public bool AnyComponentsIntersecting(Type type, MapId mapId, Box2 worldAABB, EntityUid? ignored = null, LookupFlags flags = LookupFlags.All)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		SlimPolygon shape = new SlimPolygon(worldAABB);
		Transform empty = Robust.Shared.Physics.Transform.Empty;
		return AnyComponentsIntersecting(type, mapId, shape, empty, ignored, flags);
	}

	public bool AnyComponentsIntersecting<T>(Type type, MapId mapId, T shape, Transform shapeTransform, EntityUid? ignored = null, LookupFlags flags = LookupFlags.All) where T : IPhysShape
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (mapId == MapId.Nullspace)
		{
			return false;
		}
		Box2 val = shape.ComputeAABB(shapeTransform, 0);
		if (!UseBoundsQuery(type, ((Box2)(ref val)).Height * ((Box2)(ref val)).Width))
		{
			foreach (var allComponent in EntityManager.GetAllComponents(type, includePaused: true))
			{
				EntityUid item = allComponent.Uid;
				TransformComponent component = _xformQuery.GetComponent(item);
				if (IsIntersecting(mapId, item, component, shape, shapeTransform, val, flags))
				{
					return true;
				}
			}
		}
		else
		{
			EntityQuery<IComponent> entityQuery = EntityManager.GetEntityQuery(type);
			(EntityLookupSystem, Box2, LookupFlags, EntityQuery<IComponent>, EntityUid?, bool) state = (this, val, flags, entityQuery, ignored, false);
			_mapManager.FindGridsIntersecting<(EntityLookupSystem, Box2, LookupFlags, EntityQuery<IComponent>, EntityUid?, bool)>(mapId, val, ref state, delegate(EntityUid uid, MapGridComponent grid, ref (EntityLookupSystem system, Box2 worldAABB, LookupFlags flags, EntityQuery<IComponent> query, EntityUid? ignored, bool found) tuple)
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				if (!tuple.system.AnyLocalComponentsIntersecting(uid, tuple.worldAABB, tuple.flags, tuple.query, tuple.ignored))
				{
					return true;
				}
				tuple.found = true;
				return false;
			}, approx: true, includeMap: false);
			if (state.Item6)
			{
				return true;
			}
			EntityUid mapOrInvalid = _map.GetMapOrInvalid(mapId);
			AnyLocalComponentsIntersecting(mapOrInvalid, val, flags, entityQuery, ignored);
		}
		return false;
	}

	public void GetEntitiesIntersecting(Type type, MapId mapId, Box2 worldAABB, HashSet<Entity<IComponent>> intersecting, LookupFlags flags = LookupFlags.All)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!(mapId == MapId.Nullspace))
		{
			SlimPolygon shape = new SlimPolygon(worldAABB);
			Transform empty = Robust.Shared.Physics.Transform.Empty;
			GetEntitiesIntersecting(type, mapId, shape, empty, intersecting, flags);
		}
	}

	public void GetEntitiesIntersecting<T>(MapId mapId, Box2Rotated worldBounds, HashSet<Entity<T>> entities, LookupFlags flags = LookupFlags.All) where T : IComponent
	{
		if (!(mapId == MapId.Nullspace))
		{
			SlimPolygon shape = new SlimPolygon(in worldBounds);
			Transform empty = Robust.Shared.Physics.Transform.Empty;
			GetEntitiesIntersecting(mapId, shape, empty, entities, flags);
		}
	}

	public void GetEntitiesIntersecting<T>(MapId mapId, Box2 worldAABB, HashSet<Entity<T>> entities, LookupFlags flags = LookupFlags.All) where T : IComponent
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!(mapId == MapId.Nullspace))
		{
			SlimPolygon shape = new SlimPolygon(worldAABB);
			Transform empty = Robust.Shared.Physics.Transform.Empty;
			GetEntitiesIntersecting(mapId, shape, empty, entities, flags);
		}
	}

	public void GetEntitiesIntersecting<T>(Type type, MapId mapId, T shape, Transform shapeTransform, HashSet<Entity<IComponent>> intersecting, LookupFlags flags = LookupFlags.All) where T : IPhysShape
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (mapId == MapId.Nullspace)
		{
			return;
		}
		Box2 worldAABB = shape.ComputeAABB(shapeTransform, 0);
		if (!UseBoundsQuery(type, ((Box2)(ref worldAABB)).Height * ((Box2)(ref worldAABB)).Width))
		{
			foreach (var allComponent in EntityManager.GetAllComponents(type, includePaused: true))
			{
				EntityUid item = allComponent.Uid;
				IComponent item2 = allComponent.Component;
				TransformComponent component = _xformQuery.GetComponent(item);
				if (IsIntersecting(mapId, item, component, shape, shapeTransform, worldAABB, flags))
				{
					intersecting.Add((Owner: item, Comp: item2));
				}
			}
			return;
		}
		EntityQuery<IComponent> entityQuery = EntityManager.GetEntityQuery(type);
		GridQueryState<IComponent, T> state = new GridQueryState<IComponent, T>(intersecting, shape, shapeTransform, this, _physics, flags, entityQuery);
		_mapManager.FindGridsIntersecting<GridQueryState<IComponent, T>>(mapId, worldAABB, ref state, delegate(EntityUid uid, MapGridComponent grid, ref GridQueryState<IComponent, T> reference)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			Transform relativePhysicsTransform2 = reference.Physics.GetRelativePhysicsTransform(reference.Transform, uid);
			Box2 localAABB2 = reference.Shape.ComputeAABB(relativePhysicsTransform2, 0);
			reference.Lookup.AddEntitiesIntersecting(uid, reference.Intersecting, reference.Shape, localAABB2, relativePhysicsTransform2, reference.Flags, reference.Query);
			return true;
		}, approx: true, includeMap: false);
		EntityUid mapOrInvalid = _map.GetMapOrInvalid(mapId);
		Transform relativePhysicsTransform = state.Physics.GetRelativePhysicsTransform(state.Transform, mapOrInvalid);
		Box2 localAABB = state.Shape.ComputeAABB(relativePhysicsTransform, 0);
		AddEntitiesIntersecting(mapOrInvalid, intersecting, shape, localAABB, relativePhysicsTransform, flags, entityQuery);
		AddContained(intersecting, flags, entityQuery);
	}

	public void GetEntitiesIntersecting<T, TShape>(MapId mapId, TShape shape, Transform shapeTransform, HashSet<Entity<T>> entities, LookupFlags flags = LookupFlags.All) where T : IComponent where TShape : IPhysShape
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (mapId == MapId.Nullspace)
		{
			return;
		}
		Box2 worldAABB = shape.ComputeAABB(shapeTransform, 0);
		if (!UseBoundsQuery<T>(((Box2)(ref worldAABB)).Height * ((Box2)(ref worldAABB)).Width))
		{
			AllEntityQueryEnumerator<T, TransformComponent> allEntityQueryEnumerator = AllEntityQuery<T, TransformComponent>();
			EntityUid uid;
			T comp;
			TransformComponent comp2;
			while (allEntityQueryEnumerator.MoveNext(out uid, out comp, out comp2))
			{
				if (IsIntersecting(mapId, uid, comp2, shape, shapeTransform, worldAABB, flags))
				{
					entities.Add((Owner: uid, Comp: comp));
				}
			}
			return;
		}
		EntityQuery<T> entityQuery = GetEntityQuery<T>();
		GridQueryState<T, TShape> state = new GridQueryState<T, TShape>(entities, shape, shapeTransform, this, _physics, flags, entityQuery);
		_mapManager.FindGridsIntersecting(mapId, worldAABB, ref state, delegate(EntityUid entityUid, MapGridComponent grid, ref GridQueryState<T, TShape> reference)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			Transform relativePhysicsTransform2 = reference.Physics.GetRelativePhysicsTransform(reference.Transform, entityUid);
			Box2 localAABB2 = reference.Shape.ComputeAABB(relativePhysicsTransform2, 0);
			reference.Lookup.AddEntitiesIntersecting(entityUid, reference.Intersecting, reference.Shape, localAABB2, relativePhysicsTransform2, reference.Flags, reference.Query);
			return true;
		}, approx: true, includeMap: false);
		EntityUid mapOrInvalid = _map.GetMapOrInvalid(mapId);
		Transform relativePhysicsTransform = state.Physics.GetRelativePhysicsTransform(state.Transform, mapOrInvalid);
		Box2 localAABB = state.Shape.ComputeAABB(relativePhysicsTransform, 0);
		AddEntitiesIntersecting(mapOrInvalid, entities, shape, localAABB, relativePhysicsTransform, flags, entityQuery);
		AddContained(entities, flags, entityQuery);
	}

	public void GetEntitiesInRange<T>(EntityCoordinates coordinates, float range, HashSet<Entity<T>> entities, LookupFlags flags = LookupFlags.All) where T : IComponent
	{
		MapCoordinates coordinates2 = _transform.ToMapCoordinates(coordinates);
		GetEntitiesInRange(coordinates2, range, entities, flags);
	}

	public HashSet<Entity<T>> GetEntitiesInRange<T>(EntityCoordinates coordinates, float range, LookupFlags flags = LookupFlags.All) where T : IComponent
	{
		HashSet<Entity<T>> hashSet = new HashSet<Entity<T>>();
		GetEntitiesInRange(coordinates, range, hashSet, flags);
		return hashSet;
	}

	public HashSet<Entity<IComponent>> GetEntitiesInRange(Type type, MapCoordinates coordinates, float range)
	{
		HashSet<Entity<IComponent>> hashSet = new HashSet<Entity<IComponent>>();
		GetEntitiesInRange(type, coordinates, range, hashSet);
		return hashSet;
	}

	public void GetEntitiesInRange(Type type, MapCoordinates coordinates, float range, HashSet<Entity<IComponent>> entities, LookupFlags flags = LookupFlags.All)
	{
		GetEntitiesInRange(type, coordinates.MapId, coordinates.Position, range, entities, flags);
	}

	public void GetEntitiesInRange<T>(MapCoordinates coordinates, float range, HashSet<Entity<T>> entities, LookupFlags flags = LookupFlags.All) where T : IComponent
	{
		GetEntitiesInRange(coordinates.MapId, coordinates.Position, range, entities, flags);
	}

	public HashSet<Entity<T>> GetEntitiesInRange<T>(MapCoordinates coordinates, float range, LookupFlags flags = LookupFlags.All) where T : IComponent
	{
		HashSet<Entity<T>> hashSet = new HashSet<Entity<T>>();
		GetEntitiesInRange(coordinates.MapId, coordinates.Position, range, hashSet, flags);
		return hashSet;
	}

	public void GetEntitiesInRange(Type type, MapId mapId, Vector2 worldPos, float range, HashSet<Entity<IComponent>> entities, LookupFlags flags = LookupFlags.All)
	{
		if (!(mapId == MapId.Nullspace))
		{
			PhysShapeCircle shape = new PhysShapeCircle(range);
			Transform shapeTransform = new Transform(worldPos, 0f);
			GetEntitiesIntersecting(type, mapId, shape, shapeTransform, entities, flags);
		}
	}

	public void GetEntitiesInRange<T>(MapId mapId, Vector2 worldPos, float range, HashSet<Entity<T>> entities, LookupFlags flags = LookupFlags.All) where T : IComponent
	{
		PhysShapeCircle shape = new PhysShapeCircle(range, worldPos);
		Transform empty = Robust.Shared.Physics.Transform.Empty;
		GetEntitiesInRange(mapId, shape, empty, entities, flags);
	}

	public void GetEntitiesInRange<T, TShape>(MapId mapId, TShape shape, Transform transform, HashSet<Entity<T>> entities, LookupFlags flags = LookupFlags.All) where T : IComponent where TShape : IPhysShape
	{
		if (!(mapId == MapId.Nullspace))
		{
			GetEntitiesIntersecting(mapId, shape, transform, entities, flags);
		}
	}

	public void GetEntitiesOnMap<TComp1>(MapId mapId, HashSet<Entity<TComp1>> entities) where TComp1 : IComponent
	{
		AllEntityQueryEnumerator<TComp1, TransformComponent> allEntityQueryEnumerator = AllEntityQuery<TComp1, TransformComponent>();
		EntityUid uid;
		TComp1 comp;
		TransformComponent comp2;
		while (allEntityQueryEnumerator.MoveNext(out uid, out comp, out comp2))
		{
			if (!(comp2.MapID != mapId))
			{
				entities.Add((Owner: uid, Comp: comp));
			}
		}
	}

	public void GetEntitiesOnMap<TComp1, TComp2>(MapId mapId, HashSet<Entity<TComp1, TComp2>> entities) where TComp1 : IComponent where TComp2 : IComponent
	{
		AllEntityQueryEnumerator<TComp1, TComp2, TransformComponent> allEntityQueryEnumerator = AllEntityQuery<TComp1, TComp2, TransformComponent>();
		EntityUid uid;
		TComp1 comp;
		TComp2 comp2;
		TransformComponent comp3;
		while (allEntityQueryEnumerator.MoveNext(out uid, out comp, out comp2, out comp3))
		{
			if (!(comp3.MapID != mapId))
			{
				entities.Add((Owner: uid, Comp1: comp, Comp2: comp2));
			}
		}
	}

	public void GetLocalEntitiesIntersecting<T>(EntityUid gridUid, Vector2i localTile, HashSet<Entity<T>> intersecting, float enlargement = -0.04f, LookupFlags flags = LookupFlags.All, MapGridComponent? gridComp = null) where T : IComponent
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		ushort tileSize = 1;
		if (_gridQuery.Resolve(gridUid, ref gridComp))
		{
			tileSize = gridComp.TileSize;
		}
		Box2 localAABB = GetLocalBounds(localTile, tileSize);
		localAABB = ((Box2)(ref localAABB)).Enlarged(enlargement);
		GetLocalEntitiesIntersecting(gridUid, localAABB, intersecting, flags);
	}

	public void GetLocalEntitiesIntersecting<T>(EntityUid gridUid, Box2 localAABB, HashSet<Entity<T>> intersecting, LookupFlags flags = LookupFlags.All) where T : IComponent
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery<T> entityQuery = GetEntityQuery<T>();
		AddLocalEntitiesIntersecting(gridUid, intersecting, localAABB, flags, entityQuery);
		AddContained(intersecting, flags, entityQuery);
	}

	public void GetLocalEntitiesIntersecting<T>(Entity<BroadphaseComponent> grid, Box2 localAABB, HashSet<Entity<T>> intersecting, EntityQuery<T> query, LookupFlags flags = LookupFlags.All) where T : IComponent
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		AddLocalEntitiesIntersecting(grid, intersecting, localAABB, flags, query, grid.Comp);
		AddContained(intersecting, flags, query);
	}

	public void GetGridEntities<TComp1>(EntityUid gridUid, HashSet<Entity<TComp1>> entities) where TComp1 : IComponent
	{
		AllEntityQueryEnumerator<TComp1, TransformComponent> allEntityQueryEnumerator = AllEntityQuery<TComp1, TransformComponent>();
		EntityUid uid;
		TComp1 comp;
		TransformComponent comp2;
		while (allEntityQueryEnumerator.MoveNext(out uid, out comp, out comp2))
		{
			if (!(comp2.GridUid != gridUid))
			{
				entities.Add((Owner: uid, Comp: comp));
			}
		}
	}

	public void GetChildEntities<TComp1>(EntityUid parentUid, HashSet<Entity<TComp1>> entities) where TComp1 : IComponent
	{
		AllEntityQueryEnumerator<TComp1, TransformComponent> allEntityQueryEnumerator = AllEntityQuery<TComp1, TransformComponent>();
		EntityUid uid;
		TComp1 comp;
		TransformComponent comp2;
		while (allEntityQueryEnumerator.MoveNext(out uid, out comp, out comp2))
		{
			if (!(comp2.ParentUid != parentUid))
			{
				entities.Add((Owner: uid, Comp: comp));
			}
		}
	}

	public void GetChildEntities<TComp1, TComp2>(EntityUid parentUid, HashSet<Entity<TComp1, TComp2>> entities) where TComp1 : IComponent where TComp2 : IComponent
	{
		AllEntityQueryEnumerator<TComp1, TComp2, TransformComponent> allEntityQueryEnumerator = AllEntityQuery<TComp1, TComp2, TransformComponent>();
		EntityUid uid;
		TComp1 comp;
		TComp2 comp2;
		TransformComponent comp3;
		while (allEntityQueryEnumerator.MoveNext(out uid, out comp, out comp2, out comp3))
		{
			if (!(comp3.ParentUid != parentUid))
			{
				entities.Add((Owner: uid, Comp1: comp, Comp2: comp2));
			}
		}
	}

	public override void Initialize()
	{
		base.Initialize();
		_broadQuery = GetEntityQuery<BroadphaseComponent>();
		_containerQuery = GetEntityQuery<ContainerManagerComponent>();
		_fixturesQuery = GetEntityQuery<FixturesComponent>();
		_mapQuery = GetEntityQuery<MapComponent>();
		_gridQuery = GetEntityQuery<MapGridComponent>();
		_metaQuery = GetEntityQuery<MetaDataComponent>();
		_physicsQuery = GetEntityQuery<PhysicsComponent>();
		_xformQuery = GetEntityQuery<TransformComponent>();
		SubscribeLocalEvent<BroadphaseComponent, EntityTerminatingEvent>(OnBroadphaseTerminating);
		SubscribeLocalEvent<BroadphaseComponent, ComponentAdd>(OnBroadphaseAdd);
		SubscribeLocalEvent<BroadphaseComponent, ComponentInit>(OnBroadphaseInit);
		SubscribeLocalEvent<GridAddEvent>(OnGridAdd);
		SubscribeLocalEvent<MapCreatedEvent>(OnMapChange);
		_transform.OnBeforeMoveEvent += OnMove;
		EntityManager.EntityInitialized += OnEntityInit;
		SubscribeLocalEvent<TransformComponent, PhysicsBodyTypeChangedEvent>(OnBodyTypeChange);
		SubscribeLocalEvent<PhysicsComponent, ComponentStartup>(OnBodyStartup);
		SubscribeLocalEvent<CollisionChangeEvent>(OnPhysicsUpdate);
	}

	private void OnBodyStartup(EntityUid uid, PhysicsComponent component, ComponentStartup args)
	{
		UpdatePhysicsBroadphase(uid, Transform(uid), component);
	}

	public override void Shutdown()
	{
		base.Shutdown();
		EntityManager.EntityInitialized -= OnEntityInit;
		_transform.OnBeforeMoveEvent -= OnMove;
	}

	private void OnBroadphaseTerminating(EntityUid uid, BroadphaseComponent component, ref EntityTerminatingEvent args)
	{
		TransformComponent component2 = _xformQuery.GetComponent(uid);
		RemoveChildrenFromTerminatingBroadphase(component2, component);
		RemComp(uid, component);
	}

	private void RemoveChildrenFromTerminatingBroadphase(TransformComponent xform, BroadphaseComponent component)
	{
		foreach (EntityUid child in xform._children)
		{
			if (!_xformQuery.TryGetComponent(child, out TransformComponent component2) || component2.GridUid == child || !component2.Broadphase.HasValue)
			{
				continue;
			}
			if (component2.Broadphase.Value.CanCollide && _fixturesQuery.TryGetComponent(child, out FixturesComponent component3))
			{
				IBroadPhase tree = (component2.Broadphase.Value.Static ? component.StaticTree : component.DynamicTree);
				foreach (Fixture value in component3.Fixtures.Values)
				{
					DestroyProxies(value, tree);
				}
			}
			component2.Broadphase = null;
			RemoveChildrenFromTerminatingBroadphase(component2, component);
		}
	}

	private void OnMapChange(MapCreatedEvent ev)
	{
		if (ev.MapId != MapId.Nullspace)
		{
			EnsureComp<BroadphaseComponent>(ev.Uid);
		}
	}

	private void OnGridAdd(GridAddEvent ev)
	{
		EnsureComp<BroadphaseComponent>(ev.EntityUid);
	}

	private void OnBroadphaseAdd(Entity<BroadphaseComponent> broadphase, ref ComponentAdd args)
	{
		broadphase.Comp.StaticSundriesTree = new DynamicTree<EntityUid>(delegate(in EntityUid value)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			return GetTreeAABB(value, broadphase.Owner);
		});
		broadphase.Comp.SundriesTree = new DynamicTree<EntityUid>(delegate(in EntityUid value)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			return GetTreeAABB(value, broadphase.Owner);
		});
	}

	private void OnBroadphaseInit(Entity<BroadphaseComponent> broadphase, ref ComponentInit args)
	{
		TransformComponent transformComponent = Transform(broadphase.Owner);
		_transform.InitializeMapUid(broadphase.Owner, transformComponent);
		if (!transformComponent.MapUid.HasValue)
		{
			return;
		}
		Entity<TransformComponent, BroadphaseComponent> broadphase2 = new Entity<TransformComponent, BroadphaseComponent>(broadphase, transformComponent, broadphase);
		TransformChildrenEnumerator childEnumerator = transformComponent.ChildEnumerator;
		EntityUid child;
		while (childEnumerator.MoveNext(out child))
		{
			if (!_broadQuery.HasComp(child))
			{
				InitializeChild(child, broadphase2);
			}
		}
	}

	private void InitializeChild(EntityUid child, Entity<TransformComponent, BroadphaseComponent> broadphase)
	{
		if ((int)LifeStage(child) <= 0)
		{
			return;
		}
		TransformComponent transformComponent = Transform(child);
		if (transformComponent.Broadphase.HasValue)
		{
			if (!transformComponent.Broadphase.Value.IsValid())
			{
				return;
			}
			if (!_broadQuery.TryGetComponent(transformComponent.Broadphase.Value.Uid, out BroadphaseComponent component))
			{
				if (_fixturesQuery.TryGetComponent(child, out FixturesComponent component2))
				{
					foreach (Fixture value in component2.Fixtures.Values)
					{
						value.ProxyCount = 0;
						value.Proxies = Array.Empty<FixtureProxy>();
					}
				}
				transformComponent.Broadphase = null;
			}
			else if (component != broadphase.Comp2)
			{
				RemoveFromEntityTree(transformComponent.Broadphase.Value.Uid, component, child, transformComponent);
			}
		}
		AddOrUpdateEntityTree(broadphase.Owner, broadphase.Comp2, broadphase.Comp1, child, transformComponent);
	}

	private Box2 GetTreeAABB(EntityUid entity, EntityUid tree)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		if (!_xformQuery.TryGetComponent(entity, out TransformComponent component))
		{
			base.Log.Error($"Entity tree contains a deleted entity? Tree: {ToPrettyString(tree)}, entity: {entity}");
			return default(Box2);
		}
		if (component.ParentUid == tree)
		{
			return GetAABBNoContainer(entity, component.LocalPosition, component.LocalRotation);
		}
		if (!_xformQuery.TryGetComponent(tree, out TransformComponent component2))
		{
			base.Log.Error($"Entity tree has no transform? Tree Uid: {tree}");
			return default(Box2);
		}
		Matrix3x2 invWorldMatrix = _transform.GetInvWorldMatrix(component2);
		Box2 worldAABB = GetWorldAABB(entity, component);
		return Matrix3Helpers.TransformBox(invWorldMatrix, ref worldAABB);
	}

	internal void CreateProxies(EntityUid uid, string fixtureId, Fixture fixture, TransformComponent xform, PhysicsComponent body)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetCurrentBroadphase(xform, out BroadphaseComponent broadphase))
		{
			(Vector2 WorldPosition, Angle WorldRotation) worldPositionRotation = _transform.GetWorldPositionRotation(xform);
			Vector2 item = worldPositionRotation.WorldPosition;
			Angle item2 = worldPositionRotation.WorldRotation;
			Transform transform = new Transform(item, item2);
			(Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix, Matrix3x2 InvWorldMatrix) worldPositionRotationMatrixWithInv = _transform.GetWorldPositionRotationMatrixWithInv(broadphase.Owner);
			Angle item3 = worldPositionRotationMatrixWithInv.WorldRotation;
			Matrix3x2 item4 = worldPositionRotationMatrixWithInv.InvWorldMatrix;
			Transform broadphaseTransform = new Transform(Vector2.Transform(transform.Position, item4), Angle.op_Implicit(transform.Quaternion2D.Angle) - item3);
			IBroadPhase tree = ((body.BodyType == BodyType.Static) ? broadphase.StaticTree : broadphase.DynamicTree);
			AddOrMoveProxies((Owner: uid, Comp1: body, Comp2: xform), fixtureId, fixture, tree, broadphaseTransform);
		}
	}

	internal void DestroyProxies(EntityUid uid, string fixtureId, Fixture fixture, TransformComponent xform, BroadphaseComponent broadphase)
	{
		if (xform.Broadphase.Value.CanCollide && !(xform.GridUid == uid))
		{
			if (fixture.ProxyCount == 0)
			{
				base.Log.Warning($"Tried to destroy fixture {fixtureId} on {ToPrettyString(uid)} that already has no proxies?");
			}
			else
			{
				IBroadPhase tree = (xform.Broadphase.Value.Static ? broadphase.StaticTree : broadphase.DynamicTree);
				DestroyProxies(fixture, tree);
			}
		}
	}

	private void OnPhysicsUpdate(ref CollisionChangeEvent ev)
	{
		TransformComponent xform = Transform(ev.BodyUid);
		UpdatePhysicsBroadphase(ev.BodyUid, xform, ev.Body);
	}

	private void OnBodyTypeChange(EntityUid uid, TransformComponent xform, ref PhysicsBodyTypeChangedEvent args)
	{
		if (args.Old == BodyType.Static || args.New == BodyType.Static)
		{
			UpdatePhysicsBroadphase(uid, xform, args.Component);
		}
	}

	private void UpdatePhysicsBroadphase(EntityUid uid, TransformComponent xform, PhysicsComponent body)
	{
		if ((int)body.LifeStage <= 3 || xform.GridUid == uid)
		{
			return;
		}
		BroadphaseData? broadphase = xform.Broadphase;
		if (!broadphase.HasValue)
		{
			return;
		}
		BroadphaseData valueOrDefault = broadphase.GetValueOrDefault();
		if (!valueOrDefault.Valid)
		{
			return;
		}
		xform.Broadphase = null;
		if (_broadQuery.TryGetComponent(valueOrDefault.Uid, out BroadphaseComponent component))
		{
			FixturesComponent fixturesComponent = Comp<FixturesComponent>(uid);
			if (valueOrDefault.CanCollide)
			{
				RemoveBroadTree(component, fixturesComponent, valueOrDefault.Static);
			}
			else
			{
				(valueOrDefault.Static ? component.StaticSundriesTree : component.SundriesTree).Remove(in uid);
			}
			if (body.CanCollide)
			{
				AddPhysicsTree(uid, valueOrDefault.Uid, component, xform, body, fixturesComponent);
			}
			else
			{
				AddOrUpdateSundriesTree(valueOrDefault.Uid, component, uid, xform, body.BodyType == BodyType.Static);
			}
		}
	}

	private void RemoveBroadTree(BroadphaseComponent lookup, FixturesComponent manager, bool staticBody)
	{
		IBroadPhase tree = (staticBody ? lookup.StaticTree : lookup.DynamicTree);
		foreach (Fixture value in manager.Fixtures.Values)
		{
			DestroyProxies(value, tree);
		}
	}

	internal void DestroyProxies(Fixture fixture, IBroadPhase tree)
	{
		HashSet<FixtureProxy> moveBuffer = _physics.MoveBuffer;
		for (int i = 0; i < fixture.ProxyCount; i++)
		{
			FixtureProxy fixtureProxy = fixture.Proxies[i];
			tree.RemoveProxy(fixtureProxy.ProxyId);
			moveBuffer.Remove(fixtureProxy);
		}
		fixture.ProxyCount = 0;
		fixture.Proxies = Array.Empty<FixtureProxy>();
	}

	private void AddPhysicsTree(EntityUid uid, EntityUid broadUid, BroadphaseComponent broadphase, TransformComponent xform, PhysicsComponent body, FixturesComponent fixtures)
	{
		TransformComponent component = _xformQuery.GetComponent(broadUid);
		if (!(component.MapID == MapId.Nullspace))
		{
			AddOrUpdatePhysicsTree(uid, broadUid, broadphase, component, xform, body, fixtures);
		}
	}

	private void AddOrUpdatePhysicsTree(EntityUid uid, EntityUid broadUid, BroadphaseComponent broadphase, TransformComponent broadphaseXform, TransformComponent xform, PhysicsComponent body, FixturesComponent manager)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		BroadphaseData valueOrDefault = xform.Broadphase.GetValueOrDefault();
		if (!xform.Broadphase.HasValue)
		{
			valueOrDefault = new BroadphaseData(broadUid, body.CanCollide, body.BodyType == BodyType.Static);
			xform.Broadphase = valueOrDefault;
		}
		IBroadPhase tree = ((body.BodyType == BodyType.Static) ? broadphase.StaticTree : broadphase.DynamicTree);
		(Vector2 WorldPosition, Angle WorldRotation) worldPositionRotation = _transform.GetWorldPositionRotation(xform);
		Vector2 item = worldPositionRotation.WorldPosition;
		Angle item2 = worldPositionRotation.WorldRotation;
		Transform transform = new Transform(item, item2);
		Transform broadphaseTransform = new Transform(Vector2.Transform(transform.Position, broadphaseXform.InvLocalMatrix), Angle.op_Implicit(transform.Quaternion2D.Angle) - broadphaseXform.LocalRotation);
		foreach (var (fixtureId, fixture2) in manager.Fixtures)
		{
			AddOrMoveProxies((Owner: uid, Comp1: body, Comp2: xform), fixtureId, fixture2, tree, broadphaseTransform);
		}
	}

	private void AddOrMoveProxies(Entity<PhysicsComponent, TransformComponent> ent, string fixtureId, Fixture fixture, IBroadPhase tree, Transform broadphaseTransform)
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		HashSet<FixtureProxy> moveBuffer = _physics.MoveBuffer;
		if (fixture.ProxyCount > 0)
		{
			for (int i = 0; i < fixture.ProxyCount; i++)
			{
				Box2 aabb = fixture.Shape.ComputeAABB(broadphaseTransform, i);
				FixtureProxy fixtureProxy = fixture.Proxies[i];
				tree.MoveProxy(fixtureProxy.ProxyId, in aabb);
				fixtureProxy.AABB = aabb;
				moveBuffer.Add(fixtureProxy);
			}
			return;
		}
		int childCount = fixture.Shape.ChildCount;
		FixtureProxy[] array = new FixtureProxy[childCount];
		for (int j = 0; j < childCount; j++)
		{
			Box2 val = fixture.Shape.ComputeAABB(broadphaseTransform, j);
			FixtureProxy proxy = new FixtureProxy(ent.Owner, ent.Comp1, ent.Comp2, val, fixtureId, fixture, j);
			proxy.ProxyId = tree.AddProxy(ref proxy);
			proxy.AABB = val;
			array[j] = proxy;
			moveBuffer.Add(proxy);
		}
		fixture.Proxies = array;
		fixture.ProxyCount = childCount;
	}

	private void AddOrUpdateSundriesTree(EntityUid broadUid, BroadphaseComponent broadphase, EntityUid uid, TransformComponent xform, bool staticBody, Box2? aabb = null)
	{
		BroadphaseData valueOrDefault = xform.Broadphase.GetValueOrDefault();
		if (!xform.Broadphase.HasValue)
		{
			valueOrDefault = new BroadphaseData(broadUid, CanCollide: false, staticBody);
			xform.Broadphase = valueOrDefault;
		}
		(staticBody ? broadphase.StaticSundriesTree : broadphase.SundriesTree).AddOrUpdate(uid, aabb);
	}

	private void OnEntityInit(Entity<MetaDataComponent> uid)
	{
		if (!_container.IsEntityOrParentInContainer(uid, uid) && !_mapQuery.HasComp(uid) && !_gridQuery.HasComp(uid))
		{
			FindAndAddToEntityTree(uid, recursive: false);
		}
	}

	private void OnMove(ref MoveEvent args)
	{
		if (args.Component.GridUid == args.Sender)
		{
			if (args.ParentChanged)
			{
				OnGridChangedMap(args);
			}
		}
		else if (!(args.Component.MapUid == args.Sender))
		{
			if (args.ParentChanged)
			{
				UpdateParent(args.Sender, args.Component);
			}
			else
			{
				UpdateEntityTree(args.Sender, args.Component);
			}
		}
	}

	private void OnGridChangedMap(MoveEvent args)
	{
		EntityUid entityId = args.OldPosition.EntityId;
		Terminating(entityId);
	}

	private void UpdateParent(EntityUid uid, TransformComponent xform)
	{
		BroadphaseComponent component = null;
		if (xform.Broadphase.HasValue)
		{
			if (!xform.Broadphase.Value.IsValid())
			{
				return;
			}
			if (!_broadQuery.TryGetComponent(xform.Broadphase.Value.Uid, out component))
			{
				if (_fixturesQuery.TryGetComponent(uid, out FixturesComponent component2))
				{
					foreach (Fixture value in component2.Fixtures.Values)
					{
						value.ProxyCount = 0;
						value.Proxies = Array.Empty<FixtureProxy>();
					}
				}
				xform.Broadphase = null;
			}
		}
		TryFindBroadphase(xform, out BroadphaseComponent broadphase);
		if (component != null && component != broadphase)
		{
			RemoveFromEntityTree(component.Owner, component, uid, xform);
		}
		if (broadphase != null)
		{
			TransformComponent component3 = _xformQuery.GetComponent(broadphase.Owner);
			AddOrUpdateEntityTree(broadphase.Owner, broadphase, component3, uid, xform);
		}
	}

	public void FindAndAddToEntityTree(EntityUid uid, bool recursive = true, TransformComponent? xform = null)
	{
		if (_xformQuery.Resolve(uid, ref xform) && TryFindBroadphase(xform, out BroadphaseComponent broadphase))
		{
			AddOrUpdateEntityTree(broadphase.Owner, broadphase, uid, xform, recursive);
		}
	}

	public void UpdateEntityTree(EntityUid uid, TransformComponent? xform = null)
	{
		if (_xformQuery.Resolve(uid, ref xform) && TryGetCurrentBroadphase(xform, out BroadphaseComponent broadphase))
		{
			AddOrUpdateEntityTree(broadphase.Owner, broadphase, uid, xform);
		}
	}

	private void AddOrUpdateEntityTree(EntityUid broadUid, BroadphaseComponent broadphase, EntityUid uid, TransformComponent xform, bool recursive = true)
	{
		TransformComponent component = _xformQuery.GetComponent(broadphase.Owner);
		AddOrUpdateEntityTree(broadUid, broadphase, component, uid, xform, recursive);
	}

	private void AddOrUpdateEntityTree(EntityUid broadUid, BroadphaseComponent broadphase, TransformComponent broadphaseXform, EntityUid uid, TransformComponent xform, bool recursive = true)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (xform.Broadphase.HasValue && !xform.Broadphase.Value.IsValid())
		{
			return;
		}
		if (!_physicsQuery.TryGetComponent(uid, out PhysicsComponent component) || !component.CanCollide)
		{
			(EntityCoordinates Coords, Angle worldRot) moverCoordinateRotation = _transform.GetMoverCoordinateRotation(uid, xform);
			EntityCoordinates item = moverCoordinateRotation.Coords;
			Angle angle = moverCoordinateRotation.worldRot - broadphaseXform.LocalRotation;
			Box2 aABBNoContainer = GetAABBNoContainer(uid, item.Position, angle);
			AddOrUpdateSundriesTree(broadUid, broadphase, uid, xform, component != null && component.BodyType == BodyType.Static, aABBNoContainer);
		}
		else
		{
			AddOrUpdatePhysicsTree(uid, broadUid, broadphase, broadphaseXform, xform, component, _fixturesQuery.GetComponent(uid));
		}
		if (xform.ChildCount == 0 || !recursive)
		{
			return;
		}
		if (!_containerQuery.HasComponent(uid))
		{
			foreach (EntityUid child in xform._children)
			{
				TransformComponent component2 = _xformQuery.GetComponent(child);
				AddOrUpdateEntityTree(broadUid, broadphase, broadphaseXform, child, component2, recursive);
			}
			return;
		}
		foreach (EntityUid child2 in xform._children)
		{
			if ((_metaQuery.GetComponent(child2).Flags & MetaDataFlags.InContainer) == 0)
			{
				TransformComponent component3 = _xformQuery.GetComponent(child2);
				AddOrUpdateEntityTree(broadUid, broadphase, broadphaseXform, child2, component3, recursive);
			}
		}
	}

	public void RemoveFromEntityTree(EntityUid uid, TransformComponent xform)
	{
		if (TryGetCurrentBroadphase(xform, out BroadphaseComponent broadphase))
		{
			RemoveFromEntityTree(broadphase.Owner, broadphase, uid, xform);
		}
	}

	private void RemoveFromEntityTree(EntityUid broadUid, BroadphaseComponent broadphase, EntityUid uid, TransformComponent xform, bool recursive = true)
	{
		BroadphaseData? broadphase2 = xform.Broadphase;
		if (!broadphase2.HasValue)
		{
			return;
		}
		BroadphaseData valueOrDefault = broadphase2.GetValueOrDefault();
		if (!valueOrDefault.Valid)
		{
			return;
		}
		if (valueOrDefault.Uid != broadUid)
		{
			broadUid = valueOrDefault.Uid;
		}
		if (valueOrDefault.CanCollide)
		{
			RemoveBroadTree(broadphase, _fixturesQuery.GetComponent(uid), valueOrDefault.Static);
		}
		else if (valueOrDefault.Static)
		{
			broadphase.StaticSundriesTree.Remove(in uid);
		}
		else
		{
			broadphase.SundriesTree.Remove(in uid);
		}
		xform.Broadphase = null;
		if (!recursive)
		{
			return;
		}
		foreach (EntityUid child in xform._children)
		{
			RemoveFromEntityTree(broadUid, broadphase, child, _xformQuery.GetComponent(child));
		}
	}

	public bool TryGetCurrentBroadphase(TransformComponent xform, [NotNullWhen(true)] out BroadphaseComponent? broadphase)
	{
		broadphase = null;
		BroadphaseData? broadphase2 = xform.Broadphase;
		if (broadphase2.HasValue)
		{
			BroadphaseData valueOrDefault = broadphase2.GetValueOrDefault();
			if (valueOrDefault.Valid)
			{
				if (!_broadQuery.TryGetComponent(valueOrDefault.Uid, out broadphase))
				{
					if (_fixturesQuery.TryGetComponent(xform.Owner, out FixturesComponent component))
					{
						foreach (Fixture value in component.Fixtures.Values)
						{
							value.ProxyCount = 0;
							value.Proxies = Array.Empty<FixtureProxy>();
						}
					}
					xform.Broadphase = null;
					return false;
				}
				return true;
			}
		}
		return false;
	}

	public BroadphaseComponent? GetCurrentBroadphase(TransformComponent xform)
	{
		TryGetCurrentBroadphase(xform, out BroadphaseComponent broadphase);
		return broadphase;
	}

	public BroadphaseComponent? FindBroadphase(EntityUid uid)
	{
		TryFindBroadphase(uid, out BroadphaseComponent broadphase);
		return broadphase;
	}

	public bool TryFindBroadphase(EntityUid uid, [NotNullWhen(true)] out BroadphaseComponent? broadphase)
	{
		return TryFindBroadphase(_xformQuery.GetComponent(uid), out broadphase);
	}

	public bool TryFindBroadphase(TransformComponent xform, [NotNullWhen(true)] out BroadphaseComponent? broadphase)
	{
		if (xform.MapID == MapId.Nullspace || _container.IsEntityOrParentInContainer(xform.Owner, null, xform))
		{
			broadphase = null;
			return false;
		}
		EntityUid parentUid = xform.ParentUid;
		while (parentUid.IsValid())
		{
			if (_broadQuery.TryGetComponent(parentUid, out broadphase))
			{
				return true;
			}
			parentUid = _xformQuery.GetComponent(parentUid).ParentUid;
		}
		broadphase = null;
		return false;
	}

	public Box2 GetAABB(EntityUid uid, Vector2 position, Angle angle, TransformComponent xform, EntityQuery<TransformComponent> xformQuery)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (_container.TryGetOuterContainer(uid, xform, out BaseContainer container, xformQuery))
		{
			return GetAABBNoContainer(container.Owner, position, angle);
		}
		return GetAABBNoContainer(uid, position, angle);
	}

	public Box2 GetAABBNoContainer(EntityUid uid, Vector2 position, Angle angle)
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (_fixturesQuery.TryGetComponent(uid, out FixturesComponent component))
		{
			Transform transform = new Transform(position, angle);
			Unsafe.SkipInit(out Box2 result);
			((Box2)(ref result))._002Ector(transform.Position, transform.Position);
			{
				foreach (Fixture value in component.Fixtures.Values)
				{
					for (int i = 0; i < value.Shape.ChildCount; i++)
					{
						Box2 val = value.Shape.ComputeAABB(transform, i);
						result = ((Box2)(ref result)).Union(ref val);
					}
				}
				return result;
			}
		}
		WorldAABBEvent args = new WorldAABBEvent
		{
			AABB = new Box2(position, position)
		};
		RaiseLocalEvent(uid, ref args);
		return args.AABB;
	}

	public Box2 GetWorldAABB(EntityUid uid, TransformComponent? xform = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery<TransformComponent> entityQuery = GetEntityQuery<TransformComponent>();
		if (xform == null)
		{
			xform = entityQuery.GetComponent(uid);
		}
		var (position, angle) = _transform.GetWorldPositionRotation(xform, entityQuery);
		return GetAABB(uid, position, angle, xform, entityQuery);
	}

	private void AddLocalEntitiesIntersecting(EntityUid lookupUid, HashSet<EntityUid> intersecting, Box2 localAABB, LookupFlags flags, BroadphaseComponent? lookup = null)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (_broadQuery.Resolve(lookupUid, ref lookup))
		{
			SlimPolygon shape = new SlimPolygon(localAABB);
			AddEntitiesIntersecting(lookupUid, intersecting, shape, localAABB, Robust.Shared.Physics.Transform.Empty, flags, lookup);
		}
	}

	private void AddLocalEntitiesIntersecting(EntityUid lookupUid, HashSet<EntityUid> intersecting, Box2Rotated localBounds, LookupFlags flags, BroadphaseComponent? lookup = null)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (_broadQuery.Resolve(lookupUid, ref lookup))
		{
			SlimPolygon shape = new SlimPolygon(in localBounds);
			Box2 localAABB = ((Box2Rotated)(ref localBounds)).CalcBoundingBox();
			AddEntitiesIntersecting(lookupUid, intersecting, shape, localAABB, Robust.Shared.Physics.Transform.Empty, flags);
		}
	}

	public bool AnyLocalEntitiesIntersecting(EntityUid lookupUid, Box2 localAABB, LookupFlags flags, EntityUid? ignored = null, BroadphaseComponent? lookup = null)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (!_broadQuery.Resolve(lookupUid, ref lookup))
		{
			return false;
		}
		SlimPolygon shape = new SlimPolygon(localAABB);
		return AnyEntitiesIntersecting(lookupUid, shape, localAABB, Robust.Shared.Physics.Transform.Empty, flags, ignored, lookup);
	}

	public HashSet<EntityUid> GetLocalEntitiesIntersecting(EntityUid gridId, Vector2i gridIndices, float enlargement = -0.04f, LookupFlags flags = LookupFlags.All, MapGridComponent? gridComp = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
		GetLocalEntitiesIntersecting(gridId, gridIndices, hashSet, enlargement, flags, gridComp);
		return hashSet;
	}

	public void GetLocalEntitiesIntersecting(EntityUid gridUid, IPhysShape shape, Transform localTransform, HashSet<EntityUid> intersecting, LookupFlags flags = LookupFlags.All, BroadphaseComponent? lookup = null)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Box2 localAABB = shape.ComputeAABB(localTransform, 0);
		AddEntitiesIntersecting(gridUid, intersecting, shape, localAABB, localTransform, flags, lookup);
		AddContained(intersecting, flags);
	}

	public void GetLocalEntitiesIntersecting(EntityUid gridUid, Vector2i localTile, HashSet<EntityUid> intersecting, float enlargement = -0.04f, LookupFlags flags = LookupFlags.All, MapGridComponent? gridComp = null)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		ushort tileSize = 1;
		if (_gridQuery.Resolve(gridUid, ref gridComp))
		{
			tileSize = gridComp.TileSize;
		}
		Box2 localAABB = GetLocalBounds(localTile, tileSize);
		localAABB = ((Box2)(ref localAABB)).Enlarged(enlargement);
		GetLocalEntitiesIntersecting(gridUid, localAABB, intersecting, flags);
	}

	public void GetLocalEntitiesIntersecting(EntityUid gridUid, Box2 localAABB, HashSet<EntityUid> intersecting, LookupFlags flags = LookupFlags.All)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		AddLocalEntitiesIntersecting(gridUid, intersecting, localAABB, flags);
		AddContained(intersecting, flags);
	}

	public void GetLocalEntitiesIntersecting(EntityUid gridUid, Box2Rotated localBounds, HashSet<EntityUid> intersecting, LookupFlags flags = LookupFlags.All)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		AddLocalEntitiesIntersecting(gridUid, intersecting, localBounds, flags);
		AddContained(intersecting, flags);
	}

	public HashSet<EntityUid> GetLocalEntitiesIntersecting(EntityUid gridId, IEnumerable<Vector2i> gridIndices, LookupFlags flags = LookupFlags.All)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
		if (!_gridQuery.TryGetComponent(gridId, out MapGridComponent component))
		{
			return hashSet;
		}
		foreach (Vector2i gridIndex in gridIndices)
		{
			GetLocalEntitiesIntersecting(gridId, gridIndex, hashSet, -0.04f, flags, component);
		}
		return hashSet;
	}

	public HashSet<EntityUid> GetLocalEntitiesIntersecting(BroadphaseComponent lookup, Box2 localAABB, LookupFlags flags = LookupFlags.All)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
		AddLocalEntitiesIntersecting(lookup.Owner, hashSet, localAABB, flags, lookup);
		AddContained(hashSet, flags);
		return hashSet;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IEnumerable<EntityUid> GetLocalEntitiesIntersecting(TileRef tileRef, float enlargement = -0.04f, LookupFlags flags = LookupFlags.All)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return GetLocalEntitiesIntersecting(tileRef.GridUid, tileRef.GridIndices, enlargement, flags);
	}
}
