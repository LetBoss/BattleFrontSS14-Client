// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Graphics.Eye
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;
using System.Numerics;

#nullable disable
namespace Robust.Shared.Graphics;

[Virtual]
public class Eye : IEye
{
  private Vector2 _scale = Vector2.One / 2f;
  private Angle _rotation = Angle.Zero;
  private MapCoordinates _coords;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool DrawFov { get; set; } = true;

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool DrawLight { get; set; } = true;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public virtual MapCoordinates Position
  {
    get => this._coords;
    set => this._coords = value;
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public Vector2 Offset { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public Angle Rotation
  {
    get => this._rotation;
    set => this._rotation = value;
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public Vector2 Zoom
  {
    get => new Vector2(1f / this._scale.X, 1f / this._scale.Y);
    set => this._scale = new Vector2(1f / value.X, 1f / value.Y);
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public Vector2 Scale
  {
    get => this._scale;
    set => this._scale = value;
  }

  public void GetViewMatrix(out Matrix3x2 viewMatrix, Vector2 renderScale)
  {
    viewMatrix = Matrix3Helpers.CreateInverseTransform(this._coords.Position.X + this.Offset.X, this._coords.Position.Y + this.Offset.Y, -this.Rotation.Theta, (float) (1.0 / ((double) this._scale.X * (double) renderScale.X)), (float) (1.0 / ((double) this._scale.Y * (double) renderScale.Y)));
  }

  public void GetViewMatrixNoOffset(out Matrix3x2 viewMatrix, Vector2 renderScale)
  {
    viewMatrix = Matrix3Helpers.CreateInverseTransform(this._coords.Position.X, this._coords.Position.Y, -this.Rotation.Theta, (float) (1.0 / ((double) this._scale.X * (double) renderScale.X)), (float) (1.0 / ((double) this._scale.Y * (double) renderScale.Y)));
  }

  public void GetViewMatrixInv(out Matrix3x2 viewMatrixInv, Vector2 renderScale)
  {
    viewMatrixInv = Matrix3Helpers.CreateTransform(this._coords.Position.X + this.Offset.X, this._coords.Position.Y + this.Offset.Y, -this.Rotation.Theta, (float) (1.0 / ((double) this._scale.X * (double) renderScale.X)), (float) (1.0 / ((double) this._scale.Y * (double) renderScale.Y)));
  }
}
