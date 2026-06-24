using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.Map;

[Serializable]
[NetSerializable]
public readonly struct MapId(int value) : IEquatable<MapId>
{
	public static readonly MapId Nullspace = new MapId(0);

	internal readonly int Value = value;

	public bool IsClientSide => Value < 0;

	public bool Equals(MapId other)
	{
		return Value == other.Value;
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj is MapId other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Value;
	}

	public static bool operator ==(MapId a, MapId b)
	{
		return a.Value == b.Value;
	}

	public static bool operator !=(MapId a, MapId b)
	{
		return !(a == b);
	}

	public static explicit operator int(MapId self)
	{
		return self.Value;
	}

	public override string ToString()
	{
		if (!IsClientSide)
		{
			return Value.ToString();
		}
		return $"c{-Value}";
	}
}
