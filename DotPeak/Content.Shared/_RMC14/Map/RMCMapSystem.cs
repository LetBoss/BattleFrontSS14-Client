// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Map.RMCMapSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos;
using Content.Shared.Coordinates;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Tag;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Map;

public sealed class RMCMapSystem : EntitySystem
{
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private TagSystem _tag;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private TurfSystem _turf;
  private static readonly ProtoId<TagPrototype> StructureTag = (ProtoId<TagPrototype>) "Structure";
  private Robust.Shared.GameObjects.EntityQuery<MapGridComponent> _mapGridQuery;
  public readonly ImmutableArray<AtmosDirection> AtmosCardinalDirections = ImmutableArray.Create<AtmosDirection>(AtmosDirection.South, AtmosDirection.East, AtmosDirection.North, AtmosDirection.West);
  public readonly ImmutableArray<Direction> CardinalDirections = ImmutableArray.Create<Direction>((Direction) 4, (Direction) 0, (Direction) 2, (Direction) 6);

  public override void Initialize()
  {
    this._mapGridQuery = this.GetEntityQuery<MapGridComponent>();
    this.SubscribeLocalEvent<RMCDeleteAnchoredOnInitComponent, MapInitEvent>(new EntityEventRefHandler<RMCDeleteAnchoredOnInitComponent, MapInitEvent>(this.OnDestroyAnchoredOnInit));
  }

  private void OnDestroyAnchoredOnInit(
    Entity<RMCDeleteAnchoredOnInitComponent> ent,
    ref MapInitEvent args)
  {
    if (!ent.Comp.Enabled || this._net.IsClient)
      return;
    RMCAnchoredEntitiesEnumerator entitiesEnumerator = this.GetAnchoredEntitiesEnumerator((EntityUid) ent, facing: (DirectionFlag) 0);
    EntityUid uid;
    while (entitiesEnumerator.MoveNext(out uid))
    {
      if (!(uid == ent.Owner) && !this.TerminatingOrDeleted(uid) && !this.EntityManager.IsQueuedForDeletion(uid) && !this._entityWhitelist.IsWhitelistFailOrNull(ent.Comp.Whitelist, uid))
        this.QueueDel(new EntityUid?(uid));
    }
  }

  public RMCAnchoredEntitiesEnumerator GetAnchoredEntitiesEnumerator(
    EntityUid ent,
    Direction? offset = null,
    DirectionFlag facing = 0)
  {
    return this.GetAnchoredEntitiesEnumerator(ent.ToCoordinates(), offset, facing);
  }

  public RMCAnchoredEntitiesEnumerator GetAnchoredEntitiesEnumerator(
    EntityCoordinates coords,
    Direction? offset = null,
    DirectionFlag facing = 0)
  {
    EntityUid? grid = this._transform.GetGrid(coords);
    if (grid.HasValue)
    {
      EntityUid valueOrDefault = grid.GetValueOrDefault();
      MapGridComponent component;
      if (this._mapGridQuery.TryComp(valueOrDefault, out component))
      {
        Vector2i tile = this._map.CoordinatesToTile(valueOrDefault, component, coords);
        return this.GetAnchoredEntitiesEnumerator((Entity<MapGridComponent>) (valueOrDefault, component), tile, offset, facing);
      }
    }
    return RMCAnchoredEntitiesEnumerator.Empty;
  }

  public RMCAnchoredEntitiesEnumerator GetAnchoredEntitiesEnumerator(
    MapCoordinates coords,
    Direction? offset = null,
    DirectionFlag facing = 0)
  {
    EntityUid uid;
    MapGridComponent grid;
    if (!this._mapManager.TryFindGridAt(coords, out uid, out grid))
      return RMCAnchoredEntitiesEnumerator.Empty;
    Vector2i tile = this._map.CoordinatesToTile(uid, grid, coords);
    return this.GetAnchoredEntitiesEnumerator((Entity<MapGridComponent>) (uid, grid), tile, offset, facing);
  }

  public RMCAnchoredEntitiesEnumerator GetAnchoredEntitiesEnumerator(
    Entity<MapGridComponent> grid,
    Vector2i indices,
    Direction? offset = null,
    DirectionFlag facing = 0)
  {
    if (offset.HasValue)
      indices = DirectionExtensions.Offset(indices, offset.Value);
    return new RMCAnchoredEntitiesEnumerator(this._transform, this._map.GetAnchoredEntitiesEnumerator((EntityUid) grid, (MapGridComponent) grid, indices), facing);
  }

  public RMCAnchoredEntitiesEnumerator<T> GetAnchoredEntitiesEnumerator<T>(
    EntityUid ent,
    Direction? offset = null,
    DirectionFlag facing = 0)
    where T : IComponent
  {
    return this.GetAnchoredEntitiesEnumerator<T>(ent.ToCoordinates(), offset, facing);
  }

  public RMCAnchoredEntitiesEnumerator<T> GetAnchoredEntitiesEnumerator<T>(
    EntityCoordinates coords,
    Direction? offset = null,
    DirectionFlag facing = 0)
    where T : IComponent
  {
    EntityUid? grid = this._transform.GetGrid(coords);
    if (grid.HasValue)
    {
      EntityUid valueOrDefault = grid.GetValueOrDefault();
      MapGridComponent component;
      if (this._mapGridQuery.TryComp(valueOrDefault, out component))
      {
        Vector2i tile = this._map.CoordinatesToTile(valueOrDefault, component, coords);
        return this.GetAnchoredEntitiesEnumerator<T>((Entity<MapGridComponent>) (valueOrDefault, component), tile, offset, facing);
      }
    }
    return RMCAnchoredEntitiesEnumerator<T>.Empty;
  }

  public RMCAnchoredEntitiesEnumerator<T> GetAnchoredEntitiesEnumerator<T>(
    MapCoordinates coords,
    Direction? offset = null,
    DirectionFlag facing = 0)
    where T : IComponent
  {
    EntityUid uid;
    MapGridComponent grid;
    if (!this._mapManager.TryFindGridAt(coords, out uid, out grid))
      return RMCAnchoredEntitiesEnumerator<T>.Empty;
    Vector2i tile = this._map.CoordinatesToTile(uid, grid, coords);
    return this.GetAnchoredEntitiesEnumerator<T>((Entity<MapGridComponent>) (uid, grid), tile, offset, facing);
  }

  public RMCAnchoredEntitiesEnumerator<T> GetAnchoredEntitiesEnumerator<T>(
    Entity<MapGridComponent> grid,
    Vector2i indices,
    Direction? offset = null,
    DirectionFlag facing = 0)
    where T : IComponent
  {
    if (offset.HasValue)
      indices = DirectionExtensions.Offset(indices, offset.Value);
    return new RMCAnchoredEntitiesEnumerator<T>((IEntityManager) this.EntityManager, this._transform, this._map.GetAnchoredEntitiesEnumerator((EntityUid) grid, (MapGridComponent) grid, indices), facing);
  }

  public bool HasAnchoredEntityEnumerator<T>(
    EntityCoordinates coords,
    out Entity<T> ent,
    Direction? offset = null,
    DirectionFlag facing = 0)
    where T : IComponent
  {
    ent = new Entity<T>();
    RMCAnchoredEntitiesEnumerator entitiesEnumerator = this.GetAnchoredEntitiesEnumerator(coords, offset, facing);
    EntityUid uid;
    while (entitiesEnumerator.MoveNext(out uid))
    {
      T comp;
      if (this.TryComp<T>(uid, out comp))
      {
        ent = (Entity<T>) (uid, comp);
        return true;
      }
    }
    return false;
  }

  public bool HasAnchoredEntityEnumerator<T>(
    EntityCoordinates coords,
    Direction? offset = null,
    DirectionFlag facing = 0)
    where T : IComponent
  {
    return this.HasAnchoredEntityEnumerator<T>(coords, out Entity<T> _, offset, facing);
  }

  public bool TryGetTileRefForEnt(
    EntityCoordinates ent,
    out Entity<MapGridComponent> grid,
    out TileRef tile)
  {
    grid = new Entity<MapGridComponent>();
    tile = new TileRef();
    EntityUid? grid1 = this._transform.GetGrid(ent);
    if (grid1.HasValue)
    {
      EntityUid valueOrDefault = grid1.GetValueOrDefault();
      MapGridComponent component;
      if (this._mapGridQuery.TryComp(valueOrDefault, out component))
      {
        EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(ent);
        grid = (Entity<MapGridComponent>) (valueOrDefault, component);
        return this._map.TryGetTileRef(valueOrDefault, component, moverCoordinates, out tile);
      }
    }
    return false;
  }

  public bool IsTileBlocked(EntityCoordinates coordinates, CollisionGroup group = CollisionGroup.Impassable)
  {
    TileRef? tile;
    return this._turf.TryGetTileRef(coordinates, out tile) && this._turf.IsTileBlocked(tile.Value, group);
  }

  public bool IsTileBlocked(MapCoordinates coordinates, CollisionGroup group = CollisionGroup.Impassable)
  {
    return this.IsTileBlocked(this._transform.ToCoordinates(coordinates), group);
  }

  public bool TileHasAnyTag(EntityCoordinates coordinates, params ProtoId<TagPrototype>[] tag)
  {
    RMCAnchoredEntitiesEnumerator entitiesEnumerator = this.GetAnchoredEntitiesEnumerator(coordinates, facing: (DirectionFlag) 0);
    EntityUid uid;
    while (entitiesEnumerator.MoveNext(out uid))
    {
      if (this._tag.HasAnyTag(uid, tag))
        return true;
    }
    return false;
  }

  public bool TileHasStructure(EntityCoordinates coordinates)
  {
    return this.TileHasAnyTag(coordinates, RMCMapSystem.StructureTag);
  }

  public bool TryGetTileDef(EntityCoordinates coordinates, [NotNullWhen(true)] out ContentTileDefinition? def)
  {
    def = (ContentTileDefinition) null;
    EntityUid? grid = this._transform.GetGrid(coordinates);
    if (grid.HasValue)
    {
      EntityUid valueOrDefault = grid.GetValueOrDefault();
      MapGridComponent comp;
      if (this.TryComp<MapGridComponent>(valueOrDefault, out comp))
      {
        Vector2i indices = this._map.TileIndicesFor(valueOrDefault, comp, coordinates);
        ITileDefinition tileDef;
        if (!this._map.TryGetTileDef(comp, indices, out tileDef))
          return false;
        def = (ContentTileDefinition) tileDef;
        return true;
      }
    }
    return false;
  }

  public bool TryGetTileDef(MapCoordinates coordinates, [NotNullWhen(true)] out ContentTileDefinition? def)
  {
    return this.TryGetTileDef(this._transform.ToCoordinates(coordinates), out def);
  }

  public bool CanBuildOn(EntityCoordinates coordinates, CollisionGroup group = CollisionGroup.Impassable)
  {
    return !this.IsTileBlocked(coordinates, group) && !this.TileHasStructure(coordinates);
  }

  public EntityCoordinates SnapToGrid(EntityCoordinates coordinates)
  {
    EntityUid? grid = this._transform.GetGrid(coordinates);
    MapGridComponent comp;
    if (!grid.HasValue || !this.TryComp<MapGridComponent>(grid, out comp))
    {
      MapCoordinates coordinates1 = this._transform.ToMapCoordinates(coordinates);
      coordinates1 = new MapCoordinates(new Vector2((float) (int) Math.Floor((double) coordinates1.X) + 0.5f, (float) (int) Math.Floor((double) coordinates1.Y) + 0.5f), coordinates1.MapId);
      return this._transform.ToCoordinates((Entity<TransformComponent>) coordinates.EntityId, coordinates1);
    }
    ushort tileSize = comp.TileSize;
    Vector2 position = this._transform.WithEntityId(coordinates, grid.Value).Position;
    float x = (float) (int) Math.Floor((double) position.X / (double) tileSize) + (float) tileSize / 2f;
    float y = (float) (int) Math.Floor((double) position.Y / (double) tileSize) + (float) tileSize / 2f;
    return this._transform.WithEntityId(new EntityCoordinates(grid.Value, new Vector2(x, y)), coordinates.EntityId);
  }
}
