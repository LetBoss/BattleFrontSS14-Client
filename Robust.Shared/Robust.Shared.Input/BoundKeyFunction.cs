using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.Input;

[Serializable]
[NetSerializable]
public struct BoundKeyFunction(string name) : IComparable, IComparable<BoundKeyFunction>, IEquatable<BoundKeyFunction>, ISelfSerialize
{
	public readonly string FunctionName = name;

	public static implicit operator BoundKeyFunction(string name)
	{
		return new BoundKeyFunction(name);
	}

	public override readonly string ToString()
	{
		return "KeyFunction(" + FunctionName + ")";
	}

	public readonly int CompareTo(object? obj)
	{
		if (!(obj is BoundKeyFunction other))
		{
			return 1;
		}
		return CompareTo(other);
	}

	public readonly int CompareTo(BoundKeyFunction other)
	{
		return string.Compare(FunctionName, other.FunctionName, StringComparison.InvariantCultureIgnoreCase);
	}

	public override readonly bool Equals(object? obj)
	{
		if (obj is BoundKeyFunction other)
		{
			return Equals(other);
		}
		return false;
	}

	public readonly bool Equals(BoundKeyFunction other)
	{
		return other.FunctionName == FunctionName;
	}

	public override readonly int GetHashCode()
	{
		return FunctionName.GetHashCode();
	}

	public static bool operator ==(BoundKeyFunction a, BoundKeyFunction b)
	{
		return a.FunctionName == b.FunctionName;
	}

	public static bool operator !=(BoundKeyFunction a, BoundKeyFunction b)
	{
		return !(a == b);
	}

	public void Deserialize(string value)
	{
		this = new BoundKeyFunction(value);
	}

	public readonly string Serialize()
	{
		return FunctionName;
	}
}
