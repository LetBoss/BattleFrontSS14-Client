// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.RedZoneOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG;

public sealed class RedZoneOverlay : Overlay
{
  public bool BombActive;
  public Vector2 BombCenter;
  public float BombRadius;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public RedZoneOverlay()
  {
    IoCManager.InjectDependencies<RedZoneOverlay>(this);
    this.ZIndex = new int?(-5);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (!this.BombActive || (double) this.BombRadius <= 0.0)
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    DrawingHandleWorld handle = worldHandle;
    Vector2 bombCenter = this.BombCenter;
    double bombRadius = (double) this.BombRadius;
    Color red = Color.Red;
    Color color = ((Color) ref red).WithAlpha(0.6f);
    this.DrawFilledCircle(handle, bombCenter, (float) bombRadius, color);
    this.DrawCircleOutline(worldHandle, this.BombCenter, this.BombRadius, Color.OrangeRed, 0.2f);
  }

  private void DrawFilledCircle(
    DrawingHandleWorld handle,
    Vector2 center,
    float radius,
    Color color)
  {
    Vector2[] vector2Array = new Vector2[3];
    for (int index = 0; index < 32 /*0x20*/; ++index)
    {
      float x1 = (float) ((double) index / 32.0 * 3.1415927410125732 * 2.0);
      float x2 = (float) ((double) (index + 1) / 32.0 * 3.1415927410125732 * 2.0);
      Vector2 vector2_1 = new Vector2(center.X + MathF.Cos(x1) * radius, center.Y + MathF.Sin(x1) * radius);
      Vector2 vector2_2 = new Vector2(center.X + MathF.Cos(x2) * radius, center.Y + MathF.Sin(x2) * radius);
      vector2Array[0] = center;
      vector2Array[1] = vector2_1;
      vector2Array[2] = vector2_2;
      ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 1, (ReadOnlySpan<Vector2>) vector2Array, color);
    }
  }

  private void DrawCircleOutline(
    DrawingHandleWorld handle,
    Vector2 center,
    float radius,
    Color color,
    float thickness)
  {
    for (int index = 0; index < 64 /*0x40*/; ++index)
    {
      float x1 = (float) ((double) index / 64.0 * 3.1415927410125732 * 2.0);
      float x2 = (float) ((double) (index + 1) / 64.0 * 3.1415927410125732 * 2.0);
      Vector2 vector2_1 = new Vector2(center.X + MathF.Cos(x1) * radius, center.Y + MathF.Sin(x1) * radius);
      Vector2 vector2_2 = new Vector2(center.X + MathF.Cos(x2) * radius, center.Y + MathF.Sin(x2) * radius);
      ((DrawingHandleBase) handle).DrawLine(vector2_1, vector2_2, color);
    }
  }
}
