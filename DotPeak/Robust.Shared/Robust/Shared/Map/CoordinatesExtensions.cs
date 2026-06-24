// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.CoordinatesExtensions
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Map;

public static class CoordinatesExtensions
{
  public static EntityCoordinates AlignWithClosestGridTile(
    this EntityCoordinates coords,
    float searchBoxSize = 1.5f,
    IEntityManager? entityManager = null,
    IMapManager? mapManager = null)
  {
    IoCManager.Resolve<IEntityManager, IMapManager>(ref entityManager, ref mapManager);
    SharedTransformSystem sharedTransformSystem = entityManager.System<SharedTransformSystem>();
    EntityUid? grid1 = sharedTransformSystem.GetGrid(coords);
    SharedMapSystem sharedMapSystem = entityManager.System<SharedMapSystem>();
    MapGridComponent grid2;
    if (entityManager.TryGetComponent<MapGridComponent>(grid1, out grid2))
      return sharedMapSystem.GridTileToLocal(grid1.Value, grid2, sharedMapSystem.CoordinatesToTile(grid1.Value, grid2, coords));
    MapCoordinates mapCoordinates = sharedTransformSystem.ToMapCoordinates(coords);
    EntityUid uid;
    if (mapManager.TryFindGridAt(mapCoordinates, out uid, out grid2))
      return sharedMapSystem.GridTileToLocal(uid, grid2, sharedMapSystem.CoordinatesToTile(uid, grid2, coords));
    Box2 box2_1 = ((Box2) ref Box2.UnitCentered).Scale(searchBoxSize);
    Box2 worldAABB = ((Box2) ref box2_1).Translated(mapCoordinates.Position);
    List<Entity<MapGridComponent>> grids = new List<Entity<MapGridComponent>>();
    mapManager.FindGridsIntersecting(mapCoordinates.MapId, worldAABB, ref grids);
    EntityUid entityUid = EntityUid.Invalid;
    MapGridComponent grid3 = (MapGridComponent) null;
    float num1 = float.PositiveInfinity;
    Box2 box2_2 = new Box2();
    EntityQuery<TransformComponent> entityQuery = entityManager.GetEntityQuery<TransformComponent>();
    foreach (Entity<MapGridComponent> entity in grids)
    {
      TransformComponent component = entityQuery.GetComponent(entity.Owner);
      Matrix3x2 worldMatrix = sharedTransformSystem.GetWorldMatrix(component);
      ref Box2 local1 = ref worldAABB;
      Matrix3x2 matrix3x2 = worldMatrix;
      Box2 localAabb = entity.Comp.LocalAABB;
      ref Box2 local2 = ref localAabb;
      Box2 box2_3 = Matrix3Helpers.TransformBox(matrix3x2, ref local2);
      ref Box2 local3 = ref box2_3;
      Box2 box2_4 = ((Box2) ref local1).Intersect(ref local3);
      float num2 = (((Box2) ref box2_4).Center - mapCoordinates.Position).LengthSquared();
      if ((double) num2 < (double) num1)
      {
        entityUid = entity.Owner;
        num1 = num2;
        grid3 = (MapGridComponent) entity;
        box2_2 = box2_4;
      }
    }
    if (grid3 != null)
    {
      Vector2 vector2 = mapCoordinates.Position - ((Box2) ref box2_2).Center;
      Vector2i tile = sharedMapSystem.WorldToTile(entityUid, grid3, ((Box2) ref box2_2).Center);
      Vector2 posWorld = sharedMapSystem.GridTileToWorldPos(entityUid, grid3, tile) + vector2 * (float) grid3.TileSize;
      coords = new EntityCoordinates(entityUid, sharedMapSystem.WorldToLocal(entityUid, grid3, posWorld));
    }
    return coords;
  }
}
