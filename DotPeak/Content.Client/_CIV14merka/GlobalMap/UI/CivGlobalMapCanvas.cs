// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.GlobalMap.UI.CivGlobalMapCanvas
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.Capture;
using Content.Client._CIV14merka.HeliSupply;
using Content.Client.Stylesheets;
using Content.Shared._CIV14merka.Capture;
using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.GlobalMap;
using Content.Shared._RMC14.Vehicle;
using Content.Shared.CCVar;
using Content.Shared.Maps;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.GlobalMap.UI;

public sealed class CivGlobalMapCanvas : Control
{
  [Dependency]
  private IResourceCache _resourceCache;
  [Dependency]
  private IEntityManager _entity;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IClyde _clyde;
  [Dependency]
  private ISharedPlayerManager _player;
  [Dependency]
  private IConfigurationManager _cfg;
  private readonly CivGlobalMapSystem _system;
  private readonly Font _font;
  private readonly Font _gridFont;
  private readonly SharedMapSystem _map;
  private readonly SharedTransformSystem _transform;
  private readonly TurfSystem _turf;
  private readonly VehicleSystem _vehicles;
  private readonly List<CivGlobalMapMarkerState> _markers = new List<CivGlobalMapMarkerState>();
  private readonly List<CivGlobalMapPlayerState> _players = new List<CivGlobalMapPlayerState>();
  private readonly List<CivPointCapturePointState> _points = new List<CivPointCapturePointState>();
  private readonly List<CivFobMarkerState> _fobs = new List<CivFobMarkerState>();
  private readonly List<CivCommanderOrderState> _orders = new List<CivCommanderOrderState>();
  private readonly List<CivGlobalMapDeathState> _deaths = new List<CivGlobalMapDeathState>();
  private readonly List<CivGlobalMapCanvas.MapTileDrawData> _tileCache = new List<CivGlobalMapCanvas.MapTileDrawData>();
  private readonly List<HeliMapBlip> _heliBlips = new List<HeliMapBlip>();
  private CivHeliSupplySystem? _heli;
  private MapId _mapId = MapId.Nullspace;
  private MapId _tileCacheMapId = MapId.Nullspace;
  private OwnedTexture? _tileTexture;
  private Vector2 _tileTextureWorldMin;
  private Vector2 _tileTextureWorldMax;
  private int _viewerTeamId;
  private int _viewerSquadId;
  private bool _hasBounds;
  private Vector2 _boundsMin;
  private Vector2 _boundsMax;
  private float _zoom = 1f;
  private Vector2? _viewCenter;
  private int _hoveredMarkerId;
  private int _pendingRightClickMarkerId;
  private bool _isDraggingMarker;
  private int _dragMarkerId;
  private Vector2 _dragStartLocalPosition;
  private bool _dragMoved;
  private bool _isPanning;
  private Vector2 _lastPanLocalPosition;
  private readonly Dictionary<string, Vector2> _playerSmoothed = new Dictionary<string, Vector2>();
  private readonly HashSet<string> _playerSeen = new HashSet<string>();
  private readonly List<string> _playerStale = new List<string>();
  private TimeSpan _lastPlayerLerpTime;
  private const float PlayerLerpTau = 0.45f;
  private const float PlayerSnapDist = 25f;
  private const float MinZoom = 1f;
  private const float MaxZoom = 4f;
  private const float ZoomStep = 1.12f;
  private const float MarkerDragThresholdPixels = 6f;
  private static readonly Color WallTileColor = Color.FromHex((ReadOnlySpan<char>) "#8B4513", new Color?());
  private static readonly Color EmptyTileColor = new Color((byte) 36, (byte) 45, (byte) 56, byte.MaxValue);

  public CivGlobalMapMarkerType? SelectedMarkerType { get; set; }

  public bool RemoveMode { get; set; }

  public int? CommanderSelectedSquadId { get; set; }

  public int? PendingCommanderOrderSquadId { get; set; }

  public CivCommanderOrderType? PendingCommanderOrderType { get; set; }

  public event Action? CommanderOrderPlaced;

  public CivGlobalMapCanvas(CivGlobalMapSystem system)
  {
    this._system = system;
    IoCManager.InjectDependencies<CivGlobalMapCanvas>(this);
    this._font = this._resourceCache.NotoStack("Bold");
    this._gridFont = this._resourceCache.NotoStack("Bold", 14);
    this._map = this._entity.System<SharedMapSystem>();
    this._transform = this._entity.System<SharedTransformSystem>();
    this._turf = this._entity.System<TurfSystem>();
    this._vehicles = this._entity.System<VehicleSystem>();
    this.MouseFilter = (Control.MouseFilterMode) 0;
    this.MinSize = new Vector2(300f, 300f);
  }

  public void UpdateData(
    MapId mapId,
    bool hasBounds,
    Vector2 boundsMin,
    Vector2 boundsMax,
    int viewerTeamId,
    int viewerSquadId,
    IReadOnlyList<CivGlobalMapMarkerState> markers,
    IReadOnlyList<CivGlobalMapPlayerState> players,
    IReadOnlyList<CivPointCapturePointState> points,
    IReadOnlyList<CivCommanderOrderState> orders,
    IReadOnlyList<CivGlobalMapDeathState> deaths,
    IReadOnlyList<CivFobMarkerState> fobs,
    int? commanderSelectedSquadId)
  {
    if (MapId.op_Inequality(this._mapId, mapId))
    {
      this._zoom = 1f;
      this._viewCenter = new Vector2?();
      this._hoveredMarkerId = 0;
      this._pendingRightClickMarkerId = 0;
      this._isDraggingMarker = false;
      this._dragMarkerId = 0;
      this._dragMoved = false;
      this._isPanning = false;
    }
    this._mapId = mapId;
    this._hasBounds = hasBounds;
    this._boundsMin = boundsMin;
    this._boundsMax = boundsMax;
    this._viewerTeamId = viewerTeamId;
    this._viewerSquadId = viewerSquadId;
    this.CommanderSelectedSquadId = commanderSelectedSquadId;
    this._markers.Clear();
    this._markers.AddRange((IEnumerable<CivGlobalMapMarkerState>) markers);
    this._players.Clear();
    this._players.AddRange((IEnumerable<CivGlobalMapPlayerState>) players);
    this._points.Clear();
    this._points.AddRange((IEnumerable<CivPointCapturePointState>) points);
    this._orders.Clear();
    this._orders.AddRange((IEnumerable<CivCommanderOrderState>) orders);
    this._deaths.Clear();
    this._deaths.AddRange((IEnumerable<CivGlobalMapDeathState>) deaths);
    this._fobs.Clear();
    this._fobs.AddRange((IEnumerable<CivFobMarkerState>) fobs);
    this.EnsureViewCenter();
    this.RefreshTileCache();
  }

  public void CenterOnSelf()
  {
    if (!this._hasBounds || MapId.op_Equality(this._mapId, MapId.Nullspace))
      return;
    Vector2 position;
    if (this.TryGetSelfPosition(out position))
      this._viewCenter = new Vector2?(position);
    else
      this._viewCenter = new Vector2?((this._boundsMin + this._boundsMax) * 0.5f);
  }

  protected virtual void KeyBindDown(GUIBoundKeyEventArgs args)
  {
    base.KeyBindDown(args);
    if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick))
    {
      if (this.RemoveMode)
        return;
      Vector2 localPosition = ((BoundKeyEventArgs) args).PointerLocation.Position - Vector2i.op_Implicit(this.GlobalPixelPosition);
      UIBox2 mapRect = this.GetMapRect();
      int markerId;
      if (!((UIBox2) ref mapRect).Contains(localPosition, true) || !this.TryGetMarkerAt(localPosition, out markerId, false))
        return;
      this._isDraggingMarker = true;
      this._dragMoved = false;
      this._dragMarkerId = markerId;
      this._dragStartLocalPosition = localPosition;
      ((BoundKeyEventArgs) args).Handle();
    }
    else
    {
      if (BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIRightClick))
        return;
      Vector2 localPosition = ((BoundKeyEventArgs) args).PointerLocation.Position - Vector2i.op_Implicit(this.GlobalPixelPosition);
      UIBox2 mapRect = this.GetMapRect();
      if (!((UIBox2) ref mapRect).Contains(localPosition, true))
        return;
      int markerId;
      if (this.TryGetMarkerAt(localPosition, out markerId, false))
      {
        this._pendingRightClickMarkerId = markerId;
        ((BoundKeyEventArgs) args).Handle();
      }
      else
      {
        this._pendingRightClickMarkerId = 0;
        this._isPanning = true;
        this._lastPanLocalPosition = localPosition;
        ((BoundKeyEventArgs) args).Handle();
      }
    }
  }

  protected virtual void KeyBindUp(GUIBoundKeyEventArgs args)
  {
    base.KeyBindUp(args);
    if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIRightClick))
    {
      if (this._isPanning)
      {
        this._isPanning = false;
        ((BoundKeyEventArgs) args).Handle();
      }
      else
      {
        if (this._pendingRightClickMarkerId != 0)
        {
          this._system.RequestRemoveMarker(this._pendingRightClickMarkerId);
          this._pendingRightClickMarkerId = 0;
        }
        ((BoundKeyEventArgs) args).Handle();
      }
    }
    else
    {
      if (BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick) || this._isPanning)
        return;
      Vector2 localPosition = ((BoundKeyEventArgs) args).PointerLocation.Position - Vector2i.op_Implicit(this.GlobalPixelPosition);
      if (this._isDraggingMarker)
      {
        Vector2 mapPosition;
        if (this._dragMoved && this._dragMarkerId != 0 && MapId.op_Inequality(this._mapId, MapId.Nullspace) && this.TryLocalToMapPosition(localPosition, out mapPosition))
          this._system.RequestMoveMarker(this._dragMarkerId, this._mapId, mapPosition);
        this._isDraggingMarker = false;
        this._dragMarkerId = 0;
        this._dragMoved = false;
        ((BoundKeyEventArgs) args).Handle();
      }
      else if (this.RemoveMode)
      {
        int markerId;
        if (this.TryGetMarkerAt(localPosition, out markerId, false))
          this._system.RequestRemoveMarker(markerId);
        ((BoundKeyEventArgs) args).Handle();
      }
      else
      {
        int? nullable = this.PendingCommanderOrderSquadId;
        if (nullable.HasValue)
        {
          int valueOrDefault1 = nullable.GetValueOrDefault();
          CivCommanderOrderType? commanderOrderType = this.PendingCommanderOrderType;
          if (commanderOrderType.HasValue)
          {
            CivCommanderOrderType valueOrDefault2 = commanderOrderType.GetValueOrDefault();
            if (MapId.op_Inequality(this._mapId, MapId.Nullspace))
            {
              Vector2 mapPosition;
              if (!this.TryLocalToMapPosition(localPosition, out mapPosition))
                return;
              this._system.RequestCommanderSetOrder(valueOrDefault1, valueOrDefault2, this._mapId, mapPosition);
              nullable = new int?();
              this.PendingCommanderOrderSquadId = nullable;
              this.PendingCommanderOrderType = new CivCommanderOrderType?();
              Action commanderOrderPlaced = this.CommanderOrderPlaced;
              if (commanderOrderPlaced != null)
                commanderOrderPlaced();
              ((BoundKeyEventArgs) args).Handle();
              return;
            }
          }
        }
        CivGlobalMapMarkerType? selectedMarkerType = this.SelectedMarkerType;
        Vector2 mapPosition1;
        if (!selectedMarkerType.HasValue || MapId.op_Equality(this._mapId, MapId.Nullspace) || !this.TryLocalToMapPosition(localPosition, out mapPosition1))
          return;
        CivGlobalMapSystem system = this._system;
        selectedMarkerType = this.SelectedMarkerType;
        int type = (int) selectedMarkerType.Value;
        MapId mapId = this._mapId;
        Vector2 position = mapPosition1;
        system.RequestPlaceMarker((CivGlobalMapMarkerType) type, mapId, position);
        ((BoundKeyEventArgs) args).Handle();
      }
    }
  }

  protected virtual void MouseMove(GUIMouseMoveEventArgs args)
  {
    base.MouseMove(args);
    Vector2 relativePosition = ((GUIMouseEventArgs) args).RelativePosition;
    this.UpdateHoveredMarker(relativePosition);
    if (this._isDraggingMarker)
    {
      float num = 36f * this.UIScale * this.UIScale;
      if ((double) (relativePosition - this._dragStartLocalPosition).LengthSquared() >= (double) num)
        this._dragMoved = true;
      ((InputEventArgs) args).Handle();
    }
    else
    {
      Vector2 visibleMin;
      Vector2 visibleMax;
      if (!this._isPanning || !this.TryGetVisibleBounds(out visibleMin, out visibleMax))
        return;
      Vector2 vector2_1 = relativePosition - this._lastPanLocalPosition;
      this._lastPanLocalPosition = relativePosition;
      UIBox2 mapRect = this.GetMapRect();
      if ((double) ((UIBox2) ref mapRect).Width <= 0.0 || (double) ((UIBox2) ref mapRect).Height <= 0.0)
        return;
      Vector2 vector2_2 = visibleMax - visibleMin;
      Vector2 vector2_3 = new Vector2(-vector2_1.X / ((UIBox2) ref mapRect).Width * vector2_2.X, vector2_1.Y / ((UIBox2) ref mapRect).Height * vector2_2.Y);
      this._viewCenter.GetValueOrDefault();
      if (!this._viewCenter.HasValue)
        this._viewCenter = new Vector2?((visibleMin + visibleMax) * 0.5f);
      Vector2? viewCenter = this._viewCenter;
      Vector2 vector2_4 = vector2_3;
      this._viewCenter = viewCenter.HasValue ? new Vector2?(viewCenter.GetValueOrDefault() + vector2_4) : new Vector2?();
      ((InputEventArgs) args).Handle();
    }
  }

  protected virtual void MouseWheel(GUIMouseWheelEventArgs args)
  {
    base.MouseWheel(args);
    if (!this._hasBounds || MapId.op_Equality(this._mapId, MapId.Nullspace))
      return;
    Vector2 relativePosition = ((GUIMouseEventArgs) args).RelativePosition;
    Vector2 mapPosition1;
    bool mapPosition2 = this.TryLocalToMapPosition(relativePosition, out mapPosition1);
    float zoom = this._zoom;
    if ((double) args.Delta.Y > 0.0)
      this._zoom *= 1.12f;
    else if ((double) args.Delta.Y < 0.0)
      this._zoom /= 1.12f;
    this._zoom = Math.Clamp(this._zoom, 1f, 4f);
    if ((double) Math.Abs(this._zoom - zoom) <= 9.9999997473787516E-05)
      return;
    Vector2 mapPosition3;
    if (mapPosition2 && this.TryLocalToMapPosition(relativePosition, out mapPosition3))
    {
      this._viewCenter.GetValueOrDefault();
      if (!this._viewCenter.HasValue)
        this._viewCenter = new Vector2?((this._boundsMin + this._boundsMax) * 0.5f);
      Vector2? viewCenter = this._viewCenter;
      Vector2 vector2 = mapPosition1 - mapPosition3;
      this._viewCenter = viewCenter.HasValue ? new Vector2?(viewCenter.GetValueOrDefault() + vector2) : new Vector2?();
    }
    ((InputEventArgs) args).Handle();
  }

  protected virtual void Draw(DrawingHandleScreen handle)
  {
    base.Draw(handle);
    handle.DrawRect(UIBox2i.op_Implicit(this.PixelSizeBox), new Color((byte) 13, (byte) 18, (byte) 24, byte.MaxValue), true);
    UIBox2 mapRect = this.GetMapRect();
    if ((double) ((UIBox2) ref mapRect).Width <= 1.0 || (double) ((UIBox2) ref mapRect).Height <= 1.0)
      return;
    handle.DrawRect(mapRect, new Color((byte) 21, (byte) 28, (byte) 36, byte.MaxValue), true);
    handle.DrawRect(mapRect, new Color((byte) 78, (byte) 96 /*0x60*/, (byte) 118, byte.MaxValue), false);
    if (!this._hasBounds)
    {
      this.DrawGrid(handle, mapRect);
      this.DrawCenteredText(handle, mapRect, Loc.GetString("civ-gmap-canvas-no-map-data"));
    }
    else
    {
      this.DrawMapTiles(handle);
      this.DrawGrid(handle, mapRect);
      foreach (CivGlobalMapMarkerState marker in this._markers)
      {
        Vector2 localPosition;
        if (!marker.IsObjective && !MapId.op_Inequality(marker.MapId, this._mapId) && this.TryMapToLocalPosition(marker.Position, out localPosition))
          this.DrawMarker(handle, marker, localPosition);
      }
      foreach (CivPointCapturePointState point in this._points)
      {
        Vector2 localPosition;
        if (!MapId.op_Inequality(point.MapId, this._mapId) && this.TryMapToLocalPosition(point.Position, out localPosition))
          this.DrawPoint(handle, point, localPosition);
      }
      foreach (CivCommanderOrderState order in this._orders)
      {
        Vector2 localPosition;
        if (!MapId.op_Inequality(order.MapId, this._mapId) && this.TryMapToLocalPosition(order.Position, out localPosition))
          this.DrawCommanderOrder(handle, order, localPosition);
      }
      foreach (CivFobMarkerState fob in this._fobs)
      {
        Vector2 localPosition;
        if (!MapId.op_Inequality(fob.MapId, this._mapId) && this.TryMapToLocalPosition(fob.Position, out localPosition))
          this.DrawFob(handle, localPosition);
      }
      CivGlobalMapMarkerState marker1;
      Vector2 localPosition1;
      if (this._hoveredMarkerId != 0 && this.TryGetMarkerById(this._hoveredMarkerId, out marker1) && MapId.op_Equality(marker1.MapId, this._mapId) && this.TryMapToLocalPosition(marker1.Position, out localPosition1))
        this.DrawMarkerHoverInfo(handle, marker1, localPosition1);
      foreach (CivGlobalMapDeathState death in this._deaths)
      {
        Vector2 localPosition2;
        if (!MapId.op_Inequality(death.MapId, this._mapId) && this.TryMapToLocalPosition(death.Position, out localPosition2))
          this.DrawDeath(handle, death, localPosition2);
      }
      TimeSpan curTime = this._timing.CurTime;
      float totalSeconds = (float) (curTime - this._lastPlayerLerpTime).TotalSeconds;
      this._lastPlayerLerpTime = curTime;
      float amount = (double) totalSeconds <= 0.0 ? 1f : 1f - MathF.Exp((float) (-(double) totalSeconds / 0.44999998807907104));
      this._playerSeen.Clear();
      foreach (CivGlobalMapPlayerState player in this._players)
      {
        if (!MapId.op_Inequality(player.MapId, this._mapId))
        {
          Vector2 mapPosition;
          if (player.IsSelf)
          {
            EntityUid? localEntity = this._player.LocalEntity;
            MapCoordinates coordinates;
            mapPosition = !localEntity.HasValue || !this._vehicles.TryGetDisplayMapCoordinates(localEntity.GetValueOrDefault(), out coordinates) || !MapId.op_Equality(coordinates.MapId, this._mapId) ? player.Position : coordinates.Position;
          }
          else
          {
            Vector2 position = player.Position;
            Vector2 vector2;
            mapPosition = !this._playerSmoothed.TryGetValue(player.Name, out vector2) ? position : ((double) (position - vector2).Length() > 25.0 ? position : Vector2.Lerp(vector2, position, amount));
            this._playerSmoothed[player.Name] = mapPosition;
            this._playerSeen.Add(player.Name);
          }
          Vector2 localPosition3;
          if (this.TryMapToLocalPosition(mapPosition, out localPosition3))
            this.DrawPlayer(handle, player, localPosition3);
        }
      }
      if (this._playerSmoothed.Count != this._playerSeen.Count)
      {
        this._playerStale.Clear();
        foreach (string key in this._playerSmoothed.Keys)
        {
          if (!this._playerSeen.Contains(key))
            this._playerStale.Add(key);
        }
        foreach (string key in this._playerStale)
          this._playerSmoothed.Remove(key);
      }
      if (this._heli == null)
        this._heli = this._entity.System<CivHeliSupplySystem>();
      this._heliBlips.Clear();
      this._heli.GetActiveHeliBlips(this._mapId, this._heliBlips);
      foreach (HeliMapBlip heliBlip in this._heliBlips)
      {
        Vector2 localPosition4;
        if (this.TryMapToLocalPosition(heliBlip.Position, out localPosition4))
          this.DrawHeli(handle, localPosition4, heliBlip.Heading, heliBlip.Side);
      }
      foreach (CivGlobalMapMarkerState marker2 in this._markers)
      {
        Vector2 localPosition5;
        if (marker2.IsObjective && !MapId.op_Inequality(marker2.MapId, this._mapId) && this.TryMapToLocalPosition(marker2.Position, out localPosition5))
          this.DrawMarker(handle, marker2, localPosition5);
      }
    }
  }

  private void DrawGrid(DrawingHandleScreen handle, UIBox2 mapRect)
  {
    Color color;
    // ISSUE: explicit constructor call
    ((Color) ref color).\u002Ector((byte) 66, (byte) 79, (byte) 97, (byte) 130);
    Vector2 visibleMin;
    Vector2 visibleMax;
    if (!this._hasBounds || !this.TryGetVisibleBounds(out visibleMin, out visibleMax))
    {
      for (int index = 1; index < 4; ++index)
      {
        float num = (float) index / 4f;
        float x = MathHelper.Lerp(mapRect.Left, mapRect.Right, num);
        float y = MathHelper.Lerp(mapRect.Top, mapRect.Bottom, num);
        ((DrawingHandleBase) handle).DrawLine(new Vector2(x, mapRect.Top), new Vector2(x, mapRect.Bottom), color);
        ((DrawingHandleBase) handle).DrawLine(new Vector2(mapRect.Left, y), new Vector2(mapRect.Right, y), color);
      }
    }
    else
    {
      Vector2 visibleSize = visibleMax - visibleMin;
      if ((double) visibleSize.X <= 0.0 || (double) visibleSize.Y <= 0.0)
        return;
      for (int i = 0; i <= 8; ++i)
      {
        float num = CivMapGrid.LineX(this._boundsMin, this._boundsMax, i);
        if ((double) num >= (double) visibleMin.X && (double) num <= (double) visibleMax.X)
        {
          float x = mapRect.Left + (num - visibleMin.X) / visibleSize.X * ((UIBox2) ref mapRect).Width;
          ((DrawingHandleBase) handle).DrawLine(new Vector2(x, mapRect.Top), new Vector2(x, mapRect.Bottom), color);
        }
      }
      for (int i = 0; i <= 8; ++i)
      {
        float num = CivMapGrid.LineY(this._boundsMin, this._boundsMax, i);
        if ((double) num >= (double) visibleMin.Y && (double) num <= (double) visibleMax.Y)
        {
          float y = mapRect.Bottom - (num - visibleMin.Y) / visibleSize.Y * ((UIBox2) ref mapRect).Height;
          ((DrawingHandleBase) handle).DrawLine(new Vector2(mapRect.Left, y), new Vector2(mapRect.Right, y), color);
        }
      }
      this.DrawGridLabels(handle, mapRect, visibleMin, visibleMax, visibleSize);
    }
  }

  private void DrawGridLabels(
    DrawingHandleScreen handle,
    UIBox2 mapRect,
    Vector2 visibleMin,
    Vector2 visibleMax,
    Vector2 visibleSize)
  {
    Color color = Color.FromHex((ReadOnlySpan<char>) "#FFE9A8", new Color?());
    for (int col = 0; col < 8; ++col)
    {
      float num1 = CivMapGrid.ColumnCenterX(this._boundsMin, this._boundsMax, col);
      if ((double) num1 >= (double) visibleMin.X && (double) num1 <= (double) visibleMax.X)
      {
        float num2 = mapRect.Left + (num1 - visibleMin.X) / visibleSize.X * ((UIBox2) ref mapRect).Width;
        string columnLabel = CivMapGrid.GetColumnLabel(col);
        Vector2 dimensions = handle.GetDimensions(this._gridFont, (ReadOnlySpan<char>) columnLabel, this.UIScale);
        this.DrawLabelOutlined(handle, new Vector2(num2 - dimensions.X / 2f, mapRect.Top + 2f * this.UIScale), columnLabel, color);
      }
    }
    for (int row = 0; row < 8; ++row)
    {
      float num3 = CivMapGrid.RowCenterY(this._boundsMin, this._boundsMax, row);
      if ((double) num3 >= (double) visibleMin.Y && (double) num3 <= (double) visibleMax.Y)
      {
        float num4 = mapRect.Bottom - (num3 - visibleMin.Y) / visibleSize.Y * ((UIBox2) ref mapRect).Height;
        string text = (row + 1).ToString();
        Vector2 dimensions = handle.GetDimensions(this._gridFont, (ReadOnlySpan<char>) text, this.UIScale);
        this.DrawLabelOutlined(handle, new Vector2(mapRect.Left + 2f * this.UIScale, num4 - dimensions.Y / 2f), text, color);
      }
    }
  }

  private void DrawLabelOutlined(
    DrawingHandleScreen handle,
    Vector2 pos,
    string text,
    Color color)
  {
    Color black = Color.Black;
    Color color1 = ((Color) ref black).WithAlpha(0.95f);
    handle.DrawString(this._gridFont, pos + new Vector2(-1f, 0.0f), (ReadOnlySpan<char>) text, this.UIScale, color1);
    handle.DrawString(this._gridFont, pos + new Vector2(1f, 0.0f), (ReadOnlySpan<char>) text, this.UIScale, color1);
    handle.DrawString(this._gridFont, pos + new Vector2(0.0f, -1f), (ReadOnlySpan<char>) text, this.UIScale, color1);
    handle.DrawString(this._gridFont, pos + new Vector2(0.0f, 1f), (ReadOnlySpan<char>) text, this.UIScale, color1);
    handle.DrawString(this._gridFont, pos, (ReadOnlySpan<char>) text, this.UIScale, color);
  }

  private void DrawDeath(
    DrawingHandleScreen handle,
    CivGlobalMapDeathState death,
    Vector2 deathPos)
  {
    float num = (float) (0.25 + 0.64999997615814209 * ((double) death.LifetimeSeconds > 0.0 ? (double) Math.Clamp(death.RemainingSeconds / death.LifetimeSeconds, 0.0f, 1f) : 1.0));
    Color playerColor = CivGlobalMapColorResolver.GetPlayerColor(this._viewerTeamId, this._viewerSquadId, death.TeamId, death.SquadId);
    Color color = ((Color) ref playerColor).WithAlpha(num);
    ((DrawingHandleBase) handle).DrawLine(deathPos + new Vector2(-5f, -5f), deathPos + new Vector2(5f, 5f), color);
    ((DrawingHandleBase) handle).DrawLine(deathPos + new Vector2(-5f, 5f), deathPos + new Vector2(5f, -5f), color);
    ((DrawingHandleBase) handle).DrawCircle(deathPos, 7f, ((Color) ref color).WithAlpha(num * 0.5f), false);
  }

  private void DrawMapTiles(DrawingHandleScreen handle)
  {
    Vector2 visibleMin;
    Vector2 visibleMax;
    if (this._tileCache.Count == 0 || !this.TryGetVisibleBounds(out visibleMin, out visibleMax))
      return;
    UIBox2 mapRect = this.GetMapRect();
    Vector2 vector2_1 = visibleMax - visibleMin;
    if ((double) vector2_1.X <= 0.0 || (double) vector2_1.Y <= 0.0)
      return;
    if (this._tileTexture != null)
    {
      Vector2 vector2_2 = Vector2.Max(visibleMin, this._tileTextureWorldMin);
      Vector2 vector2_3 = Vector2.Min(visibleMax, this._tileTextureWorldMax);
      if ((double) vector2_2.X >= (double) vector2_3.X || (double) vector2_2.Y >= (double) vector2_3.Y)
        return;
      UIBox2 uiBox2_1;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_1).\u002Ector(mapRect.Left + (vector2_2.X - visibleMin.X) / vector2_1.X * ((UIBox2) ref mapRect).Width, mapRect.Bottom - (vector2_3.Y - visibleMin.Y) / vector2_1.Y * ((UIBox2) ref mapRect).Height, mapRect.Left + (vector2_3.X - visibleMin.X) / vector2_1.X * ((UIBox2) ref mapRect).Width, mapRect.Bottom - (vector2_2.Y - visibleMin.Y) / vector2_1.Y * ((UIBox2) ref mapRect).Height);
      Vector2 vector2_4 = this._tileTextureWorldMax - this._tileTextureWorldMin;
      Vector2i size = ((Texture) this._tileTexture).Size;
      UIBox2 uiBox2_2;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_2).\u002Ector((vector2_2.X - this._tileTextureWorldMin.X) / vector2_4.X * (float) size.X, (this._tileTextureWorldMax.Y - vector2_3.Y) / vector2_4.Y * (float) size.Y, (vector2_3.X - this._tileTextureWorldMin.X) / vector2_4.X * (float) size.X, (this._tileTextureWorldMax.Y - vector2_2.Y) / vector2_4.Y * (float) size.Y);
      handle.DrawTextureRectRegion((Texture) this._tileTexture, uiBox2_1, new UIBox2?(uiBox2_2), new Color?());
    }
    else
    {
      Vector2 vector2_5 = new Vector2((float) ((double) ((UIBox2) ref mapRect).Width / (double) MathF.Max(1f, vector2_1.X) * 0.5), (float) ((double) ((UIBox2) ref mapRect).Height / (double) MathF.Max(1f, vector2_1.Y) * 0.5));
      foreach (CivGlobalMapCanvas.MapTileDrawData mapTileDrawData in this._tileCache)
      {
        Vector2 localPosition;
        if ((double) mapTileDrawData.WorldPosition.X >= (double) visibleMin.X - 1.0 && (double) mapTileDrawData.WorldPosition.X <= (double) visibleMax.X + 1.0 && (double) mapTileDrawData.WorldPosition.Y >= (double) visibleMin.Y - 1.0 && (double) mapTileDrawData.WorldPosition.Y <= (double) visibleMax.Y + 1.0 && this.TryMapToLocalPosition(mapTileDrawData.WorldPosition, out localPosition))
        {
          UIBox2 uiBox2_3;
          // ISSUE: explicit constructor call
          ((UIBox2) ref uiBox2_3).\u002Ector(localPosition - vector2_5, localPosition + vector2_5);
          Color color1 = mapTileDrawData.IsWall ? CivGlobalMapCanvas.WallTileColor : mapTileDrawData.Color;
          handle.DrawRect(uiBox2_3, color1, true);
          if (mapTileDrawData.IsWall)
          {
            DrawingHandleScreen drawingHandleScreen = handle;
            UIBox2 uiBox2_4 = uiBox2_3;
            Color black = Color.Black;
            Color color2 = ((Color) ref black).WithAlpha(0.72f);
            drawingHandleScreen.DrawRect(uiBox2_4, color2, false);
          }
        }
      }
    }
  }

  private void RefreshTileCache()
  {
    if (!this._hasBounds || MapId.op_Equality(this._mapId, MapId.Nullspace))
    {
      this._tileCache.Clear();
      this._tileCacheMapId = MapId.Nullspace;
      this.ClearTileTexture();
    }
    else
    {
      if (MapId.op_Equality(this._tileCacheMapId, this._mapId) && this._tileCache.Count > 0)
        return;
      this._tileCache.Clear();
      this._tileCacheMapId = this._mapId;
      EntityQueryEnumerator<MapGridComponent, TransformComponent> entityQueryEnumerator = this._entity.EntityQueryEnumerator<MapGridComponent, TransformComponent>();
      EntityUid entityUid;
      MapGridComponent mapGridComponent;
      TransformComponent transformComponent;
      while (entityQueryEnumerator.MoveNext(ref entityUid, ref mapGridComponent, ref transformComponent))
      {
        if (!MapId.op_Inequality(transformComponent.MapID, this._mapId))
        {
          Vector2 worldPosition = this._transform.GetWorldPosition(transformComponent);
          GridTileEnumerator allTilesEnumerator = this._map.GetAllTilesEnumerator(entityUid, mapGridComponent, true);
          HashSet<Vector2i> vector2iSet = new HashSet<Vector2i>();
          List<(Vector2i, Vector2, Color)> valueTupleList = new List<(Vector2i, Vector2, Color)>();
          TileRef? nullable;
          while (((GridTileEnumerator) ref allTilesEnumerator).MoveNext(ref nullable))
          {
            Vector2i gridIndices = nullable.Value.GridIndices;
            Vector2 vector2 = worldPosition + Vector2i.op_Implicit(gridIndices);
            Color color = this._turf.GetContentTileDefinition(nullable.Value.Tile).MinimapColor;
            if ((double) color.A == 0.0)
              color = CivGlobalMapCanvas.EmptyTileColor;
            vector2iSet.Add(gridIndices);
            valueTupleList.Add((gridIndices, vector2, color));
          }
          foreach ((Vector2i, Vector2, Color) valueTuple in valueTupleList)
          {
            bool IsWall = false;
            for (int index1 = -1; index1 <= 1 && !IsWall; ++index1)
            {
              for (int index2 = -1; index2 <= 1; ++index2)
              {
                if (index1 != 0 || index2 != 0)
                {
                  Vector2i vector2i;
                  // ISSUE: explicit constructor call
                  ((Vector2i) ref vector2i).\u002Ector(valueTuple.Item1.X + index1, valueTuple.Item1.Y + index2);
                  if (!vector2iSet.Contains(vector2i))
                  {
                    IsWall = true;
                    break;
                  }
                }
              }
            }
            this._tileCache.Add(new CivGlobalMapCanvas.MapTileDrawData(valueTuple.Item2, valueTuple.Item3, IsWall));
          }
        }
      }
      this.BakeTileTexture();
    }
  }

  private void BakeTileTexture()
  {
    this.ClearTileTexture();
    if (this._tileCache.Count == 0)
      return;
    Vector2 vector2_1 = this._tileCache[0].WorldPosition;
    Vector2 vector2_2 = vector2_1;
    foreach (CivGlobalMapCanvas.MapTileDrawData mapTileDrawData in this._tileCache)
    {
      vector2_1 = Vector2.Min(vector2_1, mapTileDrawData.WorldPosition);
      vector2_2 = Vector2.Max(vector2_2, mapTileDrawData.WorldPosition);
    }
    float num1 = MathF.Max(vector2_2.X - vector2_1.X, vector2_2.Y - vector2_1.Y) + 1f;
    if ((double) num1 <= 0.0 || (double) num1 > 4096.0)
      return;
    int num2 = Math.Clamp((int) (4096.0 / (double) num1), 1, 4);
    int num3 = (int) MathF.Ceiling((float) ((double) vector2_2.X - (double) vector2_1.X + 1.0) * (float) num2);
    int num4 = (int) MathF.Ceiling((float) ((double) vector2_2.Y - (double) vector2_1.Y + 1.0) * (float) num2);
    if (num3 <= 0 || num4 <= 0)
      return;
    Rgba32 rgba32_1;
    // ISSUE: explicit constructor call
    ((Rgba32) ref rgba32_1).\u002Ector(((Color) ref CivGlobalMapCanvas.WallTileColor).RByte, ((Color) ref CivGlobalMapCanvas.WallTileColor).GByte, ((Color) ref CivGlobalMapCanvas.WallTileColor).BByte, ((Color) ref CivGlobalMapCanvas.WallTileColor).AByte);
    Color color1 = Color.InterpolateBetween(CivGlobalMapCanvas.WallTileColor, Color.Black, 0.72f);
    Rgba32 rgba32_2;
    // ISSUE: explicit constructor call
    ((Rgba32) ref rgba32_2).\u002Ector(((Color) ref color1).RByte, ((Color) ref color1).GByte, ((Color) ref color1).BByte, byte.MaxValue);
    using (Image<Rgba32> image = new Image<Rgba32>(num3, num4))
    {
      foreach (CivGlobalMapCanvas.MapTileDrawData mapTileDrawData in this._tileCache)
      {
        Rgba32 rgba32_3;
        if (!mapTileDrawData.IsWall)
        {
          Color color2 = mapTileDrawData.Color;
          int rbyte = (int) ((Color) ref color2).RByte;
          Color color3 = mapTileDrawData.Color;
          int gbyte = (int) ((Color) ref color3).GByte;
          Color color4 = mapTileDrawData.Color;
          int bbyte = (int) ((Color) ref color4).BByte;
          Color color5 = mapTileDrawData.Color;
          int abyte = (int) ((Color) ref color5).AByte;
          rgba32_3 = new Rgba32((byte) rbyte, (byte) gbyte, (byte) bbyte, (byte) abyte);
        }
        else
          rgba32_3 = rgba32_1;
        Rgba32 rgba32_4 = rgba32_3;
        int num5 = (int) MathF.Round((mapTileDrawData.WorldPosition.X - vector2_1.X) * (float) num2);
        int num6 = (int) MathF.Round((vector2_2.Y - mapTileDrawData.WorldPosition.Y) * (float) num2);
        for (int index1 = 0; index1 < num2; ++index1)
        {
          for (int index2 = 0; index2 < num2; ++index2)
          {
            int num7 = num5 + index1;
            int num8 = num6 + index2;
            if (num7 >= 0 && num8 >= 0 && num7 < num3 && num8 < num4)
            {
              bool flag = mapTileDrawData.IsWall && num2 >= 3 && (index1 == 0 || index2 == 0 || index1 == num2 - 1 || index2 == num2 - 1);
              image[num7, num8] = flag ? rgba32_2 : rgba32_4;
            }
          }
        }
      }
      this._tileTexture = this._clyde.LoadTextureFromImage<Rgba32>(image, "civ-gmap-tiles", new TextureLoadParameters?());
      this._tileTextureWorldMin = vector2_1 - new Vector2(0.5f, 0.5f);
      this._tileTextureWorldMax = vector2_2 + new Vector2(0.5f, 0.5f);
    }
  }

  private void ClearTileTexture()
  {
    this._tileTexture?.Dispose();
    this._tileTexture = (OwnedTexture) null;
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    this.ClearTileTexture();
  }

  private void DrawMarker(
    DrawingHandleScreen handle,
    CivGlobalMapMarkerState marker,
    Vector2 markerPos)
  {
    Color color1 = CivGlobalMapColorResolver.GetColor(marker.Type);
    float num1 = marker.Type.IsGlobal() ? 7f : 5f;
    if (marker.IsObjective)
    {
      this.DrawObjectiveMarker(handle, marker, markerPos, color1);
    }
    else
    {
      if (marker.Type.IsGlobal())
      {
        Vector2 vector2_1 = markerPos + new Vector2(0.0f, (float) (-(double) num1 * 1.5));
        Vector2 vector2_2 = markerPos + new Vector2(-num1, num1);
        Vector2 vector2_3 = markerPos + new Vector2(num1, num1);
        ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 1, (ReadOnlySpan<Vector2>) new Vector2[3]
        {
          vector2_1,
          vector2_2,
          vector2_3
        }, color1);
        ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 5, (ReadOnlySpan<Vector2>) new Vector2[4]
        {
          vector2_1,
          vector2_2,
          vector2_3,
          vector2_1
        }, Color.Black);
      }
      else
      {
        ((DrawingHandleBase) handle).DrawCircle(markerPos, num1, color1, true);
        ((DrawingHandleBase) handle).DrawCircle(markerPos, num1 + 1f, Color.Black, false);
      }
      if (this.RemoveMode && !marker.IsObjective)
      {
        DrawingHandleScreen drawingHandleScreen = handle;
        Vector2 vector2 = markerPos;
        double num2 = (double) num1 + 4.0;
        Color white = Color.White;
        Color color2 = ((Color) ref white).WithAlpha(0.8f);
        ((DrawingHandleBase) drawingHandleScreen).DrawCircle(vector2, (float) num2, color2, false);
      }
      string shortText = CivGlobalMapColorResolver.GetShortText(marker.Type);
      Vector2 dimensions = handle.GetDimensions(this._font, (ReadOnlySpan<char>) shortText, this.UIScale);
      Vector2 vector2_4 = markerPos - dimensions / 2f;
      handle.DrawString(this._font, vector2_4, (ReadOnlySpan<char>) shortText, this.UIScale, Color.White);
    }
  }

  private void DrawCommanderOrder(
    DrawingHandleScreen handle,
    CivCommanderOrderState order,
    Vector2 orderPos)
  {
    Color color1;
    switch (order.Order)
    {
      case CivCommanderOrderType.Attack:
        color1 = Color.FromHex((ReadOnlySpan<char>) "#ff5449", new Color?());
        break;
      case CivCommanderOrderType.Defense:
        color1 = Color.FromHex((ReadOnlySpan<char>) "#5ca8ff", new Color?());
        break;
      case CivCommanderOrderType.Artillery:
        color1 = Color.FromHex((ReadOnlySpan<char>) "#ffd85a", new Color?());
        break;
      default:
        color1 = Color.White;
        break;
    }
    Color color2 = color1;
    ((DrawingHandleBase) handle).DrawCircle(orderPos, 11f, ((Color) ref color2).WithAlpha(0.18f), true);
    ((DrawingHandleBase) handle).DrawCircle(orderPos, 11f, color2, false);
    DrawingHandleScreen drawingHandleScreen1 = handle;
    Vector2 vector2_1 = orderPos;
    Color black1 = Color.Black;
    Color color3 = ((Color) ref black1).WithAlpha(0.8f);
    ((DrawingHandleBase) drawingHandleScreen1).DrawCircle(vector2_1, 13f, color3, false);
    string str1;
    switch (order.Order)
    {
      case CivCommanderOrderType.Attack:
        str1 = "ATK";
        break;
      case CivCommanderOrderType.Defense:
        str1 = "DEF";
        break;
      case CivCommanderOrderType.Artillery:
        str1 = "ART";
        break;
      default:
        str1 = "ORD";
        break;
    }
    string str2 = str1;
    Vector2 dimensions1 = handle.GetDimensions(this._font, (ReadOnlySpan<char>) str2, this.UIScale);
    Vector2 vector2_2 = orderPos - dimensions1 / 2f;
    handle.DrawString(this._font, vector2_2, (ReadOnlySpan<char>) str2, this.UIScale, Color.White);
    string str3 = $"{order.SquadLabel} {str2}";
    Vector2 dimensions2 = handle.GetDimensions(this._font, (ReadOnlySpan<char>) str3, this.UIScale);
    Vector2 vector2_3 = orderPos + new Vector2((float) (-(double) dimensions2.X / 2.0), (float) (11.0 + 4.0 * (double) this.UIScale));
    DrawingHandleScreen drawingHandleScreen2 = handle;
    Font font = this._font;
    Vector2 vector2_4 = vector2_3 + new Vector2(1f, 1f);
    ReadOnlySpan<char> readOnlySpan = (ReadOnlySpan<char>) str3;
    double uiScale = (double) this.UIScale;
    Color black2 = Color.Black;
    Color color4 = ((Color) ref black2).WithAlpha(0.85f);
    drawingHandleScreen2.DrawString(font, vector2_4, readOnlySpan, (float) uiScale, color4);
    handle.DrawString(this._font, vector2_3, (ReadOnlySpan<char>) str3, this.UIScale, color2);
  }

  private void DrawFob(DrawingHandleScreen handle, Vector2 fobPos)
  {
    Color color = Color.FromHex((ReadOnlySpan<char>) "#43d96b", new Color?());
    UIBox2 uiBox2;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2).\u002Ector(fobPos - new Vector2(9f, 9f), fobPos + new Vector2(9f, 9f));
    handle.DrawRect(uiBox2, ((Color) ref color).WithAlpha(0.25f), true);
    handle.DrawRect(uiBox2, color, false);
    string str = "FOB";
    Vector2 dimensions = handle.GetDimensions(this._font, (ReadOnlySpan<char>) str, this.UIScale);
    Vector2 vector2 = fobPos - dimensions / 2f;
    handle.DrawString(this._font, vector2, (ReadOnlySpan<char>) str, this.UIScale, Color.White);
  }

  private void DrawPoint(
    DrawingHandleScreen handle,
    CivPointCapturePointState point,
    Vector2 pointPos)
  {
    Color relationColor1 = CivPointCaptureColorResolver.GetRelationColor(this._viewerTeamId, point.OwnerTeamId);
    bool flag = point.CapturingTeamId != 0 && (double) point.CaptureProgress > 0.0;
    TimeSpan curTime;
    Color color1;
    if (!flag)
    {
      color1 = relationColor1;
    }
    else
    {
      int viewerTeamId = this._viewerTeamId;
      int ownerTeamId = point.OwnerTeamId;
      int capturingTeamId = point.CapturingTeamId;
      curTime = this._timing.CurTime;
      double totalSeconds = curTime.TotalSeconds;
      color1 = CivPointCaptureColorResolver.GetCapturePulseColor(viewerTeamId, ownerTeamId, capturingTeamId, (float) totalSeconds);
    }
    Color color2 = color1;
    double num1;
    if (!flag)
    {
      num1 = 0.0;
    }
    else
    {
      curTime = this._timing.CurTime;
      num1 = (double) CivPointCaptureColorResolver.GetCapturePulseAmount((float) curTime.TotalSeconds);
    }
    float num2 = (float) (24.0 + num1 * 4.0);
    ((DrawingHandleBase) handle).DrawCircle(pointPos, num2, ((Color) ref color2).WithAlpha(0.18f), true);
    ((DrawingHandleBase) handle).DrawCircle(pointPos, num2, ((Color) ref color2).WithAlpha(0.98f), false);
    ((DrawingHandleBase) handle).DrawCircle(pointPos, 15f, ((Color) ref color2).WithAlpha(0.8f), false);
    DrawingHandleScreen drawingHandleScreen1 = handle;
    Vector2 vector2_1 = pointPos;
    double num3 = (double) num2 + 2.0;
    Color black = Color.Black;
    Color color3 = ((Color) ref black).WithAlpha(0.75f);
    ((DrawingHandleBase) drawingHandleScreen1).DrawCircle(vector2_1, (float) num3, color3, false);
    ((DrawingHandleBase) handle).DrawCircle(pointPos, 5f, relationColor1, true);
    ((DrawingHandleBase) handle).DrawCircle(pointPos, 6f, Color.Black, false);
    if (flag)
    {
      Color relationColor2 = CivPointCaptureColorResolver.GetRelationColor(this._viewerTeamId, point.CapturingTeamId);
      CivGlobalMapCanvas.DrawProgressArc(handle, pointPos, num2 + 5f, point.CaptureProgress, relationColor2);
    }
    string str = string.IsNullOrWhiteSpace(point.Label) ? "P" : point.Label;
    Vector2 dimensions = handle.GetDimensions(this._font, (ReadOnlySpan<char>) str, this.UIScale);
    Vector2 vector2_2 = pointPos - new Vector2(dimensions.X / 2f, (float) (24.0 + (double) dimensions.Y + 4.0 * (double) this.UIScale));
    DrawingHandleScreen drawingHandleScreen2 = handle;
    Font font = this._font;
    Vector2 vector2_3 = vector2_2 + new Vector2(1f, 1f);
    ReadOnlySpan<char> readOnlySpan = (ReadOnlySpan<char>) str;
    double uiScale = (double) this.UIScale;
    black = Color.Black;
    Color color4 = ((Color) ref black).WithAlpha(0.85f);
    drawingHandleScreen2.DrawString(font, vector2_3, readOnlySpan, (float) uiScale, color4);
    handle.DrawString(this._font, vector2_2, (ReadOnlySpan<char>) str, this.UIScale, flag ? color2 : relationColor1);
  }

  private static void DrawProgressArc(
    DrawingHandleScreen handle,
    Vector2 center,
    float radius,
    float progress,
    Color color)
  {
    progress = Math.Clamp(progress, 0.0f, 1f);
    if ((double) progress <= 0.0)
      return;
    int num = Math.Max(6, (int) MathF.Ceiling(28f * progress));
    Vector2[] vector2Array = new Vector2[num + 1];
    for (int index = 0; index <= num; ++index)
    {
      float x = (float) (6.2831854820251465 * (double) (progress * (float) index / (float) num) - 1.5707963705062866);
      vector2Array[index] = center + new Vector2(MathF.Cos(x), MathF.Sin(x)) * radius;
    }
    ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 5, (ReadOnlySpan<Vector2>) vector2Array, color);
  }

  private void DrawObjectiveMarker(
    DrawingHandleScreen handle,
    CivGlobalMapMarkerState marker,
    Vector2 markerPos,
    Color color)
  {
    ((DrawingHandleBase) handle).DrawCircle(markerPos, 28f, ((Color) ref color).WithAlpha(0.1f), true);
    ((DrawingHandleBase) handle).DrawCircle(markerPos, 28f, ((Color) ref color).WithAlpha(0.95f), false);
    ((DrawingHandleBase) handle).DrawCircle(markerPos, 18f, ((Color) ref color).WithAlpha(0.75f), false);
    DrawingHandleScreen drawingHandleScreen1 = handle;
    Vector2 vector2_1 = markerPos;
    Color black1 = Color.Black;
    Color color1 = ((Color) ref black1).WithAlpha(0.75f);
    ((DrawingHandleBase) drawingHandleScreen1).DrawCircle(vector2_1, 30f, color1, false);
    ((DrawingHandleBase) handle).DrawCircle(markerPos, 6f, color, true);
    ((DrawingHandleBase) handle).DrawCircle(markerPos, 7f, Color.Black, false);
    float num = 10f;
    ((DrawingHandleBase) handle).DrawLine(markerPos + new Vector2(-num, 0.0f), markerPos + new Vector2(num, 0.0f), color);
    ((DrawingHandleBase) handle).DrawLine(markerPos + new Vector2(0.0f, -num), markerPos + new Vector2(0.0f, num), color);
    string str = marker.Type == CivGlobalMapMarkerType.Attack ? "ATK" : "DEF";
    Vector2 dimensions = handle.GetDimensions(this._font, (ReadOnlySpan<char>) str, this.UIScale);
    Vector2 vector2_2 = markerPos - new Vector2(dimensions.X / 2f, (float) (28.0 + (double) dimensions.Y + 4.0 * (double) this.UIScale));
    DrawingHandleScreen drawingHandleScreen2 = handle;
    Font font = this._font;
    Vector2 vector2_3 = vector2_2 + new Vector2(1f, 1f);
    ReadOnlySpan<char> readOnlySpan = (ReadOnlySpan<char>) str;
    double uiScale = (double) this.UIScale;
    Color black2 = Color.Black;
    Color color2 = ((Color) ref black2).WithAlpha(0.85f);
    drawingHandleScreen2.DrawString(font, vector2_3, readOnlySpan, (float) uiScale, color2);
    handle.DrawString(this._font, vector2_2, (ReadOnlySpan<char>) str, this.UIScale, Color.White);
  }

  private void DrawMarkerHoverInfo(
    DrawingHandleScreen handle,
    CivGlobalMapMarkerState marker,
    Vector2 markerPos)
  {
    string str1;
    if (!marker.IsObjective)
    {
      if (marker.SquadId <= 0)
      {
        str1 = marker.PlacedByName;
      }
      else
      {
        string str2;
        if (!marker.PlacedBySquadLeader)
          str2 = $"[S{marker.SquadId}]";
        else
          str2 = $"[LS{marker.SquadId}]";
        string placedByName = marker.PlacedByName;
        str1 = $"{str2} {placedByName}";
      }
    }
    else
      str1 = marker.PlacedByName;
    string str3 = str1;
    Vector2 vector2_1 = markerPos + new Vector2(10f * this.UIScale, -22f * this.UIScale);
    DrawingHandleScreen drawingHandleScreen = handle;
    Font font = this._font;
    Vector2 vector2_2 = vector2_1 + new Vector2(1f, 1f);
    ReadOnlySpan<char> readOnlySpan = (ReadOnlySpan<char>) str3;
    double uiScale = (double) this.UIScale;
    Color black = Color.Black;
    Color color = ((Color) ref black).WithAlpha(0.85f);
    drawingHandleScreen.DrawString(font, vector2_2, readOnlySpan, (float) uiScale, color);
    handle.DrawString(this._font, vector2_1, (ReadOnlySpan<char>) str3, this.UIScale, Color.White);
  }

  private void DrawPlayer(
    DrawingHandleScreen handle,
    CivGlobalMapPlayerState player,
    Vector2 playerPos)
  {
    int? commanderSelectedSquadId = this.CommanderSelectedSquadId;
    int num1;
    if (commanderSelectedSquadId.HasValue)
    {
      int valueOrDefault = commanderSelectedSquadId.GetValueOrDefault();
      num1 = player.SquadId == valueOrDefault ? 1 : 0;
    }
    else
      num1 = 0;
    bool flag = num1 != 0;
    Color color1 = player.IsSelf ? Color.White : (flag ? CivGlobalMapColorResolver.SquadColor : CivGlobalMapColorResolver.GetPlayerColor(this._viewerTeamId, this._viewerSquadId, player.TeamId, player.SquadId));
    float num2 = player.IsSelf ? 7f : 6f;
    if (flag && !player.IsSelf)
      ((DrawingHandleBase) handle).DrawCircle(playerPos, num2 + 4f, ((Color) ref color1).WithAlpha(0.18f), true);
    if (player.IsSquadLeader && !player.IsSelf)
    {
      Vector2 vector2_1 = playerPos + new Vector2(0.0f, (float) (-(double) num2 * 1.3500000238418579));
      Vector2 vector2_2 = playerPos + new Vector2(-num2, num2);
      Vector2 vector2_3 = playerPos + new Vector2(num2, num2);
      ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 1, (ReadOnlySpan<Vector2>) new Vector2[3]
      {
        vector2_1,
        vector2_2,
        vector2_3
      }, color1);
      ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 5, (ReadOnlySpan<Vector2>) new Vector2[4]
      {
        vector2_1,
        vector2_2,
        vector2_3,
        vector2_1
      }, Color.Black);
    }
    else
    {
      ((DrawingHandleBase) handle).DrawCircle(playerPos, num2, color1, true);
      ((DrawingHandleBase) handle).DrawCircle(playerPos, num2 + 1f, Color.Black, false);
    }
    if ((player.IsSelf || this._cfg.GetCVar<bool>(CCVars.Civ14ShowForeignNames) ? 1 : (this._viewerSquadId == 0 ? 0 : (player.SquadId == this._viewerSquadId ? 1 : 0))) == 0)
      return;
    string str1;
    if (!player.IsSelf)
    {
      if (player.SquadId != 0)
      {
        if (!player.IsSquadLeader)
          str1 = $"[S{player.SquadId}]";
        else
          str1 = $"[LS{player.SquadId}]";
      }
      else
        str1 = Loc.GetString("civ-gmap-canvas-player-reserve");
    }
    else
      str1 = Loc.GetString("civ-gmap-canvas-player-self");
    string name = player.Name;
    string str2 = $"{str1} {name}";
    Vector2 vector2_4 = playerPos + new Vector2(9f * this.UIScale, -14f * this.UIScale);
    DrawingHandleScreen drawingHandleScreen = handle;
    Font font = this._font;
    Vector2 vector2_5 = vector2_4 + new Vector2(1f, 1f);
    ReadOnlySpan<char> readOnlySpan = (ReadOnlySpan<char>) str2;
    double uiScale = (double) this.UIScale;
    Color black = Color.Black;
    Color color2 = ((Color) ref black).WithAlpha(0.85f);
    drawingHandleScreen.DrawString(font, vector2_5, readOnlySpan, (float) uiScale, color2);
    handle.DrawString(this._font, vector2_4, (ReadOnlySpan<char>) str2, this.UIScale, color1);
  }

  private void DrawHeli(
    DrawingHandleScreen handle,
    Vector2 pos,
    Vector2 heading,
    CivAirstrikeSide side)
  {
    Color playerColor = CivGlobalMapColorResolver.GetPlayerColor(this._viewerTeamId, this._viewerSquadId, side == CivAirstrikeSide.Ru ? 2 : 1, 0);
    Vector2 vector2_1 = new Vector2(heading.X, -heading.Y);
    float x = (double) vector2_1.LengthSquared() > 9.9999997473787516E-05 ? MathF.Atan2(vector2_1.Y, vector2_1.X) : 0.0f;
    Vector2 vector2_2 = new Vector2(MathF.Cos(x), MathF.Sin(x));
    Vector2 vector2_3 = new Vector2(-vector2_2.Y, vector2_2.X);
    Vector2 vector2_4 = pos + vector2_2 * 8f;
    Vector2 vector2_5 = pos - vector2_2 * 4.8f + vector2_3 * 6f;
    Vector2 vector2_6 = pos - vector2_2 * 4.8f - vector2_3 * 6f;
    ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 1, (ReadOnlySpan<Vector2>) new Vector2[3]
    {
      vector2_4,
      vector2_5,
      vector2_6
    }, playerColor);
    ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 5, (ReadOnlySpan<Vector2>) new Vector2[4]
    {
      vector2_4,
      vector2_5,
      vector2_6,
      vector2_4
    }, Color.Black);
    string str = Loc.GetString("civ-gmap-canvas-heli");
    Vector2 vector2_7 = pos + new Vector2(9f * this.UIScale, -14f * this.UIScale);
    DrawingHandleScreen drawingHandleScreen = handle;
    Font font = this._font;
    Vector2 vector2_8 = vector2_7 + new Vector2(1f, 1f);
    ReadOnlySpan<char> readOnlySpan = (ReadOnlySpan<char>) str;
    double uiScale = (double) this.UIScale;
    Color black = Color.Black;
    Color color = ((Color) ref black).WithAlpha(0.85f);
    drawingHandleScreen.DrawString(font, vector2_8, readOnlySpan, (float) uiScale, color);
    handle.DrawString(this._font, vector2_7, (ReadOnlySpan<char>) str, this.UIScale, playerColor);
  }

  private void DrawCenteredText(DrawingHandleScreen handle, UIBox2 mapRect, string text)
  {
    Vector2 dimensions = handle.GetDimensions(this._font, (ReadOnlySpan<char>) text, this.UIScale);
    Vector2 vector2 = ((UIBox2) ref mapRect).Center - dimensions / 2f;
    handle.DrawString(this._font, vector2, (ReadOnlySpan<char>) text, this.UIScale, Color.LightGray);
  }

  private UIBox2 GetMapRect()
  {
    float num1 = 14f * this.UIScale;
    Vector2 vector2 = Vector2i.op_Implicit(this.PixelSize) - new Vector2(num1 * 2f, num1 * 2f);
    if ((double) vector2.X <= 1.0 || (double) vector2.Y <= 1.0)
      return UIBox2.FromDimensions(Vector2.Zero, Vector2.Zero);
    float num2 = MathF.Min(vector2.X, vector2.Y);
    return UIBox2.FromDimensions(new Vector2((float) (((double) vector2.X - (double) num2) * 0.5) + num1, (float) (((double) vector2.Y - (double) num2) * 0.5) + num1), new Vector2(num2, num2));
  }

  private bool TryLocalToMapPosition(Vector2 localPosition, out Vector2 mapPosition)
  {
    mapPosition = new Vector2();
    Vector2 visibleMin;
    Vector2 visibleMax;
    if (!this.TryGetVisibleBounds(out visibleMin, out visibleMax))
      return false;
    UIBox2 mapRect = this.GetMapRect();
    if (!((UIBox2) ref mapRect).Contains(localPosition, true))
      return false;
    Vector2 vector2 = visibleMax - visibleMin;
    if ((double) vector2.X <= 0.0 || (double) vector2.Y <= 0.0)
      return false;
    float num1 = (localPosition.X - mapRect.Left) / ((UIBox2) ref mapRect).Width;
    float num2 = (mapRect.Bottom - localPosition.Y) / ((UIBox2) ref mapRect).Height;
    mapPosition = new Vector2(visibleMin.X + num1 * vector2.X, visibleMin.Y + num2 * vector2.Y);
    return true;
  }

  private bool TryMapToLocalPosition(Vector2 mapPosition, out Vector2 localPosition)
  {
    localPosition = new Vector2();
    Vector2 visibleMin;
    Vector2 visibleMax;
    if (!this.TryGetVisibleBounds(out visibleMin, out visibleMax))
      return false;
    Vector2 vector2 = visibleMax - visibleMin;
    if ((double) vector2.X <= 0.0 || (double) vector2.Y <= 0.0)
      return false;
    UIBox2 mapRect = this.GetMapRect();
    float num1 = (mapPosition.X - visibleMin.X) / vector2.X;
    float num2 = (mapPosition.Y - visibleMin.Y) / vector2.Y;
    if ((double) num1 < 0.0 || (double) num1 > 1.0 || (double) num2 < 0.0 || (double) num2 > 1.0)
      return false;
    localPosition = new Vector2(mapRect.Left + num1 * ((UIBox2) ref mapRect).Width, mapRect.Bottom - num2 * ((UIBox2) ref mapRect).Height);
    return true;
  }

  private bool TryGetVisibleBounds(out Vector2 visibleMin, out Vector2 visibleMax)
  {
    visibleMin = new Vector2();
    visibleMax = new Vector2();
    if (!this._hasBounds)
      return false;
    Vector2 vector2_1 = this._boundsMax - this._boundsMin;
    if ((double) vector2_1.X <= 0.0 || (double) vector2_1.Y <= 0.0)
      return false;
    this.EnsureViewCenter();
    Vector2 vector2_2 = this._viewCenter ?? (this._boundsMin + this._boundsMax) * 0.5f;
    float num1 = Math.Clamp(this._zoom, 1f, 4f);
    Vector2 vector2_3 = vector2_1 / (2f * num1);
    Vector2 min = this._boundsMin + vector2_3;
    Vector2 max = this._boundsMax - vector2_3;
    if ((double) min.X > (double) max.X)
    {
      float num2 = (float) (((double) this._boundsMin.X + (double) this._boundsMax.X) * 0.5);
      min.X = num2;
      max.X = num2;
    }
    if ((double) min.Y > (double) max.Y)
    {
      float num3 = (float) (((double) this._boundsMin.Y + (double) this._boundsMax.Y) * 0.5);
      min.Y = num3;
      max.Y = num3;
    }
    Vector2 vector2_4 = Vector2.Clamp(vector2_2, min, max);
    this._viewCenter = new Vector2?(vector2_4);
    visibleMin = vector2_4 - vector2_3;
    visibleMax = vector2_4 + vector2_3;
    return true;
  }

  private void EnsureViewCenter()
  {
    if (!this._hasBounds)
    {
      this._viewCenter = new Vector2?();
    }
    else
    {
      if (this._viewCenter.HasValue)
        return;
      Vector2 position;
      if (this.TryGetSelfPosition(out position))
        this._viewCenter = new Vector2?(position);
      else
        this._viewCenter = new Vector2?((this._boundsMin + this._boundsMax) * 0.5f);
    }
  }

  private bool TryGetSelfPosition(out Vector2 position)
  {
    position = new Vector2();
    foreach (CivGlobalMapPlayerState player in this._players)
    {
      if (player.IsSelf && !MapId.op_Inequality(player.MapId, this._mapId))
      {
        position = player.Position;
        return true;
      }
    }
    return false;
  }

  private void UpdateHoveredMarker(Vector2 localPosition)
  {
    UIBox2 mapRect = this.GetMapRect();
    if (!((UIBox2) ref mapRect).Contains(localPosition, true))
    {
      this._hoveredMarkerId = 0;
    }
    else
    {
      int markerId;
      this._hoveredMarkerId = this.TryGetMarkerAt(localPosition, out markerId, true) ? markerId : 0;
    }
  }

  private bool TryGetMarkerById(int markerId, out CivGlobalMapMarkerState marker)
  {
    foreach (CivGlobalMapMarkerState marker1 in this._markers)
    {
      if (marker1.Id == markerId)
      {
        marker = marker1;
        return true;
      }
    }
    marker = (CivGlobalMapMarkerState) null;
    return false;
  }

  private bool TryGetMarkerAt(Vector2 localPosition, out int markerId, bool includeObjectives)
  {
    markerId = 0;
    float num1 = float.MaxValue;
    foreach (CivGlobalMapMarkerState marker in this._markers)
    {
      Vector2 localPosition1;
      if (!MapId.op_Inequality(marker.MapId, this._mapId) && (includeObjectives || !marker.IsObjective) && this.TryMapToLocalPosition(marker.Position, out localPosition1))
      {
        double num2 = marker.IsObjective ? 25.0 : 11.0;
        float num3 = (float) (num2 * num2) * this.UIScale * this.UIScale;
        float num4 = (localPosition1 - localPosition).LengthSquared();
        if ((double) num4 <= (double) num3 && (double) num4 < (double) num1)
        {
          num1 = num4;
          markerId = marker.Id;
        }
      }
    }
    return markerId != 0;
  }

  private readonly record struct MapTileDrawData(Vector2 WorldPosition, Color Color, bool IsWall);
}
