// Decompiled with JetBrains decompiler
// Type: Content.Client.NPC.PathfindingOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.NPC;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

#nullable enable
namespace Content.Client.NPC;

public sealed class PathfindingOverlay : Overlay
{
  private readonly IEntityManager _entManager;
  private readonly IEyeManager _eyeManager;
  private readonly IInputManager _inputManager;
  private readonly IMapManager _mapManager;
  private readonly PathfindingSystem _system;
  private readonly MapSystem _mapSystem;
  private readonly SharedTransformSystem _transformSystem;
  private readonly Font _font;
  private List<Entity<MapGridComponent>> _grids = new List<Entity<MapGridComponent>>();

  public virtual OverlaySpace Space => (OverlaySpace) 6;

  public PathfindingOverlay(
    IEntityManager entManager,
    IEyeManager eyeManager,
    IInputManager inputManager,
    IMapManager mapManager,
    IResourceCache cache,
    PathfindingSystem system,
    MapSystem mapSystem,
    SharedTransformSystem transformSystem)
  {
    this._entManager = entManager;
    this._eyeManager = eyeManager;
    this._inputManager = inputManager;
    this._mapManager = mapManager;
    this._system = system;
    this._mapSystem = mapSystem;
    this._transformSystem = transformSystem;
    this._font = (Font) new VectorFont(cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), 10);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    switch (args.DrawingHandle)
    {
      case DrawingHandleScreen screenHandle:
        this.DrawScreen(args, screenHandle);
        break;
      case DrawingHandleWorld worldHandle:
        this.DrawWorld(args, worldHandle);
        break;
    }
  }

  private void DrawScreen(OverlayDrawArgs args, DrawingHandleScreen screenHandle)
  {
    ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
    MapCoordinates map = this._eyeManager.PixelToMap(mouseScreenPosition);
    Box2 box2_1;
    // ISSUE: explicit constructor call
    ((Box2) ref box2_1).\u002Ector(map.Position - SharedPathfindingSystem.ChunkSizeVec, map.Position + SharedPathfindingSystem.ChunkSizeVec);
    EntityQuery<TransformComponent> entityQuery = this._entManager.GetEntityQuery<TransformComponent>();
    if ((this._system.Modes & PathfindingDebugMode.Crumb) != PathfindingDebugMode.None && MapId.op_Equality(map.MapId, args.MapId))
    {
      bool flag = false;
      this._grids.Clear();
      this._mapManager.FindGridsIntersecting(map.MapId, box2_1, ref this._grids, false, true);
      foreach (Entity<MapGridComponent> grid in this._grids)
      {
        NetEntity netEntity = this._entManager.GetNetEntity(Entity<MapGridComponent>.op_Implicit(grid), (MetaDataComponent) null);
        Dictionary<Vector2i, List<PathfindingBreadcrumb>> dictionary;
        TransformComponent transformComponent;
        if (!flag && this._system.Breadcrumbs.TryGetValue(netEntity, out dictionary) && entityQuery.TryGetComponent(Entity<MapGridComponent>.op_Implicit(grid), ref transformComponent))
        {
          (Vector2 _, Angle _, Matrix3x2 matrix, Matrix3x2 matrix3x2) = this._transformSystem.GetWorldPositionRotationMatrixWithInv(transformComponent);
          Box2 box2_2 = ((Box2) ref box2_1).Enlarged(-8f);
          ref Box2 local = ref box2_2;
          Box2 box2_3 = Matrix3Helpers.TransformBox(matrix3x2, ref local);
          foreach (KeyValuePair<Vector2i, List<PathfindingBreadcrumb>> keyValuePair in dictionary)
          {
            if (!flag)
            {
              Vector2i vector2i = Vector2i.op_Multiply(keyValuePair.Key, 8);
              Box2 box2_4;
              // ISSUE: explicit constructor call
              ((Box2) ref box2_4).\u002Ector(Vector2i.op_Implicit(vector2i), Vector2i.op_Implicit(Vector2i.op_Addition(vector2i, 8)));
              if (((Box2) ref box2_4).Intersects(ref box2_3))
              {
                PathfindingBreadcrumb? nullable = new PathfindingBreadcrumb?();
                float num1 = float.MaxValue;
                foreach (PathfindingBreadcrumb pathfindingBreadcrumb in keyValuePair.Value)
                {
                  float num2 = (Vector2.Transform(this._system.GetCoordinate(keyValuePair.Key, pathfindingBreadcrumb.Coordinates), matrix) - map.Position).Length();
                  if ((double) num2 < (double) num1)
                  {
                    num1 = num2;
                    nullable = new PathfindingBreadcrumb?(pathfindingBreadcrumb);
                  }
                }
                if (nullable.HasValue)
                {
                  StringBuilder stringBuilder = new StringBuilder();
                  string str1 = "Point coordinates: " + nullable.Value.Coordinates.ToString();
                  string str2 = "Grid coordinates: " + this._system.GetCoordinate(keyValuePair.Key, nullable.Value.Coordinates).ToString();
                  string str3 = "Layer: " + nullable.Value.Data.CollisionLayer.ToString();
                  string str4 = "Mask: " + nullable.Value.Data.CollisionMask.ToString();
                  stringBuilder.AppendLine(str1);
                  stringBuilder.AppendLine(str2);
                  stringBuilder.AppendLine(str3);
                  stringBuilder.AppendLine(str4);
                  stringBuilder.AppendLine("Flags:");
                  foreach (PathfindingBreadcrumbFlag pathfindingBreadcrumbFlag in Enum.GetValues<PathfindingBreadcrumbFlag>())
                  {
                    if ((pathfindingBreadcrumbFlag & nullable.Value.Data.Flags) != PathfindingBreadcrumbFlag.None)
                    {
                      string str5 = "- " + pathfindingBreadcrumbFlag.ToString();
                      stringBuilder.AppendLine(str5);
                    }
                  }
                  screenHandle.DrawString(this._font, mouseScreenPosition.Position, stringBuilder.ToString());
                  flag = true;
                  break;
                }
              }
            }
          }
        }
      }
    }
    EntityUid entityUid;
    MapGridComponent mapGridComponent;
    TransformComponent transformComponent1;
    Dictionary<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>> dictionary1;
    if ((this._system.Modes & PathfindingDebugMode.Poly) == PathfindingDebugMode.None || !MapId.op_Equality(map.MapId, args.MapId) || !this._mapManager.TryFindGridAt(map, ref entityUid, ref mapGridComponent) || !entityQuery.TryGetComponent(entityUid, ref transformComponent1) || !this._system.Polys.TryGetValue(this._entManager.GetNetEntity(entityUid, (MetaDataComponent) null), out dictionary1))
      return;
    Vector2i gridIndices = ((SharedMapSystem) this._mapSystem).GetTileRef(entityUid, mapGridComponent, map).GridIndices;
    Vector2i key = Vector2i.op_Division(gridIndices, 8);
    Dictionary<Vector2i, List<DebugPathPoly>> dictionary2;
    List<DebugPathPoly> debugPathPolyList;
    if (!dictionary1.TryGetValue(key, out dictionary2) || !dictionary2.TryGetValue(new Vector2i(gridIndices.X % 8, gridIndices.Y % 8), out debugPathPolyList))
      return;
    Matrix3x2 invWorldMatrix = this._transformSystem.GetInvWorldMatrix(transformComponent1);
    DebugPathPoly debugPathPoly1 = (DebugPathPoly) null;
    foreach (DebugPathPoly debugPathPoly2 in debugPathPolyList)
    {
      if (((Box2) ref debugPathPoly2.Box).Contains(Vector2.Transform(map.Position, invWorldMatrix), true))
      {
        debugPathPoly1 = debugPathPoly2;
        break;
      }
    }
    if (debugPathPoly1 == null)
      return;
    StringBuilder stringBuilder1 = new StringBuilder();
  }

  private void DrawWorld(OverlayDrawArgs args, DrawingHandleWorld worldHandle)
  {
    MapCoordinates map = this._eyeManager.PixelToMap(this._inputManager.MouseScreenPosition);
    Box2 box2_1;
    // ISSUE: explicit constructor call
    ((Box2) ref box2_1).\u002Ector(map.Position - Vector2.One / 4f, map.Position + Vector2.One / 4f);
    EntityQuery<TransformComponent> entityQuery = this._entManager.GetEntityQuery<TransformComponent>();
    if ((this._system.Modes & PathfindingDebugMode.Breadcrumbs) != PathfindingDebugMode.None && MapId.op_Equality(map.MapId, args.MapId))
    {
      this._grids.Clear();
      this._mapManager.FindGridsIntersecting(map.MapId, box2_1, ref this._grids, false, true);
      foreach (Entity<MapGridComponent> grid in this._grids)
      {
        Dictionary<Vector2i, List<PathfindingBreadcrumb>> dictionary;
        TransformComponent transformComponent;
        if (this._system.Breadcrumbs.TryGetValue(this._entManager.GetNetEntity(Entity<MapGridComponent>.op_Implicit(grid), (MetaDataComponent) null), out dictionary) && entityQuery.TryGetComponent(Entity<MapGridComponent>.op_Implicit(grid), ref transformComponent))
        {
          (Vector2 _, Angle _, Matrix3x2 matrix3x2_1, Matrix3x2 matrix3x2_2) = this._transformSystem.GetWorldPositionRotationMatrixWithInv(transformComponent);
          ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2_1);
          ref Box2 local = ref box2_1;
          Box2 box2_2 = Matrix3Helpers.TransformBox(matrix3x2_2, ref local);
          foreach (KeyValuePair<Vector2i, List<PathfindingBreadcrumb>> keyValuePair in dictionary)
          {
            Vector2i vector2i = Vector2i.op_Multiply(keyValuePair.Key, 8);
            Box2 box2_3;
            // ISSUE: explicit constructor call
            ((Box2) ref box2_3).\u002Ector(Vector2i.op_Implicit(vector2i), Vector2i.op_Implicit(Vector2i.op_Addition(vector2i, 8)));
            if (((Box2) ref box2_3).Intersects(ref box2_2))
            {
              foreach (PathfindingBreadcrumb pathfindingBreadcrumb in keyValuePair.Value)
              {
                if (!pathfindingBreadcrumb.Equals(PathfindingBreadcrumb.Invalid))
                {
                  Vector2 vector2 = new Vector2(1f / 16f, 1f / 16f);
                  bool flag = pathfindingBreadcrumb.Data.CollisionMask != 0 || pathfindingBreadcrumb.Data.CollisionLayer != 0;
                  Color color = (pathfindingBreadcrumb.Data.Flags & PathfindingBreadcrumbFlag.Space) == PathfindingBreadcrumbFlag.None ? (!flag ? Color.Orange : Color.Blue) : Color.Green;
                  Vector2 coordinate = this._system.GetCoordinate(keyValuePair.Key, pathfindingBreadcrumb.Coordinates);
                  worldHandle.DrawRect(new Box2(coordinate - vector2, coordinate + vector2), ((Color) ref color).WithAlpha(0.25f), true);
                }
              }
            }
          }
        }
      }
    }
    if ((this._system.Modes & PathfindingDebugMode.Polys) != PathfindingDebugMode.None && MapId.op_Equality(map.MapId, args.MapId))
    {
      this._grids.Clear();
      this._mapManager.FindGridsIntersecting(args.MapId, box2_1, ref this._grids, false, true);
      foreach (Entity<MapGridComponent> grid in this._grids)
      {
        Dictionary<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>> dictionary;
        TransformComponent transformComponent;
        if (this._system.Polys.TryGetValue(this._entManager.GetNetEntity(Entity<MapGridComponent>.op_Implicit(grid), (MetaDataComponent) null), out dictionary) && entityQuery.TryGetComponent(Entity<MapGridComponent>.op_Implicit(grid), ref transformComponent))
        {
          (Vector2 _, Angle _, Matrix3x2 matrix3x2_3, Matrix3x2 matrix3x2_4) = this._transformSystem.GetWorldPositionRotationMatrixWithInv(transformComponent);
          ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2_3);
          ref Box2 local = ref box2_1;
          Box2 box2_4 = Matrix3Helpers.TransformBox(matrix3x2_4, ref local);
          foreach (KeyValuePair<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>> keyValuePair1 in dictionary)
          {
            Vector2i vector2i = Vector2i.op_Multiply(keyValuePair1.Key, 8);
            Box2 box2_5;
            // ISSUE: explicit constructor call
            ((Box2) ref box2_5).\u002Ector(Vector2i.op_Implicit(vector2i), Vector2i.op_Implicit(Vector2i.op_Addition(vector2i, 8)));
            if (((Box2) ref box2_5).Intersects(ref box2_4))
            {
              foreach (KeyValuePair<Vector2i, List<DebugPathPoly>> keyValuePair2 in keyValuePair1.Value)
              {
                foreach (DebugPathPoly debugPathPoly in keyValuePair2.Value)
                {
                  DrawingHandleWorld drawingHandleWorld = worldHandle;
                  Box2 box = debugPathPoly.Box;
                  Color green = Color.Green;
                  Color color = ((Color) ref green).WithAlpha(0.25f);
                  drawingHandleWorld.DrawRect(box, color, true);
                  worldHandle.DrawRect(debugPathPoly.Box, Color.Red, false);
                }
              }
            }
          }
        }
      }
    }
    if ((this._system.Modes & PathfindingDebugMode.PolyNeighbors) != PathfindingDebugMode.None && MapId.op_Equality(map.MapId, args.MapId))
    {
      this._grids.Clear();
      this._mapManager.FindGridsIntersecting(args.MapId, box2_1, ref this._grids, false, true);
      foreach (Entity<MapGridComponent> grid in this._grids)
      {
        Dictionary<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>> dictionary;
        TransformComponent transformComponent;
        if (this._system.Polys.TryGetValue(this._entManager.GetNetEntity(Entity<MapGridComponent>.op_Implicit(grid), (MetaDataComponent) null), out dictionary) && entityQuery.TryGetComponent(Entity<MapGridComponent>.op_Implicit(grid), ref transformComponent))
        {
          (Vector2 _, Angle _, Matrix3x2 matrix3x2, Matrix3x2 matrix) = this._transformSystem.GetWorldPositionRotationMatrixWithInv(transformComponent);
          ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2);
          Box2 box2_6 = Matrix3Helpers.TransformBox(matrix, ref box2_1);
          foreach (KeyValuePair<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>> keyValuePair3 in dictionary)
          {
            Vector2i vector2i = Vector2i.op_Multiply(keyValuePair3.Key, 8);
            Box2 box2_7;
            // ISSUE: explicit constructor call
            ((Box2) ref box2_7).\u002Ector(Vector2i.op_Implicit(vector2i), Vector2i.op_Implicit(Vector2i.op_Addition(vector2i, 8)));
            if (((Box2) ref box2_7).Intersects(ref box2_6))
            {
              foreach (KeyValuePair<Vector2i, List<DebugPathPoly>> keyValuePair4 in keyValuePair3.Value)
              {
                foreach (DebugPathPoly debugPathPoly in keyValuePair4.Value)
                {
                  foreach (NetCoordinates neighbor in debugPathPoly.Neighbors)
                  {
                    Color color;
                    Vector2 vector2;
                    if (NetEntity.op_Inequality(neighbor.NetEntity, debugPathPoly.GraphUid))
                    {
                      color = Color.Green;
                      MapCoordinates mapCoordinates = this._transformSystem.ToMapCoordinates(this._entManager.GetCoordinates(neighbor), true);
                      if (!MapId.op_Inequality(mapCoordinates.MapId, args.MapId))
                        vector2 = Vector2.Transform(mapCoordinates.Position, matrix);
                      else
                        continue;
                    }
                    else
                    {
                      color = Color.Blue;
                      vector2 = neighbor.Position;
                    }
                    ((DrawingHandleBase) worldHandle).DrawLine(((Box2) ref debugPathPoly.Box).Center, vector2, color);
                  }
                }
              }
            }
          }
        }
      }
    }
    if ((this._system.Modes & PathfindingDebugMode.Chunks) != PathfindingDebugMode.None)
    {
      this._grids.Clear();
      this._mapManager.FindGridsIntersecting(args.MapId, args.WorldBounds, ref this._grids, false, true);
      foreach (Entity<MapGridComponent> grid in this._grids)
      {
        Dictionary<Vector2i, List<PathfindingBreadcrumb>> dictionary;
        TransformComponent transformComponent;
        if (this._system.Breadcrumbs.TryGetValue(this._entManager.GetNetEntity(Entity<MapGridComponent>.op_Implicit(grid), (MetaDataComponent) null), out dictionary) && entityQuery.TryGetComponent(Entity<MapGridComponent>.op_Implicit(grid), ref transformComponent))
        {
          (Vector2 _, Angle _, Matrix3x2 matrix3x2_5, Matrix3x2 matrix3x2_6) = this._transformSystem.GetWorldPositionRotationMatrixWithInv(transformComponent);
          ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2_5);
          ref Box2Rotated local = ref args.WorldBounds;
          Box2 box2_8 = Matrix3Helpers.TransformBox(matrix3x2_6, ref local);
          foreach (KeyValuePair<Vector2i, List<PathfindingBreadcrumb>> keyValuePair in dictionary)
          {
            Vector2i vector2i = Vector2i.op_Multiply(keyValuePair.Key, 8);
            Box2 box2_9;
            // ISSUE: explicit constructor call
            ((Box2) ref box2_9).\u002Ector(Vector2i.op_Implicit(vector2i), Vector2i.op_Implicit(Vector2i.op_Addition(vector2i, 8)));
            if (((Box2) ref box2_9).Intersects(ref box2_8))
              worldHandle.DrawRect(box2_9, Color.Red, false);
          }
        }
      }
    }
    if ((this._system.Modes & PathfindingDebugMode.Routes) != PathfindingDebugMode.None)
    {
      foreach ((TimeSpan Time, PathRouteMessage Message) route in this._system.Routes)
      {
        foreach (DebugPathPoly debugPathPoly in route.Message.Path)
        {
          TransformComponent transformComponent;
          if (this._entManager.TryGetComponent<TransformComponent>(this._entManager.GetEntity(debugPathPoly.GraphUid), ref transformComponent))
          {
            DrawingHandleWorld drawingHandleWorld1 = worldHandle;
            Matrix3x2 worldMatrix = this._transformSystem.GetWorldMatrix(transformComponent);
            ref Matrix3x2 local = ref worldMatrix;
            ((DrawingHandleBase) drawingHandleWorld1).SetTransform(ref local);
            DrawingHandleWorld drawingHandleWorld2 = worldHandle;
            Box2 box = debugPathPoly.Box;
            Color orange = Color.Orange;
            Color color = ((Color) ref orange).WithAlpha(0.1f);
            drawingHandleWorld2.DrawRect(box, color, true);
          }
        }
      }
    }
    if ((this._system.Modes & PathfindingDebugMode.RouteCosts) != PathfindingDebugMode.None)
    {
      EntityUid entityUid = EntityUid.Invalid;
      foreach ((TimeSpan Time, PathRouteMessage Message) route in this._system.Routes)
      {
        float num1 = route.Message.Costs.Values.Max();
        foreach ((DebugPathPoly key, float num2) in route.Message.Costs)
        {
          EntityUid entity = this._entManager.GetEntity(key.GraphUid);
          if (EntityUid.op_Inequality(entityUid, entity))
          {
            TransformComponent transformComponent;
            if (this._entManager.TryGetComponent<TransformComponent>(entity, ref transformComponent))
            {
              entityUid = entity;
              DrawingHandleWorld drawingHandleWorld = worldHandle;
              Matrix3x2 worldMatrix = this._transformSystem.GetWorldMatrix(transformComponent);
              ref Matrix3x2 local = ref worldMatrix;
              ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local);
            }
            else
              continue;
          }
          worldHandle.DrawRect(key.Box, new Color(0.0f, num2 / num1, (float) (1.0 - (double) num2 / (double) num1), 0.1f), true);
        }
      }
    }
    DrawingHandleWorld drawingHandleWorld3 = worldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local1 = ref identity;
    ((DrawingHandleBase) drawingHandleWorld3).SetTransform(ref local1);
  }
}
