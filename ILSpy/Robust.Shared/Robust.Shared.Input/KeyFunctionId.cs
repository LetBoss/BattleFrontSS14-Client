using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.Input;

[Serializable]
[NetSerializable]
public readonly struct KeyFunctionId(int id) : IEquatable<KeyFunctionId>
{
	private readonly int _value = id;

	public static explicit operator int(KeyFunctionId funcId)
	{
		return funcId._value;
	}

	public override string ToString()
	{
		return _value.ToString();
	}

	public bool Equals(KeyFunctionId other)
	{
		return _value == other._value;
	}

	public override bool Equals(object? obj)
	{
		if (obj is KeyFunctionId other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _value;
	}

	public static bool operator ==(KeyFunctionId left, KeyFunctionId right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(KeyFunctionId left, KeyFunctionId right)
	{
		return !left.Equals(right);
	}
}
