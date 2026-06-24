// Decompiled with JetBrains decompiler
// Type: Content.Client.NodeContainer.NodeVisualizationOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Resources;
using Content.Shared.NodeContainer;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

#nullable enable
namespace Content.Client.NodeContainer;

public sealed class NodeVisualizationOverlay : Overlay
{
  private readonly NodeGroupSystem _system;
  private readonly EntityLookupSystem _lookup;
  private readonly IMapManager _mapManager;
  private readonly IInputManager _inputManager;
  private readonly IEntityManager _entityManager;
  private readonly SharedTransformSystem _transformSystem;
  private readonly SharedMapSystem _mapSystem;
  private readonly Dictionary<(int, int), NodeVisualizationOverlay.NodeRenderData> _nodeIndex = new Dictionary<(int, int), NodeVisualizationOverlay.NodeRenderData>();
  private readonly Dictionary<EntityUid, Dictionary<Vector2i, List<(NodeVis.GroupData, NodeVis.NodeDatum)>>> _gridIndex = new Dictionary<EntityUid, Dictionary<Vector2i, List<(NodeVis.GroupData, NodeVis.NodeDatum)>>>();
  private List<Entity<MapGridComponent>> _grids = new List<Entity<MapGridComponent>>();
  private readonly Font _font;
  private Vector2 _mouseWorldPos;
  private (int group, int node)? _hovered;
  private float _time;

  public virtual OverlaySpace Space => (OverlaySpace) 6;

  public NodeVisualizationOverlay(
    NodeGroupSystem system,
    EntityLookupSystem lookup,
    IMapManager mapManager,
    IInputManager inputManager,
    IResourceCache cache,
    IEntityManager entityManager)
  {
    this._system = system;
    this._lookup = lookup;
    this._mapManager = mapManager;
    this._inputManager = inputManager;
    this._entityManager = entityManager;
    this._transformSystem = this._entityManager.System<SharedTransformSystem>();
    this._mapSystem = this._entityManager.System<SharedMapSystem>();
    this._font = cache.GetFont("/Fonts/NotoSans/NotoSans-Regular.ttf", 12);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if ((args.Space & 4) != null)
    {
      this.DrawWorld(in args);
    }
    else
    {
      if ((args.Space & 2) == null)
        return;
      this.DrawScreen(in args);
    }
  }

  private void DrawScreen(in OverlayDrawArgs args)
  {
    Vector2 position = this._inputManager.MouseScreenPosition.Position;
    this._mouseWorldPos = args.ViewportControl.PixelToMap(position).Position;
    if (!this._hovered.HasValue)
      return;
    (int num, int node) = this._hovered.Value;
    NodeVis.GroupData group = this._system.Groups[num];
    NodeVis.NodeDatum nodeDatum = this._system.NodeLookup[(num, node)];
    TransformComponent component = this._entityManager.GetComponent<TransformComponent>(this._entityManager.GetEntity(nodeDatum.Entity));
    MapGridComponent mapGridComponent;
    if (!this._entityManager.TryGetComponent<MapGridComponent>(component.GridUid, ref mapGridComponent))
      return;
    Vector2i vector2i = this._mapSystem.TileIndicesFor(Entity<MapGridComponent>.op_Implicit((component.GridUid.Value, mapGridComponent)), component.Coordinates);
    StringBuilder stringBuilder1 = new StringBuilder();
    StringBuilder stringBuilder2 = stringBuilder1;
    StringBuilder stringBuilder3 = stringBuilder2;
    StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(9, 1, stringBuilder2);
    interpolatedStringHandler.AppendLiteral("entity: ");
    interpolatedStringHandler.AppendFormatted<NetEntity>(nodeDatum.Entity);
    interpolatedStringHandler.AppendLiteral("\n");
    ref StringBuilder.AppendInterpolatedStringHandler local1 = ref interpolatedStringHandler;
    stringBuilder3.Append(ref local1);
    StringBuilder stringBuilder4 = stringBuilder1;
    StringBuilder stringBuilder5 = stringBuilder4;
    interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(11, 1, stringBuilder4);
    interpolatedStringHandler.AppendLiteral("group id: ");
    interpolatedStringHandler.AppendFormatted(group.GroupId);
    interpolatedStringHandler.AppendLiteral("\n");
    ref StringBuilder.AppendInterpolatedStringHandler local2 = ref interpolatedStringHandler;
    stringBuilder5.Append(ref local2);
    StringBuilder stringBuilder6 = stringBuilder1;
    StringBuilder stringBuilder7 = stringBuilder6;
    interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder6);
    interpolatedStringHandler.AppendLiteral("node: ");
    interpolatedStringHandler.AppendFormatted(nodeDatum.Name);
    interpolatedStringHandler.AppendLiteral("\n");
    ref StringBuilder.AppendInterpolatedStringHandler local3 = ref interpolatedStringHandler;
    stringBuilder7.Append(ref local3);
    StringBuilder stringBuilder8 = stringBuilder1;
    StringBuilder stringBuilder9 = stringBuilder8;
    interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder8);
    interpolatedStringHandler.AppendLiteral("type: ");
    interpolatedStringHandler.AppendFormatted(nodeDatum.Type);
    interpolatedStringHandler.AppendLiteral("\n");
    ref StringBuilder.AppendInterpolatedStringHandler local4 = ref interpolatedStringHandler;
    stringBuilder9.Append(ref local4);
    StringBuilder stringBuilder10 = stringBuilder1;
    StringBuilder stringBuilder11 = stringBuilder10;
    interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(11, 1, stringBuilder10);
    interpolatedStringHandler.AppendLiteral("grid pos: ");
    interpolatedStringHandler.AppendFormatted<Vector2i>(vector2i);
    interpolatedStringHandler.AppendLiteral("\n");
    ref StringBuilder.AppendInterpolatedStringHandler local5 = ref interpolatedStringHandler;
    stringBuilder11.Append(ref local5);
    stringBuilder1.Append(group.DebugData);
    ((OverlayDrawArgs) ref args).ScreenHandle.DrawString(this._font, position + new Vector2(20f, -20f), stringBuilder1.ToString());
  }

  private void DrawWorld(in OverlayDrawArgs overlayDrawArgs)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref overlayDrawArgs).WorldHandle;
    IEye eye = overlayDrawArgs.Viewport.Eye;
    MapId mapId = eye != null ? eye.Position.MapId : new MapId();
    if (MapId.op_Equality(mapId, MapId.Nullspace))
      return;
    this._hovered = new (int, int)?();
    Box2 box2_1 = Box2.CenteredAround(this._mouseWorldPos, new Vector2(0.25f, 0.25f));
    Box2 worldAabb = overlayDrawArgs.WorldAABB;
    EntityQuery<TransformComponent> entityQuery = this._entityManager.GetEntityQuery<TransformComponent>();
    this._grids.Clear();
    this._mapManager.FindGridsIntersecting(mapId, worldAabb, ref this._grids, false, true);
    foreach (Entity<MapGridComponent> grid in this._grids)
    {
      foreach (EntityUid key in this._lookup.GetEntitiesIntersecting(Entity<MapGridComponent>.op_Implicit(grid), worldAabb, (LookupFlags) 110))
      {
        (NodeVis.GroupData group, NodeVis.NodeDatum node)[] tupleArray;
        if (this._system.Entities.TryGetValue(key, out tupleArray))
        {
          Dictionary<Vector2i, List<(NodeVis.GroupData, NodeVis.NodeDatum)>> orNew1 = Extensions.GetOrNew<EntityUid, Dictionary<Vector2i, List<(NodeVis.GroupData, NodeVis.NodeDatum)>>>(this._gridIndex, Entity<MapGridComponent>.op_Implicit(grid));
          EntityCoordinates coordinates = entityQuery.GetComponent(key).Coordinates;
          if (!float.IsNaN(coordinates.Position.X) && !float.IsNaN(coordinates.Position.Y))
          {
            List<(NodeVis.GroupData, NodeVis.NodeDatum)> orNew2 = Extensions.GetOrNew<Vector2i, List<(NodeVis.GroupData, NodeVis.NodeDatum)>>(orNew1, this._mapSystem.TileIndicesFor(grid, coordinates));
            foreach ((NodeVis.GroupData group, NodeVis.NodeDatum node) in tupleArray)
            {
              if (!this._system.Filtered.Contains(group.GroupId))
                orNew2.Add((group, node));
            }
          }
        }
      }
    }
    foreach ((EntityUid key1, Dictionary<Vector2i, List<(NodeVis.GroupData, NodeVis.NodeDatum)>> dictionary) in this._gridIndex)
    {
      MapGridComponent component = this._entityManager.GetComponent<MapGridComponent>(key1);
      (Vector2, Angle, Matrix3x2, Matrix3x2) rotationMatrixWithInv = this._transformSystem.GetWorldPositionRotationMatrixWithInv(key1);
      Matrix3x2 matrix3x2 = rotationMatrixWithInv.Item3;
      Box2 box2_2 = Matrix3Helpers.TransformBox(rotationMatrixWithInv.Item4, ref box2_1);
      foreach ((Vector2i key2, List<(NodeVis.GroupData, NodeVis.NodeDatum)> tupleList) in dictionary)
      {
        Vector2 vector2 = Vector2i.op_Implicit(key2) + component.TileSizeHalfVector;
        tupleList.Sort((IComparer<(NodeVis.GroupData, NodeVis.NodeDatum)>) NodeVisualizationOverlay.NodeDisplayComparer.Instance);
        float num = (float) ((double) -(tupleList.Count - 1) * (3.0 / 16.0) / 2.0);
        foreach ((NodeVis.GroupData groupData, NodeVis.NodeDatum nodeDatum) in tupleList)
        {
          Vector2 nodePos = vector2 + new Vector2(num, num);
          if (((Box2) ref box2_2).Contains(nodePos, true))
            this._hovered = new (int, int)?((groupData.NetId, nodeDatum.NetId));
          this._nodeIndex[(groupData.NetId, nodeDatum.NetId)] = new NodeVisualizationOverlay.NodeRenderData(groupData, nodeDatum, nodePos);
          num += 3f / 16f;
        }
      }
      ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2);
      foreach (NodeVisualizationOverlay.NodeRenderData nodeRenderData1 in this._nodeIndex.Values)
      {
        Vector2 nodePos = nodeRenderData1.NodePos;
        Box2 box2_3 = Box2.CenteredAround(nodePos, new Vector2(0.25f, 0.25f));
        NodeVis.GroupData groupData = nodeRenderData1.GroupData;
        Color color = groupData.Color;
        color.A = this._hovered.HasValue ? (this._hovered.Value.group == groupData.NetId ? (float) (0.75 + (double) MathF.Sin(this._time * 4f) * 0.25) : 0.2f) : 0.5f;
        worldHandle.DrawRect(box2_3, color, true);
        foreach (int num in nodeRenderData1.NodeDatum.Reachable)
        {
          NodeVisualizationOverlay.NodeRenderData nodeRenderData2;
          if (this._nodeIndex.TryGetValue((groupData.NetId, num), out nodeRenderData2))
            ((DrawingHandleBase) worldHandle).DrawLine(nodePos, nodeRenderData2.NodePos, color);
        }
      }
      this._nodeIndex.Clear();
    }
    DrawingHandleWorld drawingHandleWorld = worldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local = ref identity;
    ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local);
    this._gridIndex.Clear();
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    this._time += ((FrameEventArgs) ref args).DeltaSeconds;
  }

  private sealed class NodeDisplayComparer : IComparer<(NodeVis.GroupData, NodeVis.NodeDatum)>
  {
    public static readonly NodeVisualizationOverlay.NodeDisplayComparer Instance = new NodeVisualizationOverlay.NodeDisplayComparer();

    public int Compare(
      (NodeVis.GroupData, NodeVis.NodeDatum) x,
      (NodeVis.GroupData, NodeVis.NodeDatum) y)
    {
      (NodeVis.GroupData groupData1, NodeVis.NodeDatum nodeDatum1) = x;
      (NodeVis.GroupData groupData2, NodeVis.NodeDatum nodeDatum2) = y;
      int num = groupData1.NetId.CompareTo(groupData2.NetId);
      return num != 0 ? num : nodeDatum1.NetId.CompareTo(nodeDatum2.NetId);
    }
  }

  private sealed class NodeRenderData
  {
    public NodeVis.GroupData GroupData;
    public NodeVis.NodeDatum NodeDatum;
    public Vector2 NodePos;

    public NodeRenderData(
      NodeVis.GroupData groupData,
      NodeVis.NodeDatum nodeDatum,
      Vector2 nodePos)
    {
      this.GroupData = groupData;
      this.NodeDatum = nodeDatum;
      this.NodePos = nodePos;
    }
  }
}
