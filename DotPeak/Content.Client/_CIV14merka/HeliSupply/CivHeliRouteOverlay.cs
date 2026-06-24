// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.HeliSupply.CivHeliRouteOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.HeliSupply;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.HeliSupply;

public sealed class CivHeliRouteOverlay : Overlay
{
  private readonly CivHeliSupplySystem _system;
  private const float LineThickness = 0.3f;
  private const float PointRadius = 0.25f;
  private static readonly Color RouteColor = Color.FromHex((ReadOnlySpan<char>) "#FFD23F", new Color?());
  private static readonly Color DropColor = Color.FromHex((ReadOnlySpan<char>) "#E63946", new Color?());

  public virtual OverlaySpace Space => (OverlaySpace) 8;

  public CivHeliRouteOverlay(CivHeliSupplySystem system) => this._system = system;

  protected virtual bool BeforeDraw(in OverlayDrawArgs args) => this._system.IsRouteMode;

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    IReadOnlyList<Vector2> routePoints = this._system.RoutePoints;
    if (routePoints.Count > 0 && MapId.op_Inequality(this._system.RouteMapId, args.MapId))
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    CivHeliStateMessage lastState = this._system.LastState;
    if (lastState != null && lastState.HasOrigin && MapId.op_Equality(lastState.OriginMapId, args.MapId))
    {
      Vector2 origin = lastState.Origin;
      ((DrawingHandleBase) worldHandle).DrawCircle(origin, 0.325f, ((Color) ref CivHeliRouteOverlay.RouteColor).WithAlpha(0.7f), true);
      if (routePoints.Count > 0)
        CivHeliRouteOverlay.DrawThickLine(worldHandle, origin, routePoints[0], 0.24000001f, ((Color) ref CivHeliRouteOverlay.RouteColor).WithAlpha(0.6f));
      else if (MapId.op_Equality(this._system.GetCursorMapId(), args.MapId))
        CivHeliRouteOverlay.DrawThickLine(worldHandle, origin, this._system.GetCursorWorldPosition(), 0.18f, ((Color) ref CivHeliRouteOverlay.RouteColor).WithAlpha(0.35f));
    }
    for (int index = 0; index < routePoints.Count - 1; ++index)
      CivHeliRouteOverlay.DrawThickLine(worldHandle, routePoints[index], routePoints[index + 1], 0.3f, ((Color) ref CivHeliRouteOverlay.RouteColor).WithAlpha(0.8f));
    for (int index = 0; index < routePoints.Count; ++index)
    {
      bool flag = index == routePoints.Count - 1;
      ((DrawingHandleBase) worldHandle).DrawCircle(routePoints[index], flag ? 0.4f : 0.25f, flag ? CivHeliRouteOverlay.DropColor : CivHeliRouteOverlay.RouteColor, true);
    }
    if (routePoints.Count <= 0 || !MapId.op_Equality(this._system.GetCursorMapId(), args.MapId))
      return;
    Vector2 cursorWorldPosition = this._system.GetCursorWorldPosition();
    DrawingHandleWorld handle = worldHandle;
    IReadOnlyList<Vector2> vector2List = routePoints;
    Vector2 a = vector2List[vector2List.Count - 1];
    Vector2 b = cursorWorldPosition;
    Color color = ((Color) ref CivHeliRouteOverlay.RouteColor).WithAlpha(0.45f);
    CivHeliRouteOverlay.DrawThickLine(handle, a, b, 0.210000008f, color);
    ((DrawingHandleBase) worldHandle).DrawCircle(cursorWorldPosition, 0.2f, ((Color) ref CivHeliRouteOverlay.RouteColor).WithAlpha(0.6f), true);
  }

  private static void DrawThickLine(
    DrawingHandleWorld handle,
    Vector2 a,
    Vector2 b,
    float thickness,
    Color color)
  {
    Vector2 vector2_1 = b - a;
    float num1 = vector2_1.Length();
    if ((double) num1 < 1.0 / 1000.0)
      return;
    Vector2 vector2_2 = vector2_1 / num1;
    Vector2 vector2_3 = new Vector2(-vector2_2.Y, vector2_2.X);
    float x = thickness * 0.5f;
    ((DrawingHandleBase) handle).DrawLine(a, b, color);
    int num2 = (int) MathF.Ceiling(x / 0.04f);
    for (int index = 1; index <= num2; ++index)
    {
      float num3 = MathF.Min(x, (float) index * 0.04f);
      Vector2 vector2_4 = vector2_3 * num3;
      ((DrawingHandleBase) handle).DrawLine(a + vector2_4, b + vector2_4, color);
      ((DrawingHandleBase) handle).DrawLine(a - vector2_4, b - vector2_4, color);
    }
  }
}
