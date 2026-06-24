// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Systems.SharedBroadphaseSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Dynamics.Contacts;
using Robust.Shared.Threading;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Physics.Systems;

public abstract class SharedBroadphaseSystem : EntitySystem
{
  [Robust.Shared.IoC.Dependency]
  private readonly IConfigurationManager _cfg;
  [Robust.Shared.IoC.Dependency]
  private readonly IMapManagerInternal _mapManager;
  [Robust.Shared.IoC.Dependency]
  private readonly IParallelManager _parallel;
  [Robust.Shared.IoC.Dependency]
  private readonly EntityLookupSystem _lookup;
  [Robust.Shared.IoC.Dependency]
  private readonly SharedGridTraversalSystem _traversal;
  [Robust.Shared.IoC.Dependency]
  private readonly SharedMapSystem _map;
  [Robust.Shared.IoC.Dependency]
  private readonly SharedPhysicsSystem _physicsSystem;
  [Robust.Shared.IoC.Dependency]
  private readonly SharedTransformSystem _transform;
  private Robust.Shared.GameObjects.EntityQuery<BroadphaseComponent> _broadphaseQuery;
  private Robust.Shared.GameObjects.EntityQuery<FixturesComponent> _fixturesQuery;
  private Robust.Shared.GameObjects.EntityQuery<MapGridComponent> _gridQuery;
  private Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> _physicsQuery;
  private Robust.Shared.GameObjects.EntityQuery<TransformComponent> _xformQuery;
  private readonly HashSet<FixtureProxy> _gridMoveBuffer = new HashSet<FixtureProxy>();
  private float _frameTime;
  private SharedBroadphaseSystem.BroadphaseContactJob _contactJob;

  public override void Initialize()
  {
    base.Initialize();
    this._contactJob = new SharedBroadphaseSystem.BroadphaseContactJob()
    {
      MapManager = (IMapManager) this._mapManager,
      System = this,
      TransformSys = this.EntityManager.System<SharedTransformSystem>(),
      XformQuery = this.GetEntityQuery<TransformComponent>()
    };
    this._broadphaseQuery = this.GetEntityQuery<BroadphaseComponent>();
    this._fixturesQuery = this.GetEntityQuery<FixturesComponent>();
    this._gridQuery = this.GetEntityQuery<MapGridComponent>();
    this._physicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this._xformQuery = this.GetEntityQuery<TransformComponent>();
    this.UpdatesOutsidePrediction = true;
    this.UpdatesAfter.Add(typeof (SharedTransformSystem));
    this.Subs.CVar<int>(this._cfg, CVars.TargetMinimumTickrate, (Action<int>) (val => this._frameTime = 1f / (float) val), true);
  }

  public void Rebuild(BroadphaseComponent component, bool fullBuild)
  {
    component.StaticTree.Rebuild(fullBuild);
    component.DynamicTree.Rebuild(fullBuild);
    component.SundriesTree._b2Tree.Rebuild(fullBuild);
    component.StaticSundriesTree._b2Tree.Rebuild(fullBuild);
  }

  public void RebuildBottomUp(BroadphaseComponent component)
  {
    component.StaticTree.RebuildBottomUp();
    component.DynamicTree.RebuildBottomUp();
    component.SundriesTree._b2Tree.RebuildBottomUp();
    component.StaticSundriesTree._b2Tree.RebuildBottomUp();
  }

  private void FindGridContacts(HashSet<EntityUid> movedGrids)
  {
    if (movedGrids.Count == 0)
      return;
    HashSet<FixtureProxy> moveBuffer = this._physicsSystem.MoveBuffer;
    this._gridMoveBuffer.Clear();
    foreach (EntityUid movedGrid in movedGrids)
    {
      MapGridComponent component1 = this._gridQuery.GetComponent(movedGrid);
      TransformComponent component2 = this._xformQuery.GetComponent(movedGrid);
      BroadphaseComponent component3;
      if (this._broadphaseQuery.TryComp(component2.MapUid, out component3))
      {
        Matrix3x2 worldMatrix = this._transform.GetWorldMatrix(component2);
        Box2 localAabb = component1.LocalAABB;
        ref Box2 local = ref localAabb;
        Box2 box2 = Matrix3Helpers.TransformBox(worldMatrix, ref local);
        Box2 enlargedAABB = ((Box2) ref box2).Enlarged(this.GetBroadphaseExpand(this._physicsQuery.GetComponent(movedGrid), this._frameTime));
        (HashSet<FixtureProxy>, HashSet<FixtureProxy>) state = (moveBuffer, this._gridMoveBuffer);
        this.QueryMapBroadphase(component3.DynamicTree, ref state, enlargedAABB);
        this.QueryMapBroadphase(component3.StaticTree, ref state, enlargedAABB);
      }
    }
    foreach (FixtureProxy fixtureProxy in this._gridMoveBuffer)
    {
      moveBuffer.Add(fixtureProxy);
      this._traversal.CheckTraverse((Entity<TransformComponent>) (fixtureProxy.Entity, this._xformQuery.GetComponent(fixtureProxy.Entity)));
    }
  }

  private float GetBroadphaseExpand(PhysicsComponent body, float frameTime)
  {
    return body.LinearVelocity.Length() * 1.2f * frameTime;
  }

  private void QueryMapBroadphase(
    IBroadPhase broadPhase,
    ref (HashSet<FixtureProxy>, HashSet<FixtureProxy>) state,
    Box2 enlargedAABB)
  {
    broadPhase.QueryAabb<(HashSet<FixtureProxy>, HashSet<FixtureProxy>)>(ref state, (DynamicTree<FixtureProxy>.QueryCallbackDelegate<(HashSet<FixtureProxy>, HashSet<FixtureProxy>)>) ((ref (HashSet<FixtureProxy> moveBuffer, HashSet<FixtureProxy> gridMoveBuffer) tuple, in FixtureProxy value) =>
    {
      if (tuple.Item1.Contains(value))
        return true;
      tuple.Item2.Add(value);
      return true;
    }), enlargedAABB, true);
  }

  internal void FindNewContacts()
  {
    this._contactJob.FrameTime = this._frameTime;
    this._contactJob.Pairs.Clear();
    HashSet<FixtureProxy> moveBuffer = this._physicsSystem.MoveBuffer;
    HashSet<EntityUid> movedGrids = this._physicsSystem.MovedGrids;
    this.FindGridContacts(movedGrids);
    this.HandleGridCollisions(movedGrids);
    if (moveBuffer.Count == 0)
      return;
    this._contactJob.MoveBuffer.Clear();
    foreach (FixtureProxy fixtureProxy in moveBuffer)
      this._contactJob.MoveBuffer.Add(fixtureProxy);
    this._parallel.ProcessNow((IParallelRobustJob) this._contactJob, moveBuffer.Count);
    foreach ((FixtureProxy proxyA, FixtureProxy proxyB, SharedBroadphaseSystem.PairFlag pairFlag) in this._contactJob.Pairs)
    {
      PhysicsComponent body = proxyB.Body;
      ContactFlags flags = ContactFlags.None;
      if ((pairFlag & SharedBroadphaseSystem.PairFlag.Wake) == SharedBroadphaseSystem.PairFlag.Wake)
      {
        this._physicsSystem.WakeBody(proxyA.Entity, true, body: proxyA.Body);
        this._physicsSystem.WakeBody(proxyB.Entity, true, body: body);
      }
      if ((SharedBroadphaseSystem.PairFlag.Grid & pairFlag) == SharedBroadphaseSystem.PairFlag.Grid)
        flags |= ContactFlags.Grid;
      this._physicsSystem.AddPair(proxyA.FixtureId, proxyB.FixtureId, in proxyA, in proxyB, flags);
    }
    moveBuffer.Clear();
    movedGrids.Clear();
  }

  private void HandleGridCollisions(HashSet<EntityUid> movedGrids)
  {
    foreach (EntityUid movedGrid in movedGrids)
    {
      MapGridComponent component1 = this._gridQuery.GetComponent(movedGrid);
      TransformComponent component2 = this._xformQuery.GetComponent(movedGrid);
      if (!(component2.MapID == MapId.Nullspace))
      {
        (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix1, Matrix3x2 InvWorldMatrix1) = this._transform.GetWorldPositionRotationMatrixWithInv(component2);
        Box2Rotated box2Rotated = new Box2Rotated(component1.LocalAABB, WorldRotation);
        Box2 box2_1 = ((Box2Rotated) ref box2Rotated).CalcBoundingBox();
        Box2 worldAABB = ((Box2) ref box2_1).Translated(WorldPosition);
        FixturesComponent comp1 = this._fixturesQuery.Comp(movedGrid);
        PhysicsComponent comp3_1 = this._physicsQuery.Comp(movedGrid);
        Robust.Shared.Physics.Transform physicsTransform1 = this._physicsSystem.GetPhysicsTransform(movedGrid);
        (Entity<FixturesComponent, MapGridComponent, PhysicsComponent, TransformComponent>, Robust.Shared.Physics.Transform, Matrix3x2, Matrix3x2, SharedMapSystem, SharedPhysicsSystem, SharedTransformSystem, Robust.Shared.GameObjects.EntityQuery<FixturesComponent>, Robust.Shared.GameObjects.EntityQuery<PhysicsComponent>, Robust.Shared.GameObjects.EntityQuery<TransformComponent>) state = (new Entity<FixturesComponent, MapGridComponent, PhysicsComponent, TransformComponent>(movedGrid, comp1, component1, comp3_1, component2), physicsTransform1, WorldMatrix1, InvWorldMatrix1, this._map, this._physicsSystem, this._transform, this._fixturesQuery, this._physicsQuery, this._xformQuery);
        this._mapManager.FindGridsIntersecting<(Entity<FixturesComponent, MapGridComponent, PhysicsComponent, TransformComponent>, Robust.Shared.Physics.Transform, Matrix3x2, Matrix3x2, SharedMapSystem, SharedPhysicsSystem, SharedTransformSystem, Robust.Shared.GameObjects.EntityQuery<FixturesComponent>, Robust.Shared.GameObjects.EntityQuery<PhysicsComponent>, Robust.Shared.GameObjects.EntityQuery<TransformComponent>)>(component2.MapID, worldAABB, ref state, (GridCallback<(Entity<FixturesComponent, MapGridComponent, PhysicsComponent, TransformComponent>, Robust.Shared.Physics.Transform, Matrix3x2, Matrix3x2, SharedMapSystem, SharedPhysicsSystem, SharedTransformSystem, Robust.Shared.GameObjects.EntityQuery<FixturesComponent>, Robust.Shared.GameObjects.EntityQuery<PhysicsComponent>, Robust.Shared.GameObjects.EntityQuery<TransformComponent>)>) ((EntityUid uid, MapGridComponent component, ref (Entity<FixturesComponent, MapGridComponent, PhysicsComponent, TransformComponent> grid, Robust.Shared.Physics.Transform transform, Matrix3x2 worldMatrix, Matrix3x2 invWorldMatrix, SharedMapSystem _map, SharedPhysicsSystem _physicsSystem, SharedTransformSystem xformSystem, Robust.Shared.GameObjects.EntityQuery<FixturesComponent> fixturesQuery, Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> physicsQuery, Robust.Shared.GameObjects.EntityQuery<TransformComponent> xformQuery) tuple) =>
        {
          TransformComponent component3;
          if (tuple.Item1.Owner == uid || !tuple.Item10.TryGetComponent(uid, out component3) || tuple.Item1.Owner.Id > uid.Id && tuple.Item6.MovedGrids.Contains(uid))
            return true;
          (Vector2 _, Angle _, Matrix3x2 WorldMatrix3, Matrix3x2 InvWorldMatrix3) = tuple.Item7.GetWorldPositionRotationMatrixWithInv(component3);
          Matrix3x2 matrix3x2_1 = WorldMatrix3;
          Box2 localAabb1 = component.LocalAABB;
          ref Box2 local1 = ref localAabb1;
          Box2 box2_2 = Matrix3Helpers.TransformBox(matrix3x2_1, ref local1);
          Robust.Shared.Physics.Transform physicsTransform2 = tuple.Item6.GetPhysicsTransform(uid);
          Box2 localAabb2 = tuple.Item1.Comp2.LocalAABB;
          ref Box2 local2 = ref localAabb2;
          Box2 box2_3 = Matrix3Helpers.TransformBox(tuple.Item4, ref box2_2);
          ref Box2 local3 = ref box2_3;
          Box2 localAABB1 = ((Box2) ref local2).Intersect(ref local3);
          ChunkEnumerator localMapChunks1 = tuple.Item5.GetLocalMapChunks(tuple.Item1.Owner, (MapGridComponent) tuple.Item1, localAABB1);
          PhysicsComponent comp3_2 = tuple.Item1.Comp3;
          PhysicsComponent component4 = tuple.Item9.GetComponent(uid);
          FixturesComponent fixturesComponent = tuple.Item8.Comp(uid);
          MapChunk chunk1;
          while (localMapChunks1.MoveNext(out chunk1))
          {
            Matrix3x2 matrix3x2_2 = tuple.Item3;
            Box2i cachedBounds = chunk1.CachedBounds;
            Box2 box2_4 = Box2i.op_Implicit(((Box2i) ref cachedBounds).Translated(Vector2i.op_Multiply(chunk1.Indices, (int) tuple.Item1.Comp2.ChunkSize)));
            ref Box2 local4 = ref box2_4;
            Box2 box2_5 = Matrix3Helpers.TransformBox(matrix3x2_2, ref local4);
            Box2 localAABB2 = Matrix3Helpers.TransformBox(InvWorldMatrix3, ref box2_5);
            ChunkEnumerator localMapChunks2 = tuple.Item5.GetLocalMapChunks(uid, component, localAABB2);
label_5:
            MapChunk chunk2;
            if (localMapChunks2.MoveNext(out chunk2))
            {
              using (HashSet<string>.Enumerator enumerator = chunk1.Fixtures.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  string current = enumerator.Current;
                  Fixture fixture1 = tuple.Item1.Comp1.Fixtures[current];
                  for (int index1 = 0; index1 < fixture1.Shape.ChildCount; ++index1)
                  {
                    Box2 aabb1 = fixture1.Shape.ComputeAABB(tuple.Item2, index1);
                    foreach (string fixture2 in chunk2.Fixtures)
                    {
                      Fixture fixture3 = fixturesComponent.Fixtures[fixture2];
                      if (!fixture1.Contacts.ContainsKey(fixture3))
                      {
                        for (int index2 = 0; index2 < fixture3.Shape.ChildCount; ++index2)
                        {
                          Box2 aabb2 = fixture3.Shape.ComputeAABB(physicsTransform2, index2);
                          if (((Box2) ref aabb1).Intersects(ref aabb2))
                          {
                            tuple.Item6.AddPair((Entity<PhysicsComponent, TransformComponent>) (tuple.Item1.Owner, tuple.Item1.Comp3, tuple.Item1.Comp4), (Entity<PhysicsComponent, TransformComponent>) (uid, component4, component3), current, fixture2, fixture1, index1, fixture3, index2, comp3_2, component4, ContactFlags.Grid);
                            break;
                          }
                        }
                      }
                      else
                        break;
                    }
                  }
                }
                goto label_5;
              }
            }
          }
          return true;
        }), true, false);
      }
    }
  }

  private void FindPairs(
    FixtureProxy proxy,
    Box2 worldAABB,
    EntityUid broadphase,
    List<(FixtureProxy, FixtureProxy, SharedBroadphaseSystem.PairFlag)> pairBuffer)
  {
    TransformComponent component1;
    if (proxy.Entity == broadphase || !this._xformQuery.TryGetComponent(proxy.Entity, out component1))
      return;
    BroadphaseComponent broadphase1;
    if (!this._lookup.TryGetCurrentBroadphase(component1, out broadphase1))
    {
      this.Log.Error($"Found null broadphase for {this.ToPrettyString((Entity<MetaDataComponent>) proxy.Entity)}");
    }
    else
    {
      Box2 aabb = !(broadphase1.Owner == broadphase) ? Matrix3Helpers.TransformBox(this._transform.GetInvWorldMatrix(broadphase), ref worldAABB) : proxy.AABB;
      BroadphaseComponent component2 = this._broadphaseQuery.GetComponent(broadphase);
      (List<(FixtureProxy, FixtureProxy, SharedBroadphaseSystem.PairFlag)>, HashSet<FixtureProxy>, SharedBroadphaseSystem, SharedPhysicsSystem, FixtureProxy) state = (pairBuffer, this._physicsSystem.MoveBuffer, this, this._physicsSystem, proxy);
      this.QueryBroadphase(component2.DynamicTree, state, aabb);
      if ((proxy.Body.BodyType & BodyType.Static) != BodyType.Kinematic)
        return;
      this.QueryBroadphase(component2.StaticTree, state, aabb);
    }
  }

  private void QueryBroadphase(
    IBroadPhase broadPhase,
    (List<(FixtureProxy, FixtureProxy, SharedBroadphaseSystem.PairFlag)>, HashSet<FixtureProxy> MoveBuffer, SharedBroadphaseSystem Broadphase, SharedPhysicsSystem PhysicsSystem, FixtureProxy) state,
    Box2 aabb)
  {
    broadPhase.QueryAabb<(List<(FixtureProxy, FixtureProxy, SharedBroadphaseSystem.PairFlag)>, HashSet<FixtureProxy>, SharedBroadphaseSystem, SharedPhysicsSystem, FixtureProxy)>(ref state, (DynamicTree<FixtureProxy>.QueryCallbackDelegate<(List<(FixtureProxy, FixtureProxy, SharedBroadphaseSystem.PairFlag)>, HashSet<FixtureProxy>, SharedBroadphaseSystem, SharedPhysicsSystem, FixtureProxy)>) ((ref (List<(FixtureProxy, FixtureProxy, SharedBroadphaseSystem.PairFlag)> pairs, HashSet<FixtureProxy> moveBuffer, SharedBroadphaseSystem broadphase, SharedPhysicsSystem physicsSystem, FixtureProxy proxy) tuple, in FixtureProxy other) =>
    {
      if (tuple.Item5.Entity == other.Entity || !SharedPhysicsSystem.ShouldCollide(tuple.Item5.Fixture, other.Fixture) || tuple.Item5.Entity.Id > other.Entity.Id && tuple.Item2.Contains(other) || tuple.Item5.Fixture.Contacts.ContainsKey(other.Fixture) || !tuple.Item4.ShouldCollideJoints((Entity<JointComponent>) tuple.Item5.Entity, (Entity<JointComponent>) other.Entity))
        return true;
      SharedBroadphaseSystem.PairFlag pairFlag = SharedBroadphaseSystem.PairFlag.None;
      if (tuple.Item5.Fixture.Hard && other.Fixture.Hard && (tuple.Item3._gridMoveBuffer.Contains(tuple.Item5) || tuple.Item3._gridMoveBuffer.Contains(other)))
        pairFlag |= SharedBroadphaseSystem.PairFlag.Wake;
      lock (tuple.Item1)
        tuple.Item1.Add((tuple.Item5, other, pairFlag));
      return true;
    }), aabb, true);
  }

  [Obsolete("Use Entity<T> variant")]
  public void RegenerateContacts(
    EntityUid uid,
    PhysicsComponent body,
    FixturesComponent? fixtures = null,
    TransformComponent? xform = null)
  {
    this.RegenerateContacts((Entity<PhysicsComponent, FixturesComponent, TransformComponent>) (uid, body, fixtures, xform));
  }

  public void RegenerateContacts(
    Entity<PhysicsComponent?, FixturesComponent?, TransformComponent?> entity)
  {
    if (!this.Resolve<PhysicsComponent>(entity.Owner, ref entity.Comp1))
      return;
    this._physicsSystem.DestroyContacts(entity.Comp1);
    if (!this.Resolve<FixturesComponent, TransformComponent>(entity.Owner, ref entity.Comp2, ref entity.Comp3) || !entity.Comp3.MapUid.HasValue)
      return;
    ref Robust.Shared.GameObjects.EntityQuery<TransformComponent> local1 = ref this._xformQuery;
    ref BroadphaseData? local2 = ref entity.Comp3.Broadphase;
    EntityUid? uid = local2.HasValue ? new EntityUid?(local2.GetValueOrDefault().Uid) : new EntityUid?();
    TransformComponent transformComponent;
    ref TransformComponent local3 = ref transformComponent;
    if (!local1.TryGetComponent(uid, out local3))
      return;
    this._physicsSystem.SetAwake((Entity<PhysicsComponent>) entity, true);
    foreach (Fixture fixture in entity.Comp2.Fixtures.Values)
      this.TouchProxies(fixture);
  }

  internal void TouchProxies(Fixture fixture)
  {
    foreach (FixtureProxy proxy in fixture.Proxies)
      this.AddToMoveBuffer(proxy);
  }

  private void AddToMoveBuffer(FixtureProxy proxy) => this._physicsSystem.MoveBuffer.Add(proxy);

  public void Refilter(EntityUid uid, Fixture fixture, TransformComponent? xform = null)
  {
    foreach (Contact contact in fixture.Contacts.Values)
      contact.Flags |= ContactFlags.Filter;
    if (!this.Resolve(uid, ref xform) || !xform.MapUid.HasValue)
      return;
    ref Robust.Shared.GameObjects.EntityQuery<TransformComponent> local1 = ref this._xformQuery;
    ref BroadphaseData? local2 = ref xform.Broadphase;
    EntityUid? uid1 = local2.HasValue ? new EntityUid?(local2.GetValueOrDefault().Uid) : new EntityUid?();
    TransformComponent transformComponent;
    ref TransformComponent local3 = ref transformComponent;
    if (!local1.TryGetComponent(uid1, out local3))
      return;
    this.TouchProxies(fixture);
  }

  internal void GetBroadphases(
    MapId mapId,
    Box2 aabb,
    SharedBroadphaseSystem.BroadphaseCallback callback)
  {
    (SharedBroadphaseSystem.BroadphaseCallback, Robust.Shared.GameObjects.EntityQuery<BroadphaseComponent>) state = (callback, this._broadphaseQuery);
    EntityUid? uid1;
    if (!this._map.TryGetMap(new MapId?(mapId), out uid1))
      return;
    BroadphaseComponent component1;
    if (this._broadphaseQuery.TryGetComponent(uid1.Value, out component1))
      callback((Entity<BroadphaseComponent>) (uid1.Value, component1));
    this._mapManager.FindGridsIntersecting<(SharedBroadphaseSystem.BroadphaseCallback, Robust.Shared.GameObjects.EntityQuery<BroadphaseComponent>)>(uid1.Value, aabb, ref state, (GridCallback<(SharedBroadphaseSystem.BroadphaseCallback, Robust.Shared.GameObjects.EntityQuery<BroadphaseComponent>)>) ((EntityUid uid, MapGridComponent _, ref (SharedBroadphaseSystem.BroadphaseCallback callback, Robust.Shared.GameObjects.EntityQuery<BroadphaseComponent> _broadphaseQuery) tuple) =>
    {
      BroadphaseComponent component2;
      if (tuple.Item2.TryComp(uid, out component2))
        tuple.Item1((Entity<BroadphaseComponent>) (uid, component2));
      return true;
    }), true, false);
  }

  internal void GetBroadphases<TState>(
    MapId mapId,
    Box2 aabb,
    ref TState state,
    SharedBroadphaseSystem.BroadphaseCallback<TState> callback)
  {
    (TState, SharedBroadphaseSystem.BroadphaseCallback<TState>, Robust.Shared.GameObjects.EntityQuery<BroadphaseComponent>) state1 = (state, callback, this._broadphaseQuery);
    EntityUid? uid1;
    if (!this._map.TryGetMap(new MapId?(mapId), out uid1))
      return;
    BroadphaseComponent component1;
    if (this._broadphaseQuery.TryGetComponent(uid1.Value, out component1))
      callback((Entity<BroadphaseComponent>) (uid1.Value, component1), ref state);
    this._mapManager.FindGridsIntersecting<(TState, SharedBroadphaseSystem.BroadphaseCallback<TState>, Robust.Shared.GameObjects.EntityQuery<BroadphaseComponent>)>(uid1.Value, aabb, ref state1, (GridCallback<(TState, SharedBroadphaseSystem.BroadphaseCallback<TState>, Robust.Shared.GameObjects.EntityQuery<BroadphaseComponent>)>) ((EntityUid uid, MapGridComponent _, ref (
    #nullable disable
    TState state, 
    #nullable enable
    SharedBroadphaseSystem.BroadphaseCallback<
    #nullable disable
    TState> callback, Robust.Shared.GameObjects.EntityQuery<
    #nullable enable
    BroadphaseComponent> _broadphaseQuery) tuple) =>
    {
      BroadphaseComponent component2;
      if (tuple.Item3.TryComp(uid, out component2))
        tuple.Item2((Entity<BroadphaseComponent>) (uid, component2), ref tuple.Item1);
      return true;
    }), true, false);
    state = state1.Item1;
  }

  internal delegate void BroadphaseCallback(Entity<BroadphaseComponent> entity);

  internal delegate void BroadphaseCallback<TState>(
    Entity<BroadphaseComponent> entity,
    ref TState state);

  private record struct BroadphaseContactJob : IParallelRobustJob, IParallelRangeRobustJob
  {
    public SharedBroadphaseSystem System;
    public SharedTransformSystem TransformSys;
    public IMapManager MapManager;
    public Robust.Shared.GameObjects.EntityQuery<TransformComponent> XformQuery;
    public readonly List<FixtureProxy> MoveBuffer;
    public List<(FixtureProxy, FixtureProxy, SharedBroadphaseSystem.PairFlag)> Pairs;
    public float FrameTime;

    public BroadphaseContactJob()
    {
      this.XformQuery = new Robust.Shared.GameObjects.EntityQuery<TransformComponent>();
      this.FrameTime = 0.0f;
      this.System = (SharedBroadphaseSystem) null;
      this.TransformSys = (SharedTransformSystem) null;
      this.MapManager = (IMapManager) null;
      this.MoveBuffer = new List<FixtureProxy>();
      this.Pairs = new List<(FixtureProxy, FixtureProxy, SharedBroadphaseSystem.PairFlag)>(64 /*0x40*/);
    }

    public int BatchSize => 16 /*0x10*/;

    public void Execute(int index)
    {
      FixtureProxy proxy = this.MoveBuffer[index];
      ref BroadphaseData? local1 = ref this.XformQuery.GetComponent(proxy.Entity).Broadphase;
      Box2 worldAABB = Matrix3Helpers.TransformBox(this.TransformSys.GetWorldMatrix((local1.HasValue ? new EntityUid?(local1.GetValueOrDefault().Uid) : new EntityUid?()).Value), ref proxy.AABB);
      EntityUid entityUid = this.XformQuery.GetComponent(proxy.Entity).MapUid ?? EntityUid.Invalid;
      float broadphaseExpand = this.System.GetBroadphaseExpand(proxy.Body, this.FrameTime);
      PhysicsComponent body = proxy.Body;
      (SharedBroadphaseSystem, FixtureProxy, Box2, List<(FixtureProxy, FixtureProxy, SharedBroadphaseSystem.PairFlag)>) state = (this.System, proxy, worldAABB, this.Pairs);
      this.MapManager.FindGridsIntersecting<(SharedBroadphaseSystem, FixtureProxy, Box2, List<(FixtureProxy, FixtureProxy, SharedBroadphaseSystem.PairFlag)>)>(entityUid, ((Box2) ref worldAABB).Enlarged(broadphaseExpand), ref state, (GridCallback<(SharedBroadphaseSystem, FixtureProxy, Box2, List<(FixtureProxy, FixtureProxy, SharedBroadphaseSystem.PairFlag)>)>) ((EntityUid uid, MapGridComponent _, ref (SharedBroadphaseSystem system, FixtureProxy proxy, Box2 worldAABB, List<(FixtureProxy, FixtureProxy, SharedBroadphaseSystem.PairFlag)> pairBuffer) tuple) =>
      {
        // ISSUE: explicit reference operation
        ref List<(FixtureProxy, FixtureProxy, SharedBroadphaseSystem.PairFlag)> local2 = @tuple.Item4;
        tuple.Item1.FindPairs(tuple.Item2, tuple.Item3, uid, local2);
        return true;
      }), true, false);
      this.System.FindPairs(proxy, worldAABB, entityUid, this.Pairs);
    }

    [CompilerGenerated]
    public override readonly int GetHashCode()
    {
      return (((((EqualityComparer<SharedBroadphaseSystem>.Default.GetHashCode(this.System) * -1521134295 + EqualityComparer<SharedTransformSystem>.Default.GetHashCode(this.TransformSys)) * -1521134295 + EqualityComparer<IMapManager>.Default.GetHashCode(this.MapManager)) * -1521134295 + EqualityComparer<Robust.Shared.GameObjects.EntityQuery<TransformComponent>>.Default.GetHashCode(this.XformQuery)) * -1521134295 + EqualityComparer<List<FixtureProxy>>.Default.GetHashCode(this.MoveBuffer)) * -1521134295 + EqualityComparer<List<(FixtureProxy, FixtureProxy, SharedBroadphaseSystem.PairFlag)>>.Default.GetHashCode(this.Pairs)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.FrameTime);
    }

    [CompilerGenerated]
    public readonly bool Equals(SharedBroadphaseSystem.BroadphaseContactJob other)
    {
      return EqualityComparer<SharedBroadphaseSystem>.Default.Equals(this.System, other.System) && EqualityComparer<SharedTransformSystem>.Default.Equals(this.TransformSys, other.TransformSys) && EqualityComparer<IMapManager>.Default.Equals(this.MapManager, other.MapManager) && EqualityComparer<Robust.Shared.GameObjects.EntityQuery<TransformComponent>>.Default.Equals(this.XformQuery, other.XformQuery) && EqualityComparer<List<FixtureProxy>>.Default.Equals(this.MoveBuffer, other.MoveBuffer) && EqualityComparer<List<(FixtureProxy, FixtureProxy, SharedBroadphaseSystem.PairFlag)>>.Default.Equals(this.Pairs, other.Pairs) && EqualityComparer<float>.Default.Equals(this.FrameTime, other.FrameTime);
    }
  }

  [Flags]
  private enum PairFlag : byte
  {
    None = 0,
    Wake = 1,
    Grid = 2,
  }
}
