using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.Map;

[Serializable]
[NetSerializable]
public readonly struct WindowId(int value) : IEquatable<WindowId>
{
	public static readonly WindowId Invalid = default(WindowId);

	public static readonly WindowId Main = new WindowId(1);

	internal readonly int Value = value;

	public bool Equals(WindowId other)
	{
		return Value == other.Value;
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj is WindowId other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Value;
	}

	public static bool operator ==(WindowId a, WindowId b)
	{
		return a.Value == b.Value;
	}

	public static bool operator !=(WindowId a, WindowId b)
	{
		return !(a == b);
	}

	public static explicit operator int(WindowId self)
	{
		return self.Value;
	}

	public override string ToString()
	{
		return $"Window {Value}";
	}
}
