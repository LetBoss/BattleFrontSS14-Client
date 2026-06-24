using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;

namespace Robust.Shared.Utility;

[Serializable]
[NetSerializable]
public sealed class MarkupNode : IComparable<MarkupNode>, IEquatable<MarkupNode>
{
	public readonly string? Name;

	public readonly MarkupParameter Value;

	public readonly Dictionary<string, MarkupParameter> Attributes;

	public readonly bool Closing;

	public MarkupNode(string text)
	{
		Attributes = new Dictionary<string, MarkupParameter>();
		Value = new MarkupParameter(text);
	}

	public MarkupNode(string? name, MarkupParameter? value, Dictionary<string, MarkupParameter>? attributes, bool closing = false)
	{
		Name = name;
		Value = value.GetValueOrDefault();
		Attributes = attributes ?? new Dictionary<string, MarkupParameter>();
		Closing = closing;
	}

	public override string ToString()
	{
		if (Name == null)
		{
			return FormattedMessage.EscapeText(Value.StringValue ?? "");
		}
		string text = "";
		foreach (KeyValuePair<string, MarkupParameter> attribute in Attributes)
		{
			attribute.Deconstruct(out var key, out var value);
			string value2 = key;
			MarkupParameter value3 = value;
			text += $" {value2}{value3}";
		}
		return $"[{(Closing ? "/" : "")}{Name}{Value.ToString().ReplaceLineEndings("\\n")}{text}]";
	}

	public override bool Equals(object? obj)
	{
		if (obj is MarkupNode node)
		{
			return Equals(node);
		}
		return false;
	}

	public bool Equals(MarkupNode? node)
	{
		if (node == null)
		{
			return false;
		}
		if (Name != node.Name)
		{
			return false;
		}
		if (!Value.Equals(node.Value))
		{
			return false;
		}
		if (Closing != node.Closing)
		{
			return false;
		}
		if (Attributes.Count != node.Attributes.Count)
		{
			return false;
		}
		foreach (var (key, markupParameter2) in Attributes)
		{
			if (!node.Attributes.TryGetValue(key, out var value))
			{
				return false;
			}
			if (!markupParameter2.Equals(value))
			{
				return false;
			}
		}
		return true;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Name, Value, Closing);
	}

	public int CompareTo(MarkupNode? other)
	{
		if (this == other)
		{
			return 0;
		}
		if (other == null)
		{
			return 1;
		}
		int num = string.Compare(Name, other.Name, StringComparison.Ordinal);
		if (num != 0)
		{
			return num;
		}
		return Closing.CompareTo(other.Closing);
	}
}
