// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Collision.ManifoldPoint
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics.Collision;

public struct ManifoldPoint : IEquatable<ManifoldPoint>, IApproxEquatable<ManifoldPoint>
{
  public ContactID Id;
  public Vector2 LocalPoint;
  public float NormalImpulse;
  public float TangentImpulse;

  public static bool operator ==(ManifoldPoint point, ManifoldPoint other)
  {
    return point.Id == other.Id && point.LocalPoint.Equals(other.LocalPoint) && point.NormalImpulse.Equals(other.NormalImpulse) && point.TangentImpulse.Equals(other.TangentImpulse);
  }

  public static bool operator !=(ManifoldPoint point, ManifoldPoint other) => !(point == other);

  public override bool Equals(object? obj)
  {
    return obj is ManifoldPoint manifoldPoint && this == manifoldPoint;
  }

  public bool Equals(ManifoldPoint other) => this == other;

  public override int GetHashCode()
  {
    return ((this.Id.GetHashCode() * 397 ^ this.LocalPoint.GetHashCode()) * 397 ^ this.NormalImpulse.GetHashCode()) * 397 ^ this.TangentImpulse.GetHashCode();
  }

  public bool EqualsApprox(ManifoldPoint other)
  {
    return this.Id == other.Id && Vector2Helpers.EqualsApprox(this.LocalPoint, other.LocalPoint) && MathHelper.CloseToPercent(this.NormalImpulse, other.NormalImpulse, 1E-05) && MathHelper.CloseToPercent(this.TangentImpulse, other.TangentImpulse, 1E-05);
  }

  public bool EqualsApprox(ManifoldPoint other, double tolerance)
  {
    return this.Id == other.Id && Vector2Helpers.EqualsApprox(this.LocalPoint, other.LocalPoint, tolerance) && MathHelper.CloseToPercent(this.NormalImpulse, other.NormalImpulse, tolerance) && MathHelper.CloseToPercent(this.TangentImpulse, other.TangentImpulse, tolerance);
  }
}
