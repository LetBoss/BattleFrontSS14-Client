// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Vertices
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics;

public static class Vertices
{
  public static Vector2[] ForceCounterClockwise(Span<Vector2> vertices)
  {
    if (vertices.Length < 3 || Vertices.IsCounterClockwise(vertices))
      return vertices.ToArray();
    vertices.Reverse<Vector2>();
    return vertices.ToArray();
  }

  public static bool IsCounterClockwise(Span<Vector2> vertices)
  {
    return vertices.Length >= 3 && (double) Vertices.GetSignedArea(vertices) > 0.0;
  }

  public static float GetSignedArea(Span<Vector2> vertices)
  {
    int length = vertices.Length;
    if (length < 3)
      return 0.0f;
    float num = 0.0f;
    for (int index1 = 0; index1 < length; ++index1)
    {
      int index2 = (index1 + 1) % length;
      Vector2 vector2_1 = vertices[index1];
      Vector2 vector2_2 = vertices[index2];
      num = num + vector2_1.X * vector2_2.Y - vector2_1.Y * vector2_2.X;
    }
    return num / 2f;
  }
}
