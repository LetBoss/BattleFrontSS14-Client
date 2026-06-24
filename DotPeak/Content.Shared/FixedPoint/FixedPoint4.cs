// Decompiled with JetBrains decompiler
// Type: Content.Shared.FixedPoint.FixedPoint4
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#nullable enable
namespace Content.Shared.FixedPoint;

[CopyByRef]
[Serializable]
public struct FixedPoint4 : 
  ISelfSerialize,
  IComparable<FixedPoint4>,
  IEquatable<FixedPoint4>,
  IFormattable
{
  private const long Shift = 4;
  private const long ShiftConstant = 10000;
  private const float FloatEpsilon = 1E-05f;

  public long Value { get; private set; }

  public static FixedPoint4 MaxValue { get; } = new FixedPoint4(long.MaxValue);

  public static FixedPoint4 Epsilon { get; } = new FixedPoint4(1L);

  public static FixedPoint4 Zero { get; } = new FixedPoint4(0L);

  private readonly double ShiftDown() => (double) this.Value / 10000.0;

  private FixedPoint4(long value) => this.Value = value;

  public static FixedPoint4 New(long value) => new FixedPoint4(value * 10000L);

  public static FixedPoint4 FromTenThousandths(long value) => new FixedPoint4(value);

  public static FixedPoint4 New(float value)
  {
    return new FixedPoint4((long) FixedPoint4.ApplyFloatEpsilon(value * 10000f));
  }

  private static float ApplyFloatEpsilon(float value) => value + 1E-05f * (float) Math.Sign(value);

  private static double ApplyFloatEpsilon(double value)
  {
    return value + 9.9999997473787516E-06 * (double) Math.Sign(value);
  }

  public static FixedPoint4 NewCeiling(float value)
  {
    return new FixedPoint4((long) MathF.Ceiling(value * 10000f));
  }

  public static FixedPoint4 New(double value)
  {
    return new FixedPoint4((long) FixedPoint4.ApplyFloatEpsilon(value * 10000.0));
  }

  public static FixedPoint4 New(string value)
  {
    return FixedPoint4.New(Parse.Float((ReadOnlySpan<char>) value));
  }

  public static FixedPoint4 operator +(FixedPoint4 a) => a;

  public static FixedPoint4 operator -(FixedPoint4 a) => new FixedPoint4(-a.Value);

  public static FixedPoint4 operator +(FixedPoint4 a, FixedPoint4 b)
  {
    return new FixedPoint4(a.Value + b.Value);
  }

  public static FixedPoint4 operator -(FixedPoint4 a, FixedPoint4 b)
  {
    return new FixedPoint4(a.Value - b.Value);
  }

  public static FixedPoint4 operator *(FixedPoint4 a, FixedPoint4 b)
  {
    return new FixedPoint4(b.Value * a.Value / 10000L);
  }

  public static FixedPoint4 operator *(FixedPoint4 a, float b)
  {
    return new FixedPoint4((long) FixedPoint4.ApplyFloatEpsilon((float) a.Value * b));
  }

  public static FixedPoint4 operator *(FixedPoint4 a, double b)
  {
    return new FixedPoint4((long) FixedPoint4.ApplyFloatEpsilon((double) a.Value * b));
  }

  public static FixedPoint4 operator *(FixedPoint4 a, long b) => new FixedPoint4(a.Value * b);

  public static FixedPoint4 operator /(FixedPoint4 a, FixedPoint4 b)
  {
    return new FixedPoint4(10000L * a.Value / b.Value);
  }

  public static FixedPoint4 operator /(FixedPoint4 a, float b)
  {
    return new FixedPoint4((long) FixedPoint4.ApplyFloatEpsilon((float) a.Value / b));
  }

  public static bool operator <=(FixedPoint4 a, long b) => a <= FixedPoint4.New(b);

  public static bool operator >=(FixedPoint4 a, long b) => a >= FixedPoint4.New(b);

  public static bool operator <(FixedPoint4 a, long b) => a < FixedPoint4.New(b);

  public static bool operator >(FixedPoint4 a, long b) => a > FixedPoint4.New(b);

  public static bool operator ==(FixedPoint4 a, long b) => a == FixedPoint4.New(b);

  public static bool operator !=(FixedPoint4 a, long b) => a != FixedPoint4.New(b);

  public static bool operator ==(FixedPoint4 a, FixedPoint4 b) => a.Equals(b);

  public static bool operator !=(FixedPoint4 a, FixedPoint4 b) => !a.Equals(b);

  public static bool operator <=(FixedPoint4 a, FixedPoint4 b) => a.Value <= b.Value;

  public static bool operator >=(FixedPoint4 a, FixedPoint4 b) => a.Value >= b.Value;

  public static bool operator <(FixedPoint4 a, FixedPoint4 b) => a.Value < b.Value;

  public static bool operator >(FixedPoint4 a, FixedPoint4 b) => a.Value > b.Value;

  public readonly float Float() => (float) this.ShiftDown();

  public readonly double Double() => this.ShiftDown();

  public readonly long Long() => this.Value / 10000L;

  public readonly int Int() => (int) this.Long();

  public static implicit operator FixedPoint4(FixedPoint2 n) => FixedPoint4.New((long) n.Int());

  public static implicit operator FixedPoint4(float n) => FixedPoint4.New(n);

  public static implicit operator FixedPoint4(double n) => FixedPoint4.New(n);

  public static implicit operator FixedPoint4(int n) => FixedPoint4.New((long) n);

  public static implicit operator FixedPoint4(long n) => FixedPoint4.New(n);

  public static explicit operator FixedPoint2(FixedPoint4 n) => (FixedPoint2) n.Int();

  public static explicit operator float(FixedPoint4 n) => n.Float();

  public static explicit operator double(FixedPoint4 n) => n.Double();

  public static explicit operator int(FixedPoint4 n) => n.Int();

  public static explicit operator long(FixedPoint4 n) => n.Long();

  public static FixedPoint4 Min(params FixedPoint4[] fixedPoints)
  {
    return ((IEnumerable<FixedPoint4>) fixedPoints).Min<FixedPoint4>();
  }

  public static FixedPoint4 Min(FixedPoint4 a, FixedPoint4 b) => !(a < b) ? b : a;

  public static FixedPoint4 Max(FixedPoint4 a, FixedPoint4 b) => !(a > b) ? b : a;

  public static long Sign(FixedPoint4 value)
  {
    if (value < FixedPoint4.Zero)
      return -1;
    return value > FixedPoint4.Zero ? 1L : 0L;
  }

  public static FixedPoint4 Abs(FixedPoint4 a) => FixedPoint4.FromTenThousandths(Math.Abs(a.Value));

  public static FixedPoint4 Dist(FixedPoint4 a, FixedPoint4 b) => FixedPoint4.Abs(a - b);

  public static FixedPoint4 Clamp(FixedPoint4 number, FixedPoint4 min, FixedPoint4 max)
  {
    if (min > max)
      throw new ArgumentException($"{nameof (min)} {min} cannot be larger than {nameof (max)} {max}");
    if (number < min)
      return min;
    return !(number > max) ? number : max;
  }

  public override readonly bool Equals(object? obj)
  {
    return obj is FixedPoint4 fixedPoint4 && this.Value == fixedPoint4.Value;
  }

  public override readonly int GetHashCode() => HashCode.Combine<long>(this.Value);

  public void Deserialize(string value)
  {
    if (value == "MaxValue")
      this.Value = (long) int.MaxValue;
    else
      this = FixedPoint4.New(Parse.Double((ReadOnlySpan<char>) value));
  }

  public override readonly string ToString()
  {
    return this.ShiftDown().ToString((IFormatProvider) CultureInfo.InvariantCulture) ?? "";
  }

  public string ToString(string? format, IFormatProvider? formatProvider) => this.ToString();

  public readonly string Serialize()
  {
    return this.Value == (long) int.MaxValue ? "MaxValue" : this.ToString();
  }

  public readonly bool Equals(FixedPoint4 other) => this.Value == other.Value;

  public readonly int CompareTo(FixedPoint4 other)
  {
    if (other.Value > this.Value)
      return -1;
    return other.Value < this.Value ? 1 : 0;
  }
}
