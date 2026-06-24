// Decompiled with JetBrains decompiler
// Type: Content.Shared.Maps.TurfSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Physics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Shared.Maps;

public sealed class TurfSystem : EntitySystem
{
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedMapSystem _mapSystem;
  [Dependency]
  private ITileDefinitionManager _tileDefinitions;

  public TileRef? GetTileRef(EntityCoordinates coordinates)
  {
    if (!coordinates.IsValid((IEntityManager) this.EntityManager))
      return new TileRef?();
    EntityUid uid;
    MapGridComponent grid;
    if (!this._mapManager.TryFindGridAt(this._transform.ToMapCoordinates(coordinates), out uid, out grid))
      return new TileRef?();
    TileRef tile;
    return !this._mapSystem.TryGetTileRef(uid, grid, coordinates, out tile) ? new TileRef?() : new TileRef?(tile);
  }

  public bool TryGetTileRef(EntityCoordinates coordinates, [NotNullWhen(true)] out TileRef? tile)
  {
    return (tile = this.GetTileRef(coordinates)).HasValue;
  }

  public bool IsTileBlocked(TileRef turf, CollisionGroup mask, float minIntersectionArea = 0.1f)
  {
    return this.IsTileBlocked(turf.GridUid, turf.GridIndices, mask, minIntersectionArea: minIntersectionArea);
  }

  public bool IsTileBlocked(
    EntityUid gridUid,
    Vector2i indices,
    CollisionGroup mask,
    MapGridComponent? grid = null,
    TransformComponent? gridXform = null,
    float minIntersectionArea = 0.1f)
  {
    if (!this.Resolve<MapGridComponent, TransformComponent>(gridUid, ref grid, ref gridXform))
      return false;
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> entityQuery1 = this.GetEntityQuery<TransformComponent>();
    (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 matrix3x2) = this._transform.GetWorldPositionRotationMatrix(gridXform, entityQuery1);
    ushort tileSize = grid.TileSize;
    Vector2 position = new Vector2((float) (indices.X * (int) tileSize) + (float) tileSize / 2f, (float) (indices.Y * (int) tileSize) + (float) tileSize / 2f);
    Vector2 vector2_1 = Vector2.Transform(position, matrix3x2);
    Box2 box2_1 = ((Box2) ref Box2.UnitCentered).Scale(0.95f * (float) tileSize);
    Box2Rotated worldBounds;
    // ISSUE: explicit constructor call
    ((Box2Rotated) ref worldBounds).\u002Ector(((Box2) ref box2_1).Translated(vector2_1), WorldRotation, vector2_1);
    box2_1 = ((Box2) ref box2_1).Translated(position);
    float num = 0.0f;
    Robust.Shared.GameObjects.EntityQuery<FixturesComponent> entityQuery2 = this.GetEntityQuery<FixturesComponent>();
    foreach (EntityUid uid in this._entityLookup.GetEntitiesIntersecting(gridUid, worldBounds, LookupFlags.Dynamic | LookupFlags.Static))
    {
      FixturesComponent component;
      if (entityQuery2.TryGetComponent(uid, out component))
      {
        (Vector2 WorldPosition, Angle WorldRotation) positionRotation = this._transform.GetWorldPositionRotation(entityQuery1.GetComponent(uid), entityQuery1);
        Vector2 worldPosition = positionRotation.WorldPosition;
        Angle angle1 = Angle.op_Subtraction(positionRotation.WorldRotation, WorldRotation);
        Angle angle2 = Angle.op_UnaryNegation(WorldRotation);
        ref Angle local1 = ref angle2;
        Vector2 vector2_2 = worldPosition - WorldPosition;
        ref Vector2 local2 = ref vector2_2;
        Robust.Shared.Physics.Transform transform = new Robust.Shared.Physics.Transform(((Angle) ref local1).RotateVec(ref local2), (float) angle1.Theta);
        foreach (Fixture fixture in component.Fixtures.Values)
        {
          if (fixture.Hard && ((CollisionGroup) fixture.CollisionLayer & mask) != CollisionGroup.None)
          {
            for (int childIndex = 0; childIndex < fixture.Shape.ChildCount; ++childIndex)
            {
              Box2 aabb = fixture.Shape.ComputeAABB(transform, childIndex);
              Box2 box2_2 = ((Box2) ref aabb).Intersect(ref box2_1);
              num += ((Box2) ref box2_2).Width * ((Box2) ref box2_2).Height;
              if ((double) num > (double) minIntersectionArea)
                return true;
            }
          }
        }
      }
    }
    return false;
  }

  public bool IsSpace(Tile tile) => this.GetContentTileDefinition(tile).MapAtmosphere;

  public bool IsSpace(TileRef tile) => this.IsSpace(tile.Tile);

  public EntityCoordinates GetTileCenter(TileRef turf)
  {
    MapGridComponent mapGridComponent = this.Comp<MapGridComponent>(turf.GridUid);
    Vector2 position = (Vector2i.op_Implicit(turf.GridIndices) + new Vector2(0.5f, 0.5f)) * (float) mapGridComponent.TileSize;
    return new EntityCoordinates(turf.GridUid, position);
  }

  public ContentTileDefinition GetContentTileDefinition(Tile tile)
  {
    return (ContentTileDefinition) this._tileDefinitions[tile.TypeId];
  }

  public ContentTileDefinition GetContentTileDefinition(TileRef tile)
  {
    return this.GetContentTileDefinition(tile.Tile);
  }

  public void GetEntitiesInTile(
    EntityCoordinates coords,
    HashSet<EntityUid> intersecting,
    LookupFlags flags = LookupFlags.Static)
  {
    TileRef? tile;
    if (!this.TryGetTileRef(coords, out tile))
      return;
    this._entityLookup.GetEntitiesInTile(tile.Value, intersecting, flags);
  }

  public HashSet<EntityUid> GetEntitiesInTile(EntityCoordinates coords, LookupFlags flags = LookupFlags.Static)
  {
    TileRef? tile;
    return !this.TryGetTileRef(coords, out tile) ? new HashSet<EntityUid>() : this._entityLookup.GetEntitiesInTile(tile.Value, flags);
  }
}
