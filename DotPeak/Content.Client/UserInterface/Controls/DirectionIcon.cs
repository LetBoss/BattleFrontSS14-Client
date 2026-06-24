// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.DirectionIcon
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Controls;

public sealed class DirectionIcon : TextureRect
{
  public static string StyleClassDirectionIconArrow = "direction-icon-arrow";
  public static string StyleClassDirectionIconHere = "direction-icon-here";
  public static string StyleClassDirectionIconUnknown = "direction-icon-unknown";
  private Angle? _rotation;
  private bool _snap;
  private float _minDistance;

  public Angle? Rotation
  {
    get => this._rotation;
    set
    {
      this._rotation = value;
      ((Control) this).SetOnlyStyleClass(!value.HasValue ? DirectionIcon.StyleClassDirectionIconUnknown : DirectionIcon.StyleClassDirectionIconArrow);
    }
  }

  public DirectionIcon()
  {
    this.Stretch = (TextureRect.StretchMode) 7;
    ((Control) this).SetOnlyStyleClass(DirectionIcon.StyleClassDirectionIconUnknown);
  }

  public DirectionIcon(bool snap = true, float minDistance = 0.1f)
    : this()
  {
    this._snap = snap;
    this._minDistance = minDistance;
  }

  public void UpdateDirection(Direction direction)
  {
    this.Rotation = new Angle?(DirectionExtensions.ToAngle(direction));
  }

  public void UpdateDirection(Vector2 direction, Angle relativeAngle)
  {
    if (Vector2Helpers.EqualsApprox(direction, Vector2.Zero, (double) this._minDistance))
    {
      ((Control) this).SetOnlyStyleClass(DirectionIcon.StyleClassDirectionIconHere);
    }
    else
    {
      Angle angle = Angle.op_Subtraction(DirectionExtensions.ToWorldAngle(direction), relativeAngle);
      this.Rotation = new Angle?(this._snap ? DirectionExtensions.ToAngle(((Angle) ref angle).GetDir()) : angle);
    }
  }

  protected virtual void Draw(DrawingHandleScreen handle)
  {
    if (this._rotation.HasValue)
    {
      Angle angle1 = Angle.op_UnaryNegation(this._rotation.Value);
      ref Angle local1 = ref angle1;
      Vector2 vector2_1 = ((Control) this).Size * ((Control) this).UIScale / 2f;
      ref Vector2 local2 = ref vector2_1;
      Vector2 vector2_2 = ((Angle) ref local1).RotateVec(ref local2) - ((Control) this).Size * ((Control) this).UIScale / 2f;
      DrawingHandleScreen drawingHandleScreen = handle;
      Vector2 vector2_3 = Vector2i.op_Implicit(((Control) this).GlobalPixelPosition) - vector2_2;
      ref Vector2 local3 = ref vector2_3;
      Angle angle2 = Angle.op_UnaryNegation(this._rotation.Value);
      ref Angle local4 = ref angle2;
      Matrix3x2 transform = Matrix3Helpers.CreateTransform(ref local3, ref local4);
      ref Matrix3x2 local5 = ref transform;
      ((DrawingHandleBase) drawingHandleScreen).SetTransform(ref local5);
    }
    base.Draw(handle);
  }
}
