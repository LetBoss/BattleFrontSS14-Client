// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Ray
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics;

[Serializable]
public struct Ray(Vector2 position, Vector2 direction) : IEquatable<Ray>
{
  public Vector2 Position = position;
  public Vector2 Direction = direction;

  public readonly bool Intersects(Box2 box, out float distance, out Vector2 hitPos)
  {
    hitPos = Vector2.Zero;
    distance = 0.0f;
    float y1 = 0.0f;
    float maxValue = float.MaxValue;
    if ((double) MathF.Abs(this.Direction.X) < 1.0000000116860974E-07 && ((double) this.Position.X < (double) MathF.Min(box.Left, box.Right) || (double) this.Position.X > (double) MathF.Max(box.Left, box.Right)))
      return false;
    float num1 = 1f / this.Direction.X;
    float x1 = (MathF.Min(box.Left, box.Right) - this.Position.X) * num1;
    float x2 = (MathF.Max(box.Left, box.Right) - this.Position.X) * num1;
    if ((double) x1 > (double) x2)
      MathHelper.Swap(ref x1, ref x2);
    float y2 = MathF.Max(x1, y1);
    float y3 = MathF.Min(x2, maxValue);
    if ((double) y2 > (double) y3 || (double) MathF.Abs(this.Direction.Y) < 1.0000000116860974E-07 && ((double) this.Position.Y < (double) MathF.Min(box.Top, box.Bottom) || (double) this.Position.Y > (double) MathF.Max(box.Top, box.Bottom)))
      return false;
    float num2 = 1f / this.Direction.Y;
    float x3 = (MathF.Min(box.Top, box.Bottom) - this.Position.Y) * num2;
    float x4 = (MathF.Max(box.Top, box.Bottom) - this.Position.Y) * num2;
    if ((double) x3 > (double) x4)
      MathHelper.Swap(ref x3, ref x4);
    float num3 = MathF.Max(x3, y2);
    float num4 = MathF.Min(x4, y3);
    if ((double) num3 > (double) num4)
      return false;
    hitPos = this.Position + this.Direction * num3;
    distance = num3;
    return true;
  }

  public readonly bool Equals(Ray other)
  {
    return this.Position.Equals(other.Position) && this.Direction.Equals(other.Direction);
  }

  public override readonly bool Equals(object? obj)
  {
    return obj != null && obj is Ray other && this.Equals(other);
  }

  public override readonly int GetHashCode()
  {
    return this.Position.GetHashCode() * 397 ^ this.Direction.GetHashCode();
  }

  public static bool operator ==(Ray a, Ray b) => a.Equals(b);

  public static bool operator !=(Ray a, Ray b) => !(a == b);
}
