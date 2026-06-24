// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.PubgZoneOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG;

public sealed class PubgZoneOverlay : Overlay
{
  private readonly IResourceCache _resourceCache;
  public Vector2? CurrentCenter;
  public float CurrentRadius;
  public Vector2? NextCenter;
  public float NextRadius;
  public ZoneState State;
  public bool Active;
  public bool Visible;
  public MapId ZoneMapId;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public PubgZoneOverlay(IResourceCache resourceCache)
  {
    IoCManager.InjectDependencies<PubgZoneOverlay>(this);
    this._resourceCache = resourceCache;
    this.ZIndex = new int?(-10);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (!this.Active || !this.Visible || !this.CurrentCenter.HasValue || MapId.op_Inequality(args.MapId, this.ZoneMapId))
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    Box2 viewport = ((Box2Rotated) ref args.WorldBounds).CalcBoundingBox();
    this.DrawFogOutsideZone(worldHandle, viewport, this.CurrentCenter.Value, this.CurrentRadius);
    if (this.NextCenter.HasValue)
      this.DrawZoneCircle(worldHandle, this.NextCenter.Value, this.NextRadius, Color.White, 0.5f);
    Color color = this.State == ZoneState.Waiting ? Color.CornflowerBlue : Color.OrangeRed;
    float alpha = this.State == ZoneState.Waiting ? 0.7f : 1f;
    float thickness = this.State == ZoneState.Waiting ? 0.3f : 0.5f;
    this.DrawZoneCircle(worldHandle, this.CurrentCenter.Value, this.CurrentRadius, color, alpha, thickness);
  }

  private void DrawFogOutsideZone(
    DrawingHandleWorld handle,
    Box2 viewport,
    Vector2 center,
    float radius)
  {
    Color color;
    // ISSUE: explicit constructor call
    ((Color) ref color).\u002Ector((byte) 80 /*0x50*/, (byte) 60, (byte) 60, (byte) 180);
    ((Box2) ref viewport).Enlarged(radius * 2f);
    Vector2[] vector2Array = new Vector2[6];
    for (int index = 0; index < 64 /*0x40*/; ++index)
    {
      float x1 = (float) ((double) index / 64.0 * 3.1415927410125732 * 2.0);
      float x2 = (float) ((double) (index + 1) / 64.0 * 3.1415927410125732 * 2.0);
      Vector2 vector2_1 = new Vector2(center.X + MathF.Cos(x1) * radius, center.Y + MathF.Sin(x1) * radius);
      Vector2 vector2_2 = new Vector2(center.X + MathF.Cos(x2) * radius, center.Y + MathF.Sin(x2) * radius);
      float num = radius + Math.Max(((Box2) ref viewport).Width, ((Box2) ref viewport).Height);
      Vector2 vector2_3 = new Vector2(center.X + MathF.Cos(x1) * num, center.Y + MathF.Sin(x1) * num);
      Vector2 vector2_4 = new Vector2(center.X + MathF.Cos(x2) * num, center.Y + MathF.Sin(x2) * num);
      vector2Array[0] = vector2_1;
      vector2Array[1] = vector2_2;
      vector2Array[2] = vector2_3;
      vector2Array[3] = vector2_2;
      vector2Array[4] = vector2_4;
      vector2Array[5] = vector2_3;
      ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 1, (ReadOnlySpan<Vector2>) vector2Array, color);
    }
  }

  private void DrawZoneCircle(
    DrawingHandleWorld handle,
    Vector2 center,
    float radius,
    Color color,
    float alpha,
    float thickness = 0.3f)
  {
    for (int index = 0; index < 128 /*0x80*/; ++index)
    {
      float x1 = (float) ((double) index / 128.0 * 3.1415927410125732 * 2.0);
      float x2 = (float) ((double) (index + 1) / 128.0 * 3.1415927410125732 * 2.0);
      Vector2 vector2_1 = new Vector2(center.X + MathF.Cos(x1) * radius, center.Y + MathF.Sin(x1) * radius);
      Vector2 vector2_2 = new Vector2(center.X + MathF.Cos(x2) * radius, center.Y + MathF.Sin(x2) * radius);
      ((DrawingHandleBase) handle).DrawLine(vector2_1, vector2_2, ((Color) ref color).WithAlpha(alpha));
    }
  }
}
