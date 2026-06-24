// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.EntityLookupSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Collections;
using Robust.Shared.Containers;
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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.GameObjects;

public sealed class EntityLookupSystem : EntitySystem
{
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
  private Robust.Shared.GameObjects.EntityQuery<BroadphaseComponent> _broadQuery;
  private Robust.Shared.GameObjects.EntityQuery<ContainerManagerComponent> _containerQuery;
  private Robust.Shared.GameObjects.EntityQuery<FixturesComponent> _fixturesQuery;
  private Robust.Shared.GameObjects.EntityQuery<MapComponent> _mapQuery;
  private Robust.Shared.GameObjects.EntityQuery<MapGridComponent> _gridQuery;
  private Robust.Shared.GameObjects.EntityQuery<MetaDataComponent> _metaQuery;
  private Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> _physicsQuery;
  private Robust.Shared.GameObjects.EntityQuery<TransformComponent> _xformQuery;
  public const float TileEnlargementRadius = -0.04f;
  public const float LookupEpsilon = 1.401298E-44f;
  public const LookupFlags DefaultFlags = LookupFlags.All;

  private void RecursiveAdd(EntityUid uid, ref ValueList<EntityUid> toAdd)
  {
    TransformComponent component;
    if (!this._xformQuery.TryGetComponent(uid, out component))
    {
      this.Log.Error($"Encountered deleted entity {uid} while performing entity lookup.");
    }
    else
    {
      toAdd.Add(uid);
      foreach (EntityUid child in component._children)
        this.RecursiveAdd(child, ref toAdd);
    }
  }

  private void AddContained(HashSet<EntityUid> intersecting, LookupFlags flags)
  {
    if ((flags & LookupFlags.Contained) == LookupFlags.None || intersecting.Count == 0)
      return;
    ValueList<EntityUid> toAdd = new ValueList<EntityUid>();
    foreach (EntityUid uid in intersecting)
    {
      ContainerManagerComponent component;
      if (this._containerQuery.TryGetComponent(uid, out component))
      {
        foreach (BaseContainer allContainer in this._container.GetAllContainers(uid, component))
        {
          foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) allContainer.ContainedEntities)
            this.RecursiveAdd(containedEntity, ref toAdd);
        }
      }
    }
    Span<EntityUid> span = toAdd.Span;
    for (int index = 0; index < span.Length; ++index)
    {
      EntityUid entityUid = span[index];
      intersecting.Add(entityUid);
    }
  }

  private void AddEntitiesIntersecting<T>(
    MapId mapId,
    HashSet<EntityUid> intersecting,
    T shape,
    Robust.Shared.Physics.Transform shapeTransform,
    LookupFlags flags)
    where T : IPhysShape
  {
    Box2 aabb1 = shape.ComputeAABB(shapeTransform, 0);
    EntityLookupSystem.EntityQueryState<T> state1 = new EntityLookupSystem.EntityQueryState<T>(intersecting, shape, shapeTransform, this._fixtures, this, this._physics, this._manifoldManager, this._fixturesQuery, flags);
    this._mapManager.FindGridsIntersecting<EntityLookupSystem.EntityQueryState<T>>(mapId, aabb1, ref state1, (GridCallback<EntityLookupSystem.EntityQueryState<T>>) ((EntityUid uid, MapGridComponent _, ref EntityLookupSystem.EntityQueryState<T> state) =>
    {
      Robust.Shared.Physics.Transform physicsTransform = state.Physics.GetRelativePhysicsTransform(state.Transform, (Entity<TransformComponent>) uid);
      Box2 aabb2 = state.Shape.ComputeAABB(physicsTransform, 0);
      state.Lookup.AddEntitiesIntersecting<T>(uid, state.Intersecting, state.Shape, aabb2, physicsTransform, state.Flags);
      return true;
    }), true, false);
    EntityUid mapOrInvalid = this._map.GetMapOrInvalid(new MapId?(mapId));
    Robust.Shared.Physics.Transform physicsTransform1 = state1.Physics.GetRelativePhysicsTransform(state1.Transform, (Entity<TransformComponent>) mapOrInvalid);
    Box2 aabb3 = state1.Shape.ComputeAABB(physicsTransform1, 0);
    this.AddEntitiesIntersecting<T>(mapOrInvalid, intersecting, shape, aabb3, physicsTransform1, flags);
    this.AddContained(intersecting, flags);
  }

  private void AddEntitiesIntersecting<T>(
    EntityUid lookupUid,
    HashSet<EntityUid> intersecting,
    T shape,
    Box2 localAABB,
    Robust.Shared.Physics.Transform localShapeTransform,
    LookupFlags flags,
    BroadphaseComponent? lookup = null)
    where T : IPhysShape
  {
    if (!this._broadQuery.Resolve(lookupUid, ref lookup))
      return;
    EntityLookupSystem.EntityQueryState<T> state = new EntityLookupSystem.EntityQueryState<T>(intersecting, shape, localShapeTransform, this._fixtures, this, this._physics, this._manifoldManager, this._fixturesQuery, flags);
    if ((flags & LookupFlags.Dynamic) != LookupFlags.None)
      lookup.DynamicTree.QueryAabb<EntityLookupSystem.EntityQueryState<T>>(ref state, new DynamicTree<FixtureProxy>.QueryCallbackDelegate<EntityLookupSystem.EntityQueryState<T>>(PhysicsQuery), localAABB, true);
    if ((flags & LookupFlags.Static) != LookupFlags.None)
      lookup.StaticTree.QueryAabb<EntityLookupSystem.EntityQueryState<T>>(ref state, new DynamicTree<FixtureProxy>.QueryCallbackDelegate<EntityLookupSystem.EntityQueryState<T>>(PhysicsQuery), localAABB, true);
    if ((flags & LookupFlags.StaticSundries) == LookupFlags.StaticSundries)
      lookup.StaticSundriesTree.QueryAabb<EntityLookupSystem.EntityQueryState<T>>(ref state, new DynamicTree<EntityUid>.QueryCallbackDelegate<EntityLookupSystem.EntityQueryState<T>>(SundriesQuery), localAABB, true);
    if ((flags & LookupFlags.Sundries) == LookupFlags.None)
      return;
    lookup.SundriesTree.QueryAabb<EntityLookupSystem.EntityQueryState<T>>(ref state, new DynamicTree<EntityUid>.QueryCallbackDelegate<EntityLookupSystem.EntityQueryState<T>>(SundriesQuery), localAABB, true);

    static bool PhysicsQuery(
      ref EntityLookupSystem.EntityQueryState<T> state,
      in FixtureProxy value)
    {
      if ((state.Flags & LookupFlags.Sensors) == 0 && !value.Fixture.Hard)
        return true;
      if ((state.Flags & LookupFlags.Approximate) == 0)
      {
        Robust.Shared.Physics.Transform xfB = state.Physics.GetLocalPhysicsTransform(value.Entity);
        if (!state.Manifolds.TestOverlap<T, IPhysShape>(state.Shape, 0, value.Fixture.Shape, value.ChildIndex, state.Transform, in xfB))
          return true;
      }
      state.Intersecting.Add(value.Entity);
      return true;
    }

    static bool SundriesQuery(ref EntityLookupSystem.EntityQueryState<T> state, in EntityUid value)
    {
      if ((state.Flags & LookupFlags.Approximate) != 0)
      {
        state.Intersecting.Add(value);
        return true;
      }
      Robust.Shared.Physics.Transform xfB = state.Physics.GetLocalPhysicsTransform(value);
      FixturesComponent component;
      if (state.FixturesQuery.TryGetComponent(value, out component))
      {
        bool flag1 = (state.Flags & LookupFlags.Sensors) != 0;
        bool flag2 = false;
        foreach (Fixture fixture in component.Fixtures.Values)
        {
          if (flag1 || fixture.Hard)
          {
            flag2 = true;
            for (int indexB = 0; indexB < fixture.Shape.ChildCount; ++indexB)
            {
              if (state.Manifolds.TestOverlap<T, IPhysShape>(state.Shape, 0, fixture.Shape, indexB, state.Transform, in xfB))
              {
                state.Intersecting.Add(value);
                return true;
              }
            }
          }
        }
        if (flag2)
          return true;
      }
      if (state.Fixtures.TestPoint<T>(state.Shape, state.Transform, xfB.Position))
        state.Intersecting.Add(value);
      return true;
    }
  }

  private bool AnyEntitiesIntersecting<T>(
    MapId mapId,
    T shape,
    Robust.Shared.Physics.Transform shapeTransform,
    LookupFlags flags,
    EntityUid? ignored = null)
    where T : IPhysShape
  {
    Box2 aabb1 = shape.ComputeAABB(shapeTransform, 0);
    EntityLookupSystem.AnyEntityQueryState<T> state1 = new EntityLookupSystem.AnyEntityQueryState<T>(false, ignored, shape, shapeTransform, this._fixtures, this, this._physics, this._manifoldManager, this._fixturesQuery, flags);
    this._mapManager.FindGridsIntersecting<EntityLookupSystem.AnyEntityQueryState<T>>(mapId, aabb1, ref state1, (GridCallback<EntityLookupSystem.AnyEntityQueryState<T>>) ((EntityUid uid, MapGridComponent _, ref EntityLookupSystem.AnyEntityQueryState<T> state) =>
    {
      Robust.Shared.Physics.Transform physicsTransform = state.Physics.GetRelativePhysicsTransform(state.Transform, (Entity<TransformComponent>) uid);
      Box2 aabb2 = state.Shape.ComputeAABB(physicsTransform, 0);
      if (!state.Lookup.AnyEntitiesIntersecting<T>(uid, state.Shape, aabb2, physicsTransform, state.Flags, state.Ignored))
        return true;
      state.Found = true;
      return false;
    }), true, false);
    if (!state1.Found)
    {
      EntityUid mapOrInvalid = this._map.GetMapOrInvalid(new MapId?(mapId));
      Robust.Shared.Physics.Transform physicsTransform = state1.Physics.GetRelativePhysicsTransform(state1.Transform, (Entity<TransformComponent>) mapOrInvalid);
      Box2 aabb3 = state1.Shape.ComputeAABB(physicsTransform, 0);
      state1.Found = this.AnyEntitiesIntersecting<T>(mapOrInvalid, shape, aabb3, physicsTransform, flags, ignored);
    }
    return state1.Found;
  }

  private bool AnyEntitiesIntersecting<T>(
    EntityUid lookupUid,
    T shape,
    Box2 localAABB,
    Robust.Shared.Physics.Transform shapeTransform,
    LookupFlags flags,
    EntityUid? ignored = null,
    BroadphaseComponent? lookup = null)
    where T : IPhysShape
  {
    if (!this._broadQuery.Resolve(lookupUid, ref lookup))
      return false;
    EntityLookupSystem.AnyEntityQueryState<T> state = new EntityLookupSystem.AnyEntityQueryState<T>(false, ignored, shape, shapeTransform, this._fixtures, this, this._physics, this._manifoldManager, this._fixturesQuery, flags);
    if ((flags & LookupFlags.Dynamic) != LookupFlags.None)
    {
      lookup.DynamicTree.QueryAabb<EntityLookupSystem.AnyEntityQueryState<T>>(ref state, new DynamicTree<FixtureProxy>.QueryCallbackDelegate<EntityLookupSystem.AnyEntityQueryState<T>>(PhysicsQuery), localAABB, true);
      if (state.Found)
        return true;
    }
    if ((flags & LookupFlags.Static) != LookupFlags.None)
    {
      lookup.StaticTree.QueryAabb<EntityLookupSystem.AnyEntityQueryState<T>>(ref state, new DynamicTree<FixtureProxy>.QueryCallbackDelegate<EntityLookupSystem.AnyEntityQueryState<T>>(PhysicsQuery), localAABB, true);
      if (state.Found)
        return true;
    }
    if ((flags & LookupFlags.StaticSundries) == LookupFlags.StaticSundries)
    {
      lookup.StaticSundriesTree.QueryAabb<EntityLookupSystem.AnyEntityQueryState<T>>(ref state, new DynamicTree<EntityUid>.QueryCallbackDelegate<EntityLookupSystem.AnyEntityQueryState<T>>(SundriesQuery), localAABB, true);
      if (state.Found)
        return true;
    }
    if ((flags & LookupFlags.Sundries) != LookupFlags.None)
      lookup.SundriesTree.QueryAabb<EntityLookupSystem.AnyEntityQueryState<T>>(ref state, new DynamicTree<EntityUid>.QueryCallbackDelegate<EntityLookupSystem.AnyEntityQueryState<T>>(SundriesQuery), localAABB, true);
    return state.Found;

    static bool PhysicsQuery(
      ref EntityLookupSystem.AnyEntityQueryState<T> state,
      in FixtureProxy value)
    {
      EntityUid? ignored = state.Ignored;
      EntityUid entity = value.Entity;
      if ((ignored.HasValue ? (ignored.GetValueOrDefault() == entity ? 1 : 0) : 0) != 0 || (state.Flags & LookupFlags.Sensors) == 0 && !value.Fixture.Hard)
        return true;
      if ((state.Flags & LookupFlags.Approximate) == 0)
      {
        Robust.Shared.Physics.Transform xfB = state.Physics.GetLocalPhysicsTransform(value.Entity);
        if (!state.Manifolds.TestOverlap<T, IPhysShape>(state.Shape, 0, value.Fixture.Shape, value.ChildIndex, state.Transform, in xfB))
          return true;
      }
      state.Found = true;
      return false;
    }

    static bool SundriesQuery(
      ref EntityLookupSystem.AnyEntityQueryState<T> state,
      in EntityUid value)
    {
      EntityUid? ignored = state.Ignored;
      EntityUid entityUid = value;
      if ((ignored.HasValue ? (ignored.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0)
        return true;
      if ((state.Flags & LookupFlags.Approximate) != 0)
      {
        state.Found = true;
        return false;
      }
      Robust.Shared.Physics.Transform xfB = state.Physics.GetLocalPhysicsTransform(value);
      FixturesComponent component;
      if (state.FixturesQuery.TryGetComponent(value, out component))
      {
        bool flag1 = (state.Flags & LookupFlags.Sensors) != 0;
        bool flag2 = false;
        foreach (Fixture fixture in component.Fixtures.Values)
        {
          if (flag1 || fixture.Hard)
          {
            flag2 = true;
            for (int indexB = 0; indexB < fixture.Shape.ChildCount; ++indexB)
            {
              if (state.Manifolds.TestOverlap<T, IPhysShape>(state.Shape, 0, fixture.Shape, indexB, state.Transform, in xfB))
              {
                state.Found = true;
                return false;
              }
            }
          }
        }
        if (flag2)
          return true;
      }
      if (!state.Fixtures.TestPoint<T>(state.Shape, state.Transform, xfB.Position))
        return true;
      state.Found = true;
      return false;
    }
  }

  private bool AnyEntitiesIntersecting(
    EntityUid lookupUid,
    Box2Rotated worldBounds,
    LookupFlags flags,
    EntityUid? ignored = null)
  {
    Matrix3x2 transform = this._transform.GetInvWorldMatrix(lookupUid);
    Box2 aabb;
    SlimPolygon shape = new SlimPolygon(in worldBounds, in transform, out aabb);
    return this.AnyEntitiesIntersecting<SlimPolygon>(lookupUid, shape, aabb, Robust.Shared.Physics.Transform.Empty, flags, ignored);
  }

  public IEnumerable<EntityUid> GetEntitiesInArc(
    EntityCoordinates coordinates,
    float range,
    Angle direction,
    float arcWidth,
    LookupFlags flags = LookupFlags.All)
  {
    return this.GetEntitiesInArc(this._transform.ToMapCoordinates(coordinates), range, direction, arcWidth, flags);
  }

  public IEnumerable<EntityUid> GetEntitiesInArc(
    MapCoordinates coordinates,
    float range,
    Angle direction,
    float arcWidth,
    LookupFlags flags = LookupFlags.All)
  {
    foreach (EntityUid uid in this.GetEntitiesInRange(coordinates, range * 2f, flags))
    {
      Angle angle;
      // ISSUE: explicit constructor call
      ((Angle) ref angle).\u002Ector(this._transform.GetWorldPosition(uid) - coordinates.Position);
      if (((Angle) ref angle).Degrees < ((Angle) ref direction).Degrees + (double) arcWidth / 2.0 && ((Angle) ref angle).Degrees > ((Angle) ref direction).Degrees - (double) arcWidth / 2.0)
        yield return uid;
    }
  }

  public bool AnyEntitiesIntersecting(MapId mapId, Box2 worldAABB, LookupFlags flags = LookupFlags.All)
  {
    if (mapId == MapId.Nullspace)
      return false;
    SlimPolygon shape = new SlimPolygon(worldAABB);
    return this.AnyEntitiesIntersecting<SlimPolygon>(mapId, shape, Robust.Shared.Physics.Transform.Empty, flags);
  }

  public HashSet<EntityUid> GetEntitiesIntersecting(MapId mapId, Box2 worldAABB, LookupFlags flags = LookupFlags.All)
  {
    HashSet<EntityUid> intersecting = new HashSet<EntityUid>();
    this.GetEntitiesIntersecting(mapId, worldAABB, intersecting, flags);
    return intersecting;
  }

  public void GetEntitiesIntersecting(
    MapId mapId,
    Box2 worldAABB,
    HashSet<EntityUid> intersecting,
    LookupFlags flags = LookupFlags.All)
  {
    if (mapId == MapId.Nullspace)
      return;
    SlimPolygon shape = new SlimPolygon(worldAABB);
    this.AddEntitiesIntersecting<SlimPolygon>(mapId, intersecting, shape, Robust.Shared.Physics.Transform.Empty, flags);
  }

  public bool AnyEntitiesIntersecting(MapId mapId, Box2Rotated worldBounds, LookupFlags flags = LookupFlags.All)
  {
    SlimPolygon shape = new SlimPolygon(in worldBounds);
    return this.AnyEntitiesIntersecting<SlimPolygon>(mapId, shape, Robust.Shared.Physics.Transform.Empty, flags);
  }

  public HashSet<EntityUid> GetEntitiesIntersecting(
    MapId mapId,
    Box2Rotated worldBounds,
    LookupFlags flags = LookupFlags.All)
  {
    HashSet<EntityUid> intersecting = new HashSet<EntityUid>();
    SlimPolygon shape = new SlimPolygon(in worldBounds);
    this.AddEntitiesIntersecting<SlimPolygon>(mapId, intersecting, shape, Robust.Shared.Physics.Transform.Empty, flags);
    return intersecting;
  }

  public bool AnyEntitiesIntersecting(EntityUid uid, LookupFlags flags = LookupFlags.All)
  {
    return this.AnyEntitiesInRange(uid, 1.401298E-44f, flags);
  }

  public bool AnyEntitiesInRange(EntityUid uid, float range, LookupFlags flags = LookupFlags.All)
  {
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(uid);
    if (mapCoordinates.MapId == MapId.Nullspace)
      return false;
    PhysShapeCircle shape = new PhysShapeCircle(range, mapCoordinates.Position);
    return this.AnyEntitiesIntersecting<PhysShapeCircle>(mapCoordinates.MapId, shape, Robust.Shared.Physics.Transform.Empty, flags, new EntityUid?(uid));
  }

  public HashSet<EntityUid> GetEntitiesInRange(EntityUid uid, float range, LookupFlags flags = LookupFlags.All)
  {
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(uid);
    if (mapCoordinates.MapId == MapId.Nullspace)
      return new HashSet<EntityUid>();
    HashSet<EntityUid> entitiesInRange = this.GetEntitiesInRange(mapCoordinates, range, flags);
    entitiesInRange.Remove(uid);
    return entitiesInRange;
  }

  public void GetEntitiesInRange(
    EntityUid uid,
    float range,
    HashSet<EntityUid> entities,
    LookupFlags flags = LookupFlags.All)
  {
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(uid);
    if (mapCoordinates.MapId == MapId.Nullspace)
      return;
    this.GetEntitiesInRange(mapCoordinates.MapId, mapCoordinates.Position, range, entities, flags);
    entities.Remove(uid);
  }

  public void GetEntitiesIntersecting(
    EntityUid uid,
    HashSet<EntityUid> intersecting,
    LookupFlags flags = LookupFlags.All)
  {
    TransformComponent component = this._xformQuery.GetComponent(uid);
    MapId mapId = component.MapID;
    if (mapId == MapId.Nullspace)
      return;
    (Vector2 vector2, Angle angle) = this._transform.GetWorldPositionRotation(component);
    Box2 aabbNoContainer = this.GetAABBNoContainer(uid, vector2, angle);
    int num = intersecting.Contains(uid) ? 1 : 0;
    Robust.Shared.Physics.Transform transform = new Robust.Shared.Physics.Transform(vector2, angle);
    (EntityUid, Robust.Shared.Physics.Transform, HashSet<EntityUid>, Robust.Shared.GameObjects.EntityQuery<FixturesComponent>, EntityLookupSystem, SharedPhysicsSystem, LookupFlags) state1 = (uid, transform, intersecting, this._fixturesQuery, this, this._physics, flags);
    this._mapManager.FindGridsIntersecting<(EntityUid, Robust.Shared.Physics.Transform, HashSet<EntityUid>, Robust.Shared.GameObjects.EntityQuery<FixturesComponent>, EntityLookupSystem, SharedPhysicsSystem, LookupFlags)>(mapId, aabbNoContainer, ref state1, (GridCallback<(EntityUid, Robust.Shared.Physics.Transform, HashSet<EntityUid>, Robust.Shared.GameObjects.EntityQuery<FixturesComponent>, EntityLookupSystem, SharedPhysicsSystem, LookupFlags)>) ((EntityUid gridUid, MapGridComponent grid, ref (EntityUid entity, Robust.Shared.Physics.Transform transform, HashSet<EntityUid> intersecting, Robust.Shared.GameObjects.EntityQuery<FixturesComponent> fixturesQuery, EntityLookupSystem lookup, SharedPhysicsSystem physics, LookupFlags flags) state) =>
    {
      EntityIntersectingQuery(gridUid, state);
      return true;
    }), true, false);
    EntityIntersectingQuery(this._map.GetMapOrInvalid(new MapId?(mapId)), state1);
    if (num != 0)
      return;
    intersecting.Remove(uid);

    static void EntityIntersectingQuery(
      EntityUid lookupUid,
      (EntityUid entity, Robust.Shared.Physics.Transform shapeTransform, HashSet<EntityUid> intersecting, Robust.Shared.GameObjects.EntityQuery<FixturesComponent> fixturesQuery, EntityLookupSystem lookup, SharedPhysicsSystem physics, LookupFlags flags) state)
    {
      Robust.Shared.Physics.Transform physicsTransform = state.physics.GetRelativePhysicsTransform(state.shapeTransform, (Entity<TransformComponent>) lookupUid);
      FixturesComponent component;
      if (state.fixturesQuery.TryGetComponent(state.entity, out component))
      {
        foreach (Fixture fixture in component.Fixtures.Values)
        {
          if (fixture.Hard || (state.flags & LookupFlags.Sensors) != LookupFlags.None)
          {
            Box2 aabb = fixture.Shape.ComputeAABB(physicsTransform, 0);
            state.lookup.AddEntitiesIntersecting<IPhysShape>(lookupUid, state.intersecting, fixture.Shape, aabb, physicsTransform, state.flags);
          }
        }
      }
      else
      {
        PhysShapeCircle shape = new PhysShapeCircle(1.401298E-44f);
        Box2 aabb = shape.ComputeAABB(physicsTransform, 0);
        state.lookup.AddEntitiesIntersecting<PhysShapeCircle>(lookupUid, state.intersecting, shape, aabb, physicsTransform, state.flags);
      }
    }
  }

  public HashSet<EntityUid> GetEntitiesIntersecting(EntityUid uid, LookupFlags flags = LookupFlags.All)
  {
    MapId mapId = this._xformQuery.GetComponent(uid).MapID;
    HashSet<EntityUid> intersecting = new HashSet<EntityUid>();
    MapId nullspace = MapId.Nullspace;
    if (mapId == nullspace)
      return intersecting;
    this.GetEntitiesIntersecting(uid, intersecting, flags);
    return intersecting;
  }

  public bool AnyEntitiesIntersecting(EntityCoordinates coordinates, LookupFlags flags = LookupFlags.All)
  {
    return coordinates.IsValid((IEntityManager) this.EntityManager) && this.AnyEntitiesIntersecting(this._transform.ToMapCoordinates(coordinates), flags);
  }

  public bool AnyEntitiesInRange(EntityCoordinates coordinates, float range, LookupFlags flags = LookupFlags.All)
  {
    return coordinates.IsValid((IEntityManager) this.EntityManager) && this.AnyEntitiesInRange(this._transform.ToMapCoordinates(coordinates), range, flags);
  }

  public HashSet<EntityUid> GetEntitiesIntersecting(
    EntityCoordinates coordinates,
    LookupFlags flags = LookupFlags.All)
  {
    return this.GetEntitiesIntersecting(this._transform.ToMapCoordinates(coordinates), flags);
  }

  public HashSet<EntityUid> GetEntitiesInRange(
    EntityCoordinates coordinates,
    float range,
    LookupFlags flags = LookupFlags.All)
  {
    HashSet<EntityUid> entities = new HashSet<EntityUid>();
    this.GetEntitiesInRange(coordinates, range, entities, flags);
    return entities;
  }

  public void GetEntitiesInRange(
    EntityCoordinates coordinates,
    float range,
    HashSet<EntityUid> entities,
    LookupFlags flags = LookupFlags.All)
  {
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(coordinates);
    if (mapCoordinates.MapId == MapId.Nullspace)
      return;
    this.GetEntitiesInRange(mapCoordinates.MapId, mapCoordinates.Position, range, entities, flags);
  }

  public bool AnyEntitiesIntersecting(MapCoordinates coordinates, LookupFlags flags = LookupFlags.All)
  {
    return !(coordinates.MapId == MapId.Nullspace) && this.AnyEntitiesInRange(coordinates, 1.401298E-44f, flags);
  }

  public bool AnyEntitiesInRange(MapCoordinates coordinates, float range, LookupFlags flags = LookupFlags.All)
  {
    if (coordinates.MapId == MapId.Nullspace)
      return false;
    PhysShapeCircle shape = new PhysShapeCircle(range, coordinates.Position);
    return this.AnyEntitiesIntersecting<PhysShapeCircle>(coordinates.MapId, shape, Robust.Shared.Physics.Transform.Empty, flags);
  }

  public HashSet<EntityUid> GetEntitiesIntersecting(MapCoordinates coordinates, LookupFlags flags = LookupFlags.All)
  {
    return coordinates.MapId == MapId.Nullspace ? new HashSet<EntityUid>() : this.GetEntitiesInRange(coordinates, 1.401298E-44f, flags);
  }

  public HashSet<EntityUid> GetEntitiesInRange(
    MapCoordinates coordinates,
    float range,
    LookupFlags flags = LookupFlags.All)
  {
    return this.GetEntitiesInRange(coordinates.MapId, coordinates.Position, range, flags);
  }

  public HashSet<EntityUid> GetEntitiesInRange(
    MapId mapId,
    Vector2 worldPos,
    float range,
    LookupFlags flags = LookupFlags.All)
  {
    HashSet<EntityUid> entities = new HashSet<EntityUid>();
    this.GetEntitiesInRange(mapId, worldPos, range, entities, flags);
    return entities;
  }

  public void GetEntitiesIntersecting<T>(
    MapId mapId,
    T shape,
    Robust.Shared.Physics.Transform transform,
    HashSet<EntityUid> entities,
    LookupFlags flags = LookupFlags.All)
    where T : IPhysShape
  {
    if (mapId == MapId.Nullspace)
      return;
    this.AddEntitiesIntersecting<T>(mapId, entities, shape, transform, flags);
  }

  public void GetEntitiesInRange(
    MapId mapId,
    Vector2 worldPos,
    float range,
    HashSet<EntityUid> entities,
    LookupFlags flags = LookupFlags.All)
  {
    if (mapId == MapId.Nullspace)
      return;
    PhysShapeCircle shape = new PhysShapeCircle(range, worldPos);
    this.AddEntitiesIntersecting<PhysShapeCircle>(mapId, entities, shape, Robust.Shared.Physics.Transform.Empty, flags);
  }

  public HashSet<EntityUid> GetEntitiesIntersecting(
    EntityUid gridId,
    Box2 worldAABB,
    LookupFlags flags = LookupFlags.All)
  {
    HashSet<EntityUid> intersecting = new HashSet<EntityUid>();
    this.GetEntitiesIntersecting(gridId, worldAABB, intersecting, flags);
    return intersecting;
  }

  public HashSet<EntityUid> GetEntitiesIntersecting(
    EntityUid gridId,
    Box2Rotated worldBounds,
    LookupFlags flags = LookupFlags.All)
  {
    HashSet<EntityUid> intersecting = new HashSet<EntityUid>();
    this.GetEntitiesIntersecting(gridId, worldBounds, intersecting, flags);
    return intersecting;
  }

  public void GetEntitiesIntersecting(
    EntityUid gridId,
    Box2 worldAABB,
    HashSet<EntityUid> intersecting,
    LookupFlags flags = LookupFlags.All)
  {
    BroadphaseComponent component;
    if (!this._broadQuery.TryGetComponent(gridId, out component))
      return;
    Box2 box2 = Matrix3Helpers.TransformBox(this._transform.GetInvWorldMatrix(gridId), ref worldAABB);
    SlimPolygon shape = new SlimPolygon(box2);
    this.AddEntitiesIntersecting<SlimPolygon>(gridId, intersecting, shape, box2, Robust.Shared.Physics.Transform.Empty, flags, component);
    this.AddContained(intersecting, flags);
  }

  public void GetEntitiesIntersecting(
    EntityUid gridId,
    Box2Rotated worldBounds,
    HashSet<EntityUid> intersecting,
    LookupFlags flags = LookupFlags.All)
  {
    BroadphaseComponent component;
    if (!this._broadQuery.TryGetComponent(gridId, out component))
      return;
    Box2 aabb;
    SlimPolygon shape = new SlimPolygon(in worldBounds, this._transform.GetInvWorldMatrix(gridId), out aabb);
    this.AddEntitiesIntersecting<SlimPolygon>(gridId, intersecting, shape, aabb, Robust.Shared.Physics.Transform.Empty, flags, component);
    this.AddContained(intersecting, flags);
  }

  public void FindLookupsIntersecting(
    MapId mapId,
    Box2Rotated worldBounds,
    ComponentQueryCallback<BroadphaseComponent> callback)
  {
    if (mapId == MapId.Nullspace)
      return;
    (ComponentQueryCallback<BroadphaseComponent>, Robust.Shared.GameObjects.EntityQuery<BroadphaseComponent>) state = (callback, this._broadQuery);
    this._mapManager.FindGridsIntersecting<(ComponentQueryCallback<BroadphaseComponent>, Robust.Shared.GameObjects.EntityQuery<BroadphaseComponent>)>(mapId, worldBounds, ref state, (GridCallback<(ComponentQueryCallback<BroadphaseComponent>, Robust.Shared.GameObjects.EntityQuery<BroadphaseComponent>)>) ((EntityUid uid, MapGridComponent grid, ref (ComponentQueryCallback<BroadphaseComponent> callback, Robust.Shared.GameObjects.EntityQuery<BroadphaseComponent> _broadQuery) tuple) =>
    {
      tuple.Item1(uid, tuple.Item2.GetComponent(uid));
      return true;
    }), true, false);
    EntityUid mapOrInvalid = this._map.GetMapOrInvalid(new MapId?(mapId));
    callback(mapOrInvalid, this._broadQuery.GetComponent(mapOrInvalid));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Box2 GetLocalBounds(Vector2i gridIndices, ushort tileSize)
  {
    return new Box2(Vector2i.op_Implicit(Vector2i.op_Multiply(gridIndices, (int) tileSize)), Vector2i.op_Implicit(Vector2i.op_Multiply(Vector2i.op_Addition(gridIndices, 1), (int) tileSize)));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Box2 GetLocalBounds(TileRef tileRef, ushort tileSize)
  {
    return this.GetLocalBounds(tileRef.GridIndices, tileSize);
  }

  public Box2Rotated GetWorldBounds(TileRef tileRef, Matrix3x2? worldMatrix = null, Angle? angle = null)
  {
    MapGridComponent component = this._gridQuery.GetComponent(tileRef.GridUid);
    if (!worldMatrix.HasValue || !angle.HasValue)
    {
      (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix) positionRotationMatrix = this._transform.GetWorldPositionRotationMatrix(tileRef.GridUid);
      Angle worldRotation = positionRotationMatrix.WorldRotation;
      worldMatrix = new Matrix3x2?(positionRotationMatrix.WorldMatrix);
      angle = new Angle?(worldRotation);
    }
    Vector2 vector2_1 = new Vector2(0.5f, 0.5f);
    Vector2 vector2_2 = Vector2.Transform(Vector2i.op_Implicit(tileRef.GridIndices) + vector2_1, worldMatrix.Value) * (float) component.TileSize;
    return new Box2Rotated(Box2.CenteredAround(vector2_2, new Vector2((float) component.TileSize, (float) component.TileSize)), Angle.op_UnaryNegation(angle.Value), vector2_2);
  }

  private void RecursiveAdd<T>(EntityUid uid, ref ValueList<Entity<T>> toAdd, Robust.Shared.GameObjects.EntityQuery<T> query) where T : IComponent
  {
    foreach (EntityUid child in this._xformQuery.GetComponent(uid)._children)
    {
      T component;
      if (query.TryGetComponent(child, out component))
        toAdd.Add((Entity<T>) (child, component));
      this.RecursiveAdd<T>(child, ref toAdd, query);
    }
  }

  private void AddContained<T>(
    HashSet<Entity<T>> intersecting,
    LookupFlags flags,
    Robust.Shared.GameObjects.EntityQuery<T> query)
    where T : IComponent
  {
    if ((flags & LookupFlags.Contained) == LookupFlags.None)
      return;
    ValueList<Entity<T>> toAdd = new ValueList<Entity<T>>();
    foreach (Entity<T> uid in intersecting)
    {
      ContainerManagerComponent component1;
      if (this._containerQuery.TryGetComponent((EntityUid) uid, out component1))
      {
        foreach (BaseContainer allContainer in this._container.GetAllContainers((EntityUid) uid, component1))
        {
          foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) allContainer.ContainedEntities)
          {
            T component2;
            if (query.TryGetComponent(containedEntity, out component2))
              toAdd.Add((Entity<T>) (containedEntity, component2));
            this.RecursiveAdd<T>(containedEntity, ref toAdd, query);
          }
        }
      }
    }
    Span<Entity<T>> span = toAdd.Span;
    for (int index = 0; index < span.Length; ++index)
    {
      Entity<T> entity = span[index];
      intersecting.Add(entity);
    }
  }

  private bool IsIntersecting<TShape>(
    MapId mapId,
    EntityUid uid,
    TransformComponent xform,
    TShape shape,
    Robust.Shared.Physics.Transform shapeTransform,
    Box2 worldAABB,
    LookupFlags flags)
    where TShape : IPhysShape
  {
    (Vector2 vector2, Angle angle) = this._transform.GetWorldPositionRotation(xform);
    if (xform.MapID != mapId || !((Box2) ref worldAABB).Contains(vector2, true) || (flags & LookupFlags.Contained) == LookupFlags.None && this._container.IsEntityOrParentInContainer(uid, this._metaQuery.GetComponent(uid), xform))
      return false;
    FixturesComponent component;
    if (this._fixturesQuery.TryGetComponent(uid, out component))
    {
      Robust.Shared.Physics.Transform xfB = new Robust.Shared.Physics.Transform(vector2, angle);
      bool flag1 = false;
      bool flag2 = (flags & LookupFlags.Sensors) != 0;
      foreach (Fixture fixture in component.Fixtures.Values)
      {
        if (flag2 || fixture.Hard)
        {
          flag1 = true;
          for (int indexB = 0; indexB < fixture.Shape.ChildCount; ++indexB)
          {
            if (this._manifoldManager.TestOverlap<TShape, IPhysShape>(shape, 0, fixture.Shape, indexB, in shapeTransform, in xfB))
              return true;
          }
        }
      }
      if (flag1)
        return false;
    }
    return this._fixtures.TestPoint<TShape>(shape, shapeTransform, vector2);
  }

  private void AddLocalEntitiesIntersecting<T>(
    EntityUid lookupUid,
    HashSet<Entity<T>> intersecting,
    Box2 localAABB,
    LookupFlags flags,
    Robust.Shared.GameObjects.EntityQuery<T> query,
    BroadphaseComponent? lookup = null)
    where T : IComponent
  {
    if (!this._broadQuery.Resolve(lookupUid, ref lookup))
      return;
    SlimPolygon shape = new SlimPolygon(localAABB);
    this.AddEntitiesIntersecting<T, SlimPolygon>(lookupUid, intersecting, shape, localAABB, Robust.Shared.Physics.Transform.Empty, flags, query, lookup);
  }

  private void AddEntitiesIntersecting<T, TShape>(
    EntityUid lookupUid,
    HashSet<Entity<T>> intersecting,
    TShape shape,
    Box2 localAABB,
    Robust.Shared.Physics.Transform localTransform,
    LookupFlags flags,
    Robust.Shared.GameObjects.EntityQuery<T> query,
    BroadphaseComponent? lookup = null)
    where T : IComponent
    where TShape : IPhysShape
  {
    if (!this._broadQuery.Resolve(lookupUid, ref lookup))
      return;
    EntityLookupSystem.QueryState<T, TShape> state = new EntityLookupSystem.QueryState<T, TShape>(intersecting, shape, localTransform, this._fixtures, this._physics, this._manifoldManager, query, this._fixturesQuery, (flags & LookupFlags.Sensors) != 0, (flags & LookupFlags.Approximate) != 0);
    if ((flags & LookupFlags.Dynamic) != LookupFlags.None)
      lookup.DynamicTree.QueryAabb<EntityLookupSystem.QueryState<T, TShape>>(ref state, new DynamicTree<FixtureProxy>.QueryCallbackDelegate<EntityLookupSystem.QueryState<T, TShape>>(PhysicsQuery), localAABB, true);
    if ((flags & LookupFlags.Static) != LookupFlags.None)
      lookup.StaticTree.QueryAabb<EntityLookupSystem.QueryState<T, TShape>>(ref state, new DynamicTree<FixtureProxy>.QueryCallbackDelegate<EntityLookupSystem.QueryState<T, TShape>>(PhysicsQuery), localAABB, true);
    if ((flags & LookupFlags.StaticSundries) == LookupFlags.StaticSundries)
      lookup.StaticSundriesTree.QueryAabb<EntityLookupSystem.QueryState<T, TShape>>(ref state, new DynamicTree<EntityUid>.QueryCallbackDelegate<EntityLookupSystem.QueryState<T, TShape>>(SundriesQuery), localAABB, true);
    if ((flags & LookupFlags.Sundries) == LookupFlags.None)
      return;
    lookup.SundriesTree.QueryAabb<EntityLookupSystem.QueryState<T, TShape>>(ref state, new DynamicTree<EntityUid>.QueryCallbackDelegate<EntityLookupSystem.QueryState<T, TShape>>(SundriesQuery), localAABB, true);

    static bool PhysicsQuery(
      ref EntityLookupSystem.QueryState<T, TShape> state,
      in FixtureProxy value)
    {
      T component;
      if (!state.Sensors && !value.Fixture.Hard || !state.Query.TryGetComponent(value.Entity, out component))
        return true;
      if (!state.Approximate)
      {
        Robust.Shared.Physics.Transform xfB = state.Physics.GetLocalPhysicsTransform(value.Entity);
        if (!state.Manifolds.TestOverlap<TShape, IPhysShape>(state.Shape, 0, value.Fixture.Shape, value.ChildIndex, state.Transform, in xfB))
          return true;
      }
      state.Intersecting.Add((Entity<T>) (value.Entity, component));
      return true;
    }

    static bool SundriesQuery(
      ref EntityLookupSystem.QueryState<T, TShape> state,
      in EntityUid value)
    {
      T component1;
      if (!state.Query.TryGetComponent(value, out component1))
        return true;
      if (state.Approximate)
      {
        state.Intersecting.Add((Entity<T>) (value, component1));
        return true;
      }
      Robust.Shared.Physics.Transform xfB = state.Physics.GetLocalPhysicsTransform(value);
      FixturesComponent component2;
      if (state.FixturesQuery.TryGetComponent(value, out component2))
      {
        bool flag = false;
        foreach (Fixture fixture in component2.Fixtures.Values)
        {
          if (state.Sensors || fixture.Hard)
          {
            flag = true;
            for (int indexB = 0; indexB < fixture.Shape.ChildCount; ++indexB)
            {
              if (state.Manifolds.TestOverlap<TShape, IPhysShape>(state.Shape, 0, fixture.Shape, indexB, state.Transform, in xfB))
              {
                state.Intersecting.Add((Entity<T>) (value, component1));
                return true;
              }
            }
          }
        }
        if (flag)
          return true;
      }
      if (state.Fixtures.TestPoint<TShape>(state.Shape, state.Transform, xfB.Position))
        state.Intersecting.Add((Entity<T>) (value, component1));
      return true;
    }
  }

  private bool AnyLocalComponentsIntersecting<T>(
    EntityUid lookupUid,
    Box2 localAABB,
    LookupFlags flags,
    Robust.Shared.GameObjects.EntityQuery<T> query,
    EntityUid? ignored = null,
    BroadphaseComponent? lookup = null)
    where T : IComponent
  {
    if (!this._broadQuery.Resolve(lookupUid, ref lookup))
      return false;
    SlimPolygon shape = new SlimPolygon(localAABB);
    (Vector2 WorldPosition, Angle WorldRotation) positionRotation = this._transform.GetWorldPositionRotation(lookupUid);
    Robust.Shared.Physics.Transform shapeTransform = new Robust.Shared.Physics.Transform(positionRotation.WorldPosition, positionRotation.WorldRotation);
    return this.AnyComponentsIntersecting<T, SlimPolygon>(lookupUid, shape, localAABB, shapeTransform, flags, query, ignored, lookup);
  }

  private bool AnyComponentsIntersecting<T, TShape>(
    EntityUid lookupUid,
    TShape shape,
    Box2 localAABB,
    Robust.Shared.Physics.Transform shapeTransform,
    LookupFlags flags,
    Robust.Shared.GameObjects.EntityQuery<T> query,
    EntityUid? ignored = null,
    BroadphaseComponent? lookup = null)
    where T : IComponent
    where TShape : IPhysShape
  {
    if (!this._broadQuery.Resolve(lookupUid, ref lookup))
      return false;
    EntityLookupSystem.AnyQueryState<T, TShape> state = new EntityLookupSystem.AnyQueryState<T, TShape>(false, ignored, shape, shapeTransform, this._fixtures, this._physics, this._manifoldManager, query, this._fixturesQuery, flags);
    if ((flags & LookupFlags.Dynamic) != LookupFlags.None)
    {
      lookup.DynamicTree.QueryAabb<EntityLookupSystem.AnyQueryState<T, TShape>>(ref state, new DynamicTree<FixtureProxy>.QueryCallbackDelegate<EntityLookupSystem.AnyQueryState<T, TShape>>(PhysicsQuery), localAABB, true);
      if (state.Found)
        return true;
    }
    if ((flags & LookupFlags.Static) != LookupFlags.None)
    {
      lookup.StaticTree.QueryAabb<EntityLookupSystem.AnyQueryState<T, TShape>>(ref state, new DynamicTree<FixtureProxy>.QueryCallbackDelegate<EntityLookupSystem.AnyQueryState<T, TShape>>(PhysicsQuery), localAABB, true);
      if (state.Found)
        return true;
    }
    if ((flags & LookupFlags.StaticSundries) == LookupFlags.StaticSundries)
    {
      lookup.StaticSundriesTree.QueryAabb<EntityLookupSystem.AnyQueryState<T, TShape>>(ref state, new DynamicTree<EntityUid>.QueryCallbackDelegate<EntityLookupSystem.AnyQueryState<T, TShape>>(SundriesQuery), localAABB, true);
      if (state.Found)
        return true;
    }
    if ((flags & LookupFlags.Sundries) != LookupFlags.None)
      lookup.SundriesTree.QueryAabb<EntityLookupSystem.AnyQueryState<T, TShape>>(ref state, new DynamicTree<EntityUid>.QueryCallbackDelegate<EntityLookupSystem.AnyQueryState<T, TShape>>(SundriesQuery), localAABB, true);
    return state.Found;

    static bool PhysicsQuery(
      ref EntityLookupSystem.AnyQueryState<T, TShape> state,
      in FixtureProxy value)
    {
      EntityUid entity = value.Entity;
      EntityUid? ignored = state.Ignored;
      if ((ignored.HasValue ? (entity == ignored.GetValueOrDefault() ? 1 : 0) : 0) != 0 || !state.Query.HasComponent(value.Entity))
        return true;
      if ((state.Flags & LookupFlags.Approximate) == 0)
      {
        Robust.Shared.Physics.Transform xfB = state.Physics.GetPhysicsTransform(value.Entity);
        if (!state.Manifolds.TestOverlap<TShape, IPhysShape>(state.Shape, 0, value.Fixture.Shape, value.ChildIndex, state.Transform, in xfB))
          return true;
      }
      state.Found = true;
      return false;
    }

    static bool SundriesQuery(
      ref EntityLookupSystem.AnyQueryState<T, TShape> state,
      in EntityUid value)
    {
      EntityUid? ignored = state.Ignored;
      EntityUid entityUid = value;
      if ((ignored.HasValue ? (ignored.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0 || !state.Query.HasComponent(value))
        return true;
      if ((state.Flags & LookupFlags.Approximate) != 0)
      {
        state.Found = true;
        return false;
      }
      Robust.Shared.Physics.Transform xfB = state.Physics.GetPhysicsTransform(value);
      FixturesComponent component;
      if (state.FixturesQuery.TryGetComponent(value, out component))
      {
        bool flag1 = (state.Flags & LookupFlags.Sensors) != 0;
        bool flag2 = false;
        foreach (Fixture fixture in component.Fixtures.Values)
        {
          if (flag1 || fixture.Hard)
          {
            flag2 = true;
            for (int indexB = 0; indexB < fixture.Shape.ChildCount; ++indexB)
            {
              if (state.Manifolds.TestOverlap<TShape, IPhysShape>(state.Shape, 0, fixture.Shape, indexB, state.Transform, in xfB))
              {
                state.Found = true;
                return false;
              }
            }
          }
        }
        if (flag2)
          return true;
      }
      if (!state.Fixtures.TestPoint<TShape>(state.Shape, state.Transform, xfB.Position))
        return true;
      state.Found = true;
      return false;
    }
  }

  private bool UseBoundsQuery(Type type, float area) => (double) this.Count(type) > (double) area;

  private bool UseBoundsQuery<T>(float area) where T : IComponent
  {
    return (double) this.Count<T>() > (double) area;
  }

  public bool AnyComponentsIntersecting(
    Type type,
    MapId mapId,
    Box2 worldAABB,
    EntityUid? ignored = null,
    LookupFlags flags = LookupFlags.All)
  {
    SlimPolygon shape = new SlimPolygon(worldAABB);
    Robust.Shared.Physics.Transform empty = Robust.Shared.Physics.Transform.Empty;
    return this.AnyComponentsIntersecting<SlimPolygon>(type, mapId, shape, empty, ignored, flags);
  }

  public bool AnyComponentsIntersecting<T>(
    Type type,
    MapId mapId,
    T shape,
    Robust.Shared.Physics.Transform shapeTransform,
    EntityUid? ignored = null,
    LookupFlags flags = LookupFlags.All)
    where T : IPhysShape
  {
    if (mapId == MapId.Nullspace)
      return false;
    Box2 aabb = shape.ComputeAABB(shapeTransform, 0);
    if (!this.UseBoundsQuery(type, ((Box2) ref aabb).Height * ((Box2) ref aabb).Width))
    {
      foreach ((EntityUid entityUid, IComponent _) in this.EntityManager.GetAllComponents(type, true))
      {
        TransformComponent component = this._xformQuery.GetComponent(entityUid);
        if (this.IsIntersecting<T>(mapId, entityUid, component, shape, shapeTransform, aabb, flags))
          return true;
      }
    }
    else
    {
      Robust.Shared.GameObjects.EntityQuery<IComponent> entityQuery = this.EntityManager.GetEntityQuery(type);
      (EntityLookupSystem, Box2, LookupFlags, Robust.Shared.GameObjects.EntityQuery<IComponent>, EntityUid?, bool) state = (this, aabb, flags, entityQuery, ignored, false);
      this._mapManager.FindGridsIntersecting<(EntityLookupSystem, Box2, LookupFlags, Robust.Shared.GameObjects.EntityQuery<IComponent>, EntityUid?, bool)>(mapId, aabb, ref state, (GridCallback<(EntityLookupSystem, Box2, LookupFlags, Robust.Shared.GameObjects.EntityQuery<IComponent>, EntityUid?, bool)>) ((EntityUid uid, MapGridComponent grid, ref (EntityLookupSystem system, Box2 worldAABB, LookupFlags flags, Robust.Shared.GameObjects.EntityQuery<IComponent> query, EntityUid? ignored, bool found) tuple) =>
      {
        if (!tuple.Item1.AnyLocalComponentsIntersecting<IComponent>(uid, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5))
          return true;
        tuple.Item6 = true;
        return false;
      }), true, false);
      if (state.Item6)
        return true;
      this.AnyLocalComponentsIntersecting<IComponent>(this._map.GetMapOrInvalid(new MapId?(mapId)), aabb, flags, entityQuery, ignored);
    }
    return false;
  }

  public void GetEntitiesIntersecting(
    Type type,
    MapId mapId,
    Box2 worldAABB,
    HashSet<Entity<IComponent>> intersecting,
    LookupFlags flags = LookupFlags.All)
  {
    if (mapId == MapId.Nullspace)
      return;
    SlimPolygon shape = new SlimPolygon(worldAABB);
    Robust.Shared.Physics.Transform empty = Robust.Shared.Physics.Transform.Empty;
    this.GetEntitiesIntersecting<SlimPolygon>(type, mapId, shape, empty, intersecting, flags);
  }

  public void GetEntitiesIntersecting<T>(
    MapId mapId,
    Box2Rotated worldBounds,
    HashSet<Entity<T>> entities,
    LookupFlags flags = LookupFlags.All)
    where T : IComponent
  {
    if (mapId == MapId.Nullspace)
      return;
    SlimPolygon shape = new SlimPolygon(in worldBounds);
    Robust.Shared.Physics.Transform empty = Robust.Shared.Physics.Transform.Empty;
    this.GetEntitiesIntersecting<T, SlimPolygon>(mapId, shape, empty, entities, flags);
  }

  public void GetEntitiesIntersecting<T>(
    MapId mapId,
    Box2 worldAABB,
    HashSet<Entity<T>> entities,
    LookupFlags flags = LookupFlags.All)
    where T : IComponent
  {
    if (mapId == MapId.Nullspace)
      return;
    SlimPolygon shape = new SlimPolygon(worldAABB);
    Robust.Shared.Physics.Transform empty = Robust.Shared.Physics.Transform.Empty;
    this.GetEntitiesIntersecting<T, SlimPolygon>(mapId, shape, empty, entities, flags);
  }

  public void GetEntitiesIntersecting<T>(
    Type type,
    MapId mapId,
    T shape,
    Robust.Shared.Physics.Transform shapeTransform,
    HashSet<Entity<IComponent>> intersecting,
    LookupFlags flags = LookupFlags.All)
    where T : IPhysShape
  {
    if (mapId == MapId.Nullspace)
      return;
    Box2 aabb1 = shape.ComputeAABB(shapeTransform, 0);
    if (!this.UseBoundsQuery(type, ((Box2) ref aabb1).Height * ((Box2) ref aabb1).Width))
    {
      foreach ((EntityUid entityUid, IComponent Component) in this.EntityManager.GetAllComponents(type, true))
      {
        TransformComponent component = this._xformQuery.GetComponent(entityUid);
        if (this.IsIntersecting<T>(mapId, entityUid, component, shape, shapeTransform, aabb1, flags))
          intersecting.Add((Entity<IComponent>) (entityUid, Component));
      }
    }
    else
    {
      Robust.Shared.GameObjects.EntityQuery<IComponent> entityQuery = this.EntityManager.GetEntityQuery(type);
      EntityLookupSystem.GridQueryState<IComponent, T> state1 = new EntityLookupSystem.GridQueryState<IComponent, T>(intersecting, shape, shapeTransform, this, this._physics, flags, entityQuery);
      this._mapManager.FindGridsIntersecting<EntityLookupSystem.GridQueryState<IComponent, T>>(mapId, aabb1, ref state1, (GridCallback<EntityLookupSystem.GridQueryState<IComponent, T>>) ((EntityUid uid, MapGridComponent grid, ref EntityLookupSystem.GridQueryState<IComponent, T> state) =>
      {
        Robust.Shared.Physics.Transform physicsTransform = state.Physics.GetRelativePhysicsTransform(state.Transform, (Entity<TransformComponent>) uid);
        Box2 aabb2 = state.Shape.ComputeAABB(physicsTransform, 0);
        state.Lookup.AddEntitiesIntersecting<IComponent, T>(uid, state.Intersecting, state.Shape, aabb2, physicsTransform, state.Flags, state.Query);
        return true;
      }), true, false);
      EntityUid mapOrInvalid = this._map.GetMapOrInvalid(new MapId?(mapId));
      Robust.Shared.Physics.Transform physicsTransform1 = state1.Physics.GetRelativePhysicsTransform(state1.Transform, (Entity<TransformComponent>) mapOrInvalid);
      Box2 aabb3 = state1.Shape.ComputeAABB(physicsTransform1, 0);
      this.AddEntitiesIntersecting<IComponent, T>(mapOrInvalid, intersecting, shape, aabb3, physicsTransform1, flags, entityQuery);
      this.AddContained<IComponent>(intersecting, flags, entityQuery);
    }
  }

  public void GetEntitiesIntersecting<T, TShape>(
    MapId mapId,
    TShape shape,
    Robust.Shared.Physics.Transform shapeTransform,
    HashSet<Entity<T>> entities,
    LookupFlags flags = LookupFlags.All)
    where T : IComponent
    where TShape : IPhysShape
  {
    if (mapId == MapId.Nullspace)
      return;
    Box2 aabb1 = shape.ComputeAABB(shapeTransform, 0);
    if (!this.UseBoundsQuery<T>(((Box2) ref aabb1).Height * ((Box2) ref aabb1).Width))
    {
      AllEntityQueryEnumerator<T, TransformComponent> entityQueryEnumerator = this.AllEntityQuery<T, TransformComponent>();
      EntityUid uid;
      T comp1;
      TransformComponent comp2;
      while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
      {
        if (this.IsIntersecting<TShape>(mapId, uid, comp2, shape, shapeTransform, aabb1, flags))
          entities.Add((Entity<T>) (uid, comp1));
      }
    }
    else
    {
      Robust.Shared.GameObjects.EntityQuery<T> entityQuery = this.GetEntityQuery<T>();
      EntityLookupSystem.GridQueryState<T, TShape> state1 = new EntityLookupSystem.GridQueryState<T, TShape>(entities, shape, shapeTransform, this, this._physics, flags, entityQuery);
      this._mapManager.FindGridsIntersecting<EntityLookupSystem.GridQueryState<T, TShape>>(mapId, aabb1, ref state1, (GridCallback<EntityLookupSystem.GridQueryState<T, TShape>>) ((EntityUid uid, MapGridComponent grid, ref EntityLookupSystem.GridQueryState<T, TShape> state) =>
      {
        Robust.Shared.Physics.Transform physicsTransform = state.Physics.GetRelativePhysicsTransform(state.Transform, (Entity<TransformComponent>) uid);
        Box2 aabb2 = state.Shape.ComputeAABB(physicsTransform, 0);
        state.Lookup.AddEntitiesIntersecting<T, TShape>(uid, state.Intersecting, state.Shape, aabb2, physicsTransform, state.Flags, state.Query);
        return true;
      }), true, false);
      EntityUid mapOrInvalid = this._map.GetMapOrInvalid(new MapId?(mapId));
      Robust.Shared.Physics.Transform physicsTransform1 = state1.Physics.GetRelativePhysicsTransform(state1.Transform, (Entity<TransformComponent>) mapOrInvalid);
      Box2 aabb3 = state1.Shape.ComputeAABB(physicsTransform1, 0);
      this.AddEntitiesIntersecting<T, TShape>(mapOrInvalid, entities, shape, aabb3, physicsTransform1, flags, entityQuery);
      this.AddContained<T>(entities, flags, entityQuery);
    }
  }

  public void GetEntitiesInRange<T>(
    EntityCoordinates coordinates,
    float range,
    HashSet<Entity<T>> entities,
    LookupFlags flags = LookupFlags.All)
    where T : IComponent
  {
    this.GetEntitiesInRange<T>(this._transform.ToMapCoordinates(coordinates), range, entities, flags);
  }

  public HashSet<Entity<T>> GetEntitiesInRange<T>(
    EntityCoordinates coordinates,
    float range,
    LookupFlags flags = LookupFlags.All)
    where T : IComponent
  {
    HashSet<Entity<T>> entities = new HashSet<Entity<T>>();
    this.GetEntitiesInRange<T>(coordinates, range, entities, flags);
    return entities;
  }

  public HashSet<Entity<IComponent>> GetEntitiesInRange(
    Type type,
    MapCoordinates coordinates,
    float range)
  {
    HashSet<Entity<IComponent>> entities = new HashSet<Entity<IComponent>>();
    this.GetEntitiesInRange(type, coordinates, range, entities);
    return entities;
  }

  public void GetEntitiesInRange(
    Type type,
    MapCoordinates coordinates,
    float range,
    HashSet<Entity<IComponent>> entities,
    LookupFlags flags = LookupFlags.All)
  {
    this.GetEntitiesInRange(type, coordinates.MapId, coordinates.Position, range, entities, flags);
  }

  public void GetEntitiesInRange<T>(
    MapCoordinates coordinates,
    float range,
    HashSet<Entity<T>> entities,
    LookupFlags flags = LookupFlags.All)
    where T : IComponent
  {
    this.GetEntitiesInRange<T>(coordinates.MapId, coordinates.Position, range, entities, flags);
  }

  public HashSet<Entity<T>> GetEntitiesInRange<T>(
    MapCoordinates coordinates,
    float range,
    LookupFlags flags = LookupFlags.All)
    where T : IComponent
  {
    HashSet<Entity<T>> entities = new HashSet<Entity<T>>();
    this.GetEntitiesInRange<T>(coordinates.MapId, coordinates.Position, range, entities, flags);
    return entities;
  }

  public void GetEntitiesInRange(
    Type type,
    MapId mapId,
    Vector2 worldPos,
    float range,
    HashSet<Entity<IComponent>> entities,
    LookupFlags flags = LookupFlags.All)
  {
    if (mapId == MapId.Nullspace)
      return;
    PhysShapeCircle shape = new PhysShapeCircle(range);
    Robust.Shared.Physics.Transform shapeTransform = new Robust.Shared.Physics.Transform(worldPos, 0.0f);
    this.GetEntitiesIntersecting<PhysShapeCircle>(type, mapId, shape, shapeTransform, entities, flags);
  }

  public void GetEntitiesInRange<T>(
    MapId mapId,
    Vector2 worldPos,
    float range,
    HashSet<Entity<T>> entities,
    LookupFlags flags = LookupFlags.All)
    where T : IComponent
  {
    PhysShapeCircle shape = new PhysShapeCircle(range, worldPos);
    Robust.Shared.Physics.Transform empty = Robust.Shared.Physics.Transform.Empty;
    this.GetEntitiesInRange<T, PhysShapeCircle>(mapId, shape, empty, entities, flags);
  }

  public void GetEntitiesInRange<T, TShape>(
    MapId mapId,
    TShape shape,
    Robust.Shared.Physics.Transform transform,
    HashSet<Entity<T>> entities,
    LookupFlags flags = LookupFlags.All)
    where T : IComponent
    where TShape : IPhysShape
  {
    if (mapId == MapId.Nullspace)
      return;
    this.GetEntitiesIntersecting<T, TShape>(mapId, shape, transform, entities, flags);
  }

  public void GetEntitiesOnMap<TComp1>(MapId mapId, HashSet<Entity<TComp1>> entities) where TComp1 : IComponent
  {
    AllEntityQueryEnumerator<TComp1, TransformComponent> entityQueryEnumerator = this.AllEntityQuery<TComp1, TransformComponent>();
    EntityUid uid;
    TComp1 comp1;
    TransformComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (!(comp2.MapID != mapId))
        entities.Add((Entity<TComp1>) (uid, comp1));
    }
  }

  public void GetEntitiesOnMap<TComp1, TComp2>(
    MapId mapId,
    HashSet<Entity<TComp1, TComp2>> entities)
    where TComp1 : IComponent
    where TComp2 : IComponent
  {
    AllEntityQueryEnumerator<TComp1, TComp2, TransformComponent> entityQueryEnumerator = this.AllEntityQuery<TComp1, TComp2, TransformComponent>();
    EntityUid uid;
    TComp1 comp1;
    TComp2 comp2;
    TransformComponent comp3;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2, out comp3))
    {
      if (!(comp3.MapID != mapId))
        entities.Add((Entity<TComp1, TComp2>) (uid, comp1, comp2));
    }
  }

  public void GetLocalEntitiesIntersecting<T>(
    EntityUid gridUid,
    Vector2i localTile,
    HashSet<Entity<T>> intersecting,
    float enlargement = -0.04f,
    LookupFlags flags = LookupFlags.All,
    MapGridComponent? gridComp = null)
    where T : IComponent
  {
    ushort tileSize = 1;
    if (this._gridQuery.Resolve(gridUid, ref gridComp))
      tileSize = gridComp.TileSize;
    Box2 localBounds = this.GetLocalBounds(localTile, tileSize);
    Box2 localAABB = ((Box2) ref localBounds).Enlarged(enlargement);
    this.GetLocalEntitiesIntersecting<T>(gridUid, localAABB, intersecting, flags);
  }

  public void GetLocalEntitiesIntersecting<T>(
    EntityUid gridUid,
    Box2 localAABB,
    HashSet<Entity<T>> intersecting,
    LookupFlags flags = LookupFlags.All)
    where T : IComponent
  {
    Robust.Shared.GameObjects.EntityQuery<T> entityQuery = this.GetEntityQuery<T>();
    this.AddLocalEntitiesIntersecting<T>(gridUid, intersecting, localAABB, flags, entityQuery);
    this.AddContained<T>(intersecting, flags, entityQuery);
  }

  public void GetLocalEntitiesIntersecting<T>(
    Entity<BroadphaseComponent> grid,
    Box2 localAABB,
    HashSet<Entity<T>> intersecting,
    Robust.Shared.GameObjects.EntityQuery<T> query,
    LookupFlags flags = LookupFlags.All)
    where T : IComponent
  {
    this.AddLocalEntitiesIntersecting<T>((EntityUid) grid, intersecting, localAABB, flags, query, grid.Comp);
    this.AddContained<T>(intersecting, flags, query);
  }

  public void GetGridEntities<TComp1>(EntityUid gridUid, HashSet<Entity<TComp1>> entities) where TComp1 : IComponent
  {
    AllEntityQueryEnumerator<TComp1, TransformComponent> entityQueryEnumerator = this.AllEntityQuery<TComp1, TransformComponent>();
    EntityUid uid;
    TComp1 comp1;
    TransformComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      EntityUid? gridUid1 = comp2.GridUid;
      EntityUid entityUid = gridUid;
      if ((gridUid1.HasValue ? (gridUid1.GetValueOrDefault() != entityUid ? 1 : 0) : 1) == 0)
        entities.Add((Entity<TComp1>) (uid, comp1));
    }
  }

  public void GetChildEntities<TComp1>(EntityUid parentUid, HashSet<Entity<TComp1>> entities) where TComp1 : IComponent
  {
    AllEntityQueryEnumerator<TComp1, TransformComponent> entityQueryEnumerator = this.AllEntityQuery<TComp1, TransformComponent>();
    EntityUid uid;
    TComp1 comp1;
    TransformComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (!(comp2.ParentUid != parentUid))
        entities.Add((Entity<TComp1>) (uid, comp1));
    }
  }

  public void GetChildEntities<TComp1, TComp2>(
    EntityUid parentUid,
    HashSet<Entity<TComp1, TComp2>> entities)
    where TComp1 : IComponent
    where TComp2 : IComponent
  {
    AllEntityQueryEnumerator<TComp1, TComp2, TransformComponent> entityQueryEnumerator = this.AllEntityQuery<TComp1, TComp2, TransformComponent>();
    EntityUid uid;
    TComp1 comp1;
    TComp2 comp2;
    TransformComponent comp3;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2, out comp3))
    {
      if (!(comp3.ParentUid != parentUid))
        entities.Add((Entity<TComp1, TComp2>) (uid, comp1, comp2));
    }
  }

  public override void Initialize()
  {
    base.Initialize();
    this._broadQuery = this.GetEntityQuery<BroadphaseComponent>();
    this._containerQuery = this.GetEntityQuery<ContainerManagerComponent>();
    this._fixturesQuery = this.GetEntityQuery<FixturesComponent>();
    this._mapQuery = this.GetEntityQuery<MapComponent>();
    this._gridQuery = this.GetEntityQuery<MapGridComponent>();
    this._metaQuery = this.GetEntityQuery<MetaDataComponent>();
    this._physicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this._xformQuery = this.GetEntityQuery<TransformComponent>();
    this.SubscribeLocalEvent<BroadphaseComponent, EntityTerminatingEvent>(new ComponentEventRefHandler<BroadphaseComponent, EntityTerminatingEvent>(this.OnBroadphaseTerminating));
    this.SubscribeLocalEvent<BroadphaseComponent, ComponentAdd>(new EntityEventRefHandler<BroadphaseComponent, ComponentAdd>(this.OnBroadphaseAdd));
    this.SubscribeLocalEvent<BroadphaseComponent, ComponentInit>(new EntityEventRefHandler<BroadphaseComponent, ComponentInit>(this.OnBroadphaseInit));
    this.SubscribeLocalEvent<GridAddEvent>(new EntityEventHandler<GridAddEvent>(this.OnGridAdd));
    this.SubscribeLocalEvent<MapCreatedEvent>(new EntityEventHandler<MapCreatedEvent>(this.OnMapChange));
    this._transform.OnBeforeMoveEvent += new SharedTransformSystem.MoveEventHandler(this.OnMove);
    this.EntityManager.EntityInitialized += new Action<Entity<MetaDataComponent>>(this.OnEntityInit);
    this.SubscribeLocalEvent<TransformComponent, PhysicsBodyTypeChangedEvent>(new ComponentEventRefHandler<TransformComponent, PhysicsBodyTypeChangedEvent>(this.OnBodyTypeChange));
    this.SubscribeLocalEvent<PhysicsComponent, ComponentStartup>(new ComponentEventHandler<PhysicsComponent, ComponentStartup>(this.OnBodyStartup));
    this.SubscribeLocalEvent<CollisionChangeEvent>(new EntityEventRefHandler<CollisionChangeEvent>(this.OnPhysicsUpdate));
  }

  private void OnBodyStartup(EntityUid uid, PhysicsComponent component, ComponentStartup args)
  {
    this.UpdatePhysicsBroadphase(uid, this.Transform(uid), component);
  }

  public override void Shutdown()
  {
    base.Shutdown();
    this.EntityManager.EntityInitialized -= new Action<Entity<MetaDataComponent>>(this.OnEntityInit);
    this._transform.OnBeforeMoveEvent -= new SharedTransformSystem.MoveEventHandler(this.OnMove);
  }

  private void OnBroadphaseTerminating(
    EntityUid uid,
    BroadphaseComponent component,
    ref EntityTerminatingEvent args)
  {
    this.RemoveChildrenFromTerminatingBroadphase(this._xformQuery.GetComponent(uid), component);
    this.RemComp(uid, (IComponent) component);
  }

  private void RemoveChildrenFromTerminatingBroadphase(
    TransformComponent xform,
    BroadphaseComponent component)
  {
    foreach (EntityUid child in xform._children)
    {
      TransformComponent component1;
      if (this._xformQuery.TryGetComponent(child, out component1))
      {
        EntityUid? gridUid = component1.GridUid;
        EntityUid entityUid = child;
        if ((gridUid.HasValue ? (gridUid.GetValueOrDefault() == entityUid ? 1 : 0) : 0) == 0 && component1.Broadphase.HasValue)
        {
          BroadphaseData broadphaseData = component1.Broadphase.Value;
          FixturesComponent component2;
          if (broadphaseData.CanCollide && this._fixturesQuery.TryGetComponent(child, out component2))
          {
            broadphaseData = component1.Broadphase.Value;
            IBroadPhase tree = broadphaseData.Static ? component.StaticTree : component.DynamicTree;
            foreach (Fixture fixture in component2.Fixtures.Values)
              this.DestroyProxies(fixture, tree);
          }
          component1.Broadphase = new BroadphaseData?();
          this.RemoveChildrenFromTerminatingBroadphase(component1, component);
        }
      }
    }
  }

  private void OnMapChange(MapCreatedEvent ev)
  {
    if (!(ev.MapId != MapId.Nullspace))
      return;
    this.EnsureComp<BroadphaseComponent>(ev.Uid);
  }

  private void OnGridAdd(GridAddEvent ev) => this.EnsureComp<BroadphaseComponent>(ev.EntityUid);

  private void OnBroadphaseAdd(Entity<BroadphaseComponent> broadphase, ref ComponentAdd args)
  {
    broadphase.Comp.StaticSundriesTree = new DynamicTree<EntityUid>((DynamicTree<EntityUid>.ExtractAabbDelegate) ((in EntityUid value) => this.GetTreeAABB(value, broadphase.Owner)));
    broadphase.Comp.SundriesTree = new DynamicTree<EntityUid>((DynamicTree<EntityUid>.ExtractAabbDelegate) ((in EntityUid value) => this.GetTreeAABB(value, broadphase.Owner)));
  }

  private void OnBroadphaseInit(Entity<BroadphaseComponent> broadphase, ref ComponentInit args)
  {
    TransformComponent transformComponent = this.Transform(broadphase.Owner);
    this._transform.InitializeMapUid(broadphase.Owner, transformComponent);
    if (!transformComponent.MapUid.HasValue)
      return;
    Entity<TransformComponent, BroadphaseComponent> broadphase1 = new Entity<TransformComponent, BroadphaseComponent>((EntityUid) broadphase, transformComponent, (BroadphaseComponent) broadphase);
    TransformChildrenEnumerator childEnumerator = transformComponent.ChildEnumerator;
    EntityUid child;
    while (childEnumerator.MoveNext(out child))
    {
      if (!this._broadQuery.HasComp(child))
        this.InitializeChild(child, broadphase1);
    }
  }

  private void InitializeChild(
    EntityUid child,
    Entity<TransformComponent, BroadphaseComponent> broadphase)
  {
    if (this.LifeStage(child) <= EntityLifeStage.PreInit)
      return;
    TransformComponent xform = this.Transform(child);
    if (xform.Broadphase.HasValue)
    {
      if (!xform.Broadphase.Value.IsValid())
        return;
      ref Robust.Shared.GameObjects.EntityQuery<BroadphaseComponent> local1 = ref this._broadQuery;
      BroadphaseData broadphaseData = xform.Broadphase.Value;
      EntityUid uid = broadphaseData.Uid;
      BroadphaseComponent broadphase1;
      ref BroadphaseComponent local2 = ref broadphase1;
      if (!local1.TryGetComponent(uid, out local2))
      {
        FixturesComponent component;
        if (this._fixturesQuery.TryGetComponent(child, out component))
        {
          foreach (Fixture fixture in component.Fixtures.Values)
          {
            fixture.ProxyCount = 0;
            fixture.Proxies = Array.Empty<FixtureProxy>();
          }
        }
        xform.Broadphase = new BroadphaseData?();
      }
      else if (broadphase1 != broadphase.Comp2)
      {
        broadphaseData = xform.Broadphase.Value;
        this.RemoveFromEntityTree(broadphaseData.Uid, broadphase1, child, xform);
      }
    }
    this.AddOrUpdateEntityTree(broadphase.Owner, broadphase.Comp2, broadphase.Comp1, child, xform);
  }

  private Box2 GetTreeAABB(EntityUid entity, EntityUid tree)
  {
    TransformComponent component1;
    if (!this._xformQuery.TryGetComponent(entity, out component1))
    {
      this.Log.Error($"Entity tree contains a deleted entity? Tree: {this.ToPrettyString((Entity<MetaDataComponent>) tree)}, entity: {entity}");
      return new Box2();
    }
    if (component1.ParentUid == tree)
      return this.GetAABBNoContainer(entity, component1.LocalPosition, component1.LocalRotation);
    TransformComponent component2;
    if (!this._xformQuery.TryGetComponent(tree, out component2))
    {
      this.Log.Error($"Entity tree has no transform? Tree Uid: {tree}");
      return new Box2();
    }
    Matrix3x2 invWorldMatrix = this._transform.GetInvWorldMatrix(component2);
    Box2 worldAabb = this.GetWorldAABB(entity, component1);
    ref Box2 local = ref worldAabb;
    return Matrix3Helpers.TransformBox(invWorldMatrix, ref local);
  }

  internal void CreateProxies(
    EntityUid uid,
    string fixtureId,
    Fixture fixture,
    TransformComponent xform,
    PhysicsComponent body)
  {
    BroadphaseComponent broadphase;
    if (!this.TryGetCurrentBroadphase(xform, out broadphase))
      return;
    (Vector2 WorldPosition, Angle WorldRotation) positionRotation = this._transform.GetWorldPositionRotation(xform);
    Robust.Shared.Physics.Transform transform = new Robust.Shared.Physics.Transform(positionRotation.WorldPosition, positionRotation.WorldRotation);
    (Vector2 _, Angle WorldRotation, Matrix3x2 _, Matrix3x2 matrix3x2) = this._transform.GetWorldPositionRotationMatrixWithInv(broadphase.Owner);
    Robust.Shared.Physics.Transform broadphaseTransform = new Robust.Shared.Physics.Transform(Vector2.Transform(transform.Position, matrix3x2), Angle.op_Subtraction(Angle.op_Implicit(transform.Quaternion2D.Angle), WorldRotation));
    IBroadPhase tree = body.BodyType == BodyType.Static ? broadphase.StaticTree : broadphase.DynamicTree;
    this.AddOrMoveProxies((Entity<PhysicsComponent, TransformComponent>) (uid, body, xform), fixtureId, fixture, tree, broadphaseTransform);
  }

  internal void DestroyProxies(
    EntityUid uid,
    string fixtureId,
    Fixture fixture,
    TransformComponent xform,
    BroadphaseComponent broadphase)
  {
    if (!xform.Broadphase.Value.CanCollide)
      return;
    EntityUid? gridUid = xform.GridUid;
    EntityUid entityUid = uid;
    if ((gridUid.HasValue ? (gridUid.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0)
      return;
    if (fixture.ProxyCount == 0)
    {
      this.Log.Warning($"Tried to destroy fixture {fixtureId} on {this.ToPrettyString((Entity<MetaDataComponent>) uid)} that already has no proxies?");
    }
    else
    {
      IBroadPhase tree = xform.Broadphase.Value.Static ? broadphase.StaticTree : broadphase.DynamicTree;
      this.DestroyProxies(fixture, tree);
    }
  }

  private void OnPhysicsUpdate(ref CollisionChangeEvent ev)
  {
    TransformComponent xform = this.Transform(ev.BodyUid);
    this.UpdatePhysicsBroadphase(ev.BodyUid, xform, ev.Body);
  }

  private void OnBodyTypeChange(
    EntityUid uid,
    TransformComponent xform,
    ref PhysicsBodyTypeChangedEvent args)
  {
    if (args.Old != BodyType.Static && args.New != BodyType.Static)
      return;
    this.UpdatePhysicsBroadphase(uid, xform, args.Component);
  }

  private void UpdatePhysicsBroadphase(
    EntityUid uid,
    TransformComponent xform,
    PhysicsComponent body)
  {
    if (body.LifeStage <= ComponentLifeStage.Initializing)
      return;
    EntityUid? gridUid = xform.GridUid;
    EntityUid entityUid = uid;
    if ((gridUid.HasValue ? (gridUid.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0)
      return;
    BroadphaseData? broadphase = xform.Broadphase;
    if (!broadphase.HasValue)
      return;
    BroadphaseData valueOrDefault = broadphase.GetValueOrDefault();
    if (!valueOrDefault.Valid)
      return;
    xform.Broadphase = new BroadphaseData?();
    BroadphaseComponent component;
    if (!this._broadQuery.TryGetComponent(valueOrDefault.Uid, out component))
      return;
    FixturesComponent fixturesComponent = this.Comp<FixturesComponent>(uid);
    if (valueOrDefault.CanCollide)
      this.RemoveBroadTree(component, fixturesComponent, valueOrDefault.Static);
    else
      (valueOrDefault.Static ? component.StaticSundriesTree : component.SundriesTree).Remove(in uid);
    if (body.CanCollide)
      this.AddPhysicsTree(uid, valueOrDefault.Uid, component, xform, body, fixturesComponent);
    else
      this.AddOrUpdateSundriesTree(valueOrDefault.Uid, component, uid, xform, body.BodyType == BodyType.Static);
  }

  private void RemoveBroadTree(
    BroadphaseComponent lookup,
    FixturesComponent manager,
    bool staticBody)
  {
    IBroadPhase tree = staticBody ? lookup.StaticTree : lookup.DynamicTree;
    foreach (Fixture fixture in manager.Fixtures.Values)
      this.DestroyProxies(fixture, tree);
  }

  internal void DestroyProxies(Fixture fixture, IBroadPhase tree)
  {
    HashSet<FixtureProxy> moveBuffer = this._physics.MoveBuffer;
    for (int index = 0; index < fixture.ProxyCount; ++index)
    {
      FixtureProxy proxy = fixture.Proxies[index];
      tree.RemoveProxy(proxy.ProxyId);
      moveBuffer.Remove(proxy);
    }
    fixture.ProxyCount = 0;
    fixture.Proxies = Array.Empty<FixtureProxy>();
  }

  private void AddPhysicsTree(
    EntityUid uid,
    EntityUid broadUid,
    BroadphaseComponent broadphase,
    TransformComponent xform,
    PhysicsComponent body,
    FixturesComponent fixtures)
  {
    TransformComponent component = this._xformQuery.GetComponent(broadUid);
    if (component.MapID == MapId.Nullspace)
      return;
    this.AddOrUpdatePhysicsTree(uid, broadUid, broadphase, component, xform, body, fixtures);
  }

  private void AddOrUpdatePhysicsTree(
    EntityUid uid,
    EntityUid broadUid,
    BroadphaseComponent broadphase,
    TransformComponent broadphaseXform,
    TransformComponent xform,
    PhysicsComponent body,
    FixturesComponent manager)
  {
    TransformComponent transformComponent = xform;
    BroadphaseData broadphaseData = transformComponent.Broadphase.GetValueOrDefault();
    if (!transformComponent.Broadphase.HasValue)
    {
      broadphaseData = new BroadphaseData(broadUid, body.CanCollide, body.BodyType == BodyType.Static);
      transformComponent.Broadphase = new BroadphaseData?(broadphaseData);
    }
    IBroadPhase tree = body.BodyType == BodyType.Static ? broadphase.StaticTree : broadphase.DynamicTree;
    (Vector2 WorldPosition, Angle WorldRotation) positionRotation = this._transform.GetWorldPositionRotation(xform);
    Robust.Shared.Physics.Transform transform = new Robust.Shared.Physics.Transform(positionRotation.WorldPosition, positionRotation.WorldRotation);
    Robust.Shared.Physics.Transform broadphaseTransform = new Robust.Shared.Physics.Transform(Vector2.Transform(transform.Position, broadphaseXform.InvLocalMatrix), Angle.op_Subtraction(Angle.op_Implicit(transform.Quaternion2D.Angle), broadphaseXform.LocalRotation));
    foreach ((string str, Fixture fixture) in manager.Fixtures)
      this.AddOrMoveProxies((Entity<PhysicsComponent, TransformComponent>) (uid, body, xform), str, fixture, tree, broadphaseTransform);
  }

  private void AddOrMoveProxies(
    Entity<PhysicsComponent, TransformComponent> ent,
    string fixtureId,
    Fixture fixture,
    IBroadPhase tree,
    Robust.Shared.Physics.Transform broadphaseTransform)
  {
    HashSet<FixtureProxy> moveBuffer = this._physics.MoveBuffer;
    if (fixture.ProxyCount > 0)
    {
      for (int childIndex = 0; childIndex < fixture.ProxyCount; ++childIndex)
      {
        Box2 aabb = fixture.Shape.ComputeAABB(broadphaseTransform, childIndex);
        FixtureProxy proxy = fixture.Proxies[childIndex];
        tree.MoveProxy(proxy.ProxyId, in aabb);
        proxy.AABB = aabb;
        moveBuffer.Add(proxy);
      }
    }
    else
    {
      int childCount = fixture.Shape.ChildCount;
      FixtureProxy[] fixtureProxyArray = new FixtureProxy[childCount];
      for (int childIndex = 0; childIndex < childCount; ++childIndex)
      {
        Box2 aabb = fixture.Shape.ComputeAABB(broadphaseTransform, childIndex);
        FixtureProxy proxy = new FixtureProxy(ent.Owner, ent.Comp1, ent.Comp2, aabb, fixtureId, fixture, childIndex);
        proxy.ProxyId = tree.AddProxy(ref proxy);
        proxy.AABB = aabb;
        fixtureProxyArray[childIndex] = proxy;
        moveBuffer.Add(proxy);
      }
      fixture.Proxies = fixtureProxyArray;
      fixture.ProxyCount = childCount;
    }
  }

  private void AddOrUpdateSundriesTree(
    EntityUid broadUid,
    BroadphaseComponent broadphase,
    EntityUid uid,
    TransformComponent xform,
    bool staticBody,
    Box2? aabb = null)
  {
    TransformComponent transformComponent = xform;
    BroadphaseData broadphaseData = transformComponent.Broadphase.GetValueOrDefault();
    if (!transformComponent.Broadphase.HasValue)
    {
      broadphaseData = new BroadphaseData(broadUid, false, staticBody);
      transformComponent.Broadphase = new BroadphaseData?(broadphaseData);
    }
    (staticBody ? broadphase.StaticSundriesTree : broadphase.SundriesTree).AddOrUpdate(uid, aabb);
  }

  private void OnEntityInit(Entity<MetaDataComponent> uid)
  {
    if (this._container.IsEntityOrParentInContainer((EntityUid) uid, (MetaDataComponent) uid) || this._mapQuery.HasComp((EntityUid) uid) || this._gridQuery.HasComp((EntityUid) uid))
      return;
    this.FindAndAddToEntityTree((EntityUid) uid, false);
  }

  private void OnMove(ref MoveEvent args)
  {
    EntityUid? gridUid = args.Component.GridUid;
    EntityUid sender1 = args.Sender;
    if ((gridUid.HasValue ? (gridUid.GetValueOrDefault() == sender1 ? 1 : 0) : 0) != 0)
    {
      if (!args.ParentChanged)
        return;
      this.OnGridChangedMap(args);
    }
    else
    {
      EntityUid? mapUid = args.Component.MapUid;
      EntityUid sender2 = args.Sender;
      if ((mapUid.HasValue ? (mapUid.GetValueOrDefault() == sender2 ? 1 : 0) : 0) != 0)
        return;
      if (args.ParentChanged)
        this.UpdateParent(args.Sender, args.Component);
      else
        this.UpdateEntityTree(args.Sender, args.Component);
    }
  }

  private void OnGridChangedMap(MoveEvent args) => this.Terminating(args.OldPosition.EntityId);

  private void UpdateParent(EntityUid uid, TransformComponent xform)
  {
    BroadphaseComponent component1 = (BroadphaseComponent) null;
    if (xform.Broadphase.HasValue)
    {
      if (!xform.Broadphase.Value.IsValid())
        return;
      if (!this._broadQuery.TryGetComponent(xform.Broadphase.Value.Uid, out component1))
      {
        FixturesComponent component2;
        if (this._fixturesQuery.TryGetComponent(uid, out component2))
        {
          foreach (Fixture fixture in component2.Fixtures.Values)
          {
            fixture.ProxyCount = 0;
            fixture.Proxies = Array.Empty<FixtureProxy>();
          }
        }
        xform.Broadphase = new BroadphaseData?();
      }
    }
    BroadphaseComponent broadphase;
    this.TryFindBroadphase(xform, out broadphase);
    if (component1 != null && component1 != broadphase)
      this.RemoveFromEntityTree(component1.Owner, component1, uid, xform);
    if (broadphase == null)
      return;
    TransformComponent component3 = this._xformQuery.GetComponent(broadphase.Owner);
    this.AddOrUpdateEntityTree(broadphase.Owner, broadphase, component3, uid, xform);
  }

  public void FindAndAddToEntityTree(EntityUid uid, bool recursive = true, TransformComponent? xform = null)
  {
    BroadphaseComponent broadphase;
    if (!this._xformQuery.Resolve(uid, ref xform) || !this.TryFindBroadphase(xform, out broadphase))
      return;
    this.AddOrUpdateEntityTree(broadphase.Owner, broadphase, uid, xform, recursive);
  }

  public void UpdateEntityTree(EntityUid uid, TransformComponent? xform = null)
  {
    BroadphaseComponent broadphase;
    if (!this._xformQuery.Resolve(uid, ref xform) || !this.TryGetCurrentBroadphase(xform, out broadphase))
      return;
    this.AddOrUpdateEntityTree(broadphase.Owner, broadphase, uid, xform);
  }

  private void AddOrUpdateEntityTree(
    EntityUid broadUid,
    BroadphaseComponent broadphase,
    EntityUid uid,
    TransformComponent xform,
    bool recursive = true)
  {
    TransformComponent component = this._xformQuery.GetComponent(broadphase.Owner);
    this.AddOrUpdateEntityTree(broadUid, broadphase, component, uid, xform, recursive);
  }

  private void AddOrUpdateEntityTree(
    EntityUid broadUid,
    BroadphaseComponent broadphase,
    TransformComponent broadphaseXform,
    EntityUid uid,
    TransformComponent xform,
    bool recursive = true)
  {
    if (xform.Broadphase.HasValue && !xform.Broadphase.Value.IsValid())
      return;
    PhysicsComponent component1;
    if (!this._physicsQuery.TryGetComponent(uid, out component1) || !component1.CanCollide)
    {
      (EntityCoordinates Coords, Angle worldRot) coordinateRotation = this._transform.GetMoverCoordinateRotation(uid, xform);
      EntityCoordinates coords = coordinateRotation.Coords;
      Angle angle = Angle.op_Subtraction(coordinateRotation.worldRot, broadphaseXform.LocalRotation);
      Box2 aabbNoContainer = this.GetAABBNoContainer(uid, coords.Position, angle);
      this.AddOrUpdateSundriesTree(broadUid, broadphase, uid, xform, component1 != null && component1.BodyType == BodyType.Static, new Box2?(aabbNoContainer));
    }
    else
      this.AddOrUpdatePhysicsTree(uid, broadUid, broadphase, broadphaseXform, xform, component1, this._fixturesQuery.GetComponent(uid));
    if (xform.ChildCount == 0 || !recursive)
      return;
    if (!this._containerQuery.HasComponent(uid))
    {
      foreach (EntityUid child in xform._children)
      {
        TransformComponent component2 = this._xformQuery.GetComponent(child);
        this.AddOrUpdateEntityTree(broadUid, broadphase, broadphaseXform, child, component2, recursive);
      }
    }
    else
    {
      foreach (EntityUid child in xform._children)
      {
        if ((this._metaQuery.GetComponent(child).Flags & MetaDataFlags.InContainer) == MetaDataFlags.None)
        {
          TransformComponent component3 = this._xformQuery.GetComponent(child);
          this.AddOrUpdateEntityTree(broadUid, broadphase, broadphaseXform, child, component3, recursive);
        }
      }
    }
  }

  public void RemoveFromEntityTree(EntityUid uid, TransformComponent xform)
  {
    BroadphaseComponent broadphase;
    if (!this.TryGetCurrentBroadphase(xform, out broadphase))
      return;
    this.RemoveFromEntityTree(broadphase.Owner, broadphase, uid, xform);
  }

  private void RemoveFromEntityTree(
    EntityUid broadUid,
    BroadphaseComponent broadphase,
    EntityUid uid,
    TransformComponent xform,
    bool recursive = true)
  {
    BroadphaseData? broadphase1 = xform.Broadphase;
    if (!broadphase1.HasValue)
      return;
    BroadphaseData valueOrDefault = broadphase1.GetValueOrDefault();
    if (!valueOrDefault.Valid)
      return;
    if (valueOrDefault.Uid != broadUid)
      broadUid = valueOrDefault.Uid;
    if (valueOrDefault.CanCollide)
      this.RemoveBroadTree(broadphase, this._fixturesQuery.GetComponent(uid), valueOrDefault.Static);
    else if (valueOrDefault.Static)
      broadphase.StaticSundriesTree.Remove(in uid);
    else
      broadphase.SundriesTree.Remove(in uid);
    xform.Broadphase = new BroadphaseData?();
    if (!recursive)
      return;
    foreach (EntityUid child in xform._children)
      this.RemoveFromEntityTree(broadUid, broadphase, child, this._xformQuery.GetComponent(child));
  }

  public bool TryGetCurrentBroadphase(TransformComponent xform, [NotNullWhen(true)] out BroadphaseComponent? broadphase)
  {
    broadphase = (BroadphaseComponent) null;
    BroadphaseData? broadphase1 = xform.Broadphase;
    if (broadphase1.HasValue)
    {
      BroadphaseData valueOrDefault = broadphase1.GetValueOrDefault();
      if (valueOrDefault.Valid)
      {
        if (this._broadQuery.TryGetComponent(valueOrDefault.Uid, out broadphase))
          return true;
        FixturesComponent component;
        if (this._fixturesQuery.TryGetComponent(xform.Owner, out component))
        {
          foreach (Fixture fixture in component.Fixtures.Values)
          {
            fixture.ProxyCount = 0;
            fixture.Proxies = Array.Empty<FixtureProxy>();
          }
        }
        xform.Broadphase = new BroadphaseData?();
        return false;
      }
    }
    return false;
  }

  public BroadphaseComponent? GetCurrentBroadphase(TransformComponent xform)
  {
    BroadphaseComponent broadphase;
    this.TryGetCurrentBroadphase(xform, out broadphase);
    return broadphase;
  }

  public BroadphaseComponent? FindBroadphase(EntityUid uid)
  {
    BroadphaseComponent broadphase;
    this.TryFindBroadphase(uid, out broadphase);
    return broadphase;
  }

  public bool TryFindBroadphase(EntityUid uid, [NotNullWhen(true)] out BroadphaseComponent? broadphase)
  {
    return this.TryFindBroadphase(this._xformQuery.GetComponent(uid), out broadphase);
  }

  public bool TryFindBroadphase(TransformComponent xform, [NotNullWhen(true)] out BroadphaseComponent? broadphase)
  {
    if (xform.MapID == MapId.Nullspace || this._container.IsEntityOrParentInContainer(xform.Owner, xform: xform))
    {
      broadphase = (BroadphaseComponent) null;
      return false;
    }
    for (EntityUid parentUid = xform.ParentUid; parentUid.IsValid(); parentUid = this._xformQuery.GetComponent(parentUid).ParentUid)
    {
      if (this._broadQuery.TryGetComponent(parentUid, out broadphase))
        return true;
    }
    broadphase = (BroadphaseComponent) null;
    return false;
  }

  public Box2 GetAABB(
    EntityUid uid,
    Vector2 position,
    Angle angle,
    TransformComponent xform,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> xformQuery)
  {
    BaseContainer container;
    return this._container.TryGetOuterContainer(uid, xform, out container, xformQuery) ? this.GetAABBNoContainer(container.Owner, position, angle) : this.GetAABBNoContainer(uid, position, angle);
  }

  public Box2 GetAABBNoContainer(EntityUid uid, Vector2 position, Angle angle)
  {
    FixturesComponent component;
    if (this._fixturesQuery.TryGetComponent(uid, out component))
    {
      Robust.Shared.Physics.Transform transform = new Robust.Shared.Physics.Transform(position, angle);
      Box2 aabbNoContainer;
      // ISSUE: explicit constructor call
      ((Box2) ref aabbNoContainer).\u002Ector(transform.Position, transform.Position);
      foreach (Fixture fixture in component.Fixtures.Values)
      {
        for (int childIndex = 0; childIndex < fixture.Shape.ChildCount; ++childIndex)
        {
          Box2 aabb = fixture.Shape.ComputeAABB(transform, childIndex);
          aabbNoContainer = ((Box2) ref aabbNoContainer).Union(ref aabb);
        }
      }
      return aabbNoContainer;
    }
    WorldAABBEvent args = new WorldAABBEvent()
    {
      AABB = new Box2(position, position)
    };
    this.RaiseLocalEvent<WorldAABBEvent>(uid, ref args);
    return args.AABB;
  }

  public Box2 GetWorldAABB(EntityUid uid, TransformComponent? xform = null)
  {
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> entityQuery = this.GetEntityQuery<TransformComponent>();
    if (xform == null)
      xform = entityQuery.GetComponent(uid);
    (Vector2 vector2, Angle angle) = this._transform.GetWorldPositionRotation(xform, entityQuery);
    return this.GetAABB(uid, vector2, angle, xform, entityQuery);
  }

  private void AddLocalEntitiesIntersecting(
    EntityUid lookupUid,
    HashSet<EntityUid> intersecting,
    Box2 localAABB,
    LookupFlags flags,
    BroadphaseComponent? lookup = null)
  {
    if (!this._broadQuery.Resolve(lookupUid, ref lookup))
      return;
    SlimPolygon shape = new SlimPolygon(localAABB);
    this.AddEntitiesIntersecting<SlimPolygon>(lookupUid, intersecting, shape, localAABB, Robust.Shared.Physics.Transform.Empty, flags, lookup);
  }

  private void AddLocalEntitiesIntersecting(
    EntityUid lookupUid,
    HashSet<EntityUid> intersecting,
    Box2Rotated localBounds,
    LookupFlags flags,
    BroadphaseComponent? lookup = null)
  {
    if (!this._broadQuery.Resolve(lookupUid, ref lookup))
      return;
    SlimPolygon shape = new SlimPolygon(in localBounds);
    Box2 localAABB = ((Box2Rotated) ref localBounds).CalcBoundingBox();
    this.AddEntitiesIntersecting<SlimPolygon>(lookupUid, intersecting, shape, localAABB, Robust.Shared.Physics.Transform.Empty, flags);
  }

  public bool AnyLocalEntitiesIntersecting(
    EntityUid lookupUid,
    Box2 localAABB,
    LookupFlags flags,
    EntityUid? ignored = null,
    BroadphaseComponent? lookup = null)
  {
    if (!this._broadQuery.Resolve(lookupUid, ref lookup))
      return false;
    SlimPolygon shape = new SlimPolygon(localAABB);
    return this.AnyEntitiesIntersecting<SlimPolygon>(lookupUid, shape, localAABB, Robust.Shared.Physics.Transform.Empty, flags, ignored, lookup);
  }

  public HashSet<EntityUid> GetLocalEntitiesIntersecting(
    EntityUid gridId,
    Vector2i gridIndices,
    float enlargement = -0.04f,
    LookupFlags flags = LookupFlags.All,
    MapGridComponent? gridComp = null)
  {
    HashSet<EntityUid> intersecting = new HashSet<EntityUid>();
    this.GetLocalEntitiesIntersecting(gridId, gridIndices, intersecting, enlargement, flags, gridComp);
    return intersecting;
  }

  public void GetLocalEntitiesIntersecting(
    EntityUid gridUid,
    IPhysShape shape,
    Robust.Shared.Physics.Transform localTransform,
    HashSet<EntityUid> intersecting,
    LookupFlags flags = LookupFlags.All,
    BroadphaseComponent? lookup = null)
  {
    Box2 aabb = shape.ComputeAABB(localTransform, 0);
    this.AddEntitiesIntersecting<IPhysShape>(gridUid, intersecting, shape, aabb, localTransform, flags, lookup);
    this.AddContained(intersecting, flags);
  }

  public void GetLocalEntitiesIntersecting(
    EntityUid gridUid,
    Vector2i localTile,
    HashSet<EntityUid> intersecting,
    float enlargement = -0.04f,
    LookupFlags flags = LookupFlags.All,
    MapGridComponent? gridComp = null)
  {
    ushort tileSize = 1;
    if (this._gridQuery.Resolve(gridUid, ref gridComp))
      tileSize = gridComp.TileSize;
    Box2 localBounds = this.GetLocalBounds(localTile, tileSize);
    Box2 localAABB = ((Box2) ref localBounds).Enlarged(enlargement);
    this.GetLocalEntitiesIntersecting(gridUid, localAABB, intersecting, flags);
  }

  public void GetLocalEntitiesIntersecting(
    EntityUid gridUid,
    Box2 localAABB,
    HashSet<EntityUid> intersecting,
    LookupFlags flags = LookupFlags.All)
  {
    this.AddLocalEntitiesIntersecting(gridUid, intersecting, localAABB, flags);
    this.AddContained(intersecting, flags);
  }

  public void GetLocalEntitiesIntersecting(
    EntityUid gridUid,
    Box2Rotated localBounds,
    HashSet<EntityUid> intersecting,
    LookupFlags flags = LookupFlags.All)
  {
    this.AddLocalEntitiesIntersecting(gridUid, intersecting, localBounds, flags);
    this.AddContained(intersecting, flags);
  }

  public HashSet<EntityUid> GetLocalEntitiesIntersecting(
    EntityUid gridId,
    IEnumerable<Vector2i> gridIndices,
    LookupFlags flags = LookupFlags.All)
  {
    HashSet<EntityUid> intersecting = new HashSet<EntityUid>();
    MapGridComponent component;
    if (!this._gridQuery.TryGetComponent(gridId, out component))
      return intersecting;
    foreach (Vector2i gridIndex in gridIndices)
      this.GetLocalEntitiesIntersecting(gridId, gridIndex, intersecting, flags: flags, gridComp: component);
    return intersecting;
  }

  public HashSet<EntityUid> GetLocalEntitiesIntersecting(
    BroadphaseComponent lookup,
    Box2 localAABB,
    LookupFlags flags = LookupFlags.All)
  {
    HashSet<EntityUid> intersecting = new HashSet<EntityUid>();
    this.AddLocalEntitiesIntersecting(lookup.Owner, intersecting, localAABB, flags, lookup);
    this.AddContained(intersecting, flags);
    return intersecting;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public IEnumerable<EntityUid> GetLocalEntitiesIntersecting(
    TileRef tileRef,
    float enlargement = -0.04f,
    LookupFlags flags = LookupFlags.All)
  {
    return (IEnumerable<EntityUid>) this.GetLocalEntitiesIntersecting(tileRef.GridUid, tileRef.GridIndices, enlargement, flags);
  }

  private record struct AnyEntityQueryState<T>(
    bool Found,
    EntityUid? Ignored,
    T Shape,
    Robust.Shared.Physics.Transform Transform,
    FixtureSystem Fixtures,
    EntityLookupSystem Lookup,
    SharedPhysicsSystem Physics,
    IManifoldManager Manifolds,
    Robust.Shared.GameObjects.EntityQuery<FixturesComponent> FixturesQuery,
    LookupFlags Flags)
    where T : IPhysShape
  ;

  private readonly record struct EntityQueryState<T>(
    HashSet<EntityUid> Intersecting,
    T Shape,
    Robust.Shared.Physics.Transform Transform,
    FixtureSystem Fixtures,
    EntityLookupSystem Lookup,
    SharedPhysicsSystem Physics,
    IManifoldManager Manifolds,
    Robust.Shared.GameObjects.EntityQuery<FixturesComponent> FixturesQuery,
    LookupFlags Flags)
    where T : IPhysShape
  ;

  private readonly record struct GridQueryState<T, TShape>(
    HashSet<Entity<T>> Intersecting,
    TShape Shape,
    Robust.Shared.Physics.Transform Transform,
    EntityLookupSystem Lookup,
    SharedPhysicsSystem Physics,
    LookupFlags Flags,
    Robust.Shared.GameObjects.EntityQuery<T> Query)
    where T : IComponent
    where TShape : IPhysShape
  ;

  private record struct AnyQueryState<T, TShape>(
    bool Found,
    EntityUid? Ignored,
    TShape Shape,
    Robust.Shared.Physics.Transform Transform,
    FixtureSystem Fixtures,
    SharedPhysicsSystem Physics,
    IManifoldManager Manifolds,
    Robust.Shared.GameObjects.EntityQuery<T> Query,
    Robust.Shared.GameObjects.EntityQuery<FixturesComponent> FixturesQuery,
    LookupFlags Flags)
    where T : IComponent
    where TShape : IPhysShape
  ;

  private readonly record struct QueryState<T, TShape>(
    HashSet<Entity<T>> Intersecting,
    TShape Shape,
    Robust.Shared.Physics.Transform Transform,
    FixtureSystem Fixtures,
    SharedPhysicsSystem Physics,
    IManifoldManager Manifolds,
    Robust.Shared.GameObjects.EntityQuery<T> Query,
    Robust.Shared.GameObjects.EntityQuery<FixturesComponent> FixturesQuery,
    bool Sensors,
    bool Approximate)
    where T : IComponent
    where TShape : IPhysShape
  ;
}
