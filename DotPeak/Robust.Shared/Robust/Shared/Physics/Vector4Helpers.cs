// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Vector4Helpers
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Numerics;

#nullable disable
namespace Robust.Shared.Physics;

internal static class Vector4Helpers
{
  public static Vector4 Inverse(Vector4 matrix)
  {
    float x = matrix.X;
    float z = matrix.Z;
    float y = matrix.Y;
    float w = matrix.W;
    float num = (float) ((double) x * (double) w - (double) z * (double) y);
    if ((double) num != 0.0)
      num = 1f / num;
    return new Vector4(num * w, -num * y, -num * z, num * x);
  }

  public static void Inverse(Span<Vector2> matrix)
  {
    float x1 = matrix[0].X;
    float x2 = matrix[1].X;
    float y1 = matrix[0].Y;
    float y2 = matrix[1].Y;
    float num = (float) ((double) x1 * (double) y2 - (double) x2 * (double) y1);
    if ((double) num != 0.0)
      num = 1f / num;
    matrix[0].X = num * y2;
    matrix[0].Y = -num * y1;
    matrix[1].X = -num * x2;
    matrix[1].Y = num * x1;
  }
}
