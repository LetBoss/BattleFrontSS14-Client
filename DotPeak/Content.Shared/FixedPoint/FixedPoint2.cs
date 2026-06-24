// Decompiled with JetBrains decompiler
// Type: Content.Shared.FixedPoint.FixedPoint2
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
public struct FixedPoint2 : 
  ISelfSerialize,
  IComparable<FixedPoint2>,
  IEquatable<FixedPoint2>,
  IFormattable
{
  private const int Shift = 2;
  private const int ShiftConstant = 100;
  private const float FloatEpsilon = 1E-05f;

  public int Value { get; private set; }

  public static FixedPoint2 MaxValue { get; } = new FixedPoint2(int.MaxValue);

  public static FixedPoint2 Epsilon { get; } = new FixedPoint2(1);

  public static FixedPoint2 Zero { get; } = new FixedPoint2(0);

  private readonly double ShiftDown() => (double) this.Value / 100.0;

  private FixedPoint2(int value) => this.Value = value;

  public static FixedPoint2 New(int value) => new FixedPoint2(value * 100);

  public static FixedPoint2 FromCents(int value) => new FixedPoint2(value);

  public static FixedPoint2 FromHundredths(int value) => new FixedPoint2(value);

  public static FixedPoint2 New(float value)
  {
    return new FixedPoint2((int) FixedPoint2.ApplyFloatEpsilon(value * 100f));
  }

  private static float ApplyFloatEpsilon(float value) => value + 1E-05f * (float) Math.Sign(value);

  private static double ApplyFloatEpsilon(double value)
  {
    return value + 9.9999997473787516E-06 * (double) Math.Sign(value);
  }

  public static FixedPoint2 NewCeiling(float value)
  {
    return new FixedPoint2((int) MathF.Ceiling(value * 100f));
  }

  public static FixedPoint2 New(double value)
  {
    return new FixedPoint2((int) FixedPoint2.ApplyFloatEpsilon(value * 100.0));
  }

  public static FixedPoint2 New(string value)
  {
    return FixedPoint2.New(Parse.Float((ReadOnlySpan<char>) value));
  }

  public static FixedPoint2 operator +(FixedPoint2 a) => a;

  public static FixedPoint2 operator -(FixedPoint2 a) => new FixedPoint2(-a.Value);

  public static FixedPoint2 operator +(FixedPoint2 a, FixedPoint2 b)
  {
    return new FixedPoint2(a.Value + b.Value);
  }

  public static FixedPoint2 operator -(FixedPoint2 a, FixedPoint2 b)
  {
    return new FixedPoint2(a.Value - b.Value);
  }

  public static FixedPoint2 operator *(FixedPoint2 a, FixedPoint2 b)
  {
    return new FixedPoint2(b.Value * a.Value / 100);
  }

  public static FixedPoint2 operator *(FixedPoint2 a, float b)
  {
    return new FixedPoint2((int) FixedPoint2.ApplyFloatEpsilon((float) a.Value * b));
  }

  public static FixedPoint2 operator *(FixedPoint2 a, double b)
  {
    return new FixedPoint2((int) FixedPoint2.ApplyFloatEpsilon((double) a.Value * b));
  }

  public static FixedPoint2 operator *(FixedPoint2 a, int b) => new FixedPoint2(a.Value * b);

  public static FixedPoint2 operator /(FixedPoint2 a, FixedPoint2 b)
  {
    return new FixedPoint2((int) (100L * (long) a.Value / (long) b.Value));
  }

  public static FixedPoint2 operator /(FixedPoint2 a, float b)
  {
    return new FixedPoint2((int) FixedPoint2.ApplyFloatEpsilon((float) a.Value / b));
  }

  public static bool operator <=(FixedPoint2 a, int b) => a <= FixedPoint2.New(b);

  public static bool operator >=(FixedPoint2 a, int b) => a >= FixedPoint2.New(b);

  public static bool operator <(FixedPoint2 a, int b) => a < FixedPoint2.New(b);

  public static bool operator >(FixedPoint2 a, int b) => a > FixedPoint2.New(b);

  public static bool operator ==(FixedPoint2 a, int b) => a == FixedPoint2.New(b);

  public static bool operator !=(FixedPoint2 a, int b) => a != FixedPoint2.New(b);

  public static bool operator ==(FixedPoint2 a, FixedPoint2 b) => a.Equals(b);

  public static bool operator !=(FixedPoint2 a, FixedPoint2 b) => !a.Equals(b);

  public static bool operator <=(FixedPoint2 a, FixedPoint2 b) => a.Value <= b.Value;

  public static bool operator >=(FixedPoint2 a, FixedPoint2 b) => a.Value >= b.Value;

  public static bool operator <(FixedPoint2 a, FixedPoint2 b) => a.Value < b.Value;

  public static bool operator >(FixedPoint2 a, FixedPoint2 b) => a.Value > b.Value;

  public readonly float Float() => (float) this.ShiftDown();

  public readonly double Double() => this.ShiftDown();

  public readonly int Int() => this.Value / 100;

  public static implicit operator FixedPoint2(float n) => FixedPoint2.New(n);

  public static implicit operator FixedPoint2(double n) => FixedPoint2.New(n);

  public static implicit operator FixedPoint2(int n) => FixedPoint2.New(n);

  public static explicit operator float(FixedPoint2 n) => n.Float();

  public static explicit operator double(FixedPoint2 n) => n.Double();

  public static explicit operator int(FixedPoint2 n) => n.Int();

  public static FixedPoint2 Min(params FixedPoint2[] fixedPoints)
  {
    return ((IEnumerable<FixedPoint2>) fixedPoints).Min<FixedPoint2>();
  }

  public static FixedPoint2 Min(FixedPoint2 a, FixedPoint2 b) => !(a < b) ? b : a;

  public static FixedPoint2 Max(FixedPoint2 a, FixedPoint2 b) => !(a > b) ? b : a;

  public static int Sign(FixedPoint2 value)
  {
    if (value < FixedPoint2.Zero)
      return -1;
    return value > FixedPoint2.Zero ? 1 : 0;
  }

  public static FixedPoint2 Abs(FixedPoint2 a) => FixedPoint2.FromCents(Math.Abs(a.Value));

  public static FixedPoint2 Dist(FixedPoint2 a, FixedPoint2 b) => FixedPoint2.Abs(a - b);

  public static FixedPoint2 Clamp(FixedPoint2 number, FixedPoint2 min, FixedPoint2 max)
  {
    if (min > max)
      throw new ArgumentException($"{nameof (min)} {min} cannot be larger than {nameof (max)} {max}");
    if (number < min)
      return min;
    return !(number > max) ? number : max;
  }

  public override readonly bool Equals(object? obj)
  {
    return obj is FixedPoint2 fixedPoint2 && this.Value == fixedPoint2.Value;
  }

  public override readonly int GetHashCode() => HashCode.Combine<int>(this.Value);

  public void Deserialize(string value)
  {
    if (value == "MaxValue")
      this.Value = int.MaxValue;
    else
      this = FixedPoint2.New(Parse.Double((ReadOnlySpan<char>) value));
  }

  public override readonly string ToString()
  {
    return this.ShiftDown().ToString((IFormatProvider) CultureInfo.InvariantCulture) ?? "";
  }

  public string ToString(string? format, IFormatProvider? formatProvider) => this.ToString();

  public readonly string Serialize() => this.Value == int.MaxValue ? "MaxValue" : this.ToString();

  public readonly bool Equals(FixedPoint2 other) => this.Value == other.Value;

  public readonly int CompareTo(FixedPoint2 other) => this.Value.CompareTo(other.Value);
}
