// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Quaternion2D
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Numerics;

#nullable disable
namespace Robust.Shared.Physics;

public struct Quaternion2D
{
  public float C;
  public float S;

  public float Angle => MathF.Atan2(this.S, this.C);

  public Quaternion2D(float cos, float sin)
  {
    this.C = cos;
    this.S = sin;
  }

  public Quaternion2D(float angle)
  {
    this.C = MathF.Cos(angle);
    this.S = MathF.Sin(angle);
  }

  public Quaternion2D(Robust.Shared.Maths.Angle angle)
  {
    double theta = angle.Theta;
    this.C = (float) Math.Cos(theta);
    this.S = (float) Math.Sin(theta);
  }

  public Quaternion2D Set(float angle)
  {
    return (double) angle == 0.0 ? new Quaternion2D(1f, 0.0f) : new Quaternion2D(MathF.Cos(angle), MathF.Sin(angle));
  }

  public static Vector2 RotateVector(Quaternion2D q, Vector2 v)
  {
    return new Vector2((float) ((double) q.C * (double) v.X - (double) q.S * (double) v.Y), (float) ((double) q.S * (double) v.X + (double) q.C * (double) v.Y));
  }

  public static Vector2 InvRotateVector(Quaternion2D q, Vector2 v)
  {
    return new Vector2((float) ((double) q.C * (double) v.X + (double) q.S * (double) v.Y), (float) (-(double) q.S * (double) v.X + (double) q.C * (double) v.Y));
  }

  public bool IsValid()
  {
    return !float.IsNaN(this.S) && !float.IsNaN(this.C) && !float.IsInfinity(this.S) && !float.IsInfinity(this.C) && this.IsNormalized();
  }

  public bool IsNormalized()
  {
    float num = (float) ((double) this.S * (double) this.S + (double) this.C * (double) this.C);
    return 0.99940001964569092 < (double) num && (double) num < 1.0005999803543091;
  }

  public static Quaternion2D InvMulRot(Quaternion2D q, Quaternion2D r)
  {
    return new Quaternion2D((float) ((double) q.C * (double) r.C + (double) q.S * (double) r.S), (float) ((double) q.C * (double) r.S - (double) q.S * (double) r.C));
  }
}
