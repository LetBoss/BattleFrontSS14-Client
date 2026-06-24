using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.Localization;

[Serializable]
[NetSerializable]
public readonly record struct LocId(string Id) : IEquatable<string>, IComparable<LocId>
{
	public static implicit operator string(LocId locId)
	{
		return locId.Id;
	}

	public static implicit operator LocId(string id)
	{
		return new LocId(id);
	}

	public static implicit operator LocId?(string? id)
	{
		if (id != null)
		{
			return new LocId(id);
		}
		return null;
	}

	public bool Equals(string? other)
	{
		return Id == other;
	}

	public int CompareTo(LocId other)
	{
		return string.Compare(Id, other.Id, StringComparison.Ordinal);
	}

	public override string ToString()
	{
		return Id ?? string.Empty;
	}
}
