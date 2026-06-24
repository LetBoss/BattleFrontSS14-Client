// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Dynamics.Sweep
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Numerics;

#nullable disable
namespace Robust.Shared.Physics.Dynamics;

internal struct Sweep
{
  public Vector2 LocalCenter;
  public float Angle;
  public float Angle0;
  public float Alpha0;
  public Vector2 Center;
  public Vector2 Center0;

  public Transform GetTransform(float beta)
  {
    Transform transform = new Transform(this.Center0 * (1f - beta) + this.Center * beta, (float) ((1.0 - (double) beta) * (double) this.Angle0 + (double) beta * (double) this.Angle));
    transform.Position -= Transform.Mul(in transform.Quaternion2D, in this.LocalCenter);
    return transform;
  }

  public void Advance(float alpha)
  {
    float num = (float) (((double) alpha - (double) this.Alpha0) / (1.0 - (double) this.Alpha0));
    this.Center0 += (this.Center - this.Center0) * num;
    this.Angle0 += num * (this.Angle - this.Angle0);
    this.Alpha0 = alpha;
  }

  public void Normalize()
  {
    float num1 = 6.28318548f;
    float num2 = num1 * MathF.Floor(this.Angle0 / num1);
    this.Angle0 -= num2;
    this.Angle -= num2;
  }
}
