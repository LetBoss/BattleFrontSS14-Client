// Decompiled with JetBrains decompiler
// Type: Content.Client.Vehicle.GridVehicleMoverOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Vehicle;
using Content.Shared.Vehicle.Components;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Vehicle;

public sealed class GridVehicleMoverOverlay : Overlay
{
  private const float MovementDecisionFadeSeconds = 2.5f;
  private static readonly Color[] EntryColors = new Color[7]
  {
    Color.Red,
    Color.Green,
    Color.Blue,
    Color.Yellow,
    Color.Magenta,
    Color.Cyan,
    Color.Orange
  };
  private static readonly Color ClearProbeFill = new Color(0.19f, 0.95f, 0.55f, 0.08f);
  private static readonly Color ClearProbeOutline = new Color(0.18f, 1f, 0.55f, 0.74f);
  private static readonly Color AppliedProbeFill = new Color(0.22f, 0.78f, 1f, 0.08f);
  private static readonly Color AppliedProbeOutline = new Color(0.25f, 0.84f, 1f, 0.72f);
  private static readonly Color BlockedProbeFill = new Color(1f, 0.16f, 0.22f, 0.18f);
  private static readonly Color BlockedProbeOutline = new Color(1f, 0.16f, 0.22f, 0.92f);
  private readonly IEntityManager _ents;
  private readonly IGameTiming _timing;
  private readonly SharedTransformSystem _transform;
  private readonly EntityLookupSystem _lookup;
  private readonly EntityQuery<MapGridComponent> _gridQuery;
  private readonly EntityQuery<FixturesComponent> _fixturesQuery;
  private readonly EntityQuery<PhysicsComponent> _physicsQuery;
  private readonly Dictionary<EntityUid, Vector2> _lastProbePositions = new Dictionary<EntityUid, Vector2>();
  private readonly List<GridVehicleMoverOverlay.FadingMovementDecision> _movementDecisions = new List<GridVehicleMoverOverlay.FadingMovementDecision>();

  public virtual OverlaySpace Space => (OverlaySpace) 8;

  public bool DebugEnabled { get; set; }

  public bool CollisionsEnabled { get; set; } = true;

  public bool MovementEnabled { get; set; } = true;

  public GridVehicleMoverOverlay(IEntityManager ents)
  {
    this._ents = ents;
    this._timing = IoCManager.Resolve<IGameTiming>();
    this._transform = ents.System<SharedTransformSystem>();
    this._lookup = ents.System<EntityLookupSystem>();
    this._gridQuery = ents.GetEntityQuery<MapGridComponent>();
    this._fixturesQuery = ents.GetEntityQuery<FixturesComponent>();
    this._physicsQuery = ents.GetEntityQuery<PhysicsComponent>();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    if (this.DebugEnabled)
      this.DrawVehicleDebug(worldHandle, args.MapId);
    if (this.CollisionsEnabled)
      this.DrawCollisionDebug(worldHandle, args.MapId);
    if (this.MovementEnabled)
      this.DrawMovementDecisions(worldHandle, args.MapId);
    GridVehicleMoverOverlay.ClearFrameDebug();
  }

  private void DrawVehicleDebug(DrawingHandleWorld handle, MapId mapId)
  {
    EntityQueryEnumerator<GridVehicleMoverComponent, TransformComponent> entityQueryEnumerator = this._ents.EntityQueryEnumerator<GridVehicleMoverComponent, TransformComponent>();
    EntityUid uid;
    GridVehicleMoverComponent mover;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref uid, ref mover, ref transformComponent))
    {
      EntityUid? gridUid = transformComponent.GridUid;
      if (gridUid.HasValue)
      {
        EntityUid valueOrDefault = gridUid.GetValueOrDefault();
        if (this._gridQuery.HasComp(valueOrDefault))
        {
          this.DrawMovementTarget(handle, valueOrDefault, mover);
          this.DrawFacing(handle, uid, mover);
          this.DrawFixtures(handle, uid);
          this.DrawPhysics(handle, uid);
        }
      }
    }
    this.DrawEntryAndExitPoints(handle, mapId);
    this.DrawTestedTiles(handle);
  }

  private void DrawMovementTarget(
    DrawingHandleWorld handle,
    EntityUid grid,
    GridVehicleMoverComponent mover)
  {
    Vector2 mapPosition1 = this.ToMapPosition(grid, mover.Position);
    Vector2 mapPosition2 = this.ToMapPosition(grid, new Vector2((float) mover.TargetTile.X + 0.5f, (float) mover.TargetTile.Y + 0.5f));
    Color color = mover.IsCommittedToMove ? Color.White : Color.Red;
    ((DrawingHandleBase) handle).DrawLine(mapPosition1, mapPosition2, Color.Lime);
    ((DrawingHandleBase) handle).DrawCircle(mapPosition1, 0.15f, color, true);
    ((DrawingHandleBase) handle).DrawCircle(mapPosition2, 0.15f, Color.Yellow, true);
  }

  private void DrawFacing(
    DrawingHandleWorld handle,
    EntityUid uid,
    GridVehicleMoverComponent mover)
  {
    if (Vector2i.op_Equality(mover.CurrentDirection, Vector2i.Zero))
      return;
    EntityUid? gridUid = this._ents.GetComponent<TransformComponent>(uid).GridUid;
    if (!gridUid.HasValue)
      return;
    Vector2 mapPosition = this.ToMapPosition(gridUid.GetValueOrDefault(), mover.Position);
    Vector2 vector2 = Vector2.Normalize(new Vector2((float) mover.CurrentDirection.X, (float) mover.CurrentDirection.Y));
    ((DrawingHandleBase) handle).DrawLine(mapPosition, mapPosition + vector2 * 0.7f, Color.Orange);
  }

  private void DrawFixtures(DrawingHandleWorld handle, EntityUid uid)
  {
    FixturesComponent fixturesComponent;
    if (!this._physicsQuery.HasComp(uid) || !this._fixturesQuery.TryComp(uid, ref fixturesComponent))
      return;
    int num = 0;
    foreach (Fixture fixture in fixturesComponent.Fixtures.Values)
    {
      Color entryColor = GridVehicleMoverOverlay.EntryColors[num % GridVehicleMoverOverlay.EntryColors.Length];
      for (int index = 0; index < fixture.ProxyCount; ++index)
        handle.DrawRect(fixture.Proxies[index].AABB, entryColor, false);
      ++num;
    }
  }

  private void DrawPhysics(DrawingHandleWorld handle, EntityUid uid)
  {
    Box2 worldAabb = this._lookup.GetWorldAABB(uid, (TransformComponent) null);
    Vector2 worldPosition = this._transform.GetWorldPosition(uid);
    handle.DrawRect(worldAabb, Color.Magenta, false);
    ((DrawingHandleBase) handle).DrawCircle(worldPosition, 0.12f, Color.Magenta, true);
  }

  private void DrawEntryAndExitPoints(DrawingHandleWorld handle, MapId mapId)
  {
    EntityUid? firstGrid = this.FindFirstGrid(mapId);
    this.DrawEntryPoints(handle, mapId, firstGrid);
    this.DrawExitPoints(handle, mapId);
  }

  private EntityUid? FindFirstGrid(MapId mapId)
  {
    EntityQueryEnumerator<MapGridComponent, TransformComponent> entityQueryEnumerator = this._ents.EntityQueryEnumerator<MapGridComponent, TransformComponent>();
    EntityUid entityUid;
    MapGridComponent mapGridComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref mapGridComponent, ref transformComponent))
    {
      if (MapId.op_Equality(transformComponent.MapID, mapId))
        return new EntityUid?(entityUid);
    }
    return new EntityUid?();
  }

  private void DrawEntryPoints(DrawingHandleWorld handle, MapId mapId, EntityUid? interiorGrid)
  {
    EntityQueryEnumerator<VehicleEnterComponent, TransformComponent> entityQueryEnumerator = this._ents.EntityQueryEnumerator<VehicleEnterComponent, TransformComponent>();
    EntityUid entityUid;
    VehicleEnterComponent vehicleEnterComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref vehicleEnterComponent, ref transformComponent))
    {
      if (!MapId.op_Inequality(transformComponent.MapID, mapId))
      {
        Vector2 worldPosition = this._transform.GetWorldPosition(transformComponent);
        Angle localRotation = transformComponent.LocalRotation;
        for (int index = 0; index < vehicleEnterComponent.EntryPoints.Count; ++index)
        {
          VehicleEntryPoint entryPoint = vehicleEnterComponent.EntryPoints[index];
          Color entryColor = GridVehicleMoverOverlay.GetEntryColor(index);
          Vector2 position = worldPosition + ((Angle) ref localRotation).RotateVec(ref entryPoint.Offset);
          GridVehicleMoverOverlay.DrawPoint(handle, position, Math.Max(0.05f, entryPoint.Radius), entryColor);
          if (interiorGrid.HasValue)
          {
            EntityUid valueOrDefault1 = interiorGrid.GetValueOrDefault();
            Vector2? interiorCoords = entryPoint.InteriorCoords;
            if (interiorCoords.HasValue)
            {
              Vector2 valueOrDefault2 = interiorCoords.GetValueOrDefault();
              GridVehicleMoverOverlay.DrawPoint(handle, this.ToMapPosition(valueOrDefault1, valueOrDefault2), 0.15f, entryColor);
            }
          }
        }
      }
    }
  }

  private void DrawExitPoints(DrawingHandleWorld handle, MapId mapId)
  {
    EntityQueryEnumerator<VehicleExitComponent, TransformComponent> entityQueryEnumerator = this._ents.EntityQueryEnumerator<VehicleExitComponent, TransformComponent>();
    EntityUid entityUid;
    VehicleExitComponent vehicleExitComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref vehicleExitComponent, ref transformComponent))
    {
      if (!MapId.op_Inequality(transformComponent.MapID, mapId))
        GridVehicleMoverOverlay.DrawPoint(handle, this._transform.GetWorldPosition(transformComponent), 0.12f, GridVehicleMoverOverlay.GetEntryColor(vehicleExitComponent.EntryIndex));
    }
  }

  private static void DrawPoint(
    DrawingHandleWorld handle,
    Vector2 position,
    float radius,
    Color color)
  {
    ((DrawingHandleBase) handle).DrawCircle(position, radius, ((Color) ref color).WithAlpha(0.22f), true);
    ((DrawingHandleBase) handle).DrawCircle(position, radius, ((Color) ref color).WithAlpha(0.85f), false);
  }

  private void DrawTestedTiles(DrawingHandleWorld handle)
  {
    foreach ((EntityUid grid, Vector2i tile) in Content.Shared.Vehicle.GridVehicleMoverSystem.DebugTestedTiles)
    {
      if (this._gridQuery.HasComp(grid))
      {
        Vector2 mapPosition = this.ToMapPosition(grid, new Vector2((float) tile.X + 0.5f, (float) tile.Y + 0.5f));
        DrawingHandleWorld drawingHandleWorld = handle;
        Box2 box2 = ((Box2) ref Box2.UnitCentered).Translated(mapPosition);
        Color black = Color.Black;
        Color color = ((Color) ref black).WithAlpha(0.4f);
        drawingHandleWorld.DrawRect(box2, color, false);
      }
    }
  }

  private void DrawCollisionDebug(DrawingHandleWorld handle, MapId mapId)
  {
    this.DrawCollisionProbes(handle, mapId);
    GridVehicleMoverOverlay.DrawBlockedCollisions(handle, mapId);
  }

  private void DrawCollisionProbes(DrawingHandleWorld handle, MapId mapId)
  {
    this._lastProbePositions.Clear();
    foreach (Content.Shared.Vehicle.GridVehicleMoverSystem.DebugCollisionProbe debugCollisionProbe in Content.Shared.Vehicle.GridVehicleMoverSystem.DebugCollisionProbes)
    {
      if (!MapId.op_Inequality(debugCollisionProbe.Map, mapId))
      {
        this.DrawProbePath(handle, debugCollisionProbe);
        GridVehicleMoverOverlay.DrawProbeBounds(handle, debugCollisionProbe);
        GridVehicleMoverOverlay.DrawProbeFacing(handle, debugCollisionProbe);
      }
    }
  }

  private void DrawProbePath(
    DrawingHandleWorld handle,
    Content.Shared.Vehicle.GridVehicleMoverSystem.DebugCollisionProbe probe)
  {
    Vector2 vector2;
    if (this._lastProbePositions.TryGetValue(probe.Tested, out vector2))
      ((DrawingHandleBase) handle).DrawLine(vector2, probe.Position, new Color(0.55f, 0.9f, 1f, 0.38f));
    this._lastProbePositions[probe.Tested] = probe.Position;
  }

  private static void DrawProbeBounds(
    DrawingHandleWorld handle,
    Content.Shared.Vehicle.GridVehicleMoverSystem.DebugCollisionProbe probe)
  {
    Color color1 = probe.Blocked ? GridVehicleMoverOverlay.BlockedProbeFill : (probe.ApplyEffects ? GridVehicleMoverOverlay.AppliedProbeFill : GridVehicleMoverOverlay.ClearProbeFill);
    Color color2 = probe.Blocked ? GridVehicleMoverOverlay.BlockedProbeOutline : (probe.ApplyEffects ? GridVehicleMoverOverlay.AppliedProbeOutline : GridVehicleMoverOverlay.ClearProbeOutline);
    handle.DrawRect(probe.MovementAabb, color1, true);
    handle.DrawRect(probe.MovementAabb, color2, false);
    DrawingHandleWorld drawingHandleWorld1 = handle;
    Box2Rotated box2Rotated = probe.FixtureBounds;
    ref Box2Rotated local1 = ref box2Rotated;
    Color color3 = new Color(0.45f, 0.92f, 1f, 0.72f);
    drawingHandleWorld1.DrawRect(ref local1, color3, false);
    DrawingHandleWorld drawingHandleWorld2 = handle;
    box2Rotated = probe.MovementBounds;
    ref Box2Rotated local2 = ref box2Rotated;
    Color color4 = new Color(0.95f, 0.95f, 1f, 0.45f);
    drawingHandleWorld2.DrawRect(ref local2, color4, false);
    handle.DrawRect(probe.TestedAabb, new Color(0.45f, 0.92f, 1f, 0.18f), false);
  }

  private static void DrawProbeFacing(
    DrawingHandleWorld handle,
    Content.Shared.Vehicle.GridVehicleMoverSystem.DebugCollisionProbe probe)
  {
    Color color = probe.Blocked ? new Color(1f, 0.24f, 0.26f, 0.95f) : new Color(0.22f, 1f, 0.58f, 0.95f);
    Angle rotation = probe.Rotation;
    ref Angle local1 = ref rotation;
    Vector2 unitX = Vector2.UnitX;
    ref Vector2 local2 = ref unitX;
    Vector2 vector2 = ((Angle) ref local1).RotateVec(ref local2);
    ((DrawingHandleBase) handle).DrawCircle(probe.Position, probe.Blocked ? 0.11f : 0.07f, color, true);
    GridVehicleMoverOverlay.DrawArrow(handle, probe.Position, probe.Position + vector2 * 0.85f, color);
  }

  private void DrawMovementDecisions(DrawingHandleWorld handle, MapId mapId)
  {
    this.CaptureMovementDecisions();
    TimeSpan curTime = this._timing.CurTime;
    for (int index = this._movementDecisions.Count - 1; index >= 0; --index)
    {
      GridVehicleMoverOverlay.FadingMovementDecision movementDecision = this._movementDecisions[index];
      float totalSeconds = (float) (curTime - movementDecision.Created).TotalSeconds;
      if ((double) totalSeconds >= 2.5)
        this._movementDecisions.RemoveAt(index);
      else if (!MapId.op_Inequality(movementDecision.Map, mapId))
        GridVehicleMoverOverlay.DrawMovementDecision(handle, movementDecision, (float) (1.0 - (double) totalSeconds / 2.5));
    }
  }

  private void CaptureMovementDecisions()
  {
    TimeSpan curTime = this._timing.CurTime;
    foreach (Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecision movementDecision in Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisions)
    {
      if (this._gridQuery.HasComp(movementDecision.Grid))
      {
        MapCoordinates mapCoordinates1 = this._transform.ToMapCoordinates(new EntityCoordinates(movementDecision.Grid, movementDecision.Start), true);
        MapCoordinates mapCoordinates2 = this._transform.ToMapCoordinates(new EntityCoordinates(movementDecision.Grid, movementDecision.End), true);
        if (!MapId.op_Inequality(mapCoordinates1.MapId, mapCoordinates2.MapId))
          this._movementDecisions.Add(new GridVehicleMoverOverlay.FadingMovementDecision(mapCoordinates1.Position, mapCoordinates2.Position, movementDecision.Kind, movementDecision.Success, mapCoordinates1.MapId, curTime));
      }
    }
    Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisions.Clear();
  }

  private static void DrawMovementDecision(
    DrawingHandleWorld handle,
    GridVehicleMoverOverlay.FadingMovementDecision decision,
    float alpha)
  {
    Color movementDecisionColor = GridVehicleMoverOverlay.GetMovementDecisionColor(decision.Kind, decision.Success);
    Color color1 = ((Color) ref movementDecisionColor).WithAlpha(0.85f * alpha);
    Color color2 = ((Color) ref color1).WithAlpha(0.16f * alpha);
    Vector2 delta = decision.End - decision.Start;
    float num = decision.Success ? 0.08f : 0.12f;
    if ((double) delta.LengthSquared() > 9.9999997473787516E-05)
      GridVehicleMoverOverlay.DrawArrow(handle, decision.Start, decision.End, color1);
    else
      ((DrawingHandleBase) handle).DrawCircle(decision.Start, 0.1f, color1, true);
    ((DrawingHandleBase) handle).DrawCircle(decision.Start, num * 0.75f, color2, true);
    ((DrawingHandleBase) handle).DrawCircle(decision.Start, num * 0.75f, color1, false);
    ((DrawingHandleBase) handle).DrawCircle(decision.End, num, color2, true);
    ((DrawingHandleBase) handle).DrawCircle(decision.End, num, color1, false);
    if (!GridVehicleMoverOverlay.IsBlockedDecision(decision.Kind))
      return;
    GridVehicleMoverOverlay.DrawBlockedDecisionMarker(handle, decision.End, delta, ((Color) ref color1).WithAlpha(0.95f * alpha));
  }

  private static bool IsBlockedDecision(
    Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind kind)
  {
    return kind == Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind.DirectBlocked || kind == Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind.ForwardBlocked;
  }

  private static void DrawBlockedDecisionMarker(
    DrawingHandleWorld handle,
    Vector2 end,
    Vector2 delta,
    Color color)
  {
    Vector2 vector2 = (double) delta.LengthSquared() > 9.9999997473787516E-05 ? Vector2.Normalize(new Vector2(-delta.Y, delta.X)) * 0.14f : new Vector2(0.14f, 0.0f);
    ((DrawingHandleBase) handle).DrawLine(end - vector2, end + vector2, color);
    ((DrawingHandleBase) handle).DrawLine(end - new Vector2(vector2.X, -vector2.Y), end + new Vector2(vector2.X, -vector2.Y), color);
  }

  private static Color GetMovementDecisionColor(
    Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind kind,
    bool success)
  {
    Color movementDecisionColor;
    switch (kind)
    {
      case Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind.DirectClear:
        movementDecisionColor = new Color(0.2f, 1f, 0.45f, 1f);
        break;
      case Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind.DirectBlocked:
        movementDecisionColor = new Color(1f, 0.2f, 0.24f, 1f);
        break;
      case Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind.LaneCorrection:
        movementDecisionColor = new Color(0.3f, 0.72f, 1f, 1f);
        break;
      case Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind.LaneCorrectionFailed:
        movementDecisionColor = new Color(1f, 0.62f, 0.18f, 1f);
        break;
      case Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind.ForwardAfterCorrection:
        movementDecisionColor = new Color(0.78f, 0.42f, 1f, 1f);
        break;
      case Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind.ForwardBlocked:
        movementDecisionColor = new Color(1f, 0.1f, 0.12f, 1f);
        break;
      default:
        movementDecisionColor = success ? Color.Lime : Color.Red;
        break;
    }
    return movementDecisionColor;
  }

  private static void DrawBlockedCollisions(DrawingHandleWorld handle, MapId mapId)
  {
    foreach (Content.Shared.Vehicle.GridVehicleMoverSystem.DebugCollision debugCollision in Content.Shared.Vehicle.GridVehicleMoverSystem.DebugCollisions)
    {
      if (!MapId.op_Inequality(debugCollision.Map, mapId))
      {
        GridVehicleMoverOverlay.DrawCollisionAabbs(handle, debugCollision);
        GridVehicleMoverOverlay.DrawCollisionDirection(handle, debugCollision);
        GridVehicleMoverOverlay.DrawCollisionGap(handle, debugCollision);
      }
    }
  }

  private static void DrawCollisionAabbs(
    DrawingHandleWorld handle,
    Content.Shared.Vehicle.GridVehicleMoverSystem.DebugCollision hit)
  {
    handle.DrawRect(hit.TestedAabb, new Color(0.92f, 0.27f, 0.36f, 0.16f), true);
    handle.DrawRect(hit.TestedAabb, new Color(0.97f, 0.38f, 0.46f, 0.85f), false);
    handle.DrawRect(hit.BlockerAabb, new Color(1f, 0.8f, 0.32f, 0.14f), true);
    handle.DrawRect(hit.BlockerAabb, new Color(1f, 0.82f, 0.22f, 0.9f), false);
    Box2 testedAabb = hit.TestedAabb;
    ref Box2 local1 = ref testedAabb;
    Box2 blockerAabb = hit.BlockerAabb;
    ref Box2 local2 = ref blockerAabb;
    Box2 box2 = ((Box2) ref local1).Intersect(ref local2);
    if ((double) ((Box2) ref box2).Width <= 0.0 || (double) ((Box2) ref box2).Height <= 0.0)
      return;
    handle.DrawRect(box2, new Color(0.83f, 0.23f, 1f, 0.25f), true);
    handle.DrawRect(box2, new Color(0.74f, 0.16f, 0.95f, 0.9f), false);
  }

  private static void DrawCollisionDirection(
    DrawingHandleWorld handle,
    Content.Shared.Vehicle.GridVehicleMoverSystem.DebugCollision hit)
  {
    Box2 testedAabb = hit.TestedAabb;
    Vector2 center1 = ((Box2) ref testedAabb).Center;
    Box2 blockerAabb = hit.BlockerAabb;
    Vector2 center2 = ((Box2) ref blockerAabb).Center;
    Vector2 vector2 = center2 - center1;
    float x = vector2.Length();
    ((DrawingHandleBase) handle).DrawCircle(center1, 0.09f, new Color(0.97f, 0.38f, 0.46f, 0.85f), true);
    ((DrawingHandleBase) handle).DrawCircle(center2, 0.09f, new Color(1f, 0.82f, 0.22f, 0.9f), true);
    if ((double) x <= 0.0099999997764825821)
      return;
    Vector2 end = center1 + vector2 / x * MathF.Min(x, 0.9f);
    GridVehicleMoverOverlay.DrawArrow(handle, center1, end, new Color(0.7f, 0.35f, 1f, 0.85f));
  }

  private static void DrawCollisionGap(
    DrawingHandleWorld handle,
    Content.Shared.Vehicle.GridVehicleMoverSystem.DebugCollision hit)
  {
    float x = hit.Distance - hit.Skin + hit.Clearance;
    Box2 box2 = hit.TestedAabb;
    Vector2 center1 = ((Box2) ref box2).Center;
    box2 = hit.BlockerAabb;
    Vector2 center2 = ((Box2) ref box2).Center;
    Vector2 vector2 = (center1 + center2) * 0.5f;
    Color color = (double) x <= 0.0 ? new Color(1f, 0.32f, 0.32f, 0.92f) : new Color(0.45f, 1f, 0.58f, 0.95f);
    float num = 0.07f + MathF.Min(MathF.Abs(x) * 0.03f, 0.09f);
    ((DrawingHandleBase) handle).DrawCircle(vector2, num, color, true);
    ((DrawingHandleBase) handle).DrawCircle(vector2, num * 0.55f, ((Color) ref color).WithAlpha(0.55f), true);
  }

  private Vector2 ToMapPosition(EntityUid grid, Vector2 localPosition)
  {
    return this._transform.ToMapCoordinates(new EntityCoordinates(grid, localPosition), true).Position;
  }

  private static Color GetEntryColor(int entryIndex)
  {
    int num = entryIndex < 0 ? Math.Abs(entryIndex) : entryIndex;
    return GridVehicleMoverOverlay.EntryColors[num % GridVehicleMoverOverlay.EntryColors.Length];
  }

  private static void DrawArrow(
    DrawingHandleWorld handle,
    Vector2 start,
    Vector2 end,
    Color color)
  {
    ((DrawingHandleBase) handle).DrawLine(start, end, color);
    Vector2 vector2_1 = end - start;
    float num = vector2_1.Length();
    if ((double) num <= 0.0099999997764825821)
      return;
    Vector2 vector2_2 = vector2_1 / num;
    Vector2 vector2_3 = end - vector2_2 * 0.12f;
    Vector2 vector2_4 = new Vector2(-vector2_2.Y, vector2_2.X) * 0.06f;
    ((DrawingHandleBase) handle).DrawLine(end, vector2_3 + vector2_4, color);
    ((DrawingHandleBase) handle).DrawLine(end, vector2_3 - vector2_4, color);
  }

  private static void ClearFrameDebug()
  {
    Content.Shared.Vehicle.GridVehicleMoverSystem.DebugTestedTiles.Clear();
    Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisions.Clear();
    GridVehicleMoverSystem.DebugCollisionPositions.Clear();
  }

  private sealed record FadingMovementDecision(
    Vector2 Start,
    Vector2 End,
    Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind Kind,
    bool Success,
    MapId Map,
    TimeSpan Created)
  ;
}
