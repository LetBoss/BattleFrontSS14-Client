// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.CollinearSimplifier
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics;

public sealed class CollinearSimplifier : IVerticesSimplifier
{
  public List<Vector2> Simplify(List<Vector2> vertices, float tolerance = 0.0f)
  {
    if (vertices.Count <= 3)
      return vertices;
    List<Vector2> vector2List1 = new List<Vector2>(vertices.Count);
    for (int index = 0; index < vertices.Count; ++index)
    {
      Vector2 prev = vertices[index == 0 ? vertices.Count - 1 : index - 1];
      Vector2 current = vertices[index];
      Vector2 next = vertices[(index + 1) % vertices.Count];
      if (!CollinearSimplifier.IsCollinear(in prev, in current, in next, tolerance))
        vector2List1.Add(current);
    }
    if (vector2List1.Count == 0)
    {
      vector2List1.Add(vertices[0]);
      List<Vector2> vector2List2 = vector2List1;
      List<Vector2> vector2List3 = vertices;
      Vector2 vector2 = vector2List3[vector2List3.Count - 1];
      vector2List2.Add(vector2);
    }
    return vector2List1;
  }

  public static bool IsCollinear(
    in Vector2 prev,
    in Vector2 current,
    in Vector2 next,
    float tolerance)
  {
    return CollinearSimplifier.FloatInRange(CollinearSimplifier.Area(in prev, in current, in next), -tolerance, tolerance);
  }

  private static float Area(in Vector2 a, in Vector2 b, in Vector2 c)
  {
    return (float) ((double) a.X * ((double) b.Y - (double) c.Y) + (double) b.X * ((double) c.Y - (double) a.Y) + (double) c.X * ((double) a.Y - (double) b.Y));
  }

  private static bool FloatInRange(float value, float min, float max)
  {
    return (double) value >= (double) min && (double) value <= (double) max;
  }
}
