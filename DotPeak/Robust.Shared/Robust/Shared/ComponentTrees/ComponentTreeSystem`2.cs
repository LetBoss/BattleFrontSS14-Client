// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ComponentTrees.ComponentTreeSystem`2
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Robust.Shared.ComponentTrees;

public abstract class ComponentTreeSystem<TTreeComp, TComp> : EntitySystem
  where TTreeComp : Component, IComponentTreeComponent<TComp>, new()
  where TComp : Component, IComponentTreeEntry<TComp>
{
  [Dependency]
  private readonly RecursiveMoveSystem _recursiveMoveSys;
  [Dependency]
  protected readonly SharedTransformSystem XformSystem;
  [Dependency]
  private readonly IMapManager _mapManager;
  [Dependency]
  private readonly SharedMapSystem _mapSystem;
  private readonly Queue<ComponentTreeEntry<TComp>> _updateQueue = new Queue<ComponentTreeEntry<TComp>>();
  protected Robust.Shared.GameObjects.EntityQuery<TComp> Query;
  private bool _initialized;

  protected virtual bool Enabled => true;

  protected abstract bool DoFrameUpdate { get; }

  protected abstract bool DoTickUpdate { get; }

  protected virtual int InitialCapacity { get; } = 256 /*0x0100*/;

  protected abstract bool Recursive { get; }

  public override void Initialize()
  {
    base.Initialize();
    if (!this.Enabled)
      return;
    this._initialized = true;
    this.UpdatesOutsidePrediction = this.DoTickUpdate;
    this.UpdatesAfter.Add(typeof (SharedTransformSystem));
    this.UpdatesAfter.Add(typeof (SharedPhysicsSystem));
    this.SubscribeLocalEvent<MapCreatedEvent>(new EntityEventHandler<MapCreatedEvent>(this.MapManagerOnMapCreated));
    this.SubscribeLocalEvent<GridInitializeEvent>(new EntityEventHandler<GridInitializeEvent>(this.MapManagerOnGridCreated));
    this.SubscribeLocalEvent<TComp, ComponentStartup>(new ComponentEventHandler<TComp, ComponentStartup>(this.OnCompStartup));
    this.SubscribeLocalEvent<TComp, ComponentRemove>(new ComponentEventHandler<TComp, ComponentRemove>(this.OnCompRemoved));
    if (this.Recursive)
    {
      this._recursiveMoveSys.OnTreeRecursiveMove += new RecursiveMoveSystem.TreeRecursiveMoveEventHandler(this.HandleRecursiveMove);
      this._recursiveMoveSys.AddSubscription();
    }
    else
      this.SubscribeLocalEvent<TComp, MoveEvent>(new ComponentEventRefHandler<TComp, MoveEvent>(this.HandleMove));
    this.SubscribeLocalEvent<TTreeComp, EntityTerminatingEvent>(new ComponentEventRefHandler<TTreeComp, EntityTerminatingEvent>(this.OnTerminating));
    this.SubscribeLocalEvent<TTreeComp, ComponentAdd>(new ComponentEventHandler<TTreeComp, ComponentAdd>(this.OnTreeAdd));
    this.SubscribeLocalEvent<TTreeComp, ComponentRemove>(new ComponentEventHandler<TTreeComp, ComponentRemove>(this.OnTreeRemove));
    this.Query = this.GetEntityQuery<TComp>();
  }

  public override void Shutdown()
  {
    if (!this._initialized)
      return;
    this._initialized = false;
    if (!this.Recursive)
      return;
    this._recursiveMoveSys.OnTreeRecursiveMove -= new RecursiveMoveSystem.TreeRecursiveMoveEventHandler(this.HandleRecursiveMove);
  }

  private bool CheckEnabled()
  {
    if (this._initialized)
      return true;
    this.Log.Error("Attempted to use disabled lookup tree");
    return false;
  }

  private void HandleRecursiveMove(EntityUid uid, TransformComponent xform)
  {
    TComp component;
    if (!this.Query.TryGetComponent(uid, out component))
      return;
    this.QueueTreeUpdate(uid, component, xform);
  }

  private void HandleMove(EntityUid uid, TComp component, ref MoveEvent args)
  {
    this.QueueTreeUpdate(uid, component, args.Component);
  }

  public void QueueTreeUpdate(EntityUid uid, TComp component, TransformComponent? xform = null)
  {
    if (!this._initialized || component.TreeUpdateQueued || !this.Resolve(uid, ref xform))
      return;
    component.TreeUpdateQueued = true;
    this._updateQueue.Enqueue((ComponentTreeEntry<TComp>) (component, xform));
  }

  public void QueueTreeUpdate(Entity<TComp> entity, TransformComponent? xform = null)
  {
    this.QueueTreeUpdate(entity.Owner, entity.Comp, xform);
  }

  protected virtual void OnCompStartup(EntityUid uid, TComp component, ComponentStartup args)
  {
    this.QueueTreeUpdate(uid, component);
  }

  protected virtual void OnCompRemoved(EntityUid uid, TComp component, ComponentRemove args)
  {
    this.RemoveFromTree(component);
  }

  protected virtual void OnTreeAdd(EntityUid uid, TTreeComp component, ComponentAdd args)
  {
    component.Tree = new DynamicTree<ComponentTreeEntry<TComp>>(new DynamicTree<ComponentTreeEntry<TComp>>.ExtractAabbDelegate(this.ExtractAabb), capacity: this.InitialCapacity);
  }

  protected virtual void OnTreeRemove(EntityUid uid, TTreeComp component, ComponentRemove args)
  {
    foreach (ComponentTreeEntry<TComp> componentTreeEntry in component.Tree)
    {
      componentTreeEntry.Component.TreeUid = new EntityUid?();
      componentTreeEntry.Component.Tree = (DynamicTree<ComponentTreeEntry<TComp>>) null;
    }
    component.Tree.Clear();
  }

  protected virtual void OnTerminating(
    EntityUid uid,
    TTreeComp component,
    ref EntityTerminatingEvent args)
  {
    this.RemComp(uid, (IComponent) component);
  }

  private void MapManagerOnMapCreated(MapCreatedEvent e) => this.EnsureComp<TTreeComp>(e.Uid);

  private void MapManagerOnGridCreated(GridInitializeEvent ev)
  {
    this.EnsureComp<TTreeComp>(ev.EntityUid);
  }

  public override void Update(float frameTime)
  {
    if (!this.DoTickUpdate || !this._initialized)
      return;
    this.UpdateTreePositions();
  }

  public override void FrameUpdate(float frameTime)
  {
    if (!this.DoFrameUpdate || !this._initialized)
      return;
    this.UpdateTreePositions();
  }

  public void UpdateTreePositions()
  {
    try
    {
      if (!this.CheckEnabled() || this._updateQueue.Count == 0)
        return;
      Robust.Shared.GameObjects.EntityQuery<TTreeComp> entityQuery = this.GetEntityQuery<TTreeComp>();
      ComponentTreeEntry<TComp> entry;
      while (this._updateQueue.TryDequeue(out entry))
      {
        (TComp component1, TransformComponent xform) = entry;
        component1.TreeUpdateQueued = false;
        if (component1.Running)
        {
          if (component1.AddToTree && !component1.Deleted)
          {
            EntityUid? nullable1 = xform.MapUid;
            if (nullable1.HasValue)
            {
              nullable1 = xform.GridUid;
              EntityUid? uid = nullable1 ?? xform.MapUid;
              TTreeComp component2;
              if (!entityQuery.TryGetComponent(uid, out component2))
              {
                nullable1 = component1.TreeUid;
                if (!nullable1.HasValue)
                  continue;
              }
              nullable1 = component1.TreeUid;
              EntityUid? nullable2 = uid;
              if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
              {
                (Vector2 vector2, Angle angle) = this.XformSystem.GetRelativePositionRotation(entry.Transform, uid.Value);
                component2.Tree.Update(in entry, new Box2?(this.ExtractAabb(in entry, vector2, angle)));
                continue;
              }
              this.RemoveFromTree(component1);
              if ((object) component2 == null)
                break;
              component1.TreeUid = uid;
              component1.Tree = component2.Tree;
              (Vector2 vector2_1, Angle angle1) = this.XformSystem.GetRelativePositionRotation(entry.Transform, uid.Value);
              component2.Tree.Add(in entry, new Box2?(this.ExtractAabb(in entry, vector2_1, angle1)));
              continue;
            }
          }
          this.RemoveFromTree(component1);
        }
      }
    }
    finally
    {
      this._updateQueue.Clear();
    }
  }

  private void RemoveFromTree(TComp component)
  {
    DynamicTree<ComponentTreeEntry<TComp>> tree = component.Tree;
    if (tree != null)
      tree.Remove(new ComponentTreeEntry<TComp>()
      {
        Component = component
      });
    component.Tree = (DynamicTree<ComponentTreeEntry<TComp>>) null;
    component.TreeUid = new EntityUid?();
  }

  protected virtual Box2 ExtractAabb(in ComponentTreeEntry<TComp> entry)
  {
    if (!entry.Component.TreeUid.HasValue)
      return new Box2();
    (Vector2 vector2, Angle angle) = this.XformSystem.GetRelativePositionRotation(entry.Transform, entry.Component.TreeUid.Value);
    return this.ExtractAabb(in entry, vector2, angle);
  }

  protected abstract Box2 ExtractAabb(in ComponentTreeEntry<TComp> entry, Vector2 pos, Angle rot);

  public IEnumerable<(EntityUid, TTreeComp)> GetIntersectingTrees(
    MapId mapId,
    Box2Rotated worldBounds)
  {
    return this.GetIntersectingTrees(mapId, ((Box2Rotated) ref worldBounds).CalcBoundingBox());
  }

  public IEnumerable<(EntityUid Uid, TTreeComp Comp)> GetIntersectingTrees(
    MapId mapId,
    Box2 worldAABB)
  {
    if (!this.CheckEnabled())
      return (IEnumerable<(EntityUid, TTreeComp)>) Array.Empty<(EntityUid, TTreeComp)>();
    this.UpdateTreePositions();
    ValueList<(EntityUid, TTreeComp)> intersectingTrees = new ValueList<(EntityUid, TTreeComp)>();
    if (mapId == MapId.Nullspace)
      return (IEnumerable<(EntityUid, TTreeComp)>) intersectingTrees;
    (EntityManager, ValueList<(EntityUid, TTreeComp)>) state = (this.EntityManager, intersectingTrees);
    this._mapManager.FindGridsIntersecting<(EntityManager, ValueList<(EntityUid, TTreeComp)>)>(mapId, worldAABB, ref state, (GridCallback<(EntityManager, ValueList<(EntityUid, TTreeComp)>)>) ((EntityUid uid, MapGridComponent grid, ref (EntityManager EntityManager, ValueList<(EntityUid, TTreeComp)> trees) tuple) =>
    {
      TTreeComp component;
      if (tuple.Item1.TryGetComponent<TTreeComp>(uid, out component))
        tuple.Item2.Add((uid, component));
      return true;
    }), includeMap: false);
    EntityUid? uid1;
    TTreeComp comp;
    if (this._mapSystem.TryGetMap(new MapId?(mapId), out uid1) && this.TryComp<TTreeComp>(uid1, out comp))
      state.Item2.Add((uid1.Value, comp));
    return (IEnumerable<(EntityUid, TTreeComp)>) state.Item2;
  }

  public HashSet<ComponentTreeEntry<TComp>> QueryAabb(MapId mapId, Box2 worldBounds, bool approx = true)
  {
    return this.QueryAabb(mapId, new Box2Rotated(worldBounds, new Angle(), new Vector2()), approx);
  }

  public void QueryAabb(
    HashSet<Entity<TComp, TransformComponent>> results,
    MapId mapId,
    Box2 worldBounds,
    bool approx = true)
  {
    this.QueryAabb(results, mapId, new Box2Rotated(worldBounds, new Angle(), new Vector2()), approx);
  }

  public HashSet<ComponentTreeEntry<TComp>> QueryAabb(
    MapId mapId,
    Box2Rotated worldBounds,
    bool approx = true)
  {
    HashSet<ComponentTreeEntry<TComp>> results = new HashSet<ComponentTreeEntry<TComp>>();
    this.QueryAabb(results, mapId, worldBounds, approx);
    return results;
  }

  [Obsolete("Use Entity<T> variant")]
  internal void QueryAabb(
    HashSet<ComponentTreeEntry<TComp>> results,
    MapId mapId,
    Box2Rotated worldBounds,
    bool approx = true)
  {
    if (!this.CheckEnabled())
      return;
    foreach ((EntityUid uid, TTreeComp treeComp) in this.GetIntersectingTrees(mapId, worldBounds))
    {
      Box2 aabb = Matrix3Helpers.TransformBox(this.XformSystem.GetInvWorldMatrix(uid), ref worldBounds);
      treeComp.Tree.QueryAabb<HashSet<ComponentTreeEntry<TComp>>>(ref results, (DynamicTree<ComponentTreeEntry<TComp>>.QueryCallbackDelegate<HashSet<ComponentTreeEntry<TComp>>>) ((ref HashSet<ComponentTreeEntry<TComp>> state, in ComponentTreeEntry<TComp> value) =>
      {
        state.Add(value);
        return true;
      }), aabb, approx);
    }
  }

  public void QueryAabb(
    HashSet<Entity<TComp, TransformComponent>> results,
    MapId mapId,
    Box2Rotated worldBounds,
    bool approx = true)
  {
    if (!this.CheckEnabled())
      return;
    foreach ((EntityUid uid, TTreeComp treeComp) in this.GetIntersectingTrees(mapId, worldBounds))
    {
      Box2 aabb = Matrix3Helpers.TransformBox(this.XformSystem.GetInvWorldMatrix(uid), ref worldBounds);
      treeComp.Tree.QueryAabb<HashSet<Entity<TComp, TransformComponent>>>(ref results, (DynamicTree<ComponentTreeEntry<TComp>>.QueryCallbackDelegate<HashSet<Entity<TComp, TransformComponent>>>) ((ref HashSet<Entity<TComp, TransformComponent>> state, in ComponentTreeEntry<TComp> value) =>
      {
        state.Add((Entity<TComp, TransformComponent>) value);
        return true;
      }), aabb, approx);
    }
  }

  public void QueryAabb(
    List<Entity<TComp, TransformComponent>> results,
    MapId mapId,
    Box2 worldBounds,
    bool approx = true)
  {
    this.QueryAabb(results, mapId, new Box2Rotated(worldBounds, new Angle(), new Vector2()), approx);
  }

  public void QueryAabb(
    List<Entity<TComp, TransformComponent>> results,
    MapId mapId,
    Box2Rotated worldBounds,
    bool approx = true)
  {
    if (!this.CheckEnabled())
      return;
    foreach ((EntityUid uid, TTreeComp treeComp) in this.GetIntersectingTrees(mapId, worldBounds))
    {
      Box2 aabb = Matrix3Helpers.TransformBox(this.XformSystem.GetInvWorldMatrix(uid), ref worldBounds);
      treeComp.Tree.QueryAabb<List<Entity<TComp, TransformComponent>>>(ref results, (DynamicTree<ComponentTreeEntry<TComp>>.QueryCallbackDelegate<List<Entity<TComp, TransformComponent>>>) ((ref List<Entity<TComp, TransformComponent>> state, in ComponentTreeEntry<TComp> value) =>
      {
        state.Add((Entity<TComp, TransformComponent>) value);
        return true;
      }), aabb, approx);
    }
  }

  public void QueryAabb<TState>(
    ref TState state,
    DynamicTree<ComponentTreeEntry<TComp>>.QueryCallbackDelegate<TState> callback,
    MapId mapId,
    Box2 worldBounds,
    bool approx = true)
  {
    this.QueryAabb<TState>(ref state, callback, mapId, new Box2Rotated(worldBounds, new Angle(), new Vector2()), approx);
  }

  public void QueryAabb<TState>(
    ref TState state,
    DynamicTree<ComponentTreeEntry<TComp>>.QueryCallbackDelegate<TState> callback,
    MapId mapId,
    Box2Rotated worldBounds,
    bool approx = true)
  {
    if (!this.CheckEnabled())
      return;
    foreach ((EntityUid uid, TTreeComp treeComp) in this.GetIntersectingTrees(mapId, worldBounds))
    {
      Box2 aabb = Matrix3Helpers.TransformBox(this.XformSystem.GetInvWorldMatrix(uid), ref worldBounds);
      treeComp.Tree.QueryAabb<TState>(ref state, callback, aabb, approx);
    }
  }

  [Obsolete("use IntersectRay")]
  public List<RayCastResults> IntersectRayWithPredicate<TState>(
    MapId mapId,
    in Ray ray,
    float maxLength,
    TState state,
    Func<EntityUid, TState, bool> predicate,
    bool returnOnFirstHit = true)
  {
    List<RayCastResults> results = new List<RayCastResults>();
    if (!returnOnFirstHit)
    {
      this.IntersectRay<TState>(results, mapId, in ray, maxLength, state, (Func<Entity<TComp, TransformComponent>, TState, bool>) ((e, s) => predicate(e.Owner, s)));
      return results;
    }
    RayCastResults? nullable = this.IntersectRay<TState>(mapId, in ray, maxLength, state, (Func<Entity<TComp, TransformComponent>, TState, bool>) ((e, s) => predicate(e.Owner, s)));
    if (nullable.HasValue)
      results.Add(nullable.Value);
    return results;
  }

  public RayCastResults? IntersectRay(MapId mapId, in Ray ray, float length)
  {
    ComponentTreeSystem<TTreeComp, TComp>.QueryState state = new ComponentTreeSystem<TTreeComp, TComp>.QueryState(length);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.IntersectRayInternal<ComponentTreeSystem<TTreeComp, TComp>.QueryState>(mapId, in ray, length, ref state, ComponentTreeSystem<TTreeComp, TComp>.\u003C\u003EO.\u003C0\u003E__QueryCallback ?? (ComponentTreeSystem<TTreeComp, TComp>.\u003C\u003EO.\u003C0\u003E__QueryCallback = new DynamicTree<ComponentTreeEntry<TComp>>.RayQueryCallbackDelegate<ComponentTreeSystem<TTreeComp, TComp>.QueryState>(ComponentTreeSystem<TTreeComp, TComp>.QueryCallback)));
    return state.Result;
  }

  public void IntersectRay(List<RayCastResults> results, MapId mapId, in Ray ray, float maxLength)
  {
    results.Clear();
    ComponentTreeSystem<TTreeComp, TComp>.QueryState state = new ComponentTreeSystem<TTreeComp, TComp>.QueryState(maxLength, results);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.IntersectRayInternal<ComponentTreeSystem<TTreeComp, TComp>.QueryState>(mapId, in ray, maxLength, ref state, ComponentTreeSystem<TTreeComp, TComp>.\u003C\u003EO.\u003C0\u003E__QueryCallback ?? (ComponentTreeSystem<TTreeComp, TComp>.\u003C\u003EO.\u003C0\u003E__QueryCallback = new DynamicTree<ComponentTreeEntry<TComp>>.RayQueryCallbackDelegate<ComponentTreeSystem<TTreeComp, TComp>.QueryState>(ComponentTreeSystem<TTreeComp, TComp>.QueryCallback)));
  }

  public RayCastResults? IntersectRay<TState>(
    MapId mapId,
    in Ray ray,
    float length,
    TState predicateState,
    Func<Entity<TComp, TransformComponent>, TState, bool> ignore)
  {
    ComponentTreeSystem<TTreeComp, TComp>.QueryState<TState> state = new ComponentTreeSystem<TTreeComp, TComp>.QueryState<TState>(new ComponentTreeSystem<TTreeComp, TComp>.QueryState(length), predicateState, ignore);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.IntersectRayInternal<ComponentTreeSystem<TTreeComp, TComp>.QueryState<TState>>(mapId, in ray, length, ref state, ComponentTreeSystem<TTreeComp, TComp>.\u003CIntersectRay\u003EO__52_0<TState>.\u003C0\u003E__PredicateQueryCallback ?? (ComponentTreeSystem<TTreeComp, TComp>.\u003CIntersectRay\u003EO__52_0<TState>.\u003C0\u003E__PredicateQueryCallback = new DynamicTree<ComponentTreeEntry<TComp>>.RayQueryCallbackDelegate<ComponentTreeSystem<TTreeComp, TComp>.QueryState<TState>>(ComponentTreeSystem<TTreeComp, TComp>.PredicateQueryCallback<TState>)));
    return state.Inner.Result;
  }

  public void IntersectRay<TState>(
    List<RayCastResults> results,
    MapId mapId,
    in Ray ray,
    float length,
    TState predicateState,
    Func<Entity<TComp, TransformComponent>, TState, bool> ignore)
  {
    ComponentTreeSystem<TTreeComp, TComp>.QueryState<TState> state = new ComponentTreeSystem<TTreeComp, TComp>.QueryState<TState>(new ComponentTreeSystem<TTreeComp, TComp>.QueryState(length, results), predicateState, ignore);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.IntersectRayInternal<ComponentTreeSystem<TTreeComp, TComp>.QueryState<TState>>(mapId, in ray, length, ref state, ComponentTreeSystem<TTreeComp, TComp>.\u003CIntersectRay\u003EO__53_0<TState>.\u003C0\u003E__PredicateQueryCallback ?? (ComponentTreeSystem<TTreeComp, TComp>.\u003CIntersectRay\u003EO__53_0<TState>.\u003C0\u003E__PredicateQueryCallback = new DynamicTree<ComponentTreeEntry<TComp>>.RayQueryCallbackDelegate<ComponentTreeSystem<TTreeComp, TComp>.QueryState<TState>>(ComponentTreeSystem<TTreeComp, TComp>.PredicateQueryCallback<TState>)));
  }

  private void IntersectRayInternal<TState>(
    MapId mapId,
    in Ray ray,
    float maxLength,
    ref TState state,
    DynamicTree<ComponentTreeEntry<TComp>>.RayQueryCallbackDelegate<TState> callback)
    where TState : ComponentTreeSystem<
    #nullable disable
    TTreeComp, TComp>.IDone
  {
    if (mapId == MapId.Nullspace || !this.CheckEnabled())
      return;
    Vector2 vector2 = ray.Position + ray.Direction * maxLength;
    Box2 worldAABB;
    // ISSUE: explicit constructor call
    ((Box2) ref worldAABB).\u002Ector(Vector2.Min(ray.Position, vector2), Vector2.Max(ray.Position, vector2));
    foreach ((EntityUid entityUid, TTreeComp Comp) in this.GetIntersectingTrees(mapId, worldAABB))
    {
      (Vector2 _, Angle WorldRotation, Matrix3x2 matrix3x2) = this.XformSystem.GetWorldPositionRotationInvMatrix(entityUid);
      Angle angle = new Angle(-WorldRotation.Theta);
      Vector2 direction = ((Angle) ref angle).RotateVec(ref ray.Direction);
      Ray ray1 = new Ray(Vector2.Transform(ray.Position, matrix3x2), direction);
      Comp.Tree.QueryRay<TState>(ref state, callback, in ray1);
      if (state.Done)
        break;
    }
  }

  private static bool QueryCallback(
    ref ComponentTreeSystem<TTreeComp, TComp>.QueryState state,
    in ComponentTreeEntry<
    #nullable enable
    TComp> value,
    in Vector2 point,
    float dist)
  {
    if ((double) dist > (double) state.MaxLength)
      return true;
    if (state.ReturnOnFirstHit)
    {
      state.Result = new RayCastResults?(new RayCastResults(dist, point, value.Uid));
      return false;
    }
    state.List.Add(new RayCastResults(dist, point, value.Uid));
    return true;
  }

  private static bool PredicateQueryCallback<TState>(
    ref ComponentTreeSystem<
    #nullable disable
    TTreeComp, TComp>.QueryState<
    #nullable enable
    TState> state,
    in ComponentTreeEntry<TComp> value,
    in Vector2 point,
    float dist)
  {
    if ((double) dist > (double) state.Inner.MaxLength || state.Ignore((Entity<TComp, TransformComponent>) value, state.PredicateState))
      return true;
    if (state.Inner.ReturnOnFirstHit)
    {
      state.Inner.Result = new RayCastResults?(new RayCastResults(dist, point, value.Uid));
      return false;
    }
    state.Inner.List.Add(new RayCastResults(dist, point, value.Uid));
    return true;
  }

  private struct QueryState<TPredicateState>(
    ComponentTreeSystem<
    #nullable disable
    TTreeComp, TComp>.QueryState inner,
    #nullable enable
    TPredicateState predicateState,
    Func<Entity<TComp, TransformComponent>, TPredicateState, bool> ignore) : 
    ComponentTreeSystem<
    #nullable disable
    TTreeComp, TComp>.IDone
  {
    public readonly 
    #nullable enable
    TPredicateState PredicateState = predicateState;
    public readonly Func<Entity<TComp, TransformComponent>, TPredicateState, bool> Ignore = ignore;
    public ComponentTreeSystem<
    #nullable disable
    TTreeComp, TComp>.QueryState Inner = inner;

    public bool Done => this.Inner.Done;
  }

  private struct QueryState(float maxLength, 
  #nullable enable
  List<RayCastResults>? list = null) : ComponentTreeSystem<
  #nullable disable
  TTreeComp, TComp>.IDone
  {
    public readonly float MaxLength = maxLength;
    public readonly 
    #nullable enable
    List<RayCastResults>? List = list;
    public RayCastResults? Result = new RayCastResults?();

    [MemberNotNullWhen(false, "List")]
    public readonly bool ReturnOnFirstHit
    {
      [MemberNotNullWhen(false, "List")] get => this.List == null;
    }

    public bool Done => this.Result.HasValue;
  }

  private interface IDone
  {
    bool Done { get; }
  }
}
