// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.SharedMapSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Collections;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Map.Events;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.GameObjects;

public abstract class SharedMapSystem : EntitySystem
{
  [Robust.Shared.IoC.Dependency]
  private readonly ITileDefinitionManager _tileMan;
  [Robust.Shared.IoC.Dependency]
  private readonly IGameTiming _timing;
  [Robust.Shared.IoC.Dependency]
  protected readonly IMapManager MapManager;
  [Robust.Shared.IoC.Dependency]
  private readonly IMapManagerInternal _mapInternal;
  [Robust.Shared.IoC.Dependency]
  private readonly INetManager _netManager;
  [Robust.Shared.IoC.Dependency]
  private readonly FixtureSystem _fixtures;
  [Robust.Shared.IoC.Dependency]
  private readonly SharedPhysicsSystem _physics;
  [Robust.Shared.IoC.Dependency]
  private readonly SharedTransformSystem _transform;
  [Robust.Shared.IoC.Dependency]
  private readonly MetaDataSystem _meta;
  private Robust.Shared.GameObjects.EntityQuery<FixturesComponent> _fixturesQuery;
  private Robust.Shared.GameObjects.EntityQuery<MapComponent> _mapQuery;
  private Robust.Shared.GameObjects.EntityQuery<MapGridComponent> _gridQuery;
  private Robust.Shared.GameObjects.EntityQuery<MetaDataComponent> _metaQuery;
  private Robust.Shared.GameObjects.EntityQuery<TransformComponent> _xformQuery;
  protected HashSet<MapId> UsedIds = new HashSet<MapId>();
  protected int LastMapId;
  private Dictionary<EntityUid, MapId> _reserved = new Dictionary<EntityUid, MapId>();

  public EntityCoordinates AlignToGrid(EntityCoordinates coordinates)
  {
    MapGridComponent mapGridComponent;
    if (this._gridQuery.TryGetComponent(coordinates.EntityId, out mapGridComponent))
    {
      Vector2i tile = this.CoordinatesToTile(coordinates.EntityId, mapGridComponent, coordinates);
      return this.ToCenterCoordinates(coordinates.EntityId, tile, mapGridComponent);
    }
    EntityUid uid;
    if (!this._mapInternal.TryFindGridAt(this._transform.ToMapCoordinates(coordinates), out uid, out mapGridComponent))
      return coordinates;
    Vector2i tile1 = this.CoordinatesToTile(uid, mapGridComponent, coordinates);
    return this.ToCenterCoordinates(uid, tile1, mapGridComponent);
  }

  public EntityCoordinates ToCoordinates(TileRef tileRef, MapGridComponent? gridComponent = null)
  {
    return this.ToCoordinates(tileRef.GridUid, tileRef.GridIndices, gridComponent);
  }

  public EntityCoordinates ToCoordinates(
    EntityUid gridUid,
    Vector2i tile,
    MapGridComponent? gridComponent = null)
  {
    return !this._gridQuery.Resolve(gridUid, ref gridComponent) ? EntityCoordinates.Invalid : new EntityCoordinates(gridUid, Vector2i.op_Implicit(Vector2i.op_Multiply(tile, (int) gridComponent.TileSize)));
  }

  public EntityCoordinates ToCenterCoordinates(TileRef tileRef, MapGridComponent? gridComponent = null)
  {
    return this.ToCenterCoordinates(tileRef.GridUid, tileRef.GridIndices, gridComponent);
  }

  public EntityCoordinates ToCenterCoordinates(
    EntityUid gridUid,
    Vector2i tile,
    MapGridComponent? gridComponent = null)
  {
    return !this._gridQuery.Resolve(gridUid, ref gridComponent) ? EntityCoordinates.Invalid : new EntityCoordinates(gridUid, Vector2i.op_Implicit(Vector2i.op_Multiply(tile, (int) gridComponent.TileSize)) + gridComponent.TileSizeHalfVector);
  }

  internal Dictionary<MapId, EntityUid> Maps { get; } = new Dictionary<MapId, EntityUid>();

  public override void Initialize()
  {
    base.Initialize();
    this._fixturesQuery = this.GetEntityQuery<FixturesComponent>();
    this._mapQuery = this.GetEntityQuery<MapComponent>();
    this._gridQuery = this.GetEntityQuery<MapGridComponent>();
    this._metaQuery = this.GetEntityQuery<MetaDataComponent>();
    this._xformQuery = this.GetEntityQuery<TransformComponent>();
    this.InitializeMap();
    this.InitializeGrid();
    this.SubscribeLocalEvent<MapLightComponent, ComponentGetState>(new ComponentEventRefHandler<MapLightComponent, ComponentGetState>(this.OnMapLightGetState));
    this.SubscribeLocalEvent<MapLightComponent, ComponentHandleState>(new ComponentEventRefHandler<MapLightComponent, ComponentHandleState>(this.OnMapLightHandleState));
  }

  public static ulong ToBitmask(Vector2i index, byte chunkSize = 8)
  {
    return 1UL << index.X + index.Y * (int) chunkSize;
  }

  public static bool FromBitmask(Vector2i index, ulong bitmask, byte chunkSize = 8)
  {
    ulong bitmask1 = SharedMapSystem.ToBitmask(index, chunkSize);
    return ((long) bitmask1 & (long) bitmask) == (long) bitmask1;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Vector2i GetChunkIndices(Vector2 tile, int chunkSize)
  {
    return new Vector2i((int) Math.Floor((double) tile.X / (double) chunkSize), (int) Math.Floor((double) tile.Y / (double) chunkSize));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Vector2i GetChunkIndices(Vector2 tile, byte chunkSize)
  {
    return new Vector2i((int) Math.Floor((double) tile.X / (double) chunkSize), (int) Math.Floor((double) tile.Y / (double) chunkSize));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Vector2i GetChunkIndices(Vector2i tile, int chunkSize)
  {
    return new Vector2i((int) Math.Floor((double) tile.X / (double) chunkSize), (int) Math.Floor((double) tile.Y / (double) chunkSize));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Vector2i GetChunkIndices(Vector2i tile, byte chunkSize)
  {
    return new Vector2i((int) Math.Floor((double) tile.X / (double) chunkSize), (int) Math.Floor((double) tile.Y / (double) chunkSize));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Vector2i GetChunkRelative(Vector2 tile, int chunkSize)
  {
    return new Vector2i(MathHelper.Mod((int) Math.Floor((double) tile.X), chunkSize), MathHelper.Mod((int) Math.Floor((double) tile.Y), chunkSize));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Vector2i GetChunkRelative(Vector2 tile, byte chunkSize)
  {
    return new Vector2i(MathHelper.Mod((int) Math.Floor((double) tile.X), (int) chunkSize), MathHelper.Mod((int) Math.Floor((double) tile.Y), (int) chunkSize));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Vector2i GetChunkRelative(Vector2i tile, int chunkSize)
  {
    return new Vector2i(MathHelper.Mod(tile.X, chunkSize), MathHelper.Mod(tile.Y, chunkSize));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Vector2i GetChunkRelative(Vector2i tile, byte chunkSize)
  {
    return new Vector2i(MathHelper.Mod(tile.X, (int) chunkSize), MathHelper.Mod(tile.Y, (int) chunkSize));
  }

  public static Vector2i GetDirection(Vector2i position, Direction dir, int dist = 1)
  {
    switch ((int) dir)
    {
      case 0:
        return Vector2i.op_Addition(position, new Vector2i(0, -dist));
      case 1:
        return Vector2i.op_Addition(position, new Vector2i(dist, -dist));
      case 2:
        return Vector2i.op_Addition(position, new Vector2i(dist, 0));
      case 3:
        return Vector2i.op_Addition(position, new Vector2i(dist, dist));
      case 4:
        return Vector2i.op_Addition(position, new Vector2i(0, dist));
      case 5:
        return Vector2i.op_Addition(position, new Vector2i(-dist, dist));
      case 6:
        return Vector2i.op_Addition(position, new Vector2i(-dist, 0));
      case 7:
        return Vector2i.op_Addition(position, new Vector2i(-dist, -dist));
      default:
        throw new NotImplementedException();
    }
  }

  private void InitializeGrid()
  {
    this.SubscribeLocalEvent<MapGridComponent, ComponentGetState>(new ComponentEventRefHandler<MapGridComponent, ComponentGetState>(this.OnGridGetState));
    this.SubscribeLocalEvent<MapGridComponent, ComponentHandleState>(new ComponentEventRefHandler<MapGridComponent, ComponentHandleState>(this.OnGridHandleState));
    this.SubscribeLocalEvent<MapGridComponent, ComponentAdd>(new ComponentEventHandler<MapGridComponent, ComponentAdd>(this.OnGridAdd));
    this.SubscribeLocalEvent<MapGridComponent, ComponentInit>(new ComponentEventHandler<MapGridComponent, ComponentInit>(this.OnGridInit));
    this.SubscribeLocalEvent<MapGridComponent, ComponentStartup>(new ComponentEventHandler<MapGridComponent, ComponentStartup>(this.OnGridStartup));
    this.SubscribeLocalEvent<MapGridComponent, ComponentShutdown>(new ComponentEventHandler<MapGridComponent, ComponentShutdown>(this.OnGridRemove));
    this.SubscribeLocalEvent<MapGridComponent, MoveEvent>(new ComponentEventRefHandler<MapGridComponent, MoveEvent>(this.OnGridMove));
  }

  public Vector2 GetGridPosition(Entity<PhysicsComponent?> grid, Vector2 worldPos, Angle worldRot)
  {
    if (!this.Resolve<PhysicsComponent>(grid.Owner, ref grid.Comp))
      return Vector2.Zero;
    Vector2 vector2_1 = worldPos;
    ref Angle local1 = ref worldRot;
    Vector2 localCenter = grid.Comp.LocalCenter;
    ref Vector2 local2 = ref localCenter;
    Vector2 vector2_2 = ((Angle) ref local1).RotateVec(ref local2);
    return vector2_1 + vector2_2;
  }

  public Vector2 GetGridPosition(Entity<PhysicsComponent?, TransformComponent?> grid)
  {
    if (!this.Resolve<PhysicsComponent, TransformComponent>(grid.Owner, ref grid.Comp1, ref grid.Comp2))
      return Vector2.Zero;
    (Vector2 vector2, Angle angle) = this._transform.GetWorldPositionRotation(grid.Comp2);
    return this.GetGridPosition((Entity<PhysicsComponent>) (grid.Owner, grid.Comp1), vector2, angle);
  }

  private void OnGridBoundsChange(EntityUid uid, MapGridComponent component)
  {
    if (component.MapProxy == DynamicTree.Proxy.Free)
      return;
    TransformComponent xform = this.Comp<TransformComponent>(uid);
    Box2 aabb = this.GetWorldAABB(uid, component, xform);
    GridTreeComponent comp;
    if (this.TryComp<GridTreeComponent>(xform.MapUid, out comp))
      comp.Tree.MoveProxy(component.MapProxy, in aabb);
    this._physics.MovedGrids.Add(uid);
  }

  private void OnGridMove(EntityUid uid, MapGridComponent component, ref MoveEvent args)
  {
    if (args.ParentChanged)
    {
      this.OnParentChange(uid, component, ref args);
    }
    else
    {
      if (component.MapProxy == DynamicTree.Proxy.Free)
        return;
      TransformComponent component1 = args.Component;
      Box2 aabb = this.GetWorldAABB(uid, component, component1);
      GridTreeComponent comp;
      if (this.TryComp<GridTreeComponent>(component1.MapUid, out comp))
        comp.Tree.MoveProxy(component.MapProxy, in aabb);
      this._physics.MovedGrids.Add(uid);
    }
  }

  private void OnParentChange(EntityUid uid, MapGridComponent component, ref MoveEvent args)
  {
    this.UpdatePvsChunks(args.Entity);
    (EntityUid _, TransformComponent comp1, MetaDataComponent metaDataComponent) = args.Entity;
    if (metaDataComponent.EntityLifeStage < EntityLifeStage.Initialized || args.Component.LifeStage == ComponentLifeStage.Starting)
      return;
    this.Log.Info($"Grid {this.ToPrettyString(uid, metaDataComponent)} changed parent. Old parent: {this.ToPrettyString((Entity<MetaDataComponent>) args.OldPosition.EntityId)}. New parent: {this.ToPrettyString((Entity<MetaDataComponent>) comp1.ParentUid)}");
    if (!comp1.MapUid.HasValue && metaDataComponent.EntityLifeStage < EntityLifeStage.Terminating && this._netManager.IsServer)
      this.Log.Error($"Grid {this.ToPrettyString(uid, metaDataComponent)} was moved to nullspace! AAAAAAAAAAAAAAAAAAAAAAAAA! {Environment.StackTrace}");
    EntityUid parentUid = comp1.ParentUid;
    EntityUid? mapUid = comp1.MapUid;
    if ((mapUid.HasValue ? (parentUid != mapUid.GetValueOrDefault() ? 1 : 0) : 1) != 0 && metaDataComponent.EntityLifeStage < EntityLifeStage.Terminating && this._netManager.IsServer)
    {
      this.Log.Error($"Grid {this.ToPrettyString(uid, metaDataComponent)} is parented to {this.ToPrettyString((Entity<MetaDataComponent>) comp1._parent)} which is not a map.  y'all need jesus. {Environment.StackTrace}");
    }
    else
    {
      EntityUid mapOrInvalid = this.GetMapOrInvalid(new MapId?(this._transform.ToMapCoordinates(args.OldPosition).MapId));
      if (component.MapProxy != DynamicTree.Proxy.Free)
      {
        this._physics.MovedGrids.Remove(uid);
        this.RemoveGrid(uid, component, mapOrInvalid);
      }
      mapUid = comp1.MapUid;
      if (!mapUid.HasValue)
        return;
      this._physics.MovedGrids.Add(uid);
      this.AddGrid(uid, component);
    }
  }

  protected virtual void UpdatePvsChunks(Entity<TransformComponent, MetaDataComponent> grid)
  {
  }

  private void OnGridHandleState(
    EntityUid uid,
    MapGridComponent component,
    ref ComponentHandleState args)
  {
    switch (args.Current)
    {
      case MapGridComponentDeltaState componentDeltaState:
        component.ChunkSize = componentDeltaState.ChunkSize;
        if (componentDeltaState.ChunkData == null)
          return;
        foreach ((Vector2i vector2i, ChunkDatum data) in componentDeltaState.ChunkData)
          this.ApplyChunkData(uid, component, vector2i, data);
        component.LastTileModifiedTick = componentDeltaState.LastTileModifiedTick;
        break;
      case MapGridComponentState gridComponentState:
        component.LastTileModifiedTick = gridComponentState.LastTileModifiedTick;
        component.ChunkSize = gridComponentState.ChunkSize;
        foreach (Vector2i key in component.Chunks.Keys)
        {
          if (!gridComponentState.FullGridData.ContainsKey(key))
            this.ApplyChunkData(uid, component, key, ChunkDatum.Empty);
        }
        using (Dictionary<Vector2i, ChunkDatum>.Enumerator enumerator = gridComponentState.FullGridData.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            (Vector2i vector2i, ChunkDatum data) = enumerator.Current;
            this.ApplyChunkData(uid, component, vector2i, data);
          }
          break;
        }
      default:
        return;
    }
    this.RegenerateAabb(component);
    this.OnGridBoundsChange(uid, component);
  }

  private void ApplyChunkData(
    EntityUid uid,
    MapGridComponent component,
    Vector2i index,
    ChunkDatum data)
  {
    int num = 0;
    Entity<MapGridComponent> entity = new Entity<MapGridComponent>(uid, component);
    bool shapeChanged;
    if (data.IsDeleted())
    {
      MapChunk mapChunk;
      if (!component.Chunks.Remove(index, out mapChunk))
        return;
      mapChunk.SuppressCollisionRegeneration = true;
      for (ushort xIndex = 0; (int) xIndex < (int) component.ChunkSize; ++xIndex)
      {
        for (ushort yIndex = 0; (int) yIndex < (int) component.ChunkSize; ++yIndex)
        {
          Tile oldTile;
          if (mapChunk.TrySetTile(xIndex, yIndex, Tile.Empty, out oldTile, out shapeChanged))
          {
            Vector2i gridTile = mapChunk.ChunkTileToGridTile(Vector2i.op_Implicit(((int) xIndex, (int) yIndex)));
            TileRef tileRef = new TileRef(uid, gridTile, Tile.Empty);
            this._mapInternal.RaiseOnTileChanged(entity, tileRef, oldTile, index);
          }
        }
      }
      mapChunk.CachedBounds = Box2i.Empty;
      mapChunk.SuppressCollisionRegeneration = false;
    }
    else
    {
      MapChunk orAddChunk = this.GetOrAddChunk(uid, component, index);
      orAddChunk.Fixtures.Clear();
      orAddChunk.Fixtures.UnionWith((IEnumerable<string>) data.Fixtures);
      orAddChunk.SuppressCollisionRegeneration = true;
      ValueList<TileChangedEntry> valueList = new ValueList<TileChangedEntry>();
      for (ushort xIndex = 0; (int) xIndex < (int) component.ChunkSize; ++xIndex)
      {
        for (ushort yIndex = 0; (int) yIndex < (int) component.ChunkSize; ++yIndex)
        {
          Tile tile = data.TileData[num++];
          Tile oldTile;
          if (orAddChunk.TrySetTile(xIndex, yIndex, tile, out oldTile, out shapeChanged))
          {
            Vector2i chunkTile;
            // ISSUE: explicit constructor call
            ((Vector2i) ref chunkTile).\u002Ector((int) xIndex, (int) yIndex);
            Vector2i gridTile = orAddChunk.ChunkTileToGridTile(chunkTile);
            valueList.Add(new TileChangedEntry(tile, oldTile, orAddChunk.Indices, gridTile));
          }
        }
      }
      TileChangedEvent args = new TileChangedEvent(entity, valueList.ToArray());
      this.EntityManager.EventBus.RaiseLocalEvent<TileChangedEvent>(entity.Owner, ref args, true);
      orAddChunk.CachedBounds = data.CachedBounds.Value;
      orAddChunk.SuppressCollisionRegeneration = false;
    }
  }

  private void OnGridGetState(
    EntityUid uid,
    MapGridComponent component,
    ref ComponentGetState args)
  {
    if (args.FromTick <= component.CreationTick)
    {
      this.GetFullState(uid, component, ref args);
    }
    else
    {
      GameTick fromTick = args.FromTick;
      Dictionary<Vector2i, ChunkDatum> chunkData;
      if (component.LastTileModifiedTick < fromTick)
      {
        chunkData = (Dictionary<Vector2i, ChunkDatum>) null;
      }
      else
      {
        chunkData = new Dictionary<Vector2i, ChunkDatum>();
        foreach ((GameTick tick, Vector2i vector2i) in component.ChunkDeletionHistory)
        {
          if (!(tick < fromTick) || !(fromTick != GameTick.Zero))
          {
            MapChunk mapChunk;
            if (!component.Chunks.TryGetValue(vector2i, out mapChunk))
              chunkData.Add(vector2i, ChunkDatum.Empty);
            else if (mapChunk.LastTileModifiedTick < fromTick)
              this.Log.Error($"Encountered un-deleted chunk with an old last-modified tick on grid {this.ToPrettyString((Entity<MetaDataComponent>) uid)}");
          }
        }
        foreach ((Vector2i key, MapChunk mapChunk) in (IEnumerable<KeyValuePair<Vector2i, MapChunk>>) this.GetMapChunks(uid, component))
        {
          if (!(mapChunk.LastTileModifiedTick < fromTick))
          {
            Tile[] tileData = new Tile[(int) component.ChunkSize * (int) component.ChunkSize];
            for (int xIndex = 0; xIndex < (int) component.ChunkSize; ++xIndex)
            {
              for (int yIndex = 0; yIndex < (int) component.ChunkSize; ++yIndex)
                tileData[xIndex * (int) component.ChunkSize + yIndex] = mapChunk.GetTile((ushort) xIndex, (ushort) yIndex);
            }
            HashSet<string> stringSet = mapChunk.Fixtures;
            if (this._netManager.IsClient)
              stringSet = new HashSet<string>((IEnumerable<string>) stringSet);
            chunkData.Add(key, ChunkDatum.CreateModified(tileData, stringSet, mapChunk.CachedBounds));
          }
        }
      }
      args.State = (IComponentState) new MapGridComponentDeltaState(component.ChunkSize, chunkData, component.LastTileModifiedTick);
    }
  }

  private void GetFullState(EntityUid uid, MapGridComponent component, ref ComponentGetState args)
  {
    Dictionary<Vector2i, ChunkDatum> fullGridData = new Dictionary<Vector2i, ChunkDatum>();
    foreach ((Vector2i key, MapChunk mapChunk) in (IEnumerable<KeyValuePair<Vector2i, MapChunk>>) this.GetMapChunks(uid, component))
    {
      Tile[] tileData = new Tile[(int) component.ChunkSize * (int) component.ChunkSize];
      for (int xIndex = 0; xIndex < (int) component.ChunkSize; ++xIndex)
      {
        for (int yIndex = 0; yIndex < (int) component.ChunkSize; ++yIndex)
          tileData[xIndex * (int) component.ChunkSize + yIndex] = mapChunk.GetTile((ushort) xIndex, (ushort) yIndex);
      }
      HashSet<string> stringSet = mapChunk.Fixtures;
      if (this._netManager.IsClient)
        stringSet = new HashSet<string>((IEnumerable<string>) stringSet);
      fullGridData.Add(key, ChunkDatum.CreateModified(tileData, stringSet, mapChunk.CachedBounds));
    }
    args.State = (IComponentState) new MapGridComponentState(component.ChunkSize, fullGridData, component.LastTileModifiedTick);
  }

  private void OnGridAdd(EntityUid uid, MapGridComponent component, ComponentAdd args)
  {
    GridAddEvent args1 = new GridAddEvent(uid);
    this.RaiseLocalEvent<GridAddEvent>(uid, args1, true);
  }

  private void OnGridInit(EntityUid uid, MapGridComponent component, ComponentInit args)
  {
    TransformComponent component1 = this._xformQuery.GetComponent(uid);
    GameTick curTick = this._timing.CurTick;
    foreach (MapChunk mapChunk in component.Chunks.Values)
      mapChunk.LastTileModifiedTick = curTick;
    component.LastTileModifiedTick = curTick;
    EntityUid? mapUid;
    if (component1.MapUid.HasValue)
    {
      mapUid = component1.MapUid;
      EntityUid entityUid = uid;
      if ((mapUid.HasValue ? (mapUid.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0)
      {
        SharedTransformSystem transform = this._transform;
        EntityUid uid1 = uid;
        TransformComponent xform = component1;
        mapUid = component1.MapUid;
        EntityUid parent = mapUid.Value;
        transform.SetParent(uid1, xform, parent);
      }
    }
    if (!this._mapQuery.HasComponent(uid))
    {
      Box2 aabb = this.GetWorldAABB(uid, component);
      GridTreeComponent comp;
      if (this.TryComp<GridTreeComponent>(component1.MapUid, out comp))
      {
        DynamicTree.Proxy proxy = comp.Tree.CreateProxy(in aabb, uint.MaxValue, (uid, this._fixturesQuery.Comp(uid), component));
        component.MapProxy = proxy;
      }
      mapUid = component1.MapUid;
      if (mapUid.HasValue)
        this._physics.MovedGrids.Add(uid);
    }
    GridInitializeEvent args1 = new GridInitializeEvent(uid);
    this.RaiseLocalEvent<GridInitializeEvent>(uid, args1, true);
  }

  private void OnGridStartup(EntityUid uid, MapGridComponent component, ComponentStartup args)
  {
    GridStartupEvent args1 = new GridStartupEvent(uid);
    this.RaiseLocalEvent<GridStartupEvent>(uid, args1, true);
  }

  private void OnGridRemove(EntityUid uid, MapGridComponent component, ComponentShutdown args)
  {
    this.Log.Info($"Removing grid {this.ToPrettyString((Entity<MetaDataComponent>) uid)}");
    TransformComponent comp;
    if (this.TryComp(uid, out comp) && comp.MapUid.HasValue)
      this.RemoveGrid(uid, component, comp.MapUid.Value);
    component.MapProxy = DynamicTree.Proxy.Free;
    this.RaiseLocalEvent<GridRemovalEvent>(uid, new GridRemovalEvent(uid), true);
  }

  private Box2 GetWorldAABB(EntityUid uid, MapGridComponent grid, TransformComponent? xform = null)
  {
    if (!this.Resolve(uid, ref xform))
      return new Box2();
    (Vector2 WorldPosition, Angle WorldRotation) = this._transform.GetWorldPositionRotation(xform);
    Box2 localAabb = grid.LocalAABB;
    Box2Rotated box2Rotated = new Box2Rotated(((Box2) ref localAabb).Translated(WorldPosition), WorldRotation, WorldPosition);
    return ((Box2Rotated) ref box2Rotated).CalcBoundingBox();
  }

  private void AddGrid(EntityUid uid, MapGridComponent grid)
  {
    Box2 aabb = this.GetWorldAABB(uid, grid);
    TransformComponent component;
    if (!this._xformQuery.TryGetComponent(uid, out component))
      return;
    GridTreeComponent comp;
    if (this.TryComp<GridTreeComponent>(component.MapUid, out comp))
    {
      DynamicTree.Proxy proxy = comp.Tree.CreateProxy(in aabb, uint.MaxValue, (uid, this._fixturesQuery.Comp(uid), grid));
      grid.MapProxy = proxy;
    }
    if (!component.MapUid.HasValue)
      return;
    this._physics.MovedGrids.Add(uid);
  }

  private void RemoveGrid(EntityUid uid, MapGridComponent grid, EntityUid mapUid)
  {
    GridTreeComponent comp;
    if (grid.MapProxy != DynamicTree.Proxy.Free && this.TryComp<GridTreeComponent>(mapUid, out comp))
      comp.Tree.DestroyProxy(grid.MapProxy);
    grid.MapProxy = DynamicTree.Proxy.Free;
    if (!mapUid.IsValid())
      return;
    this._physics.MovedGrids.Remove(uid);
  }

  private void RemoveChunk(EntityUid uid, MapGridComponent grid, Vector2i origin)
  {
    MapChunk mapChunk;
    if (!grid.Chunks.TryGetValue(origin, out mapChunk))
      return;
    if (this._netManager.IsServer)
      grid.ChunkDeletionHistory.Add((this._timing.CurTick, mapChunk.Indices));
    mapChunk.Fixtures.Clear();
    grid.Chunks.Remove(origin);
    if (grid.Chunks.Count != 0)
      return;
    this.RaiseLocalEvent<EmptyGridEvent>(uid, new EmptyGridEvent()
    {
      GridId = uid
    }, true);
  }

  private void RegenerateCollision(EntityUid uid, MapGridComponent grid, MapChunk mapChunk)
  {
    this.RegenerateCollision(uid, grid, (IReadOnlySet<MapChunk>) new HashSet<MapChunk>()
    {
      mapChunk
    });
  }

  internal void RegenerateCollision(
    EntityUid uid,
    MapGridComponent grid,
    IReadOnlySet<MapChunk> chunks)
  {
    if (this.HasComp<MapComponent>(uid))
    {
      this.ClearEmptyMapChunks(uid, grid, chunks);
    }
    else
    {
      Dictionary<MapChunk, List<Box2i>> ChunkRectangles = new Dictionary<MapChunk, List<Box2i>>(chunks.Count);
      List<MapChunk> RemovedChunks = new List<MapChunk>();
      foreach (MapChunk chunk in (IEnumerable<MapChunk>) chunks)
      {
        Box2i bounds;
        List<Box2i> rectangles;
        GridChunkPartition.PartitionChunk(chunk, out bounds, out rectangles);
        chunk.CachedBounds = bounds;
        if (chunk.FilledTiles > 0)
        {
          ChunkRectangles.Add(chunk, rectangles);
        }
        else
        {
          FixturesComponent fixturesComponent1 = (FixturesComponent) null;
          PhysicsComponent physicsComponent = (PhysicsComponent) null;
          TransformComponent transformComponent = (TransformComponent) null;
          foreach (string fixture in chunk.Fixtures)
          {
            chunk.Fixtures.Remove(fixture);
            FixtureSystem fixtures = this._fixtures;
            EntityUid uid1 = uid;
            string id = fixture;
            FixturesComponent fixturesComponent2 = fixturesComponent1;
            PhysicsComponent body = physicsComponent;
            FixturesComponent manager = fixturesComponent2;
            TransformComponent xform = transformComponent;
            fixtures.DestroyFixture(uid1, id, false, body, manager, xform);
          }
          this.RemoveChunk(uid, grid, chunk.Indices);
          RemovedChunks.Add(chunk);
        }
      }
      this.RegenerateAabb(grid);
      if (this.Deleted(uid))
        return;
      this._physics.WakeBody(uid);
      this.OnGridBoundsChange(uid, grid);
      RegenerateGridBoundsEvent message = new RegenerateGridBoundsEvent(uid, ChunkRectangles, RemovedChunks);
      this.RaiseLocalEvent<RegenerateGridBoundsEvent>(ref message);
    }
  }

  private void RegenerateAabb(MapGridComponent grid)
  {
    grid.LocalAABB = new Box2();
    foreach (MapChunk mapChunk in grid.Chunks.Values)
    {
      Box2i cachedBounds = mapChunk.CachedBounds;
      Vector2i size = ((Box2i) ref cachedBounds).Size;
      if (!((Vector2i) ref size).Equals(Vector2i.Zero))
      {
        Box2 localAabb1 = grid.LocalAABB;
        if (((Box2) ref localAabb1).Size == Vector2.Zero)
        {
          Box2i box2i = ((Box2i) ref cachedBounds).Translated(Vector2i.op_Multiply(mapChunk.Indices, (int) mapChunk.ChunkSize));
          grid.LocalAABB = Box2i.op_Implicit(box2i);
        }
        else
        {
          Box2i box2i = ((Box2i) ref cachedBounds).Translated(Vector2i.op_Multiply(mapChunk.Indices, (int) mapChunk.ChunkSize));
          MapGridComponent mapGridComponent = grid;
          Box2 localAabb2 = grid.LocalAABB;
          ref Box2 local1 = ref localAabb2;
          Box2 box2_1 = Box2i.op_Implicit(box2i);
          ref Box2 local2 = ref box2_1;
          Box2 box2_2 = ((Box2) ref local1).Union(ref local2);
          mapGridComponent.LocalAABB = box2_2;
        }
      }
    }
  }

  private void ClearEmptyMapChunks(
    EntityUid uid,
    MapGridComponent grid,
    IReadOnlySet<MapChunk> modified)
  {
    foreach (MapChunk mapChunk in (IEnumerable<MapChunk>) modified)
    {
      if (mapChunk.FilledTiles <= 0)
        this.RemoveChunk(uid, grid, mapChunk.Indices);
    }
  }

  public TileRef GetTileRef(Entity<MapGridComponent> grid, MapCoordinates coords)
  {
    return this.GetTileRef(grid.Owner, grid.Comp, coords);
  }

  public TileRef GetTileRef(EntityUid uid, MapGridComponent grid, MapCoordinates coords)
  {
    return this.GetTileRef(uid, grid, this.CoordinatesToTile(uid, grid, coords));
  }

  public TileRef GetTileRef(Entity<MapGridComponent> grid, EntityCoordinates coords)
  {
    return this.GetTileRef(grid.Owner, grid.Comp, coords);
  }

  public TileRef GetTileRef(EntityUid uid, MapGridComponent grid, EntityCoordinates coords)
  {
    return this.GetTileRef(uid, grid, this.CoordinatesToTile(uid, grid, coords));
  }

  public TileRef GetTileRef(Entity<MapGridComponent> grid, Vector2i tileCoordinates)
  {
    return this.GetTileRef(grid.Owner, grid.Comp, tileCoordinates);
  }

  public TileRef GetTileRef(EntityUid uid, MapGridComponent grid, Vector2i tileCoordinates)
  {
    Vector2i chunkIndices = this.GridTileToChunkIndices(uid, grid, tileCoordinates);
    MapChunk mapChunk;
    if (!grid.Chunks.TryGetValue(chunkIndices, out mapChunk))
      return new TileRef(uid, tileCoordinates.X, tileCoordinates.Y, new Tile());
    Vector2i chunkTile = mapChunk.GridTileToChunkTile(tileCoordinates);
    return this.GetTileRef(uid, grid, mapChunk, (ushort) chunkTile.X, (ushort) chunkTile.Y);
  }

  internal TileRef GetTileRef(
    EntityUid uid,
    MapGridComponent grid,
    MapChunk mapChunk,
    ushort xIndex,
    ushort yIndex)
  {
    if ((int) xIndex >= (int) mapChunk.ChunkSize)
      throw new ArgumentOutOfRangeException(nameof (xIndex), "Tile indices out of bounds.");
    if ((int) yIndex >= (int) mapChunk.ChunkSize)
      throw new ArgumentOutOfRangeException(nameof (yIndex), "Tile indices out of bounds.");
    Vector2i gridTile = mapChunk.ChunkTileToGridTile(new Vector2i((int) xIndex, (int) yIndex));
    return new TileRef(uid, gridTile, mapChunk.GetTile(xIndex, yIndex));
  }

  public IEnumerable<TileRef> GetAllTiles(EntityUid uid, MapGridComponent grid, bool ignoreEmpty = true)
  {
    foreach (MapChunk chunk in grid.Chunks.Values)
    {
      for (ushort x = 0; (int) x < (int) grid.ChunkSize; ++x)
      {
        for (ushort y = 0; (int) y < (int) grid.ChunkSize; ++y)
        {
          Tile tile = chunk.GetTile(x, y);
          if (!ignoreEmpty || !tile.IsEmpty)
          {
            Vector2i vector2i = Vector2i.op_Addition(new Vector2i((int) x, (int) y), Vector2i.op_Multiply(chunk.Indices, (int) grid.ChunkSize));
            int xIndex;
            int yIndex;
            ((Vector2i) ref vector2i).Deconstruct(ref xIndex, ref yIndex);
            yield return new TileRef(uid, xIndex, yIndex, tile);
          }
        }
      }
    }
  }

  public GridTileEnumerator GetAllTilesEnumerator(
    EntityUid uid,
    MapGridComponent grid,
    bool ignoreEmpty = true)
  {
    return new GridTileEnumerator(uid, grid.Chunks.GetEnumerator(), grid.ChunkSize, ignoreEmpty);
  }

  public void SetTile(Entity<MapGridComponent> grid, EntityCoordinates coordinates, Tile tile)
  {
    this.SetTile(grid.Owner, grid.Comp, coordinates, tile);
  }

  public void SetTile(Entity<MapGridComponent> grid, Vector2i gridIndices, Tile tile)
  {
    this.SetTile(grid.Owner, grid.Comp, gridIndices, tile);
  }

  public void SetTiles(Entity<MapGridComponent> grid, List<(Vector2i GridIndices, Tile Tile)> tiles)
  {
    this.SetTiles(grid.Owner, grid.Comp, tiles);
  }

  public void SetTile(EntityUid uid, MapGridComponent grid, EntityCoordinates coords, Tile tile)
  {
    Vector2i tile1 = this.CoordinatesToTile(uid, grid, coords);
    this.SetTile(uid, grid, new Vector2i(tile1.X, tile1.Y), tile);
  }

  public void SetTile(EntityUid uid, MapGridComponent grid, Vector2i gridIndices, Tile tile)
  {
    Vector2i chunkIndices = this.GridTileToChunkIndices(uid, grid, gridIndices);
    MapChunk chunk;
    if (!grid.Chunks.TryGetValue(chunkIndices, out chunk))
    {
      if (tile.IsEmpty)
        return;
      Dictionary<Vector2i, MapChunk> chunks = grid.Chunks;
      Vector2i key = chunkIndices;
      MapChunk mapChunk = new MapChunk(chunkIndices.X, chunkIndices.Y, grid.ChunkSize);
      mapChunk.LastTileModifiedTick = this._timing.CurTick;
      chunk = mapChunk;
      chunks[key] = mapChunk;
    }
    Vector2i chunkTile = chunk.GridTileToChunkTile(gridIndices);
    this.SetChunkTile(uid, grid, chunk, (ushort) chunkTile.X, (ushort) chunkTile.Y, tile, out Tile _);
  }

  public void SetTiles(
    EntityUid uid,
    MapGridComponent grid,
    List<(Vector2i GridIndices, Tile Tile)> tiles)
  {
    if (tiles.Count == 0)
      return;
    HashSet<MapChunk> chunks1 = new HashSet<MapChunk>(Math.Max(1, tiles.Count / (int) grid.ChunkSize));
    ValueList<TileChangedEntry> valueList = new ValueList<TileChangedEntry>(tiles.Count);
    this.MapManager.SuppressOnTileChanged = true;
    foreach ((Vector2i vector2i, Tile tile) in tiles)
    {
      Vector2i chunkIndices = this.GridTileToChunkIndices(uid, grid, vector2i);
      MapChunk chunk;
      if (!grid.Chunks.TryGetValue(chunkIndices, out chunk))
      {
        if (!tile.IsEmpty)
        {
          Dictionary<Vector2i, MapChunk> chunks2 = grid.Chunks;
          Vector2i key = chunkIndices;
          MapChunk mapChunk = new MapChunk(chunkIndices.X, chunkIndices.Y, grid.ChunkSize);
          mapChunk.LastTileModifiedTick = this._timing.CurTick;
          chunk = mapChunk;
          chunks2[key] = mapChunk;
        }
        else
          continue;
      }
      Vector2i chunkTile = chunk.GridTileToChunkTile(vector2i);
      chunk.SuppressCollisionRegeneration = true;
      Tile oldTile;
      if (this.SetChunkTile(uid, grid, chunk, (ushort) chunkTile.X, (ushort) chunkTile.Y, tile, out oldTile))
      {
        chunks1.Add(chunk);
        valueList.Add(new TileChangedEntry(tile, oldTile, chunkTile, vector2i));
      }
    }
    foreach (MapChunk mapChunk in chunks1)
      mapChunk.SuppressCollisionRegeneration = false;
    TileChangedEvent args = new TileChangedEvent((Entity<MapGridComponent>) (uid, grid), valueList.ToArray());
    this.RaiseLocalEvent<TileChangedEvent>(uid, ref args, true);
    this.RegenerateCollision(uid, grid, (IReadOnlySet<MapChunk>) chunks1);
    this.MapManager.SuppressOnTileChanged = false;
  }

  public SharedMapSystem.TilesEnumerator GetLocalTilesEnumerator(
    EntityUid uid,
    MapGridComponent grid,
    Box2 aabb,
    bool ignoreEmpty = true,
    Predicate<TileRef>? predicate = null)
  {
    return new SharedMapSystem.TilesEnumerator(this, ignoreEmpty, predicate, uid, grid, aabb);
  }

  public SharedMapSystem.TilesEnumerator GetTilesEnumerator(
    EntityUid uid,
    MapGridComponent grid,
    Box2 aabb,
    bool ignoreEmpty = true,
    Predicate<TileRef>? predicate = null)
  {
    Box2 aabb1 = Matrix3Helpers.TransformBox(this._transform.GetInvWorldMatrix(uid), ref aabb);
    return new SharedMapSystem.TilesEnumerator(this, ignoreEmpty, predicate, uid, grid, aabb1);
  }

  public SharedMapSystem.TilesEnumerator GetTilesEnumerator(
    EntityUid uid,
    MapGridComponent grid,
    Box2Rotated bounds,
    bool ignoreEmpty = true,
    Predicate<TileRef>? predicate = null)
  {
    Box2 aabb = Matrix3Helpers.TransformBox(this._transform.GetInvWorldMatrix(uid), ref bounds);
    return new SharedMapSystem.TilesEnumerator(this, ignoreEmpty, predicate, uid, grid, aabb);
  }

  public IEnumerable<TileRef> GetLocalTilesIntersecting(
    EntityUid uid,
    MapGridComponent grid,
    Box2 localAABB,
    bool ignoreEmpty = true,
    Predicate<TileRef>? predicate = null)
  {
    SharedMapSystem.TilesEnumerator enumerator = new SharedMapSystem.TilesEnumerator(this, ignoreEmpty, predicate, uid, grid, localAABB);
    TileRef tile;
    while (enumerator.MoveNext(out tile))
      yield return tile;
  }

  public IEnumerable<TileRef> GetLocalTilesIntersecting(
    EntityUid uid,
    MapGridComponent grid,
    Box2Rotated localArea,
    bool ignoreEmpty = true,
    Predicate<TileRef>? predicate = null)
  {
    SharedMapSystem mapSystem = this;
    Box2 aabb = ((Box2Rotated) ref localArea).CalcBoundingBox();
    SharedMapSystem.TilesEnumerator enumerator = new SharedMapSystem.TilesEnumerator(mapSystem, ignoreEmpty, predicate, uid, grid, aabb);
    TileRef tile;
    while (enumerator.MoveNext(out tile))
      yield return tile;
  }

  public IEnumerable<TileRef> GetTilesIntersecting(
    EntityUid uid,
    MapGridComponent grid,
    Box2Rotated worldArea,
    bool ignoreEmpty = true,
    Predicate<TileRef>? predicate = null)
  {
    SharedMapSystem mapSystem = this;
    Box2 aabb = Matrix3Helpers.TransformBox(mapSystem._transform.GetInvWorldMatrix(uid), ref worldArea);
    SharedMapSystem.TilesEnumerator enumerator = new SharedMapSystem.TilesEnumerator(mapSystem, ignoreEmpty, predicate, uid, grid, aabb);
    TileRef tile;
    while (enumerator.MoveNext(out tile))
      yield return tile;
  }

  public IEnumerable<TileRef> GetTilesIntersecting(
    EntityUid uid,
    MapGridComponent grid,
    Box2 worldArea,
    bool ignoreEmpty = true,
    Predicate<TileRef>? predicate = null)
  {
    SharedMapSystem mapSystem = this;
    Box2 aabb = Matrix3Helpers.TransformBox(mapSystem._transform.GetInvWorldMatrix(uid), ref worldArea);
    SharedMapSystem.TilesEnumerator enumerator = new SharedMapSystem.TilesEnumerator(mapSystem, ignoreEmpty, predicate, uid, grid, aabb);
    TileRef tile;
    while (enumerator.MoveNext(out tile))
      yield return tile;
  }

  public IEnumerable<TileRef> GetLocalTilesIntersecting(
    EntityUid uid,
    MapGridComponent grid,
    Circle localCircle,
    bool ignoreEmpty = true,
    Predicate<TileRef>? predicate = null)
  {
    Box2 aabb;
    // ISSUE: explicit constructor call
    ((Box2) ref aabb).\u002Ector(localCircle.Position.X - localCircle.Radius, localCircle.Position.Y - localCircle.Radius, localCircle.Position.X + localCircle.Radius, localCircle.Position.Y + localCircle.Radius);
    SharedMapSystem.TilesEnumerator tileEnumerator = this.GetLocalTilesEnumerator(uid, grid, aabb, ignoreEmpty, predicate);
    TileRef tile;
    while (tileEnumerator.MoveNext(out tile))
    {
      if (Vector2Helpers.IsShorterThanOrEqualTo(Vector2i.op_Implicit(tile.GridIndices) + grid.TileSizeHalfVector - localCircle.Position, localCircle.Radius))
        yield return tile;
    }
  }

  public IEnumerable<TileRef> GetTilesIntersecting(
    EntityUid uid,
    MapGridComponent grid,
    Circle worldArea,
    bool ignoreEmpty = true,
    Predicate<TileRef>? predicate = null)
  {
    SharedMapSystem sharedMapSystem = this;
    Box2 worldArea1;
    // ISSUE: explicit constructor call
    ((Box2) ref worldArea1).\u002Ector(worldArea.Position.X - worldArea.Radius, worldArea.Position.Y - worldArea.Radius, worldArea.Position.X + worldArea.Radius, worldArea.Position.Y + worldArea.Radius);
    EntityCoordinates circleGridPos = new EntityCoordinates(uid, sharedMapSystem.WorldToLocal(uid, grid, worldArea.Position));
    foreach (TileRef tileRef in sharedMapSystem.GetTilesIntersecting(uid, grid, worldArea1, ignoreEmpty, predicate))
    {
      float distance;
      if (sharedMapSystem.GridTileToLocal(uid, grid, tileRef.GridIndices).TryDistance((IEntityManager) sharedMapSystem.EntityManager, sharedMapSystem._transform, circleGridPos, out distance) && (double) distance <= (double) worldArea.Radius)
        yield return tileRef;
    }
  }

  private bool TryGetTile(
    EntityUid uid,
    MapGridComponent grid,
    Vector2i indices,
    bool ignoreEmpty,
    [NotNullWhen(true)] out TileRef? tileRef,
    Predicate<TileRef>? predicate = null)
  {
    Vector2i chunkIndices = this.GridTileToChunkIndices(uid, grid, indices);
    MapChunk mapChunk;
    if (grid.Chunks.TryGetValue(chunkIndices, out mapChunk))
    {
      Vector2i chunkTile = mapChunk.GridTileToChunkTile(indices);
      TileRef tileRef1 = this.GetTileRef(uid, grid, mapChunk, (ushort) chunkTile.X, (ushort) chunkTile.Y);
      if (ignoreEmpty && tileRef1.Tile.IsEmpty)
      {
        tileRef = new TileRef?();
        return false;
      }
      if (predicate == null || predicate(tileRef1))
      {
        tileRef = new TileRef?(tileRef1);
        return true;
      }
    }
    else if (!ignoreEmpty)
    {
      TileRef tileRef2 = new TileRef(uid, indices.X, indices.Y, Tile.Empty);
      if (predicate == null || predicate(tileRef2))
      {
        tileRef = new TileRef?(tileRef2);
        return true;
      }
    }
    tileRef = new TileRef?();
    return false;
  }

  internal MapChunk GetOrAddChunk(EntityUid uid, MapGridComponent grid, int xIndex, int yIndex)
  {
    return this.GetOrAddChunk(uid, grid, new Vector2i(xIndex, yIndex));
  }

  internal bool TryGetChunk(
    EntityUid uid,
    MapGridComponent grid,
    Vector2i chunkIndices,
    [NotNullWhen(true)] out MapChunk? chunk)
  {
    return grid.Chunks.TryGetValue(chunkIndices, out chunk);
  }

  internal MapChunk GetOrAddChunk(EntityUid uid, MapGridComponent grid, Vector2i chunkIndices)
  {
    MapChunk orAddChunk;
    if (grid.Chunks.TryGetValue(chunkIndices, out orAddChunk))
      return orAddChunk;
    MapChunk mapChunk = new MapChunk(chunkIndices.X, chunkIndices.Y, grid.ChunkSize)
    {
      LastTileModifiedTick = this._timing.CurTick
    };
    return grid.Chunks[chunkIndices] = mapChunk;
  }

  public bool HasChunk(EntityUid uid, MapGridComponent grid, Vector2i chunkIndices)
  {
    return grid.Chunks.ContainsKey(chunkIndices);
  }

  internal IReadOnlyDictionary<Vector2i, MapChunk> GetMapChunks(
    EntityUid uid,
    MapGridComponent grid)
  {
    return (IReadOnlyDictionary<Vector2i, MapChunk>) grid.Chunks;
  }

  internal ChunkEnumerator GetMapChunks(EntityUid uid, MapGridComponent grid, Box2 worldAABB)
  {
    Box2 localAABB = Matrix3Helpers.TransformBox(this._transform.GetInvWorldMatrix(uid), ref worldAABB);
    return this.GetLocalMapChunks(uid, grid, localAABB);
  }

  internal ChunkEnumerator GetMapChunks(
    EntityUid uid,
    MapGridComponent grid,
    Box2Rotated worldArea)
  {
    Box2 localAABB = Matrix3Helpers.TransformBox(this._transform.GetInvWorldMatrix(uid), ref worldArea);
    return this.GetLocalMapChunks(uid, grid, localAABB);
  }

  internal ChunkEnumerator GetLocalMapChunks(EntityUid uid, MapGridComponent grid, Box2 localAABB)
  {
    Box2 localAABB1;
    if (this._mapQuery.HasComponent(uid))
    {
      localAABB1 = localAABB;
    }
    else
    {
      Box2 localAabb = grid.LocalAABB;
      localAABB1 = ((Box2) ref localAabb).Intersect(ref localAABB);
    }
    return new ChunkEnumerator(grid.Chunks, localAABB1, (int) grid.ChunkSize);
  }

  public int AnchoredEntityCount(EntityUid uid, MapGridComponent grid, Vector2i pos)
  {
    Vector2i chunkIndices = this.GridTileToChunkIndices(uid, grid, pos);
    MapChunk mapChunk;
    if (!grid.Chunks.TryGetValue(chunkIndices, out mapChunk))
      return 0;
    Vector2i chunkTile = mapChunk.GridTileToChunkTile(pos);
    int num1;
    int num2;
    ((Vector2i) ref chunkTile).Deconstruct(ref num1, ref num2);
    int xCell = num1;
    int yCell = num2;
    List<EntityUid> snapGrid = mapChunk.GetSnapGrid((ushort) xCell, (ushort) yCell);
    // ISSUE: explicit non-virtual call
    return snapGrid == null ? 0 : __nonvirtual (snapGrid.Count);
  }

  public IEnumerable<EntityUid> GetAnchoredEntities(
    Entity<MapGridComponent> grid,
    MapCoordinates coords)
  {
    return this.GetAnchoredEntities(grid.Owner, grid.Comp, coords);
  }

  public IEnumerable<EntityUid> GetAnchoredEntities(
    EntityUid uid,
    MapGridComponent grid,
    MapCoordinates coords)
  {
    return this.GetAnchoredEntities(uid, grid, this.TileIndicesFor(uid, grid, coords));
  }

  public IEnumerable<EntityUid> GetAnchoredEntities(
    Entity<MapGridComponent> grid,
    EntityCoordinates coords)
  {
    return this.GetAnchoredEntities(grid.Owner, grid.Comp, coords);
  }

  public IEnumerable<EntityUid> GetAnchoredEntities(
    EntityUid uid,
    MapGridComponent grid,
    EntityCoordinates coords)
  {
    return this.GetAnchoredEntities(uid, grid, this.TileIndicesFor(uid, grid, coords));
  }

  public IEnumerable<EntityUid> GetAnchoredEntities(Entity<MapGridComponent> grid, Vector2i pos)
  {
    return this.GetAnchoredEntities(grid.Owner, grid.Comp, pos);
  }

  public IEnumerable<EntityUid> GetAnchoredEntities(
    EntityUid uid,
    MapGridComponent grid,
    Vector2i pos)
  {
    Vector2i chunkIndices = this.GridTileToChunkIndices(uid, grid, pos);
    MapChunk mapChunk;
    if (!grid.Chunks.TryGetValue(chunkIndices, out mapChunk))
      return Enumerable.Empty<EntityUid>();
    Vector2i chunkTile = mapChunk.GridTileToChunkTile(pos);
    return mapChunk.GetSnapGridCell((ushort) chunkTile.X, (ushort) chunkTile.Y);
  }

  public void GetAnchoredEntities(
    Entity<MapGridComponent> grid,
    Vector2i pos,
    List<EntityUid> list)
  {
    Vector2i chunkIndices = this.GridTileToChunkIndices(grid.Owner, grid.Comp, pos);
    MapChunk mapChunk;
    if (!grid.Comp.Chunks.TryGetValue(chunkIndices, out mapChunk))
      return;
    Vector2i chunkTile = mapChunk.GridTileToChunkTile(pos);
    List<EntityUid> snapGrid = mapChunk.GetSnapGrid((ushort) chunkTile.X, (ushort) chunkTile.Y);
    if (snapGrid == null)
      return;
    list.AddRange((IEnumerable<EntityUid>) snapGrid);
  }

  public AnchoredEntitiesEnumerator GetAnchoredEntitiesEnumerator(
    EntityUid uid,
    MapGridComponent grid,
    Vector2i pos)
  {
    Vector2i chunkIndices = this.GridTileToChunkIndices(uid, grid, pos);
    MapChunk mapChunk;
    if (!grid.Chunks.TryGetValue(chunkIndices, out mapChunk))
      return AnchoredEntitiesEnumerator.Empty;
    Vector2i chunkTile = mapChunk.GridTileToChunkTile(pos);
    List<EntityUid> snapGrid = mapChunk.GetSnapGrid((ushort) chunkTile.X, (ushort) chunkTile.Y);
    return snapGrid != null ? new AnchoredEntitiesEnumerator(snapGrid.GetEnumerator()) : AnchoredEntitiesEnumerator.Empty;
  }

  public IEnumerable<EntityUid> GetLocalAnchoredEntities(
    EntityUid uid,
    MapGridComponent grid,
    Box2 localAABB)
  {
    SharedMapSystem mapSystem = this;
    SharedMapSystem.TilesEnumerator enumerator = new SharedMapSystem.TilesEnumerator(mapSystem, true, (Predicate<TileRef>) null, uid, grid, localAABB);
    TileRef tile;
    while (enumerator.MoveNext(out tile))
    {
      AnchoredEntitiesEnumerator anchoredEnumerator = mapSystem.GetAnchoredEntitiesEnumerator(uid, grid, tile.GridIndices);
      EntityUid? uid1;
      while (anchoredEnumerator.MoveNext(out uid1))
        yield return uid1.Value;
      anchoredEnumerator = new AnchoredEntitiesEnumerator();
    }
  }

  public IEnumerable<EntityUid> GetAnchoredEntities(
    EntityUid uid,
    MapGridComponent grid,
    Box2 worldAABB)
  {
    SharedMapSystem mapSystem = this;
    Box2 aabb = Matrix3Helpers.TransformBox(mapSystem._transform.GetInvWorldMatrix(uid), ref worldAABB);
    SharedMapSystem.TilesEnumerator enumerator = new SharedMapSystem.TilesEnumerator(mapSystem, true, (Predicate<TileRef>) null, uid, grid, aabb);
    TileRef tile;
    while (enumerator.MoveNext(out tile))
    {
      AnchoredEntitiesEnumerator anchoredEnumerator = mapSystem.GetAnchoredEntitiesEnumerator(uid, grid, tile.GridIndices);
      EntityUid? uid1;
      while (anchoredEnumerator.MoveNext(out uid1))
        yield return uid1.Value;
      anchoredEnumerator = new AnchoredEntitiesEnumerator();
    }
  }

  public IEnumerable<EntityUid> GetAnchoredEntities(
    EntityUid uid,
    MapGridComponent grid,
    Box2Rotated worldBounds)
  {
    foreach (TileRef tileRef in this.GetTilesIntersecting(uid, grid, worldBounds))
    {
      IEnumerator<EntityUid> enumerator = this.GetAnchoredEntities(uid, grid, tileRef.GridIndices).GetEnumerator();
      while (enumerator.MoveNext())
        yield return enumerator.Current;
      enumerator = (IEnumerator<EntityUid>) null;
    }
  }

  public Vector2i TileIndicesFor(EntityUid uid, MapGridComponent grid, EntityCoordinates coords)
  {
    return this.SnapGridLocalCellFor(uid, grid, this.LocalToGrid(uid, grid, coords));
  }

  public Vector2i TileIndicesFor(Entity<MapGridComponent> grid, EntityCoordinates coords)
  {
    return this.TileIndicesFor(grid.Owner, grid.Comp, coords);
  }

  public Vector2i TileIndicesFor(EntityUid uid, MapGridComponent grid, MapCoordinates worldPos)
  {
    Vector2 local = this.WorldToLocal(uid, grid, worldPos.Position);
    return this.SnapGridLocalCellFor(uid, grid, local);
  }

  public Vector2i TileIndicesFor(Entity<MapGridComponent> grid, MapCoordinates coords)
  {
    return this.TileIndicesFor(grid.Owner, grid.Comp, coords);
  }

  private Vector2i SnapGridLocalCellFor(EntityUid uid, MapGridComponent grid, Vector2 localPos)
  {
    return new Vector2i((int) Math.Floor((double) localPos.X / (double) grid.TileSize), (int) Math.Floor((double) localPos.Y / (double) grid.TileSize));
  }

  public bool IsAnchored(
    EntityUid uid,
    MapGridComponent grid,
    EntityCoordinates coords,
    EntityUid euid)
  {
    Vector2i pos = this.TileIndicesFor(uid, grid, coords);
    MapChunk chunk;
    Vector2i offset;
    if (!this.TryChunkAndOffsetForTile(uid, grid, pos, out chunk, out offset))
      return false;
    List<EntityUid> snapGrid = chunk.GetSnapGrid((ushort) offset.X, (ushort) offset.Y);
    // ISSUE: explicit non-virtual call
    return snapGrid != null && __nonvirtual (snapGrid.Contains(euid));
  }

  public bool AddToSnapGridCell(
    EntityUid gridUid,
    MapGridComponent grid,
    Vector2i pos,
    EntityUid euid)
  {
    MapChunk chunk;
    Vector2i offset;
    if (!this.TryChunkAndOffsetForTile(gridUid, grid, pos, out chunk, out offset) || chunk.GetTile((ushort) offset.X, (ushort) offset.Y).IsEmpty)
      return false;
    chunk.AddToSnapGridCell((ushort) offset.X, (ushort) offset.Y, euid);
    return true;
  }

  public bool AddToSnapGridCell(
    EntityUid gridUid,
    MapGridComponent grid,
    EntityCoordinates coords,
    EntityUid euid)
  {
    return this.AddToSnapGridCell(gridUid, grid, this.TileIndicesFor(gridUid, grid, coords), euid);
  }

  public void RemoveFromSnapGridCell(
    EntityUid gridUid,
    MapGridComponent grid,
    Vector2i pos,
    EntityUid euid)
  {
    Vector2i chunkIndices = this.GridTileToChunkIndices(gridUid, grid, pos);
    MapChunk mapChunk;
    if (!grid.Chunks.TryGetValue(chunkIndices, out mapChunk))
      return;
    Vector2i chunkTile = mapChunk.GridTileToChunkTile(pos);
    mapChunk.RemoveFromSnapGridCell((ushort) chunkTile.X, (ushort) chunkTile.Y, euid);
  }

  public void RemoveFromSnapGridCell(
    EntityUid gridUid,
    MapGridComponent grid,
    EntityCoordinates coords,
    EntityUid euid)
  {
    this.RemoveFromSnapGridCell(gridUid, grid, this.TileIndicesFor(gridUid, grid, coords), euid);
  }

  private bool TryChunkAndOffsetForTile(
    EntityUid uid,
    MapGridComponent grid,
    Vector2i pos,
    [NotNullWhen(true)] out MapChunk? chunk,
    out Vector2i offset)
  {
    Vector2i chunkIndices = this.GridTileToChunkIndices(uid, grid, pos);
    if (!grid.Chunks.TryGetValue(chunkIndices, out chunk))
    {
      offset = new Vector2i();
      return false;
    }
    offset = chunk.GridTileToChunkTile(pos);
    return true;
  }

  public IEnumerable<EntityUid> GetInDir(
    EntityUid uid,
    MapGridComponent grid,
    EntityCoordinates position,
    Direction dir)
  {
    Vector2i direction = SharedMapSystem.GetDirection(this.TileIndicesFor(uid, grid, position), dir);
    return this.GetAnchoredEntities(uid, grid, direction);
  }

  public IEnumerable<EntityUid> GetOffset(
    EntityUid uid,
    MapGridComponent grid,
    EntityCoordinates coords,
    Vector2i offset)
  {
    Vector2i pos = Vector2i.op_Addition(this.TileIndicesFor(uid, grid, coords), offset);
    return this.GetAnchoredEntities(uid, grid, pos);
  }

  public IEnumerable<EntityUid> GetLocal(
    EntityUid uid,
    MapGridComponent grid,
    EntityCoordinates coords)
  {
    return this.GetAnchoredEntities(uid, grid, this.TileIndicesFor(uid, grid, coords));
  }

  public EntityCoordinates DirectionToGrid(
    EntityUid uid,
    MapGridComponent grid,
    EntityCoordinates coords,
    Direction direction)
  {
    return this.GridTileToLocal(uid, grid, SharedMapSystem.GetDirection(this.TileIndicesFor(uid, grid, coords), direction));
  }

  public IEnumerable<EntityUid> GetCardinalNeighborCells(
    EntityUid uid,
    MapGridComponent grid,
    EntityCoordinates coords)
  {
    Vector2i position = this.TileIndicesFor(uid, grid, coords);
    foreach (EntityUid anchoredEntity in this.GetAnchoredEntities(uid, grid, position))
      yield return anchoredEntity;
    foreach (EntityUid anchoredEntity in this.GetAnchoredEntities(uid, grid, Vector2i.op_Addition(position, new Vector2i(0, 1))))
      yield return anchoredEntity;
    foreach (EntityUid anchoredEntity in this.GetAnchoredEntities(uid, grid, Vector2i.op_Addition(position, new Vector2i(0, -1))))
      yield return anchoredEntity;
    foreach (EntityUid anchoredEntity in this.GetAnchoredEntities(uid, grid, Vector2i.op_Addition(position, new Vector2i(1, 0))))
      yield return anchoredEntity;
    foreach (EntityUid anchoredEntity in this.GetAnchoredEntities(uid, grid, Vector2i.op_Addition(position, new Vector2i(-1, 0))))
      yield return anchoredEntity;
  }

  public IEnumerable<EntityUid> GetCellsInSquareArea(
    EntityUid uid,
    MapGridComponent grid,
    EntityCoordinates coords,
    int n)
  {
    Vector2i position = this.TileIndicesFor(uid, grid, coords);
    for (int y = -n; y <= n; ++y)
    {
      for (int x = -n; x <= n; ++x)
      {
        AnchoredEntitiesEnumerator enumerator = this.GetAnchoredEntitiesEnumerator(uid, grid, Vector2i.op_Addition(position, new Vector2i(x, y)));
        EntityUid? uid1;
        while (enumerator.MoveNext(out uid1))
          yield return uid1.Value;
        enumerator = new AnchoredEntitiesEnumerator();
      }
    }
  }

  public Vector2 WorldToLocal(EntityUid uid, MapGridComponent grid, Vector2 posWorld)
  {
    Matrix3x2 invWorldMatrix = this._transform.GetInvWorldMatrix(uid);
    return Vector2.Transform(posWorld, invWorldMatrix);
  }

  public EntityCoordinates MapToGrid(EntityUid uid, MapCoordinates posWorld)
  {
    MapId mapId = this._xformQuery.GetComponent(uid).MapID;
    if (posWorld.MapId != mapId)
      throw new ArgumentException($"Grid {uid} is on map {mapId}, but coords are on map {posWorld.MapId}.", nameof (posWorld));
    MapGridComponent component;
    return !this._gridQuery.TryGetComponent(uid, out component) ? new EntityCoordinates(this.GetMapOrInvalid(new MapId?(posWorld.MapId)), new Vector2(posWorld.X, posWorld.Y)) : new EntityCoordinates(uid, this.WorldToLocal(uid, component, posWorld.Position));
  }

  public Vector2 LocalToWorld(EntityUid uid, MapGridComponent grid, Vector2 posLocal)
  {
    Matrix3x2 worldMatrix = this._transform.GetWorldMatrix(uid);
    return Vector2.Transform(posLocal, worldMatrix);
  }

  public Vector2i WorldToTile(EntityUid uid, MapGridComponent grid, Vector2 posWorld)
  {
    Vector2 local = this.WorldToLocal(uid, grid, posWorld);
    return new Vector2i((int) Math.Floor((double) local.X / (double) grid.TileSize), (int) Math.Floor((double) local.Y / (double) grid.TileSize));
  }

  public Vector2i LocalToTile(EntityUid uid, MapGridComponent grid, EntityCoordinates coordinates)
  {
    Vector2 grid1 = this.LocalToGrid(uid, grid, coordinates);
    return new Vector2i((int) Math.Floor((double) grid1.X / (double) grid.TileSize), (int) Math.Floor((double) grid1.Y / (double) grid.TileSize));
  }

  public Vector2i CoordinatesToTile(EntityUid uid, MapGridComponent grid, MapCoordinates coords)
  {
    Vector2 local = this.WorldToLocal(uid, grid, coords.Position);
    return new Vector2i((int) Math.Floor((double) local.X / (double) grid.TileSize), (int) Math.Floor((double) local.Y / (double) grid.TileSize));
  }

  public Vector2i CoordinatesToTile(EntityUid uid, MapGridComponent grid, EntityCoordinates coords)
  {
    Vector2 grid1 = this.LocalToGrid(uid, grid, coords);
    return new Vector2i((int) Math.Floor((double) grid1.X / (double) grid.TileSize), (int) Math.Floor((double) grid1.Y / (double) grid.TileSize));
  }

  public Vector2i LocalToChunkIndices(
    EntityUid uid,
    MapGridComponent grid,
    EntityCoordinates gridPos)
  {
    Vector2 grid1 = this.LocalToGrid(uid, grid, gridPos);
    return new Vector2i((int) Math.Floor((double) grid1.X / (double) ((int) grid.TileSize * (int) grid.ChunkSize)), (int) Math.Floor((double) grid1.Y / (double) ((int) grid.TileSize * (int) grid.ChunkSize)));
  }

  public Vector2 LocalToGrid(EntityUid uid, MapGridComponent grid, EntityCoordinates position)
  {
    return !(position.EntityId == uid) ? this.WorldToLocal(uid, grid, this._transform.ToMapCoordinates(position).Position) : position.Position;
  }

  public bool CollidesWithGrid(EntityUid uid, MapGridComponent grid, Vector2i indices)
  {
    Vector2i chunkIndices = this.GridTileToChunkIndices(uid, grid, indices);
    MapChunk mapChunk;
    if (!grid.Chunks.TryGetValue(chunkIndices, out mapChunk))
      return false;
    Vector2i chunkTile = mapChunk.GridTileToChunkTile(indices);
    return mapChunk.GetTile((ushort) chunkTile.X, (ushort) chunkTile.Y).TypeId != Tile.Empty.TypeId;
  }

  public Vector2i GridTileToChunkIndices(EntityUid uid, MapGridComponent grid, Vector2i gridTile)
  {
    return this.GridTileToChunkIndices(grid, gridTile);
  }

  public Vector2i GridTileToChunkIndices(MapGridComponent grid, Vector2i gridTile)
  {
    return new Vector2i((int) Math.Floor((double) gridTile.X / (double) grid.ChunkSize), (int) Math.Floor((double) gridTile.Y / (double) grid.ChunkSize));
  }

  public EntityCoordinates GridTileToLocal(EntityUid uid, MapGridComponent grid, Vector2i gridTile)
  {
    Vector2 vector = this.TileCenterToVector(uid, grid, gridTile);
    return new EntityCoordinates(uid, vector);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Vector2 TileToVector(Entity<MapGridComponent> grid, Vector2i gridTile)
  {
    return new Vector2((float) (gridTile.X * (int) grid.Comp.TileSize), (float) (gridTile.Y * (int) grid.Comp.TileSize));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Vector2 TileCenterToVector(EntityUid uid, MapGridComponent grid, Vector2i gridTile)
  {
    return this.TileCenterToVector((Entity<MapGridComponent>) (uid, grid), gridTile);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Vector2 TileCenterToVector(Entity<MapGridComponent> grid, Vector2i gridTile)
  {
    return new Vector2((float) (gridTile.X * (int) grid.Comp.TileSize), (float) (gridTile.Y * (int) grid.Comp.TileSize)) + grid.Comp.TileSizeHalfVector;
  }

  public Vector2 GridTileToWorldPos(EntityUid uid, MapGridComponent grid, Vector2i gridTile)
  {
    return Vector2.Transform(new Vector2((float) (gridTile.X * (int) grid.TileSize) + (float) grid.TileSize / 2f, (float) (gridTile.Y * (int) grid.TileSize) + (float) grid.TileSize / 2f), this._transform.GetWorldMatrix(uid));
  }

  public MapCoordinates GridTileToWorld(EntityUid uid, MapGridComponent grid, Vector2i gridTile)
  {
    MapId mapId = this._xformQuery.GetComponent(uid).MapID;
    return new MapCoordinates(this.GridTileToWorldPos(uid, grid, gridTile), mapId);
  }

  public bool TryGetTileRef(
    EntityUid uid,
    MapGridComponent grid,
    Vector2i indices,
    out TileRef tile)
  {
    Vector2i chunkIndices = this.GridTileToChunkIndices(uid, grid, indices);
    MapChunk mapChunk;
    if (!grid.Chunks.TryGetValue(chunkIndices, out mapChunk))
    {
      tile = new TileRef();
      return false;
    }
    Vector2i chunkTile = mapChunk.GridTileToChunkTile(indices);
    tile = this.GetTileRef(uid, grid, mapChunk, (ushort) chunkTile.X, (ushort) chunkTile.Y);
    return true;
  }

  public bool TryGetTile(MapGridComponent grid, Vector2i indices, out Tile tile)
  {
    Vector2i chunkIndices = this.GridTileToChunkIndices(grid, indices);
    MapChunk mapChunk;
    if (!grid.Chunks.TryGetValue(chunkIndices, out mapChunk))
    {
      tile = new Tile();
      return false;
    }
    Vector2i chunkTile = mapChunk.GridTileToChunkTile(indices);
    tile = mapChunk.Tiles[chunkTile.X, chunkTile.Y];
    return true;
  }

  public bool TryGetTileDef(MapGridComponent grid, Vector2i indices, [NotNullWhen(true)] out ITileDefinition? tileDef)
  {
    Tile tile;
    if (!this.TryGetTile(grid, indices, out tile))
    {
      tileDef = (ITileDefinition) null;
      return false;
    }
    tileDef = this._tileMan[tile.TypeId];
    return true;
  }

  public bool TryGetTileRef(
    EntityUid uid,
    MapGridComponent grid,
    EntityCoordinates coords,
    out TileRef tile)
  {
    return this.TryGetTileRef(uid, grid, this.CoordinatesToTile(uid, grid, coords), out tile);
  }

  public bool TryGetTileRef(
    EntityUid uid,
    MapGridComponent grid,
    Vector2 worldPos,
    out TileRef tile)
  {
    return this.TryGetTileRef(uid, grid, this.WorldToTile(uid, grid, worldPos), out tile);
  }

  internal Box2 CalcWorldAABB(EntityUid uid, MapGridComponent grid, MapChunk mapChunk)
  {
    (Vector2 WorldPosition, Angle WorldRotation) = this._transform.GetWorldPositionRotation(uid);
    Vector2i indices = mapChunk.Indices;
    ushort tileSize = grid.TileSize;
    ushort chunkSize = mapChunk.ChunkSize;
    Vector2 vector2_1 = WorldPosition;
    ref Angle local1 = ref WorldRotation;
    Vector2 vector2_2 = Vector2i.op_Implicit(Vector2i.op_Multiply(Vector2i.op_Multiply(indices, (int) tileSize), (int) chunkSize));
    ref Vector2 local2 = ref vector2_2;
    Vector2 vector2_3 = ((Angle) ref local1).RotateVec(ref local2);
    Vector2 vector2_4 = vector2_1 + vector2_3;
    Box2i cachedBounds = mapChunk.CachedBounds;
    Box2 box2 = Box2i.op_Implicit(((Box2i) ref cachedBounds).Scale((int) tileSize));
    Box2Rotated box2Rotated = new Box2Rotated(((Box2) ref box2).Translated(vector2_4), WorldRotation, vector2_4);
    return ((Box2Rotated) ref box2Rotated).CalcBoundingBox();
  }

  private void OnTileModified(
    EntityUid uid,
    MapGridComponent grid,
    MapChunk mapChunk,
    Vector2i tileIndices,
    Tile newTile,
    Tile oldTile,
    bool shapeChanged)
  {
    Vector2i gridTile = mapChunk.ChunkTileToGridTile(tileIndices);
    mapChunk.LastTileModifiedTick = this._timing.CurTick;
    grid.LastTileModifiedTick = this._timing.CurTick;
    this.Dirty(uid, (IComponent) grid);
    if (!this.MapManager.SuppressOnTileChanged)
    {
      TileRef tileRef = new TileRef(uid, gridTile, newTile);
      this._mapInternal.RaiseOnTileChanged((Entity<MapGridComponent>) (uid, grid), tileRef, oldTile, mapChunk.Indices);
    }
    if (!shapeChanged || mapChunk.SuppressCollisionRegeneration)
      return;
    this.RegenerateCollision(uid, grid, mapChunk);
  }

  internal bool SetChunkTile(
    EntityUid uid,
    MapGridComponent grid,
    MapChunk chunk,
    ushort xIndex,
    ushort yIndex,
    Tile tile,
    out Tile oldTile)
  {
    bool shapeChanged;
    if (!chunk.TrySetTile(xIndex, yIndex, tile, out oldTile, out shapeChanged))
      return false;
    Vector2i tileIndices;
    // ISSUE: explicit constructor call
    ((Vector2i) ref tileIndices).\u002Ector((int) xIndex, (int) yIndex);
    this.OnTileModified(uid, grid, chunk, tileIndices, tile, oldTile, shapeChanged);
    return true;
  }

  public void SetAmbientLight(MapId mapId, Color color)
  {
    EntityUid map = this.GetMap(mapId);
    MapLightComponent mapLightComponent = this.EnsureComp<MapLightComponent>(map);
    if (((Color) ref mapLightComponent.AmbientLightColor).Equals(color))
      return;
    mapLightComponent.AmbientLightColor = color;
    this.Dirty(map, (IComponent) mapLightComponent);
  }

  private void OnMapLightGetState(
    EntityUid uid,
    MapLightComponent component,
    ref ComponentGetState args)
  {
    args.State = (IComponentState) new SharedMapSystem.MapLightComponentState()
    {
      AmbientLightColor = component.AmbientLightColor
    };
  }

  private void OnMapLightHandleState(
    EntityUid uid,
    MapLightComponent component,
    ref ComponentHandleState args)
  {
    if (!(args.Current is SharedMapSystem.MapLightComponentState current))
      return;
    component.AmbientLightColor = current.AmbientLightColor;
  }

  private void InitializeMap()
  {
    this.SubscribeLocalEvent<MapComponent, ComponentAdd>(new ComponentEventHandler<MapComponent, ComponentAdd>(this.OnComponentAdd));
    this.SubscribeLocalEvent<MapComponent, ComponentInit>(new EntityEventRefHandler<MapComponent, ComponentInit>(this.OnCompInit));
    this.SubscribeLocalEvent<MapComponent, ComponentStartup>(new ComponentEventHandler<MapComponent, ComponentStartup>(this.OnCompStartup));
    this.SubscribeLocalEvent<MapComponent, MapInitEvent>(new ComponentEventHandler<MapComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<MapComponent, ComponentShutdown>(new ComponentEventHandler<MapComponent, ComponentShutdown>(this.OnMapRemoved));
    this.SubscribeLocalEvent<MapComponent, ComponentHandleState>(new ComponentEventRefHandler<MapComponent, ComponentHandleState>(this.OnMapHandleState));
    this.SubscribeLocalEvent<MapComponent, ComponentGetState>(new ComponentEventRefHandler<MapComponent, ComponentGetState>(this.OnMapGetState));
  }

  public bool MapExists([NotNullWhen(true)] MapId? mapId)
  {
    return mapId.HasValue && this.Maps.ContainsKey(mapId.Value);
  }

  public EntityUid GetMap(MapId mapId) => this.Maps[mapId];

  public EntityUid GetMapOrInvalid(MapId? mapId)
  {
    EntityUid? uid;
    return this.TryGetMap(mapId, out uid) ? uid.Value : EntityUid.Invalid;
  }

  public bool TryGetMap([NotNullWhen(true)] MapId? mapId, [NotNullWhen(true)] out EntityUid? uid)
  {
    EntityUid entityUid;
    if (!mapId.HasValue || !this.Maps.TryGetValue(mapId.Value, out entityUid))
    {
      uid = new EntityUid?();
      return false;
    }
    uid = new EntityUid?(entityUid);
    return true;
  }

  private void OnMapHandleState(
    EntityUid uid,
    MapComponent component,
    ref ComponentHandleState args)
  {
    if (!(args.Current is MapComponentState current))
      return;
    if (component.MapId == MapId.Nullspace)
    {
      if (current.MapId == MapId.Nullspace)
        throw new Exception($"Received invalid map state for {this.ToPrettyString((Entity<MetaDataComponent>) uid)}");
      this.AssignMapId((Entity<MapComponent>) (uid, component), new MapId?(current.MapId));
      this.RecursiveMapIdUpdate(uid, uid, component.MapId);
    }
    if (component.MapId != current.MapId)
      throw new Exception($"Received invalid map state for {this.ToPrettyString((Entity<MetaDataComponent>) uid)}");
    component.LightingEnabled = current.LightingEnabled;
    component.MapInitialized = current.Initialized;
    if (this.LifeStage(uid) >= EntityLifeStage.Initialized)
      this.SetPaused((Entity<MapComponent>) uid, current.MapPaused);
    else
      component.MapPaused = current.MapPaused;
  }

  private void RecursiveMapIdUpdate(EntityUid uid, EntityUid mapUid, MapId mapId)
  {
    TransformComponent transformComponent = this.Transform(uid);
    transformComponent.MapUid = new EntityUid?(mapUid);
    transformComponent.MapID = mapId;
    transformComponent._mapIdInitialized = true;
    foreach (EntityUid child in transformComponent._children)
      this.RecursiveMapIdUpdate(child, mapUid, mapId);
  }

  private void OnMapGetState(EntityUid uid, MapComponent component, ref ComponentGetState args)
  {
    args.State = (IComponentState) new MapComponentState(component.MapId, component.LightingEnabled, component.MapPaused, component.MapInitialized);
  }

  protected MapId TakeNextMapId()
  {
    MapId nextMapId = this.GetNextMapId();
    this.LastMapId = nextMapId.Value;
    return nextMapId;
  }

  internal abstract MapId GetNextMapId();

  private void OnComponentAdd(EntityUid uid, MapComponent component, ComponentAdd args)
  {
    this.EnsureComp<GridTreeComponent>(uid);
  }

  internal MapId AllocateMapId(EntityUid ent)
  {
    MapId key = this._reserved[ent] = this.TakeNextMapId();
    this.Maps.Add(key, ent);
    this.UsedIds.Add(key);
    return key;
  }

  internal void AssignMapId(Entity<MapComponent> map, MapId? id = null)
  {
    if (map.Comp.MapId != MapId.Nullspace)
    {
      if (id.HasValue)
      {
        MapId mapId = map.Comp.MapId;
        MapId? nullable = id;
        if ((nullable.HasValue ? (mapId != nullable.GetValueOrDefault() ? 1 : 0) : 1) != 0)
        {
          this.QueueDel(new EntityUid?(map.Owner));
          throw new Exception($"Map entity {this.ToPrettyString((Entity<MetaDataComponent>) map.Owner)} has already been assigned an id");
        }
      }
      EntityUid entityUid;
      if (!this.Maps.TryGetValue(map.Comp.MapId, out entityUid) || entityUid != map.Owner)
      {
        this.QueueDel(new EntityUid?(map.Owner));
        throw new Exception($"Map entity {this.ToPrettyString((Entity<MetaDataComponent>) map.Owner)} was improperly assigned a map id?");
      }
    }
    else
    {
      MapId mapId;
      if (this._reserved.TryGetValue(map.Owner, out mapId))
      {
        map.Comp.MapId = mapId;
      }
      else
      {
        map.Comp.MapId = id ?? this.TakeNextMapId();
        if (this.IsClientSide((EntityUid) map) != map.Comp.MapId.IsClientSide)
          throw new Exception("Attempting to assign a client-side map id to a networked entity or vice-versa");
        if (!this.UsedIds.Add(map.Comp.MapId))
          this.Log.Warning($"Re-using a previously used map id ({map.Comp.MapId}) for map entity {this.ToPrettyString(new EntityUid?((EntityUid) map))}");
        if (!this.Maps.TryAdd(map.Comp.MapId, map.Owner) && !(this.Maps[map.Comp.MapId] == map.Owner))
        {
          this.QueueDel(new EntityUid?((EntityUid) map));
          throw new Exception($"Attempted to assign an existing mapId {map.Comp} to a map entity {this.ToPrettyString((Entity<MetaDataComponent>) map.Owner)}");
        }
      }
    }
  }

  private void OnCompInit(Entity<MapComponent> map, ref ComponentInit args)
  {
    this.AssignMapId(map);
    MapChangedEvent args1 = new MapChangedEvent((EntityUid) map, map.Comp.MapId, true);
    this.RaiseLocalEvent<MapChangedEvent>((EntityUid) map, args1, true);
    MapCreatedEvent args2 = new MapCreatedEvent((EntityUid) map, map.Comp.MapId);
    this.RaiseLocalEvent<MapCreatedEvent>((EntityUid) map, args2, true);
  }

  private void OnMapInit(EntityUid uid, MapComponent component, MapInitEvent args)
  {
    component.MapInitialized = true;
    this.Dirty(uid, (IComponent) component);
  }

  private void OnCompStartup(EntityUid uid, MapComponent component, ComponentStartup args)
  {
    component.MapPaused |= !component.MapInitialized;
    if (!component.MapPaused)
      return;
    component.MapPaused = false;
    this.SetPaused((Entity<MapComponent>) uid, true);
  }

  private void OnMapRemoved(EntityUid uid, MapComponent component, ComponentShutdown args)
  {
    this.Maps.Remove(component.MapId);
    MapChangedEvent args1 = new MapChangedEvent(uid, component.MapId, false);
    this.RaiseLocalEvent<MapChangedEvent>(uid, args1, true);
    MapRemovedEvent args2 = new MapRemovedEvent(uid, component.MapId);
    this.RaiseLocalEvent<MapRemovedEvent>(uid, args2, true);
  }

  public EntityUid CreateMap(out MapId mapId, bool runMapInit = true)
  {
    mapId = this.TakeNextMapId();
    return this.CreateMap(mapId, runMapInit);
  }

  public EntityUid CreateMap(bool runMapInit = true) => this.CreateMap(out MapId _, runMapInit);

  public EntityUid CreateMap(MapId mapId, bool runMapInit = true)
  {
    if (this.Maps.ContainsKey(mapId))
      throw new ArgumentException($"Map with id {mapId} already exists");
    if (mapId == MapId.Nullspace)
      throw new ArgumentException("Cannot create a null-space map");
    if (this._netManager.IsServer && mapId.IsClientSide)
      throw new ArgumentException("Attempted to create a client-side map on the server?");
    if (this._netManager.IsClient && this._netManager.IsConnected && !mapId.IsClientSide)
      throw new ArgumentException("Attempted to create a client-side map entity with a non client-side map ID?");
    if (this.UsedIds.Contains(mapId))
      this.Log.Warning($"Re-using MapId: {mapId}");
    (EntityUid map, MapComponent comp1, MetaDataComponent metaDataComponent) = this.CreateUninitializedMap();
    this.AssignMapId((Entity<MapComponent>) (map, comp1), new MapId?(mapId));
    this.EntityManager.InitializeEntity(map, metaDataComponent);
    this.EntityManager.StartEntity(map);
    if (runMapInit)
      this.InitializeMap((Entity<MapComponent>) (map, comp1));
    else
      this.SetPaused((Entity<MapComponent>) (map, comp1), true);
    return map;
  }

  public Entity<MapComponent, MetaDataComponent> CreateUninitializedMap()
  {
    MetaDataComponent meta;
    EntityUid entityUninitialized = this.EntityManager.CreateEntityUninitialized((string) null, out meta);
    this._meta.SetEntityName(entityUninitialized, "Map Entity", meta);
    return (Entity<MapComponent, MetaDataComponent>) (entityUninitialized, this.AddComp<MapComponent>(entityUninitialized), meta);
  }

  public void DeleteMap(MapId mapId)
  {
    EntityUid? uid;
    if (!this.TryGetMap(new MapId?(mapId), out uid))
      return;
    this.Del(uid);
  }

  public void QueueDeleteMap(MapId mapId)
  {
    EntityUid? uid;
    if (!this.TryGetMap(new MapId?(mapId), out uid))
      return;
    this.QueueDel(uid);
  }

  public IEnumerable<MapId> GetAllMapIds() => (IEnumerable<MapId>) this.Maps.Keys;

  public bool IsInitialized(MapId mapId)
  {
    if (mapId == MapId.Nullspace)
      return true;
    EntityUid map;
    if (!this.Maps.TryGetValue(mapId, out map))
      throw new ArgumentException($"Map {mapId} does not exist.");
    return this.IsInitialized((Entity<MapComponent>) map);
  }

  public bool IsInitialized(EntityUid? map)
  {
    return !map.HasValue || this.IsInitialized((Entity<MapComponent>) map.Value);
  }

  public bool IsInitialized(Entity<MapComponent?> map)
  {
    return this._mapQuery.Resolve((EntityUid) map, ref map.Comp) && map.Comp.MapInitialized;
  }

  public void InitializeMap(MapId mapId, bool unpause = true)
  {
    EntityUid map;
    if (!this.Maps.TryGetValue(mapId, out map))
      throw new ArgumentException($"Map {mapId} does not exist.");
    this.InitializeMap((Entity<MapComponent>) map, unpause);
  }

  public void InitializeMap(Entity<MapComponent?> map, bool unpause = true)
  {
    if (!this._mapQuery.Resolve((EntityUid) map, ref map.Comp))
      return;
    if (map.Comp.MapInitialized)
      throw new ArgumentException($"Map {this.ToPrettyString(new EntityUid?((EntityUid) map))} is already initialized.");
    this.RecursiveMapInit(map.Owner);
    if (!unpause)
      return;
    this.SetPaused(map, false);
  }

  internal void RecursiveMapInit(EntityUid entity)
  {
    List<EntityUid> entityUidList = new List<EntityUid>()
    {
      entity
    };
    for (int index = 0; index < entityUidList.Count; ++index)
    {
      EntityUid entityUid = entityUidList[index];
      MetaDataComponent component;
      if (this._metaQuery.TryComp(entityUid, out component) && component.EntityLifeStage != EntityLifeStage.MapInitialized)
      {
        entityUidList.AddRange((IEnumerable<EntityUid>) this.Transform(entityUid)._children);
        this.EntityManager.RunMapInit(entityUid, component);
      }
    }
  }

  public bool IsPaused(MapId mapId)
  {
    if (mapId == MapId.Nullspace)
      return false;
    EntityUid map;
    if (!this.Maps.TryGetValue(mapId, out map))
      throw new ArgumentException($"Map {mapId} does not exist.");
    return this.IsPaused((Entity<MapComponent>) map);
  }

  public bool IsPaused(Entity<MapComponent?> map)
  {
    if (!this._mapQuery.Resolve((EntityUid) map, ref map.Comp))
      return false;
    return map.Comp.MapPaused || !map.Comp.MapInitialized;
  }

  public void SetPaused(MapId mapId, bool paused)
  {
    EntityUid map;
    if (!this.Maps.TryGetValue(mapId, out map))
      throw new ArgumentException($"Map {mapId} does not exist.");
    this.SetPaused((Entity<MapComponent>) map, paused);
  }

  public void SetPaused(Entity<MapComponent?> map, bool paused)
  {
    if (!this._mapQuery.Resolve((EntityUid) map, ref map.Comp) || map.Comp.MapPaused == paused)
      return;
    map.Comp.MapPaused = paused;
    if (map.Comp.LifeStage < ComponentLifeStage.Initializing)
      return;
    this.Dirty<MapComponent>(map);
    this.RecursiveSetPaused((EntityUid) map, paused);
  }

  internal void RecursiveSetPaused(EntityUid entity, bool paused)
  {
    this._meta.SetEntityPaused(entity, paused);
    foreach (EntityUid child in this.Transform(entity)._children)
      this.RecursiveSetPaused(child, paused);
  }

  public struct TilesEnumerator
  {
    private readonly SharedMapSystem _mapSystem;
    private readonly EntityUid _uid;
    private readonly MapGridComponent _grid;
    private readonly bool _ignoreEmpty;
    private readonly Predicate<TileRef>? _predicate;
    private readonly int _lowerY;
    private readonly int _upperX;
    private readonly int _upperY;
    private int _x;
    private int _y;

    public TilesEnumerator(
      SharedMapSystem mapSystem,
      bool ignoreEmpty,
      Predicate<TileRef>? predicate,
      EntityUid uid,
      MapGridComponent grid,
      Box2 aabb)
    {
      this._mapSystem = mapSystem;
      this._uid = uid;
      this._grid = grid;
      this._ignoreEmpty = ignoreEmpty;
      this._predicate = predicate;
      Vector2i vector2i1;
      // ISSUE: explicit constructor call
      ((Vector2i) ref vector2i1).\u002Ector((int) Math.Floor((double) aabb.Left), (int) Math.Floor((double) aabb.Bottom));
      Vector2i vector2i2;
      // ISSUE: explicit constructor call
      ((Vector2i) ref vector2i2).\u002Ector((int) Math.Ceiling((double) aabb.Right), (int) Math.Ceiling((double) aabb.Top));
      this._x = vector2i1.X;
      this._y = vector2i1.Y;
      this._lowerY = vector2i1.Y;
      this._upperX = vector2i2.X;
      this._upperY = vector2i2.Y;
    }

    public bool MoveNext(out TileRef tile)
    {
      while (this._x < this._upperX)
      {
        Vector2i gridTile;
        // ISSUE: explicit constructor call
        ((Vector2i) ref gridTile).\u002Ector(this._x, this._y);
        ++this._y;
        if (this._y >= this._upperY)
        {
          ++this._x;
          this._y = this._lowerY;
        }
        MapChunk mapChunk;
        if (this._grid.Chunks.TryGetValue(this._mapSystem.GridTileToChunkIndices(this._uid, this._grid, gridTile), out mapChunk))
        {
          Vector2i chunkTile = mapChunk.GridTileToChunkTile(gridTile);
          tile = this._mapSystem.GetTileRef(this._uid, this._grid, mapChunk, (ushort) chunkTile.X, (ushort) chunkTile.Y);
          if ((!this._ignoreEmpty || !tile.Tile.IsEmpty) && (this._predicate == null || this._predicate(tile)))
            return true;
        }
        else if (!this._ignoreEmpty)
        {
          tile = new TileRef(this._uid, gridTile.X, gridTile.Y, Tile.Empty);
          if (this._predicate == null || this._predicate(tile))
            return true;
        }
      }
      tile = TileRef.Zero;
      return false;
    }
  }

  [NetSerializable]
  [Serializable]
  private sealed class MapLightComponentState : ComponentState
  {
    public Color AmbientLightColor;
  }
}
