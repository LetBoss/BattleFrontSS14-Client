using System;
using YamlDotNet.Core;

namespace Robust.Shared.Serialization.Markdown;

public readonly struct NodeMark : IEquatable<NodeMark>, IComparable<NodeMark>
{
	public static NodeMark Invalid => new NodeMark(-1, -1);

	public int Line { get; init; }

	public int Column { get; init; }

	public NodeMark(int line, int column)
	{
		Line = line;
		Column = column;
	}

	public NodeMark(Mark mark)
		: this((int)((Mark)(ref mark)).Line, (int)((Mark)(ref mark)).Column)
	{
	}

	public override string ToString()
	{
		return $"Line: {Line}, Col: {Column}";
	}

	public bool Equals(NodeMark other)
	{
		if (Line == other.Line)
		{
			return Column == other.Column;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is NodeMark other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Line, Column);
	}

	public int CompareTo(NodeMark other)
	{
		int num = Line.CompareTo(other.Line);
		if (num != 0)
		{
			return num;
		}
		return Column.CompareTo(other.Column);
	}

	public static implicit operator NodeMark(Mark mark)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return new NodeMark(mark);
	}

	public static bool operator ==(NodeMark left, NodeMark right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(NodeMark left, NodeMark right)
	{
		return !left.Equals(right);
	}

	public static bool operator <(NodeMark? left, NodeMark? right)
	{
		if (!left.HasValue || !right.HasValue)
		{
			return false;
		}
		return left.Value.CompareTo(right.Value) < 0;
	}

	public static bool operator >(NodeMark? left, NodeMark? right)
	{
		if (!left.HasValue || !right.HasValue)
		{
			return false;
		}
		return left.Value.CompareTo(right.Value) > 0;
	}
}
