using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.Timing;

[Serializable]
[NetSerializable]
public readonly struct GameTick(uint value) : IEquatable<GameTick>, IComparable<GameTick>
{
	public static readonly GameTick Zero = new GameTick(0u);

	public static readonly GameTick First = new GameTick(1u);

	public static readonly GameTick MaxValue = new GameTick(uint.MaxValue);

	public readonly uint Value = value;

	public bool Equals(GameTick other)
	{
		return Value == other.Value;
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj is GameTick other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (int)Value;
	}

	public static bool operator ==(GameTick a, GameTick b)
	{
		return a.Value == b.Value;
	}

	public static bool operator !=(GameTick a, GameTick b)
	{
		return a.Value != b.Value;
	}

	public int CompareTo(GameTick other)
	{
		return Value.CompareTo(other.Value);
	}

	public static bool operator >(GameTick a, GameTick b)
	{
		return a.Value > b.Value;
	}

	public static bool operator >=(GameTick a, GameTick b)
	{
		return a.Value >= b.Value;
	}

	public static bool operator <(GameTick a, GameTick b)
	{
		return a.Value < b.Value;
	}

	public static bool operator <=(GameTick a, GameTick b)
	{
		return a.Value <= b.Value;
	}

	public static GameTick operator +(GameTick a, uint b)
	{
		return new GameTick(a.Value + b);
	}

	public static GameTick operator -(GameTick a, uint b)
	{
		return new GameTick(a.Value - b);
	}

	public override string ToString()
	{
		return Value.ToString();
	}
}
