// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.MapManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Shapes;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Map;

[Virtual]
internal class MapManager : IMapManagerInternal, IMapManager, IEntityEventSubscriber
{
  [Dependency]
  public readonly IGameTiming GameTiming;
  [Dependency]
  public readonly IEntityManager EntityManager;
  [Dependency]
  private readonly IManifoldManager _manifolds;
  [Dependency]
  private readonly ILogManager _logManager;
  [Dependency]
  private readonly IConsoleHost _conhost;
  private ISawmill _sawmill;
  private SharedMapSystem _mapSystem;
  private SharedPhysicsSystem _physics;
  private SharedTransformSystem _transformSystem;
  private EntityQuery<GridTreeComponent> _gridTreeQuery;
  private EntityQuery<MapGridComponent> _gridQuery;

  public void Initialize()
  {
    this._gridTreeQuery = this.EntityManager.GetEntityQuery<GridTreeComponent>();
    this._gridQuery = this.EntityManager.GetEntityQuery<MapGridComponent>();
    this.InitializeMapPausing();
    this._sawmill = this._logManager.GetSawmill("system.map");
  }

  public void Startup()
  {
    this._physics = this.EntityManager.System<SharedPhysicsSystem>();
    this._transformSystem = this.EntityManager.System<SharedTransformSystem>();
    this._mapSystem = this.EntityManager.System<SharedMapSystem>();
    this._sawmill.Debug("Starting...");
  }

  public void Shutdown()
  {
    this._sawmill.Debug("Stopping...");
    EntityQueryEnumerator<MapComponent> entityQueryEnumerator = this.EntityManager.EntityQueryEnumerator<MapComponent>();
    EntityUid uid;
    while (entityQueryEnumerator.MoveNext(out uid, out MapComponent _))
      this.EntityManager.DeleteEntity(new EntityUid?(uid));
  }

  public void Restart()
  {
    this._sawmill.Debug("Restarting...");
    EntityQueryEnumerator<MapComponent> entityQueryEnumerator = this.EntityManager.EntityQueryEnumerator<MapComponent>();
    EntityUid uid;
    while (entityQueryEnumerator.MoveNext(out uid, out MapComponent _))
      this.EntityManager.DeleteEntity(new EntityUid?(uid));
  }

  public MapGridComponent CreateGrid(MapId currentMapId, ushort chunkSize = 16 /*0x10*/)
  {
    return (MapGridComponent) this.CreateGrid(this.GetMapEntityIdOrThrow(currentMapId), chunkSize, new EntityUid());
  }

  public MapGridComponent CreateGrid(MapId currentMapId, in GridCreateOptions options)
  {
    return (MapGridComponent) this.CreateGrid(this.GetMapEntityIdOrThrow(currentMapId), options.ChunkSize, new EntityUid());
  }

  public MapGridComponent CreateGrid(MapId currentMapId)
  {
    return this.CreateGrid(currentMapId, in GridCreateOptions.Default);
  }

  public Entity<MapGridComponent> CreateGridEntity(MapId currentMapId, GridCreateOptions? options = null)
  {
    return this.CreateGridEntity(this.GetMapEntityIdOrThrow(currentMapId), options);
  }

  public Entity<MapGridComponent> CreateGridEntity(EntityUid map, GridCreateOptions? options = null)
  {
    options.GetValueOrDefault();
    if (!options.HasValue)
      options = new GridCreateOptions?(GridCreateOptions.Default);
    return this.CreateGrid(map, options.Value.ChunkSize, new EntityUid());
  }

  [Obsolete("Use HasComponent<MapGridComponent>(uid)")]
  public bool IsGrid(EntityUid uid) => this.EntityManager.HasComponent<MapGridComponent>(uid);

  public IEnumerable<MapGridComponent> GetAllMapGrids(MapId mapId)
  {
    AllEntityQueryEnumerator<MapGridComponent, TransformComponent> query = this.EntityManager.AllEntityQueryEnumerator<MapGridComponent, TransformComponent>();
    MapGridComponent comp1;
    TransformComponent comp2;
    while (query.MoveNext(out comp1, out comp2))
    {
      if (comp2.MapID == mapId)
        yield return comp1;
    }
  }

  public IEnumerable<Entity<MapGridComponent>> GetAllGrids(MapId mapId)
  {
    AllEntityQueryEnumerator<MapGridComponent, TransformComponent> query = this.EntityManager.AllEntityQueryEnumerator<MapGridComponent, TransformComponent>();
    EntityUid uid;
    MapGridComponent comp1;
    TransformComponent comp2;
    while (query.MoveNext(out uid, out comp1, out comp2))
    {
      if (!(comp2.MapID != mapId))
        yield return (Entity<MapGridComponent>) (uid, comp1);
    }
  }

  public virtual void DeleteGrid(EntityUid euid)
  {
    MetaDataComponent component;
    if (!this.EntityManager.TryGetComponent<MapGridComponent>(euid, out MapGridComponent _) || !this.EntityManager.TryGetComponent<MetaDataComponent>(euid, out component) || component.EntityLifeStage >= EntityLifeStage.Terminating)
      return;
    this.EntityManager.DeleteEntity(new EntityUid?(euid));
  }

  public bool SuppressOnTileChanged { get; set; }

  void IMapManagerInternal.RaiseOnTileChanged(
    Entity<MapGridComponent> entity,
    TileRef tileRef,
    Tile oldTile,
    Vector2i chunk)
  {
    if (this.SuppressOnTileChanged)
      return;
    TileChangedEvent args = new TileChangedEvent(entity, tileRef, oldTile, chunk);
    this.EntityManager.EventBus.RaiseLocalEvent<TileChangedEvent>(entity.Owner, ref args, true);
  }

  protected Entity<MapGridComponent> CreateGrid(
    EntityUid map,
    ushort chunkSize,
    EntityUid forcedGridEuid)
  {
    EntityUid entityUninitialized = this.EntityManager.CreateEntityUninitialized((string) null, forcedGridEuid);
    MapGridComponent mapGridComponent = this.EntityManager.AddComponent<MapGridComponent>(entityUninitialized);
    mapGridComponent.ChunkSize = chunkSize;
    this._sawmill.Debug($"Binding new grid {entityUninitialized}");
    this.EntityManager.GetComponent<TransformComponent>(entityUninitialized).AttachParent(map);
    MetaDataComponent component = this.EntityManager.GetComponent<MetaDataComponent>(entityUninitialized);
    this.EntityManager.System<MetaDataSystem>().SetEntityName(entityUninitialized, "grid", component);
    this.EntityManager.InitializeComponents(entityUninitialized, component);
    this.EntityManager.StartComponents(entityUninitialized);
    return (Entity<MapGridComponent>) (entityUninitialized, mapGridComponent);
  }

  public virtual void DeleteMap(MapId mapId) => this._mapSystem.DeleteMap(mapId);

  public MapId CreateMap(MapId? mapId = null)
  {
    if (mapId.HasValue)
    {
      this._mapSystem.CreateMap(mapId.Value);
      return mapId.Value;
    }
    MapId mapId1;
    this._mapSystem.CreateMap(out mapId1);
    return mapId1;
  }

  public bool MapExists([NotNullWhen(true)] MapId? mapId) => this._mapSystem.MapExists(mapId);

  public EntityUid GetMapEntityId(MapId mapId)
  {
    return this._mapSystem.GetMapOrInvalid(new MapId?(mapId));
  }

  public EntityUid GetMapEntityIdOrThrow(MapId mapId) => this._mapSystem.GetMap(mapId);

  public bool TryGetMap([NotNullWhen(true)] MapId? mapId, [NotNullWhen(true)] out EntityUid? uid)
  {
    return this._mapSystem.TryGetMap(mapId, out uid);
  }

  public IEnumerable<MapId> GetAllMapIds() => this._mapSystem.GetAllMapIds();

  public bool IsMap(EntityUid uid) => this.EntityManager.HasComponent<MapComponent>(uid);

  public void SetMapPaused(MapId mapId, bool paused) => this._mapSystem.SetPaused(mapId, paused);

  public void SetMapPaused(EntityUid uid, bool paused)
  {
    this._mapSystem.SetPaused((Entity<MapComponent>) uid, paused);
  }

  public void DoMapInitialize(MapId mapId) => this._mapSystem.InitializeMap(mapId);

  public bool IsMapInitialized(MapId mapId) => this._mapSystem.IsInitialized(mapId);

  public bool IsMapPaused(MapId mapId) => this._mapSystem.IsPaused(mapId);

  public bool IsMapPaused(EntityUid uid) => this._mapSystem.IsPaused((Entity<MapComponent>) uid);

  private void InitializeMapPausing()
  {
    this._conhost.RegisterCommand("pausemap", "Pauses a map, pausing all simulation processing on it.", "pausemap <map ID>", (ConCommandCallback) ((shell, _, args) =>
    {
      if (args.Length != 1)
      {
        shell.WriteError("Need to supply a valid MapId");
      }
      else
      {
        MapId mapId = new MapId(int.Parse(args[0], (IFormatProvider) CultureInfo.InvariantCulture));
        if (!this.MapExists(new MapId?(mapId)))
          shell.WriteError("That map does not exist.");
        else
          this.SetMapPaused(mapId, true);
      }
    }));
    this._conhost.RegisterCommand("querymappaused", "Check whether a map is paused or not.", "querymappaused <map ID>", (ConCommandCallback) ((shell, _, args) =>
    {
      MapId mapId = new MapId(int.Parse(args[0], (IFormatProvider) CultureInfo.InvariantCulture));
      if (!this.MapExists(new MapId?(mapId)))
        shell.WriteError("That map does not exist.");
      else
        shell.WriteLine(this._mapSystem.IsPaused(mapId).ToString());
    }));
    this._conhost.RegisterCommand("unpausemap", "unpauses a map, resuming all simulation processing on it.", "Usage: unpausemap <map ID>", (ConCommandCallback) ((shell, _, args) =>
    {
      if (args.Length != 1)
      {
        shell.WriteLine("Need to supply a valid MapId");
      }
      else
      {
        MapId mapId = new MapId(int.Parse(args[0], (IFormatProvider) CultureInfo.InvariantCulture));
        if (!this.MapExists(new MapId?(mapId)))
          shell.WriteLine("That map does not exist.");
        else
          this.SetMapPaused(mapId, false);
      }
    }));
  }

  private bool IsIntersecting<T>(
    ChunkEnumerator enumerator,
    T shape,
    Transform shapeTransform,
    Entity<FixturesComponent> grid)
    where T : IPhysShape
  {
    Transform xfB = this._physics.GetPhysicsTransform((EntityUid) grid);
    MapChunk chunk;
    while (enumerator.MoveNext(out chunk))
    {
      foreach (string fixture1 in chunk.Fixtures)
      {
        Fixture fixture2 = grid.Comp.Fixtures[fixture1];
        for (int indexB = 0; indexB < fixture2.Shape.ChildCount; ++indexB)
        {
          if (this._manifolds.TestOverlap<T, IPhysShape>(shape, 0, fixture2.Shape, indexB, in shapeTransform, in xfB))
            return true;
        }
      }
    }
    return false;
  }

  public void FindGridsIntersecting<T>(
    MapId mapId,
    T shape,
    Transform transform,
    ref List<Entity<MapGridComponent>> grids,
    bool approx = false,
    bool includeMap = true)
    where T : IPhysShape
  {
    EntityUid? uid;
    if (!this._mapSystem.TryGetMap(new MapId?(mapId), out uid))
      return;
    this.FindGridsIntersecting<T>(uid.Value, shape, transform, ref grids, approx, includeMap);
  }

  public void FindGridsIntersecting<T>(
    MapId mapId,
    T shape,
    Transform transform,
    GridCallback callback,
    bool approx = false,
    bool includeMap = true)
    where T : IPhysShape
  {
    EntityUid? uid;
    if (!this._mapSystem.TryGetMap(new MapId?(mapId), out uid))
      return;
    this.FindGridsIntersecting<T>(uid.Value, shape, transform, callback, includeMap, approx);
  }

  public void FindGridsIntersecting(
    MapId mapId,
    Box2 worldAABB,
    GridCallback callback,
    bool approx = false,
    bool includeMap = true)
  {
    EntityUid? uid;
    if (!this._mapSystem.TryGetMap(new MapId?(mapId), out uid))
      return;
    this.FindGridsIntersecting(uid.Value, worldAABB, callback, approx, includeMap);
  }

  public void FindGridsIntersecting<TState>(
    MapId mapId,
    Box2 worldAABB,
    ref TState state,
    GridCallback<TState> callback,
    bool approx = false,
    bool includeMap = true)
  {
    EntityUid? uid;
    if (!this._mapSystem.TryGetMap(new MapId?(mapId), out uid))
      return;
    this.FindGridsIntersecting<TState>(uid.Value, worldAABB, ref state, callback, approx, includeMap);
  }

  public void FindGridsIntersecting(
    MapId mapId,
    Box2 worldAABB,
    ref List<Entity<MapGridComponent>> grids,
    bool approx = false,
    bool includeMap = true)
  {
    EntityUid? uid;
    if (!this._mapSystem.TryGetMap(new MapId?(mapId), out uid))
      return;
    this.FindGridsIntersecting(uid.Value, worldAABB, ref grids, approx, includeMap);
  }

  public void FindGridsIntersecting(
    MapId mapId,
    Box2Rotated worldBounds,
    GridCallback callback,
    bool approx = false,
    bool includeMap = true)
  {
    EntityUid? uid;
    if (!this._mapSystem.TryGetMap(new MapId?(mapId), out uid))
      return;
    this.FindGridsIntersecting(uid.Value, worldBounds, callback, approx, includeMap);
  }

  public void FindGridsIntersecting<TState>(
    MapId mapId,
    Box2Rotated worldBounds,
    ref TState state,
    GridCallback<TState> callback,
    bool approx = false,
    bool includeMap = true)
  {
    EntityUid? uid;
    if (!this._mapSystem.TryGetMap(new MapId?(mapId), out uid))
      return;
    this.FindGridsIntersecting<TState>(uid.Value, worldBounds, ref state, callback, approx, includeMap);
  }

  public void FindGridsIntersecting(
    MapId mapId,
    Box2Rotated worldBounds,
    ref List<Entity<MapGridComponent>> grids,
    bool approx = false,
    bool includeMap = true)
  {
    EntityUid? uid;
    if (!this._mapSystem.TryGetMap(new MapId?(mapId), out uid))
      return;
    this.FindGridsIntersecting(uid.Value, worldBounds, ref grids, approx, includeMap);
  }

  public void FindGridsIntersecting<T>(
    EntityUid mapEnt,
    T shape,
    Transform transform,
    GridCallback callback,
    bool approx = false,
    bool includeMap = true)
    where T : IPhysShape
  {
    this.FindGridsIntersecting<T>(mapEnt, shape, shape.ComputeAABB(transform, 0), transform, callback, approx, includeMap);
  }

  private void FindGridsIntersecting<T>(
    EntityUid mapEnt,
    T shape,
    Box2 worldAABB,
    Transform transform,
    GridCallback callback,
    bool approx = false,
    bool includeMap = true)
    where T : IPhysShape
  {
    GridCallback state1 = callback;
    this.FindGridsIntersecting<T, GridCallback>(mapEnt, shape, worldAABB, transform, ref state1, (GridCallback<GridCallback>) ((EntityUid uid, MapGridComponent grid, ref GridCallback state) => state(uid, grid)), approx, includeMap);
  }

  public void FindGridsIntersecting<T, TState>(
    EntityUid mapEnt,
    T shape,
    Transform transform,
    ref TState state,
    GridCallback<TState> callback,
    bool approx = false,
    bool includeMap = true)
    where T : IPhysShape
  {
    this.FindGridsIntersecting<T, TState>(mapEnt, shape, shape.ComputeAABB(transform, 0), transform, ref state, callback, approx, includeMap);
  }

  private void FindGridsIntersecting<T, TState>(
    EntityUid mapEnt,
    T shape,
    Box2 worldAABB,
    Transform transform,
    ref TState state1,
    GridCallback<TState> callback,
    bool approx = false,
    bool includeMap = true)
    where T : IPhysShape
  {
    GridTreeComponent component1;
    if (!this._gridTreeQuery.TryGetComponent(mapEnt, out component1))
      return;
    MapGridComponent component2;
    if (includeMap && this._gridQuery.TryGetComponent(mapEnt, out component2))
    {
      int num1 = callback(mapEnt, component2, ref state1) ? 1 : 0;
    }
    MapManager.GridQueryState<T, TState> state3 = new MapManager.GridQueryState<T, TState>(callback, state1, worldAABB, shape, transform, component1.Tree, this._mapSystem, this, this._transformSystem, approx);
    component1.Tree.Query<MapManager.GridQueryState<T, TState>>(ref state3, (B2DynamicTree<(EntityUid, FixturesComponent, MapGridComponent)>.QueryCallback<MapManager.GridQueryState<T, TState>>) ((ref MapManager.GridQueryState<T, 
    #nullable disable
    TState> state2, DynamicTree.Proxy proxy) =>
    {
      (EntityUid Uid, FixturesComponent Fixtures, MapGridComponent Grid) userData = state2.Tree.GetUserData(proxy);
      Matrix3x2 invWorldMatrix = state2.TransformSystem.GetInvWorldMatrix(userData.Uid);
      Box2 worldAabb = state2.WorldAABB;
      ref Box2 local = ref worldAabb;
      Box2 localAABB = Matrix3Helpers.TransformBox(invWorldMatrix, ref local);
      ChunkEnumerator localMapChunks = state2.MapSystem.GetLocalMapChunks(userData.Uid, userData.Grid, localAABB);
      if (state2.Approximate)
      {
        if (!localMapChunks.MoveNext(out MapChunk _))
          return true;
      }
      else if (!state2.MapManager.IsIntersecting<T>(localMapChunks, state2.Shape, state2.Transform, (Entity<FixturesComponent>) (userData.Uid, userData.Fixtures)))
        return true;
      TState state4 = state2.State;
      int num2 = state2.Callback(userData.Uid, userData.Grid, ref state4) ? 1 : 0;
      state2.State = state4;
      return num2 != 0;
    }), in worldAABB);
    state1 = state3.State;
  }

  public void FindGridsIntersecting(
    EntityUid mapEnt,
    #nullable enable
    List<IPhysShape> shapes,
    Transform transform,
    ref List<Entity<MapGridComponent>> entities,
    bool approx = false,
    bool includeMap = true)
  {
    foreach (IPhysShape shape in shapes)
      this.FindGridsIntersecting<IPhysShape>(mapEnt, shape, shape.ComputeAABB(transform, 0), transform, ref entities);
  }

  public void FindGridsIntersecting<T>(
    EntityUid mapEnt,
    T shape,
    Transform transform,
    ref List<Entity<MapGridComponent>> grids,
    bool approx = false,
    bool includeMap = true)
    where T : IPhysShape
  {
    this.FindGridsIntersecting<T>(mapEnt, shape, shape.ComputeAABB(transform, 0), transform, ref grids, approx, includeMap);
  }

  public void FindGridsIntersecting<T>(
    EntityUid mapEnt,
    T shape,
    Box2 worldAABB,
    Transform transform,
    ref List<Entity<MapGridComponent>> grids,
    bool approx = false,
    bool includeMap = true)
    where T : IPhysShape
  {
    List<Entity<MapGridComponent>> state = grids;
    this.FindGridsIntersecting<T, List<Entity<MapGridComponent>>>(mapEnt, shape, worldAABB, transform, ref state, (GridCallback<List<Entity<MapGridComponent>>>) ((EntityUid uid, MapGridComponent grid, ref List<Entity<MapGridComponent>> list) =>
    {
      list.Add((Entity<MapGridComponent>) (uid, grid));
      return true;
    }), approx, includeMap);
  }

  public void FindGridsIntersecting(
    EntityUid mapEnt,
    Box2 worldAABB,
    GridCallback callback,
    bool approx = false,
    bool includeMap = true)
  {
    SlimPolygon shape = new SlimPolygon(worldAABB);
    this.FindGridsIntersecting<SlimPolygon>(mapEnt, shape, worldAABB, Transform.Empty, callback, approx, includeMap);
  }

  public void FindGridsIntersecting<TState>(
    EntityUid mapEnt,
    Box2 worldAABB,
    ref TState state,
    GridCallback<TState> callback,
    bool approx = false,
    bool includeMap = true)
  {
    SlimPolygon shape = new SlimPolygon(worldAABB);
    this.FindGridsIntersecting<SlimPolygon, TState>(mapEnt, shape, worldAABB, Transform.Empty, ref state, callback, approx, includeMap);
  }

  public void FindGridsIntersecting(
    EntityUid mapEnt,
    Box2 worldAABB,
    ref List<Entity<MapGridComponent>> grids,
    bool approx = false,
    bool includeMap = true)
  {
    SlimPolygon shape = new SlimPolygon(worldAABB);
    this.FindGridsIntersecting<SlimPolygon>(mapEnt, shape, worldAABB, Transform.Empty, ref grids, approx, includeMap);
  }

  public void FindGridsIntersecting(
    EntityUid mapEnt,
    Box2Rotated worldBounds,
    GridCallback callback,
    bool approx = false,
    bool includeMap = true)
  {
    SlimPolygon shape = new SlimPolygon(in worldBounds);
    this.FindGridsIntersecting<SlimPolygon>(mapEnt, shape, ((Box2Rotated) ref worldBounds).CalcBoundingBox(), Transform.Empty, callback, approx, includeMap);
  }

  public void FindGridsIntersecting<TState>(
    EntityUid mapEnt,
    Box2Rotated worldBounds,
    ref TState state,
    GridCallback<TState> callback,
    bool approx = false,
    bool includeMap = true)
  {
    SlimPolygon shape = new SlimPolygon(in worldBounds);
    this.FindGridsIntersecting<SlimPolygon, TState>(mapEnt, shape, ((Box2Rotated) ref worldBounds).CalcBoundingBox(), Transform.Empty, ref state, callback, approx, includeMap);
  }

  public void FindGridsIntersecting(
    EntityUid mapEnt,
    Box2Rotated worldBounds,
    ref List<Entity<MapGridComponent>> grids,
    bool approx = false,
    bool includeMap = true)
  {
    SlimPolygon shape = new SlimPolygon(in worldBounds);
    this.FindGridsIntersecting<SlimPolygon>(mapEnt, shape, ((Box2Rotated) ref worldBounds).CalcBoundingBox(), Transform.Empty, ref grids, approx, includeMap);
  }

  public bool TryFindGridAt(
    EntityUid mapEnt,
    Vector2 worldPos,
    out EntityUid uid,
    [NotNullWhen(true)] out MapGridComponent? grid)
  {
    Vector2 vector2 = new Vector2(0.2f, 0.2f);
    Box2 worldAABB;
    // ISSUE: explicit constructor call
    ((Box2) ref worldAABB).\u002Ector(worldPos - vector2, worldPos + vector2);
    uid = EntityUid.Invalid;
    grid = (MapGridComponent) null;
    (EntityUid, MapGridComponent, Vector2, SharedMapSystem, SharedTransformSystem) state = (uid, grid, worldPos, this._mapSystem, this._transformSystem);
    this.FindGridsIntersecting<(EntityUid, MapGridComponent, Vector2, SharedMapSystem, SharedTransformSystem)>(mapEnt, worldAABB, ref state, (GridCallback<(EntityUid, MapGridComponent, Vector2, SharedMapSystem, SharedTransformSystem)>) ((EntityUid iUid, MapGridComponent iGrid, ref (EntityUid uid, MapGridComponent? grid, Vector2 worldPos, SharedMapSystem mapSystem, SharedTransformSystem xformSystem) tuple) =>
    {
      Matrix3x2 invWorldMatrix = tuple.Item5.GetInvWorldMatrix(iUid);
      Vector2 tile = Vector2.Transform(tuple.Item3, invWorldMatrix);
      Vector2i chunkIndices = SharedMapSystem.GetChunkIndices(tile, (int) iGrid.ChunkSize);
      MapChunk mapChunk;
      if (!iGrid.Chunks.TryGetValue(chunkIndices, out mapChunk))
        return true;
      Vector2i chunkRelative = SharedMapSystem.GetChunkRelative(tile, (int) iGrid.ChunkSize);
      if (mapChunk.GetTile(chunkRelative).IsEmpty)
        return true;
      tuple.Item1 = iUid;
      tuple.Item2 = iGrid;
      return false;
    }), true, false);
    MapGridComponent component;
    if (state.Item2 == null && this._gridQuery.TryGetComponent(mapEnt, out component))
    {
      uid = mapEnt;
      grid = component;
      return true;
    }
    uid = state.Item1;
    grid = state.Item2;
    return grid != null;
  }

  public bool TryFindGridAt(
    MapId mapId,
    Vector2 worldPos,
    out EntityUid uid,
    [NotNullWhen(true)] out MapGridComponent? grid)
  {
    EntityUid? uid1;
    if (this._mapSystem.TryGetMap(new MapId?(mapId), out uid1))
      return this.TryFindGridAt(uid1.Value, worldPos, out uid, out grid);
    uid = new EntityUid();
    grid = (MapGridComponent) null;
    return false;
  }

  public bool TryFindGridAt(
    MapCoordinates mapCoordinates,
    out EntityUid uid,
    [NotNullWhen(true)] out MapGridComponent? grid)
  {
    return this.TryFindGridAt(mapCoordinates.MapId, mapCoordinates.Position, out uid, out grid);
  }

  MapGridComponent IMapManager.CreateGrid(MapId currentMapId, in GridCreateOptions options)
  {
    return this.CreateGrid(currentMapId, in options);
  }

  private record struct GridQueryState<T, TState>(
    GridCallback<TState> Callback,
    TState State,
    Box2 WorldAABB,
    T Shape,
    Transform Transform,
    B2DynamicTree<(EntityUid Uid, FixturesComponent Fixtures, MapGridComponent Grid)> Tree,
    SharedMapSystem MapSystem,
    MapManager MapManager,
    SharedTransformSystem TransformSystem,
    bool Approximate)
  ;
}
