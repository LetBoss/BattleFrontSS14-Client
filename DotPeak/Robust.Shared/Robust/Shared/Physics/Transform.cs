// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Transform
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

#nullable enable
namespace Robust.Shared.Physics;

public struct Transform
{
  public static readonly Transform Empty = new Transform(0.0f);
  public Vector2 Position;
  public Quaternion2D Quaternion2D;

  public Transform(Vector2 position, Quaternion2D quat)
  {
    this.Position = position;
    this.Quaternion2D = quat;
  }

  public Transform(Vector2 position, float angle)
  {
    this.Position = position;
    this.Quaternion2D = new Quaternion2D(angle);
  }

  public Transform(float angle)
  {
    this.Position = Vector2.Zero;
    this.Quaternion2D = new Quaternion2D(angle);
  }

  public Transform(Vector2 position, Angle angle)
  {
    this.Position = position;
    this.Quaternion2D = new Quaternion2D(angle);
  }

  public static Vector2 InvTransformPoint(Transform t, Vector2 p)
  {
    float num1 = p.X - t.Position.X;
    float num2 = p.Y - t.Position.Y;
    return new Vector2((float) ((double) t.Quaternion2D.C * (double) num1 + (double) t.Quaternion2D.S * (double) num2), (float) (-(double) t.Quaternion2D.S * (double) num1 + (double) t.Quaternion2D.C * (double) num2));
  }

  public static Vector2 Mul(in Transform transform, in Vector2 vector)
  {
    return new Vector2((float) ((double) transform.Quaternion2D.C * (double) vector.X - (double) transform.Quaternion2D.S * (double) vector.Y) + transform.Position.X, (float) ((double) transform.Quaternion2D.S * (double) vector.X + (double) transform.Quaternion2D.C * (double) vector.Y) + transform.Position.Y);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static void MulSimd(
    in Transform transform,
    Vector128<float> x,
    Vector128<float> y,
    out Vector128<float> xOut,
    out Vector128<float> yOut)
  {
    Vector128<float> vector128_1 = Vector128.Create(transform.Quaternion2D.C);
    Vector128<float> vector128_2 = Vector128.Create(transform.Quaternion2D.S);
    xOut = vector128_1 * x - vector128_2 * y + Vector128.Create(transform.Position.X);
    yOut = vector128_2 * x + vector128_1 * y + Vector128.Create(transform.Position.Y);
  }

  public static Vector2 MulT(in Vector2[] A, in Vector2 v)
  {
    return new Vector2((float) ((double) v.X * (double) A[0].X + (double) v.Y * (double) A[0].Y), (float) ((double) v.X * (double) A[1].X + (double) v.Y * (double) A[1].Y));
  }

  public static Vector2 MulT(in Transform T, in Vector2 v)
  {
    float num1 = v.X - T.Position.X;
    float num2 = v.Y - T.Position.Y;
    return new Vector2((float) ((double) T.Quaternion2D.C * (double) num1 + (double) T.Quaternion2D.S * (double) num2), (float) (-(double) T.Quaternion2D.S * (double) num1 + (double) T.Quaternion2D.C * (double) num2));
  }

  public static Quaternion2D MulT(in Quaternion2D q, in Quaternion2D r)
  {
    Quaternion2D quaternion2D;
    quaternion2D.S = (float) ((double) q.C * (double) r.S - (double) q.S * (double) r.C);
    quaternion2D.C = (float) ((double) q.C * (double) r.C + (double) q.S * (double) r.S);
    return quaternion2D;
  }

  public static Transform InvMulTransforms(in Transform A, in Transform B)
  {
    return new Transform(Quaternion2D.InvRotateVector(A.Quaternion2D, Vector2.Subtract(B.Position, A.Position)), Quaternion2D.InvMulRot(A.Quaternion2D, B.Quaternion2D));
  }

  public static Transform MulT(in Transform A, in Transform B)
  {
    return new Transform()
    {
      Quaternion2D = Transform.MulT(in A.Quaternion2D, in B.Quaternion2D),
      Position = Transform.MulT(A.Quaternion2D, B.Position - A.Position)
    };
  }

  public static Vector2 MulT(Quaternion2D q, Vector2 v)
  {
    return new Vector2((float) ((double) q.C * (double) v.X + (double) q.S * (double) v.Y), (float) (-(double) q.S * (double) v.X + (double) q.C * (double) v.Y));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Vector2 Mul(in Quaternion2D quaternion2D, in Vector2 vector)
  {
    return new Vector2((float) ((double) quaternion2D.C * (double) vector.X - (double) quaternion2D.S * (double) vector.Y), (float) ((double) quaternion2D.S * (double) vector.X + (double) quaternion2D.C * (double) vector.Y));
  }

  public static Vector2 Mul(Vector4 A, Vector2 v)
  {
    return new Vector2((float) ((double) A.X * (double) v.X + (double) A.Y * (double) v.Y), (float) ((double) A.Z * (double) v.X + (double) A.W * (double) v.Y));
  }

  public static Vector2 Mul(in Vector2[] A, in Vector2 v)
  {
    return new Vector2((float) ((double) A[0].X * (double) v.X + (double) A[1].X * (double) v.Y), (float) ((double) A[0].Y * (double) v.X + (double) A[1].Y * (double) v.Y));
  }

  public static Vector2 Mul(Matrix22 A, Vector2 v)
  {
    return new Vector2((float) ((double) A.EX.X * (double) v.X + (double) A.EY.X * (double) v.Y), (float) ((double) A.EX.Y * (double) v.X + (double) A.EY.Y * (double) v.Y));
  }
}
