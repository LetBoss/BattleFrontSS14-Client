using System;
using System.Globalization;
using System.Linq;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.FixedPoint;

[Serializable]
[CopyByRef]
public struct FixedPoint2 : ISelfSerialize, IComparable<FixedPoint2>, IEquatable<FixedPoint2>, IFormattable
{
	private const int Shift = 2;

	private const int ShiftConstant = 100;

	private const float FloatEpsilon = 1E-05f;

	public int Value { get; private set; }

	public static FixedPoint2 MaxValue { get; } = new FixedPoint2(int.MaxValue);

	public static FixedPoint2 Epsilon { get; } = new FixedPoint2(1);

	public static FixedPoint2 Zero { get; } = new FixedPoint2(0);

	private readonly double ShiftDown()
	{
		return (double)Value / 100.0;
	}

	private FixedPoint2(int value)
	{
		Value = value;
	}

	public static FixedPoint2 New(int value)
	{
		return new FixedPoint2(value * 100);
	}

	public static FixedPoint2 FromCents(int value)
	{
		return new FixedPoint2(value);
	}

	public static FixedPoint2 FromHundredths(int value)
	{
		return new FixedPoint2(value);
	}

	public static FixedPoint2 New(float value)
	{
		return new FixedPoint2((int)ApplyFloatEpsilon(value * 100f));
	}

	private static float ApplyFloatEpsilon(float value)
	{
		return value + 1E-05f * (float)Math.Sign(value);
	}

	private static double ApplyFloatEpsilon(double value)
	{
		return value + (double)(1E-05f * (float)Math.Sign(value));
	}

	public static FixedPoint2 NewCeiling(float value)
	{
		return new FixedPoint2((int)MathF.Ceiling(value * 100f));
	}

	public static FixedPoint2 New(double value)
	{
		return new FixedPoint2((int)ApplyFloatEpsilon(value * 100.0));
	}

	public static FixedPoint2 New(string value)
	{
		return New(Parse.Float((ReadOnlySpan<char>)value, NumberStyles.Float));
	}

	public static FixedPoint2 operator +(FixedPoint2 a)
	{
		return a;
	}

	public static FixedPoint2 operator -(FixedPoint2 a)
	{
		return new FixedPoint2(-a.Value);
	}

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
		return new FixedPoint2((int)ApplyFloatEpsilon((float)a.Value * b));
	}

	public static FixedPoint2 operator *(FixedPoint2 a, double b)
	{
		return new FixedPoint2((int)ApplyFloatEpsilon((double)a.Value * b));
	}

	public static FixedPoint2 operator *(FixedPoint2 a, int b)
	{
		return new FixedPoint2(a.Value * b);
	}

	public static FixedPoint2 operator /(FixedPoint2 a, FixedPoint2 b)
	{
		return new FixedPoint2((int)(100L * (long)a.Value / b.Value));
	}

	public static FixedPoint2 operator /(FixedPoint2 a, float b)
	{
		return new FixedPoint2((int)ApplyFloatEpsilon((float)a.Value / b));
	}

	public static bool operator <=(FixedPoint2 a, int b)
	{
		return a <= New(b);
	}

	public static bool operator >=(FixedPoint2 a, int b)
	{
		return a >= New(b);
	}

	public static bool operator <(FixedPoint2 a, int b)
	{
		return a < New(b);
	}

	public static bool operator >(FixedPoint2 a, int b)
	{
		return a > New(b);
	}

	public static bool operator ==(FixedPoint2 a, int b)
	{
		return a == New(b);
	}

	public static bool operator !=(FixedPoint2 a, int b)
	{
		return a != New(b);
	}

	public static bool operator ==(FixedPoint2 a, FixedPoint2 b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(FixedPoint2 a, FixedPoint2 b)
	{
		return !a.Equals(b);
	}

	public static bool operator <=(FixedPoint2 a, FixedPoint2 b)
	{
		return a.Value <= b.Value;
	}

	public static bool operator >=(FixedPoint2 a, FixedPoint2 b)
	{
		return a.Value >= b.Value;
	}

	public static bool operator <(FixedPoint2 a, FixedPoint2 b)
	{
		return a.Value < b.Value;
	}

	public static bool operator >(FixedPoint2 a, FixedPoint2 b)
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

	public readonly int Int()
	{
		return Value / 100;
	}

	public static implicit operator FixedPoint2(float n)
	{
		return New(n);
	}

	public static implicit operator FixedPoint2(double n)
	{
		return New(n);
	}

	public static implicit operator FixedPoint2(int n)
	{
		return New(n);
	}

	public static explicit operator float(FixedPoint2 n)
	{
		return n.Float();
	}

	public static explicit operator double(FixedPoint2 n)
	{
		return n.Double();
	}

	public static explicit operator int(FixedPoint2 n)
	{
		return n.Int();
	}

	public static FixedPoint2 Min(params FixedPoint2[] fixedPoints)
	{
		return fixedPoints.Min();
	}

	public static FixedPoint2 Min(FixedPoint2 a, FixedPoint2 b)
	{
		if (!(a < b))
		{
			return b;
		}
		return a;
	}

	public static FixedPoint2 Max(FixedPoint2 a, FixedPoint2 b)
	{
		if (!(a > b))
		{
			return b;
		}
		return a;
	}

	public static int Sign(FixedPoint2 value)
	{
		if (value < Zero)
		{
			return -1;
		}
		if (value > Zero)
		{
			return 1;
		}
		return 0;
	}

	public static FixedPoint2 Abs(FixedPoint2 a)
	{
		return FromCents(Math.Abs(a.Value));
	}

	public static FixedPoint2 Dist(FixedPoint2 a, FixedPoint2 b)
	{
		return Abs(a - b);
	}

	public static FixedPoint2 Clamp(FixedPoint2 number, FixedPoint2 min, FixedPoint2 max)
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
		if (obj is FixedPoint2 unit)
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
			Value = int.MaxValue;
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

	public readonly bool Equals(FixedPoint2 other)
	{
		return Value == other.Value;
	}

	public readonly int CompareTo(FixedPoint2 other)
	{
		return Value.CompareTo(other.Value);
	}
}
