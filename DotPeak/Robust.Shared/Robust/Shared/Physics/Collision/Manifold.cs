// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Collision.Manifold
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics.Collision;

public struct Manifold : IEquatable<Manifold>, IApproxEquatable<Manifold>
{
  public Vector2 LocalNormal;
  public Vector2 LocalPoint;
  public int PointCount;
  internal FixedArray2<ManifoldPoint> Points;
  public ManifoldType Type;

  public bool Equals(Manifold other)
  {
    if (this.PointCount != other.PointCount || this.Type != other.Type || !this.LocalNormal.Equals(other.LocalNormal) || !this.LocalPoint.Equals(other.LocalPoint))
      return false;
    Span<ManifoldPoint> asSpan1 = this.Points.AsSpan;
    Span<ManifoldPoint> asSpan2 = other.Points.AsSpan;
    for (int index = 0; index < this.PointCount; ++index)
    {
      if (!asSpan1[index].Equals(asSpan2[index]))
        return false;
    }
    return true;
  }

  public bool EqualsApprox(Manifold other)
  {
    if (this.PointCount != other.PointCount || this.Type != other.Type || !Vector2Helpers.EqualsApprox(this.LocalNormal, other.LocalNormal) || !Vector2Helpers.EqualsApprox(this.LocalPoint, other.LocalPoint))
      return false;
    Span<ManifoldPoint> asSpan1 = this.Points.AsSpan;
    Span<ManifoldPoint> asSpan2 = other.Points.AsSpan;
    for (int index = 0; index < this.PointCount; ++index)
    {
      if (!asSpan1[index].EqualsApprox(asSpan2[index]))
        return false;
    }
    return true;
  }

  public bool EqualsApprox(Manifold other, double tolerance)
  {
    if (this.PointCount != other.PointCount || this.Type != other.Type || !Vector2Helpers.EqualsApprox(this.LocalNormal, other.LocalNormal, tolerance) || !Vector2Helpers.EqualsApprox(this.LocalPoint, other.LocalPoint, tolerance))
      return false;
    Span<ManifoldPoint> asSpan1 = this.Points.AsSpan;
    Span<ManifoldPoint> asSpan2 = other.Points.AsSpan;
    for (int index = 0; index < this.PointCount; ++index)
    {
      if (!asSpan1[index].EqualsApprox(asSpan2[index], tolerance))
        return false;
    }
    return true;
  }

  public override bool Equals(object? obj) => obj is Manifold other && this.Equals(other);

  public override int GetHashCode()
  {
    return HashCode.Combine<Vector2, Vector2, int, FixedArray2<ManifoldPoint>, int>(this.LocalNormal, this.LocalPoint, this.PointCount, this.Points, (int) this.Type);
  }
}
