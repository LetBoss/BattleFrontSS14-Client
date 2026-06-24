// Decompiled with JetBrains decompiler
// Type: Content.Shared.Coordinates.Helpers.SnapgridHelper
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared.Coordinates.Helpers;

public static class SnapgridHelper
{
  public static EntityCoordinates SnapToGrid(
    this EntityCoordinates coordinates,
    IEntityManager? entMan = null,
    IMapManager? mapManager = null)
  {
    IoCManager.Resolve<IEntityManager, IMapManager>(ref entMan, ref mapManager);
    SharedTransformSystem sharedTransformSystem = entMan.System<SharedTransformSystem>();
    EntityUid? grid = sharedTransformSystem.GetGrid(coordinates);
    if (!grid.HasValue)
    {
      MapCoordinates mapCoordinates = sharedTransformSystem.ToMapCoordinates(coordinates, true);
      float x = (float) (int) Math.Floor((double) ((MapCoordinates) ref mapCoordinates).X) + 0.5f;
      float y = (float) (int) Math.Floor((double) ((MapCoordinates) ref mapCoordinates).Y) + 0.5f;
      // ISSUE: explicit constructor call
      ((MapCoordinates) ref mapCoordinates).\u002Ector(new Vector2(x, y), mapCoordinates.MapId);
      return sharedTransformSystem.ToCoordinates(Entity<TransformComponent>.op_Implicit(coordinates.EntityId), mapCoordinates);
    }
    ushort tileSize = entMan.GetComponent<MapGridComponent>(grid.Value).TileSize;
    Vector2 position = sharedTransformSystem.WithEntityId(coordinates, grid.Value).Position;
    float x1 = (float) (int) Math.Floor((double) position.X / (double) tileSize) + (float) tileSize / 2f;
    float y1 = (float) (int) Math.Floor((double) position.Y / (double) tileSize) + (float) tileSize / 2f;
    EntityCoordinates entityCoordinates;
    // ISSUE: explicit constructor call
    ((EntityCoordinates) ref entityCoordinates).\u002Ector(grid.Value, new Vector2(x1, y1));
    return sharedTransformSystem.WithEntityId(entityCoordinates, coordinates.EntityId);
  }

  public static EntityCoordinates SnapToGrid(
    this EntityCoordinates coordinates,
    MapGridComponent grid)
  {
    ushort tileSize = grid.TileSize;
    Vector2 position = coordinates.Position;
    float num1 = (float) (int) Math.Floor((double) position.X / (double) tileSize) + (float) tileSize / 2f;
    float num2 = (float) (int) Math.Floor((double) position.Y / (double) tileSize) + (float) tileSize / 2f;
    return new EntityCoordinates(coordinates.EntityId, num1, num2);
  }
}
