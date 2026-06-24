using System;
using Robust.Shared.Toolshed.TypeParsers;

namespace Content.Client.UserInterface.Controls;

public record struct TabId(int Id) : IEquatable<int>, IComparable<TabId>, IAsType<int>
{
	public static implicit operator TabId(int id)
	{
		return new TabId(id);
	}

	public static implicit operator int(TabId id)
	{
		return id.Id;
	}

	public bool Equals(int other)
	{
		return Id == other;
	}

	public int CompareTo(TabId other)
	{
		return Id.CompareTo(other.Id);
	}

	public int AsType()
	{
		return Id;
	}
}
