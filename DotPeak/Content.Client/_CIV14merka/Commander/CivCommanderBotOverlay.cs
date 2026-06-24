// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.CivCommanderBotOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderBotOverlay : Overlay
{
  private static readonly Color SelectedRingColor = Color.FromHex((ReadOnlySpan<char>) "#ffd54f", new Color?());
  private static readonly Color BoxSelectColor = Color.FromHex((ReadOnlySpan<char>) "#4da6ff", new Color?());
  private static readonly Color PatrolColor = Color.FromHex((ReadOnlySpan<char>) "#ff9800", new Color?());
  private static readonly Color OrderIdleColor = Color.FromHex((ReadOnlySpan<char>) "#9e9e9e", new Color?());
  private static readonly Color OrderMoveColor = Color.FromHex((ReadOnlySpan<char>) "#4caf50", new Color?());
  private static readonly Color OrderAttackMoveColor = Color.FromHex((ReadOnlySpan<char>) "#f44336", new Color?());
  private static readonly Color OrderHoldColor = Color.FromHex((ReadOnlySpan<char>) "#2196f3", new Color?());
  private static readonly Color OrderFollowColor = Color.FromHex((ReadOnlySpan<char>) "#9c27b0", new Color?());
  private static readonly Color OrderDefendColor = Color.FromHex((ReadOnlySpan<char>) "#00bcd4", new Color?());
  private static readonly Color OrderPatrolColor = Color.FromHex((ReadOnlySpan<char>) "#ff9800", new Color?());
  [Dependency]
  private IEyeManager _eye;
  [Dependency]
  private IPlayerManager _player;
  private readonly IEntityManager _entity;
  private readonly CivCommanderBotControlSystem _control;
  private readonly SharedTransformSystem _transform;

  public virtual OverlaySpace Space => (OverlaySpace) 8;

  public CivCommanderBotOverlay(IEntityManager entity, CivCommanderBotControlSystem control)
  {
    IoCManager.InjectDependencies<CivCommanderBotOverlay>(this);
    this._entity = entity;
    this._control = control;
    this._transform = entity.System<SharedTransformSystem>();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    CivTeamMemberComponent teamMemberComponent1;
    if (!localEntity.HasValue || !this._entity.TryGetComponent<CivTeamMemberComponent>(localEntity.GetValueOrDefault(), ref teamMemberComponent1) || !teamMemberComponent1.IsCommander)
      return;
    int teamId = teamMemberComponent1.TeamId;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    EntityQueryEnumerator<CivCommanderBotComponent, CivTeamMemberComponent, TransformComponent> entityQueryEnumerator = this._entity.EntityQueryEnumerator<CivCommanderBotComponent, CivTeamMemberComponent, TransformComponent>();
    EntityUid entityUid;
    CivCommanderBotComponent botComp;
    CivTeamMemberComponent teamMemberComponent2;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref botComp, ref teamMemberComponent2, ref transformComponent))
    {
      MobStateComponent mobStateComponent;
      if (teamMemberComponent2.TeamId == teamId && !MapId.op_Inequality(transformComponent.MapID, args.MapId) && (!this._entity.TryGetComponent<MobStateComponent>(entityUid, ref mobStateComponent) || mobStateComponent.CurrentState != MobState.Dead))
      {
        Vector2 position = this._transform.GetMapCoordinates(transformComponent).Position;
        if (CivCommanderBotOverlay.CircleIntersects(args.WorldAABB, position, 1.5f))
        {
          bool isSelected = this._control.SelectedBots.Contains(entityUid);
          this.DrawBotIndicator(worldHandle, position, botComp, isSelected);
        }
      }
    }
    if (this._control.IsBoxSelecting)
      this.DrawBoxSelection(in args);
    if (!this._control.IsPatrolMode || this._control.PatrolPoints.Count <= 0)
      return;
    this.DrawPatrolPreview(worldHandle);
  }

  private void DrawBotIndicator(
    DrawingHandleWorld handle,
    Vector2 pos,
    CivCommanderBotComponent botComp,
    bool isSelected)
  {
    float num1 = isSelected ? 0.58f : 0.45f;
    Color orderColor = CivCommanderBotOverlay.GetOrderColor(botComp.Order);
    Color color1 = isSelected ? CivCommanderBotOverlay.SelectedRingColor : orderColor;
    if (isSelected)
      ((DrawingHandleBase) handle).DrawCircle(pos, num1 + 0.15f, ((Color) ref color1).WithAlpha(0.25f), true);
    ((DrawingHandleBase) handle).DrawCircle(pos, num1, ((Color) ref color1).WithAlpha(0.9f), false);
    DrawingHandleWorld drawingHandleWorld1 = handle;
    Vector2 vector2_1 = pos;
    double num2 = (double) num1 + 0.059999998658895493;
    Color color2 = Color.Black;
    Color color3 = ((Color) ref color2).WithAlpha(0.65f);
    ((DrawingHandleBase) drawingHandleWorld1).DrawCircle(vector2_1, (float) num2, color3, false);
    if (botComp.SquadId > 0)
    {
      Vector2 vector2_2 = pos + new Vector2(0.35f, 0.35f);
      DrawingHandleWorld drawingHandleWorld2 = handle;
      Vector2 vector2_3 = vector2_2;
      color2 = Color.Black;
      Color color4 = ((Color) ref color2).WithAlpha(0.7f);
      ((DrawingHandleBase) drawingHandleWorld2).DrawCircle(vector2_3, 0.22f, color4, true);
      if (botComp.IsLeader)
      {
        DrawingHandleWorld drawingHandleWorld3 = handle;
        Vector2 vector2_4 = vector2_2;
        color2 = Color.Gold;
        Color color5 = ((Color) ref color2).WithAlpha(0.9f);
        ((DrawingHandleBase) drawingHandleWorld3).DrawCircle(vector2_4, 0.13f, color5, true);
      }
    }
    if (string.IsNullOrEmpty(CivCommanderBotOverlay.GetOrderIndicator(botComp.Order)))
      return;
    Vector2 vector2_5 = pos + new Vector2(-0.3f, 0.4f);
    ((DrawingHandleBase) handle).DrawCircle(vector2_5, 0.18f, ((Color) ref orderColor).WithAlpha(0.8f), true);
  }

  private void DrawBoxSelection(in OverlayDrawArgs args)
  {
    MapCoordinates map1 = this._eye.ScreenToMap(this._control.BoxSelectStart);
    MapCoordinates map2 = this._eye.ScreenToMap(this._control.BoxSelectEnd);
    if (MapId.op_Inequality(map1.MapId, args.MapId) || MapId.op_Inequality(map2.MapId, args.MapId))
      return;
    Vector2 vector2_1 = new Vector2(MathF.Min(map1.Position.X, map2.Position.X), MathF.Min(map1.Position.Y, map2.Position.Y));
    Vector2 vector2_2 = new Vector2(MathF.Max(map1.Position.X, map2.Position.X), MathF.Max(map1.Position.Y, map2.Position.Y));
    Box2 box2;
    // ISSUE: explicit constructor call
    ((Box2) ref box2).\u002Ector(vector2_1, vector2_2);
    ((OverlayDrawArgs) ref args).WorldHandle.DrawRect(box2, ((Color) ref CivCommanderBotOverlay.BoxSelectColor).WithAlpha(0.15f), true);
    ((OverlayDrawArgs) ref args).WorldHandle.DrawRect(box2, ((Color) ref CivCommanderBotOverlay.BoxSelectColor).WithAlpha(0.8f), false);
  }

  private void DrawPatrolPreview(DrawingHandleWorld handle)
  {
    IReadOnlyList<Vector2> patrolPoints = this._control.PatrolPoints;
    if (patrolPoints.Count < 1)
      return;
    for (int index = 0; index < patrolPoints.Count; ++index)
    {
      Vector2 vector2_1 = patrolPoints[index];
      ((DrawingHandleBase) handle).DrawCircle(vector2_1, 0.35f, ((Color) ref CivCommanderBotOverlay.PatrolColor).WithAlpha(0.6f), true);
      ((DrawingHandleBase) handle).DrawCircle(vector2_1, 0.35f, CivCommanderBotOverlay.PatrolColor, false);
      if (index > 0)
      {
        Vector2 vector2_2 = patrolPoints[index - 1];
        ((DrawingHandleBase) handle).DrawLine(vector2_2, vector2_1, ((Color) ref CivCommanderBotOverlay.PatrolColor).WithAlpha(0.7f));
      }
    }
    if (patrolPoints.Count <= 1)
      return;
    DrawingHandleWorld drawingHandleWorld = handle;
    IReadOnlyList<Vector2> vector2List = patrolPoints;
    Vector2 vector2_3 = vector2List[vector2List.Count - 1];
    Vector2 vector2_4 = patrolPoints[0];
    Color color = ((Color) ref CivCommanderBotOverlay.PatrolColor).WithAlpha(0.4f);
    ((DrawingHandleBase) drawingHandleWorld).DrawLine(vector2_3, vector2_4, color);
  }

  private static Color GetOrderColor(CivBotOrderType order)
  {
    Color orderColor;
    switch (order)
    {
      case CivBotOrderType.Idle:
        orderColor = CivCommanderBotOverlay.OrderIdleColor;
        break;
      case CivBotOrderType.Move:
        orderColor = CivCommanderBotOverlay.OrderMoveColor;
        break;
      case CivBotOrderType.AttackMove:
        orderColor = CivCommanderBotOverlay.OrderAttackMoveColor;
        break;
      case CivBotOrderType.HoldPosition:
        orderColor = CivCommanderBotOverlay.OrderHoldColor;
        break;
      case CivBotOrderType.Follow:
        orderColor = CivCommanderBotOverlay.OrderFollowColor;
        break;
      case CivBotOrderType.Defend:
        orderColor = CivCommanderBotOverlay.OrderDefendColor;
        break;
      case CivBotOrderType.Patrol:
        orderColor = CivCommanderBotOverlay.OrderPatrolColor;
        break;
      default:
        orderColor = Color.White;
        break;
    }
    return orderColor;
  }

  private static string GetOrderIndicator(CivBotOrderType order)
  {
    string orderIndicator;
    switch (order)
    {
      case CivBotOrderType.AttackMove:
        orderIndicator = "A";
        break;
      case CivBotOrderType.HoldPosition:
        orderIndicator = "H";
        break;
      case CivBotOrderType.Follow:
        orderIndicator = "F";
        break;
      case CivBotOrderType.Defend:
        orderIndicator = "D";
        break;
      case CivBotOrderType.Patrol:
        orderIndicator = "P";
        break;
      default:
        orderIndicator = string.Empty;
        break;
    }
    return orderIndicator;
  }

  private static bool CircleIntersects(Box2 world, Vector2 center, float radius)
  {
    float x = Math.Clamp(center.X, world.Left, world.Right);
    float y = Math.Clamp(center.Y, world.Bottom, world.Top);
    return (double) (center - new Vector2(x, y)).LengthSquared() <= (double) radius * (double) radius;
  }
}
