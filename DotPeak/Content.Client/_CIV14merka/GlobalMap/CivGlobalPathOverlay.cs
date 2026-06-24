// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.GlobalMap.CivGlobalPathOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Stylesheets;
using Content.Shared._CIV14merka.GlobalMap;
using Content.Shared._RMC14.Vehicle;
using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.GlobalMap;

public sealed class CivGlobalPathOverlay : Overlay
{
  private static readonly Color SquadLeaderArrowColor = Color.FromHex((ReadOnlySpan<char>) "#ffd95a", new Color?());
  private const float HideDistanceThreshold = 17f;
  private readonly IPlayerManager _player;
  private readonly SharedTransformSystem _transform;
  private readonly CivGlobalMapSystem _mapSystem;
  private readonly VehicleSystem _vehicles;
  private readonly IConfigurationManager _cfg;
  private readonly Font _font;
  private readonly Dictionary<int, string> _distanceTextCache = new Dictionary<int, string>();
  private readonly Dictionary<CivGlobalMapMarkerType, string> _markerLabelCache = new Dictionary<CivGlobalMapMarkerType, string>();

  public virtual OverlaySpace Space => (OverlaySpace) 2;

  public CivGlobalPathOverlay(
    IPlayerManager player,
    SharedTransformSystem transform,
    CivGlobalMapSystem mapSystem,
    VehicleSystem vehicles,
    IResourceCache resourceCache,
    IConfigurationManager cfg)
  {
    this._player = player;
    this._transform = transform;
    this._mapSystem = mapSystem;
    this._vehicles = vehicles;
    this._cfg = cfg;
    this._font = resourceCache.NotoStack("Bold", 12);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (args.ViewportControl == null)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    MapId mapId;
    if (!localEntity.HasValue || !this._vehicles.TryGetDisplayMapId(localEntity.Value, out mapId) || MapId.op_Equality(mapId, MapId.Nullspace))
      return;
    Vector2 worldPosition = this._transform.GetWorldPosition(localEntity.Value);
    Vector2 center = new Vector2((float) ((UIBox2i) ref args.ViewportBounds).Width / 2f, (float) ((UIBox2i) ref args.ViewportBounds).Height / 2f);
    float radius = MathF.Min((float) ((UIBox2i) ref args.ViewportBounds).Width, (float) ((UIBox2i) ref args.ViewportBounds).Height) * 0.42f;
    if (this._cfg.GetCVar<bool>(CCVars.Civ14ShowOrderArrows))
      this.DrawSquadOrderArrows(in args, mapId, center, worldPosition, radius);
    if (!this._cfg.GetCVar<bool>(CCVars.Civ14ShowLeaderArrow))
      return;
    this.DrawSquadLeaderArrow(in args, mapId, center, worldPosition, radius);
  }

  private void DrawSquadOrderArrows(
    in OverlayDrawArgs args,
    MapId playerMapId,
    Vector2 center,
    Vector2 playerWorldPos,
    float radius)
  {
    if (args.ViewportControl == null)
      return;
    int viewerSquadId = this._mapSystem.ViewerSquadId;
    if (viewerSquadId <= 0)
      return;
    int num1 = 0;
    foreach (CivGlobalMapMarkerState visibleMarker in (IEnumerable<CivGlobalMapMarkerState>) this._mapSystem.VisibleMarkers)
    {
      if (!visibleMarker.IsObjective && visibleMarker.SquadId == viewerSquadId && visibleMarker.PlacedBySquadLeader && visibleMarker.Type.IsGlobal() && !MapId.op_Inequality(visibleMarker.MapId, playerMapId))
      {
        float worldDistance = (visibleMarker.Position - playerWorldPos).Length();
        if ((double) worldDistance >= 17.0)
        {
          Vector2 vector2 = args.ViewportControl.WorldToScreen(visibleMarker.Position) - center;
          float num2 = vector2.Length();
          if ((double) num2 >= 8.0)
          {
            Vector2 direction = vector2 / num2;
            float num3 = MathF.Min((float) ((double) radius - 60.0 + (double) num1 * 40.0), MathF.Max(36f, num2 - 20f));
            Vector2 center1 = center + direction * num3;
            Color color = CivGlobalMapColorResolver.GetColor(visibleMarker.Type);
            string label;
            if (!this._markerLabelCache.TryGetValue(visibleMarker.Type, out label))
            {
              label = Loc.GetString("civ-gmap-marker-" + visibleMarker.Type.ToString().ToLowerInvariant());
              this._markerLabelCache[visibleMarker.Type] = label;
            }
            this.DrawArrowWithDistance(in args, center1, direction, color, worldDistance, label);
            ++num1;
            if (num1 >= 3)
              break;
          }
        }
      }
    }
  }

  private static void DrawArrow(
    DrawingHandleScreen handle,
    Vector2 center,
    Vector2 direction,
    Color color)
  {
    Vector2 vector2_1 = center + direction * 14f;
    Vector2 vector2_2 = new Vector2(-direction.Y, direction.X);
    Vector2 vector2_3 = center - direction * 6f + vector2_2 * 6f;
    Vector2 vector2_4 = center - direction * 6f - vector2_2 * 6f;
    Vector2[] vector2Array1 = new Vector2[3]
    {
      vector2_1,
      vector2_3,
      vector2_4
    };
    ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 1, (ReadOnlySpan<Vector2>) vector2Array1, color);
    Vector2[] vector2Array2 = new Vector2[4]
    {
      vector2_1,
      vector2_3,
      vector2_4,
      vector2_1
    };
    ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 5, (ReadOnlySpan<Vector2>) vector2Array2, Color.Black);
    Vector2 vector2_5 = center - direction * 14f;
    Vector2 vector2_6 = center - direction * 2f;
    ((DrawingHandleBase) handle).DrawLine(vector2_5, vector2_6, ((Color) ref color).WithAlpha(0.85f));
  }

  private void DrawArrowWithDistance(
    in OverlayDrawArgs args,
    Vector2 center,
    Vector2 direction,
    Color color,
    float worldDistance,
    string? label = null)
  {
    CivGlobalPathOverlay.DrawArrow(((OverlayDrawArgs) ref args).ScreenHandle, center, direction, color);
    int key = (int) worldDistance;
    string str;
    if (!this._distanceTextCache.TryGetValue(key, out str))
    {
      str = Loc.GetString("civ-gmap-distance-meters", new (string, object)[1]
      {
        ("distance", (object) key)
      });
      this._distanceTextCache[key] = str;
    }
    Vector2 vector2_1 = center - direction * 24f;
    Vector2 dimensions1 = ((OverlayDrawArgs) ref args).ScreenHandle.GetDimensions(this._font, (ReadOnlySpan<char>) str, 1f);
    Vector2 vector2_2 = vector2_1 - dimensions1 / 2f;
    if (!string.IsNullOrEmpty(label))
    {
      Vector2 dimensions2 = ((OverlayDrawArgs) ref args).ScreenHandle.GetDimensions(this._font, (ReadOnlySpan<char>) label, 1f);
      Vector2 vector2_3 = new Vector2(vector2_1.X - dimensions2.X / 2f, vector2_2.Y - dimensions2.Y);
      ((OverlayDrawArgs) ref args).ScreenHandle.DrawString(this._font, vector2_3, (ReadOnlySpan<char>) label, 1f, color);
    }
    ((OverlayDrawArgs) ref args).ScreenHandle.DrawString(this._font, vector2_2, (ReadOnlySpan<char>) str, 1f, color);
  }

  private void DrawSquadLeaderArrow(
    in OverlayDrawArgs args,
    MapId playerMapId,
    Vector2 center,
    Vector2 playerWorldPos,
    float radius)
  {
    if (args.ViewportControl == null)
      return;
    int viewerSquadId = this._mapSystem.ViewerSquadId;
    if (viewerSquadId <= 0)
      return;
    CivGlobalMapPlayerState globalMapPlayerState = (CivGlobalMapPlayerState) null;
    foreach (CivGlobalMapPlayerState visiblePlayer in (IEnumerable<CivGlobalMapPlayerState>) this._mapSystem.VisiblePlayers)
    {
      if (!visiblePlayer.IsSelf && visiblePlayer.IsSquadLeader && visiblePlayer.SquadId == viewerSquadId && MapId.op_Equality(visiblePlayer.MapId, playerMapId))
      {
        globalMapPlayerState = visiblePlayer;
        break;
      }
    }
    if (globalMapPlayerState == null)
      return;
    float worldDistance = (globalMapPlayerState.Position - playerWorldPos).Length();
    if ((double) worldDistance < 17.0)
      return;
    Vector2 vector2 = args.ViewportControl.WorldToScreen(globalMapPlayerState.Position) - center;
    float num = vector2.Length();
    if ((double) num < 0.0099999997764825821)
      return;
    Vector2 direction = vector2 / num;
    Vector2 center1 = center + direction * MathF.Max(48f, radius - 38f);
    this.DrawArrowWithDistance(in args, center1, direction, CivGlobalPathOverlay.SquadLeaderArrowColor, worldDistance, Loc.GetString("civ-gmap-leader-arrow"));
  }
}
