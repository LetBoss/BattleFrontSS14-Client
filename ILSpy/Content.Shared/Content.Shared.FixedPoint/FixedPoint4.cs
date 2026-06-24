using System;
using System.Globalization;
using System.Linq;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.FixedPoint;

[Serializable]
[CopyByRef]
public struct FixedPoint4 : ISelfSerialize, IComparable<FixedPoint4>, IEquatable<FixedPoint4>, IFormattable
{
	private const long Shift = 4L;

	private const long ShiftConstant = 10000L;

	private const float FloatEpsilon = 1E-05f;

	public long Value { get; private set; }

	public static FixedPoint4 MaxValue { get; } = new FixedPoint4(long.MaxValue);

	public static FixedPoint4 Epsilon { get; } = new FixedPoint4(1L);

	public static FixedPoint4 Zero { get; } = new FixedPoint4(0L);

	private readonly double ShiftDown()
	{
		return (double)Value / 10000.0;
	}

	private FixedPoint4(long value)
	{
		Value = value;
	}

	public static FixedPoint4 New(long value)
	{
		return new FixedPoint4(value * 10000);
	}

	public static FixedPoint4 FromTenThousandths(long value)
	{
		return new FixedPoint4(value);
	}

	public static FixedPoint4 New(float value)
	{
		return new FixedPoint4((long)ApplyFloatEpsilon(value * 10000f));
	}

	private static float ApplyFloatEpsilon(float value)
	{
		return value + 1E-05f * (float)Math.Sign(value);
	}

	private static double ApplyFloatEpsilon(double value)
	{
		return value + (double)(1E-05f * (float)Math.Sign(value));
	}

	public static FixedPoint4 NewCeiling(float value)
	{
		return new FixedPoint4((long)MathF.Ceiling(value * 10000f));
	}

	public static FixedPoint4 New(double value)
	{
		return new FixedPoint4((long)ApplyFloatEpsilon(value * 10000.0));
	}

	public static FixedPoint4 New(string value)
	{
		return New(Parse.Float((ReadOnlySpan<char>)value, NumberStyles.Float));
	}

	public static FixedPoint4 operator +(FixedPoint4 a)
	{
		return a;
	}

	public static FixedPoint4 operator -(FixedPoint4 a)
	{
		return new FixedPoint4(-a.Value);
	}

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
		return new FixedPoint4(b.Value * a.Value / 10000);
	}

	public static FixedPoint4 operator *(FixedPoint4 a, float b)
	{
		return new FixedPoint4((long)ApplyFloatEpsilon((float)a.Value * b));
	}

	public static FixedPoint4 operator *(FixedPoint4 a, double b)
	{
		return new FixedPoint4((long)ApplyFloatEpsilon((double)a.Value * b));
	}

	public static FixedPoint4 operator *(FixedPoint4 a, long b)
	{
		return new FixedPoint4(a.Value * b);
	}

	public static FixedPoint4 operator /(FixedPoint4 a, FixedPoint4 b)
	{
		return new FixedPoint4(10000 * a.Value / b.Value);
	}

	public static FixedPoint4 operator /(FixedPoint4 a, float b)
	{
		return new FixedPoint4((long)ApplyFloatEpsilon((float)a.Value / b));
	}

	public static bool operator <=(FixedPoint4 a, long b)
	{
		return a <= New(b);
	}

	public static bool operator >=(FixedPoint4 a, long b)
	{
		return a >= New(b);
	}

	public static bool operator <(FixedPoint4 a, long b)
	{
		return a < New(b);
	}

	public static bool operator >(FixedPoint4 a, long b)
	{
		return a > New(b);
	}

	public static bool operator ==(FixedPoint4 a, long b)
	{
		return a == New(b);
	}

	public static bool operator !=(FixedPoint4 a, long b)
	{
		return a != New(b);
	}

	public static bool operator ==(FixedPoint4 a, FixedPoint4 b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(FixedPoint4 a, FixedPoint4 b)
	{
		return !a.Equals(b);
	}

	public static bool operator <=(FixedPoint4 a, FixedPoint4 b)
	{
		return a.Value <= b.Value;
	}

	public static bool operator >=(FixedPoint4 a, FixedPoint4 b)
	{
		return a.Value >= b.Value;
	}

	public static bool operator <(FixedPoint4 a, FixedPoint4 b)
	{
		return a.Value < b.Value;
	}

	public static bool operator >(FixedPoint4 a, FixedPoint4 b)
	{
		return a.Value > b.Value;
	}

	public readonly float Float()
	{
		return (float)ShiftDown();
	}

	public readonly double Double()
	{
		return ShiftDown();
	}

	public readonly long Long()
	{
		return Value / 10000;
	}

	public readonly int Int()
	{
		return (int)Long();
	}

	public static implicit operator FixedPoint4(FixedPoint2 n)
	{
		return New(n.Int());
	}

	public static implicit operator FixedPoint4(float n)
	{
		return New(n);
	}

	public static implicit operator FixedPoint4(double n)
	{
		return New(n);
	}

	public static implicit operator FixedPoint4(int n)
	{
		return New(n);
	}

	public static implicit operator FixedPoint4(long n)
	{
		return New(n);
	}

	public static explicit operator FixedPoint2(FixedPoint4 n)
	{
		return n.Int();
	}

	public static explicit operator float(FixedPoint4 n)
	{
		return n.Float();
	}

	public static explicit operator double(FixedPoint4 n)
	{
		return n.Double();
	}

	public static explicit operator int(FixedPoint4 n)
	{
		return n.Int();
	}

	public static explicit operator long(FixedPoint4 n)
	{
		return n.Long();
	}

	public static FixedPoint4 Min(params FixedPoint4[] fixedPoints)
	{
		return fixedPoints.Min();
	}

	public static FixedPoint4 Min(FixedPoint4 a, FixedPoint4 b)
	{
		if (!(a < b))
		{
			return b;
		}
		return a;
	}

	public static FixedPoint4 Max(FixedPoint4 a, FixedPoint4 b)
	{
		if (!(a > b))
		{
			return b;
		}
		return a;
	}

	public static long Sign(FixedPoint4 value)
	{
		if (value < Zero)
		{
			return -1L;
		}
		if (value > Zero)
		{
			return 1L;
		}
		return 0L;
	}

	public static FixedPoint4 Abs(FixedPoint4 a)
	{
		return FromTenThousandths(Math.Abs(a.Value));
	}

	public static FixedPoint4 Dist(FixedPoint4 a, FixedPoint4 b)
	{
		return Abs(a - b);
	}

	public static FixedPoint4 Clamp(FixedPoint4 number, FixedPoint4 min, FixedPoint4 max)
	{
		if (min > max)
		{
			throw new ArgumentException($"{"min"} {min} cannot be larger than {"max"} {max}");
		}
		if (!(number < min))
		{
			if (!(number > max))
			{
				return number;
			}
			return max;
		}
		return min;
	}

	public override readonly bool Equals(object? obj)
	{
		if (obj is FixedPoint4 unit)
		{
			return Value == unit.Value;
		}
		return false;
	}

	public override readonly int GetHashCode()
	{
		return HashCode.Combine(Value);
	}

	public void Deserialize(string value)
	{
		if (value == "MaxValue")
		{
			Value = 2147483647L;
		}
		else
		{
			this = New(Parse.Double((ReadOnlySpan<char>)value, NumberStyles.Float));
		}
	}

	public override readonly string ToString()
	{
		return ShiftDown().ToString(CultureInfo.InvariantCulture) ?? "";
	}

	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		return ToString();
	}

	public readonly string Serialize()
	{
		if (Value == int.MaxValue)
		{
			return "MaxValue";
		}
		return ToString();
	}

	public readonly bool Equals(FixedPoint4 other)
	{
		return Value == other.Value;
	}

	public readonly int CompareTo(FixedPoint4 other)
	{
		if (other.Value > Value)
		{
			return -1;
		}
		if (other.Value < Value)
		{
			return 1;
		}
		return 0;
	}
}
