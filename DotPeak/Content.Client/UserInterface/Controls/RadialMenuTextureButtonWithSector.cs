// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.RadialMenuTextureButtonWithSector
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Controls;

[Virtual]
public class RadialMenuTextureButtonWithSector : RadialMenuTextureButton, IRadialMenuItemWithSector
{
  private Vector2[]? _sectorPointsForDrawing;
  private float _angleSectorFrom;
  private float _angleSectorTo;
  private float _outerRadius;
  private float _innerRadius;
  private float _angleOffset;
  private bool _isWholeCircle;
  private Vector2? _parentCenter;
  private Color _backgroundColorSrgb = Color.ToSrgb(new Color((byte) 70, (byte) 73, (byte) 102, (byte) 128 /*0x80*/));
  private Color _hoverBackgroundColorSrgb = Color.ToSrgb(new Color((byte) 87, (byte) 91, (byte) 127 /*0x7F*/, (byte) 128 /*0x80*/));
  private Color _borderColorSrgb = Color.ToSrgb(new Color((byte) 173, (byte) 216, (byte) 230, (byte) 70));
  private Color _hoverBorderColorSrgb = Color.ToSrgb(new Color((byte) 87, (byte) 91, (byte) 127 /*0x7F*/, (byte) 128 /*0x80*/));

  public bool DrawBorder { get; set; }

  public bool DrawBackground { get; set; } = true;

  public Color BackgroundColor
  {
    get => Color.FromSrgb(this._backgroundColorSrgb);
    set => this._backgroundColorSrgb = Color.ToSrgb(value);
  }

  public Color HoverBackgroundColor
  {
    get => Color.FromSrgb(this._hoverBackgroundColorSrgb);
    set => this._hoverBackgroundColorSrgb = Color.ToSrgb(value);
  }

  public Color BorderColor
  {
    get => Color.FromSrgb(this._borderColorSrgb);
    set => this._borderColorSrgb = Color.ToSrgb(value);
  }

  public Color HoverBorderColor
  {
    get => Color.FromSrgb(this._hoverBorderColorSrgb);
    set => this._hoverBorderColorSrgb = Color.ToSrgb(value);
  }

  public Color SeparatorColor { get; set; } = new Color((byte) 128 /*0x80*/, (byte) 128 /*0x80*/, (byte) 128 /*0x80*/, (byte) 128 /*0x80*/);

  float IRadialMenuItemWithSector.AngleSectorFrom
  {
    set
    {
      this._angleSectorFrom = value;
      this._isWholeCircle = RadialMenuTextureButtonWithSector.IsWholeCircle(value, this._angleSectorTo);
    }
  }

  float IRadialMenuItemWithSector.AngleSectorTo
  {
    set
    {
      this._angleSectorTo = value;
      this._isWholeCircle = RadialMenuTextureButtonWithSector.IsWholeCircle(this._angleSectorFrom, value);
    }
  }

  float IRadialMenuItemWithSector.OuterRadius
  {
    set => this._outerRadius = value;
  }

  float IRadialMenuItemWithSector.InnerRadius
  {
    set => this._innerRadius = value;
  }

  public float AngleOffset
  {
    set => this._angleOffset = value;
  }

  Vector2 IRadialMenuItemWithSector.ParentCenter
  {
    set => this._parentCenter = new Vector2?(value);
  }

  protected virtual void Draw(DrawingHandleScreen handle)
  {
    base.Draw(handle);
    if (!this._parentCenter.HasValue)
      return;
    Vector2 center = (this._parentCenter.Value - ((Control) this).Position) * ((Control) this).UIScale;
    float angleSectorFrom = this._angleSectorFrom + this._angleOffset;
    float angleSectorTo = this._angleSectorTo + this._angleOffset;
    if (this.DrawBackground)
    {
      Color color = ((BaseButton) this).DrawMode == 2 ? this._hoverBackgroundColorSrgb : this._backgroundColorSrgb;
      this.DrawAnnulusSector(handle, center, this._innerRadius * ((Control) this).UIScale, this._outerRadius * ((Control) this).UIScale, angleSectorFrom, angleSectorTo, color);
    }
    if (this.DrawBorder)
    {
      Color color = ((BaseButton) this).DrawMode == 2 ? this._hoverBorderColorSrgb : this._borderColorSrgb;
      this.DrawAnnulusSector(handle, center, this._innerRadius * ((Control) this).UIScale, this._outerRadius * ((Control) this).UIScale, angleSectorFrom, angleSectorTo, color, false);
    }
    if (this._isWholeCircle || !this.DrawBorder)
      return;
    RadialMenuTextureButtonWithSector.DrawSeparatorLines(handle, center, this._innerRadius * ((Control) this).UIScale, this._outerRadius * ((Control) this).UIScale, angleSectorFrom, angleSectorTo, this.SeparatorColor);
  }

  protected virtual bool HasPoint(Vector2 point)
  {
    if (!this._parentCenter.HasValue)
      return ((Control) this).HasPoint(point);
    float num1 = this._outerRadius * this._outerRadius;
    float num2 = this._innerRadius * this._innerRadius;
    float num3 = (point + ((Control) this).Position - this._parentCenter.Value).LengthSquared();
    if (((double) num3 >= (double) num1 ? 0 : ((double) num3 > (double) num2 ? 1 : 0)) == 0)
      return false;
    Vector2 vector2 = point + ((Control) this).Position - this._parentCenter.Value;
    float num4 = MathF.Atan2(-vector2.Y, vector2.X) - this._angleOffset;
    if ((double) num4 < 0.0)
      num4 = 6.28318548f + num4;
    return (double) num4 >= (double) this._angleSectorFrom && (double) num4 < (double) this._angleSectorTo;
  }

  private void DrawAnnulusSector(
    DrawingHandleScreen drawingHandleScreen,
    Vector2 center,
    float radiusInner,
    float radiusOuter,
    float angleSectorFrom,
    float angleSectorTo,
    Color color,
    bool filled = true)
  {
    double num1 = (double) angleSectorTo - (double) angleSectorFrom;
    int num2 = (int) (num1 / 0.049087386578321457) + 1;
    float num3 = (float) num1 / (float) (num2 - 1);
    int length = num2 * 2;
    if ((this._sectorPointsForDrawing == null || this._sectorPointsForDrawing.Length != length) && this._sectorPointsForDrawing == null)
      this._sectorPointsForDrawing = new Vector2[length];
    for (int index = 0; index < num2; ++index)
    {
      float x = angleSectorFrom + num3 * (float) index;
      Vector2 vector2_1 = new Vector2(MathF.Cos(x), -MathF.Sin(x));
      Vector2 vector2_2 = center + vector2_1 * radiusOuter;
      Vector2 vector2_3 = center + vector2_1 * radiusInner;
      if (filled)
      {
        this._sectorPointsForDrawing[index * 2] = vector2_2;
        this._sectorPointsForDrawing[index * 2 + 1] = vector2_3;
      }
      else
      {
        this._sectorPointsForDrawing[index] = vector2_2;
        this._sectorPointsForDrawing[length - 1 - index] = vector2_3;
      }
    }
    DrawPrimitiveTopology primitiveTopology = filled ? (DrawPrimitiveTopology) 3 : (DrawPrimitiveTopology) 5;
    ((DrawingHandleBase) drawingHandleScreen).DrawPrimitives(primitiveTopology, (ReadOnlySpan<Vector2>) this._sectorPointsForDrawing, color);
  }

  private static void DrawSeparatorLines(
    DrawingHandleScreen drawingHandleScreen,
    Vector2 center,
    float radiusInner,
    float radiusOuter,
    float angleSectorFrom,
    float angleSectorTo,
    Color color)
  {
    Angle angle1 = new Angle(-(double) angleSectorFrom);
    ref Angle local1 = ref angle1;
    Vector2 unitX1 = Vector2.UnitX;
    ref Vector2 local2 = ref unitX1;
    Vector2 vector2_1 = ((Angle) ref local1).RotateVec(ref local2);
    ((DrawingHandleBase) drawingHandleScreen).DrawLine(center + vector2_1 * radiusOuter, center + vector2_1 * radiusInner, color);
    Angle angle2 = new Angle(-(double) angleSectorTo);
    ref Angle local3 = ref angle2;
    Vector2 unitX2 = Vector2.UnitX;
    ref Vector2 local4 = ref unitX2;
    Vector2 vector2_2 = ((Angle) ref local3).RotateVec(ref local4);
    ((DrawingHandleBase) drawingHandleScreen).DrawLine(center + vector2_2 * radiusOuter, center + vector2_2 * radiusInner, color);
  }

  private static bool IsWholeCircle(float angleSectorFrom, float angleSectorTo)
  {
    Angle angle = new Angle((double) angleSectorFrom);
    return ((Angle) ref angle).EqualsApprox(new Angle((double) angleSectorTo));
  }
}
