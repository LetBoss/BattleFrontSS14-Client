// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Line.LineSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Map;
using Content.Shared.Beam.Components;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Doors.Components;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Line;

public sealed class LineSystem : EntitySystem
{
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private SharedMapSystem _mapSystem;
  [Dependency]
  private TagSystem _tag;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private INetManager _net;
  private static readonly ProtoId<TagPrototype> StructureTag = (ProtoId<TagPrototype>) "Structure";
  private static readonly ProtoId<TagPrototype> WallTag = (ProtoId<TagPrototype>) "Wall";
  private static readonly float MaxBeamDistance = 500f;
  private Robust.Shared.GameObjects.EntityQuery<BarricadeComponent> _barricadeQuery;
  private Robust.Shared.GameObjects.EntityQuery<DoorComponent> _doorQuery;
  private Robust.Shared.GameObjects.EntityQuery<MapGridComponent> _mapGridQuery;

  public override void Initialize()
  {
    this._barricadeQuery = this.GetEntityQuery<BarricadeComponent>();
    this._doorQuery = this.GetEntityQuery<DoorComponent>();
    this._mapGridQuery = this.GetEntityQuery<MapGridComponent>();
  }

  public List<LineTile> DrawLine(
    EntityCoordinates start,
    EntityCoordinates end,
    TimeSpan delayPer,
    float? range,
    out EntityUid? blocker,
    bool hitBlocker = false,
    bool thick = false)
  {
    blocker = new EntityUid?();
    start = this._mapSystem.AlignToGrid(this._transform.GetMoverCoordinates(start));
    end = this._mapSystem.AlignToGrid(this._transform.GetMoverCoordinates(end));
    List<LineTile> lineTileList = new List<LineTile>();
    float distance;
    if (!start.TryDistance((IEntityManager) this.EntityManager, this._transform, end, out distance))
      return lineTileList;
    if (range.HasValue)
      distance = Math.Min(range.Value, distance);
    float num1 = end.X - start.X;
    double num2 = (double) end.Y - (double) start.Y;
    float x = start.X;
    float y = start.Y;
    float num3 = num1 / distance;
    double num4 = (double) distance;
    float num5 = (float) (num2 / num4);
    TimeSpan curTime = this._timing.CurTime;
    EntityUid? grid1 = this._transform.GetGrid((Entity<TransformComponent>) start.EntityId);
    MapGridComponent comp = !grid1.HasValue ? (MapGridComponent) null : this._mapGridQuery.CompOrNull(grid1.Value);
    Entity<MapGridComponent>? grid2 = comp == null ? new Entity<MapGridComponent>?() : new Entity<MapGridComponent>?(new Entity<MapGridComponent>(grid1.Value, comp));
    EntityCoordinates entityCoordinates1 = start;
    for (int index1 = 0; (double) index1 < (double) distance; ++index1)
    {
      x += num3;
      y += num5;
      EntityCoordinates grid3 = new EntityCoordinates(start.EntityId, x, y).SnapToGrid((IEntityManager) this.EntityManager, this._mapManager);
      if (!(grid3 == entityCoordinates1))
      {
        List<EntityCoordinates> entityCoordinatesList = new List<EntityCoordinates>(9);
        entityCoordinatesList.Add(grid3);
        if (thick && index1 > 1)
        {
          for (int index2 = -1; index2 < 2; ++index2)
          {
            for (int index3 = -1; index3 < 2; ++index3)
            {
              if (index2 != 0 || index3 != 0)
              {
                EntityCoordinates grid4 = new EntityCoordinates(start.EntityId, x + (float) index2, y + (float) index3).SnapToGrid((IEntityManager) this.EntityManager, this._mapManager);
                entityCoordinatesList.Add(grid4);
              }
            }
          }
        }
        bool flag1 = false;
        for (int index4 = 0; index4 < entityCoordinatesList.Count; ++index4)
        {
          EntityCoordinates entityCoordinates2 = entityCoordinatesList[index4];
          Angle worldAngle = DirectionExtensions.ToWorldAngle(entityCoordinates2.Position - entityCoordinates1.Position);
          bool flag2 = this.IsTileBlocked(grid2, entityCoordinates2, worldAngle, out blocker);
          if (!flag2 || hitBlocker)
          {
            MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(entityCoordinates2);
            bool flag3 = false;
            foreach (LineTile lineTile in lineTileList)
            {
              if (lineTile.Coordinates.Position == mapCoordinates.Position)
              {
                flag3 = true;
                break;
              }
            }
            if (!flag3)
            {
              float num6 = Vector2.Distance(entityCoordinates2.Position, start.Position) - 1f;
              lineTileList.Add(new LineTile(mapCoordinates, curTime + delayPer * (double) num6));
            }
            if (flag2 && index4 == 0)
            {
              flag1 = true;
              break;
            }
          }
        }
        if (!flag1)
          entityCoordinates1 = grid3;
        else
          break;
      }
    }
    return lineTileList;
  }

  private bool IsTileBlocked(
    Entity<MapGridComponent>? grid,
    EntityCoordinates coords,
    Angle angle,
    [NotNullWhen(true)] out EntityUid? blocker)
  {
    blocker = new EntityUid?();
    if (!grid.HasValue)
      return false;
    SharedMapSystem mapSystem1 = this._mapSystem;
    EntityUid uid1 = (EntityUid) grid.Value;
    Entity<MapGridComponent>? nullable = grid;
    MapGridComponent valueOrDefault1 = nullable.HasValue ? (MapGridComponent) nullable.GetValueOrDefault() : (MapGridComponent) null;
    EntityCoordinates coords1 = coords;
    Vector2i vector2i = mapSystem1.TileIndicesFor(uid1, valueOrDefault1, coords1);
    SharedMapSystem mapSystem2 = this._mapSystem;
    EntityUid uid2 = (EntityUid) grid.Value;
    nullable = grid;
    MapGridComponent valueOrDefault2 = nullable.HasValue ? (MapGridComponent) nullable.GetValueOrDefault() : (MapGridComponent) null;
    Vector2i pos = vector2i;
    AnchoredEntitiesEnumerator entitiesEnumerator = mapSystem2.GetAnchoredEntitiesEnumerator(uid2, valueOrDefault2, pos);
    EntityUid? uid3;
    while (entitiesEnumerator.MoveNext(out uid3))
    {
      if (this._barricadeQuery.HasComp(uid3))
      {
        DoorComponent component;
        if (!this._doorQuery.TryComp(uid3, out component) || component.State == DoorState.Closed)
        {
          Angle worldRotation = this._transform.GetWorldRotation(uid3.Value);
          Direction cardinalDir = ((Angle) ref worldRotation).GetCardinalDir();
          Direction dir = ((Angle) ref angle).GetDir();
          if (cardinalDir == dir || cardinalDir == DirectionExtensions.GetOpposite(dir))
          {
            blocker = new EntityUid?(uid3.Value);
            return true;
          }
          if (!dir.IsCardinal())
          {
            bool flag;
            switch (dir - 1)
            {
              case 0:
                flag = cardinalDir == 4 || cardinalDir == 6;
                break;
              case 2:
                flag = cardinalDir == null || cardinalDir == 6;
                break;
              case 4:
                flag = cardinalDir == null || cardinalDir == 2;
                break;
              case 6:
                flag = cardinalDir == 2 || cardinalDir == 4;
                break;
              default:
                flag = false;
                break;
            }
            if (flag)
            {
              blocker = new EntityUid?(uid3.Value);
              return true;
            }
          }
        }
      }
      else
      {
        DoorComponent component;
        if (this._doorQuery.TryComp(uid3, out component))
        {
          if (component.State == DoorState.Closed)
          {
            blocker = new EntityUid?(uid3.Value);
            return true;
          }
        }
        else if (this._tag.HasAnyTag(uid3.Value, LineSystem.StructureTag, LineSystem.WallTag))
        {
          blocker = new EntityUid?(uid3.Value);
          return true;
        }
      }
    }
    return false;
  }

  public bool TryCreateLine(
    EntityUid source,
    EntityUid target,
    string proto,
    out List<EntityUid> lines)
  {
    lines = new List<EntityUid>();
    if (this._net.IsClient || this.Deleted(source) || this.Deleted(target) || this._transform.GetMapId((Entity<TransformComponent>) source) != this._transform.GetMapId((Entity<TransformComponent>) target))
      return false;
    MapCoordinates mapCoordinates1 = this._transform.GetMapCoordinates(source);
    MapCoordinates mapCoordinates2 = this._transform.GetMapCoordinates(target);
    Vector2 vector2_1 = mapCoordinates2.Position - mapCoordinates1.Position;
    Angle worldAngle = DirectionExtensions.ToWorldAngle(vector2_1);
    if (mapCoordinates1.MapId != mapCoordinates2.MapId)
      return false;
    MapCoordinates coordinates = mapCoordinates1.Offset(Vector2Helpers.Normalized(vector2_1));
    if ((double) vector2_1.Length() == 0.0 || (double) vector2_1.Length() > (double) LineSystem.MaxBeamDistance)
      return false;
    Vector2 vector2_2 = vector2_1 - Vector2Helpers.Normalized(vector2_1);
    EntityUid uid1 = this.Spawn(proto, coordinates, rotation: new Angle());
    lines.Add(uid1);
    float distanceLength = vector2_2.Length();
    this.RaiseNetworkEvent((EntityEventArgs) new BeamVisualizerEvent(this.GetNetEntity(uid1), distanceLength, worldAngle, shader: "shaded"));
    for (int index = 0; (double) index < (double) distanceLength - 1.0; ++index)
    {
      coordinates = coordinates.Offset(Vector2Helpers.Normalized(vector2_1));
      EntityUid uid2 = this.Spawn(proto, coordinates, rotation: new Angle());
      lines.Add(uid2);
      this.RaiseNetworkEvent((EntityEventArgs) new BeamVisualizerEvent(this.GetNetEntity(uid2), distanceLength, worldAngle, shader: "shaded"));
    }
    return true;
  }

  public void DeleteBeam(List<EntityUid> beam)
  {
    if (this._net.IsClient)
      return;
    foreach (EntityUid entityUid in beam)
      this.QueueDel(new EntityUid?(entityUid));
    beam.Clear();
  }
}
