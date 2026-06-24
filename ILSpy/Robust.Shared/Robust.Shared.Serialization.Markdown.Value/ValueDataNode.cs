using System;
using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Robust.Shared.Serialization.Markdown.Value;

public sealed class ValueDataNode : DataNode<ValueDataNode>, IEquatable<ValueDataNode>
{
	public string Value { get; set; }

	public override bool IsNull { get; init; }

	public override bool IsEmpty => string.IsNullOrWhiteSpace(Value);

	public static ValueDataNode Null()
	{
		return new ValueDataNode((string?)null);
	}

	public ValueDataNode()
		: this(string.Empty)
	{
	}

	public ValueDataNode(string? value)
		: base(NodeMark.Invalid, NodeMark.Invalid)
	{
		Value = value ?? string.Empty;
		IsNull = value == null;
	}

	public ValueDataNode(YamlScalarNode node)
		: base((NodeMark)((YamlNode)node).Start, (NodeMark)((YamlNode)node).End)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		IsNull = CalculateIsNullValue(node.Value, ((YamlNode)node).Tag, node.Style);
		Value = node.Value ?? string.Empty;
		TagName tag = ((YamlNode)node).Tag;
		object tag2;
		if (!((TagName)(ref tag)).IsEmpty)
		{
			tag = ((YamlNode)node).Tag;
			tag2 = ((TagName)(ref tag)).Value;
		}
		else
		{
			tag2 = null;
		}
		Tag = (string?)tag2;
	}

	public ValueDataNode(Scalar scalar)
		: base((NodeMark)((ParsingEvent)scalar).Start, (NodeMark)((ParsingEvent)scalar).End)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		IsNull = CalculateIsNullValue(scalar.Value, ((NodeEvent)scalar).Tag, scalar.Style);
		Value = scalar.Value;
		TagName tag = ((NodeEvent)scalar).Tag;
		object tag2;
		if (!((TagName)(ref tag)).IsEmpty)
		{
			tag = ((NodeEvent)scalar).Tag;
			tag2 = ((TagName)(ref tag)).Value;
		}
		else
		{
			tag2 = null;
		}
		Tag = (string?)tag2;
	}

	private bool CalculateIsNullValue(string? content, TagName tag, ScalarStyle style)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		if ((int)style != 3 && (int)style != 2)
		{
			if (!IsNullLiteral(content))
			{
				if (string.IsNullOrWhiteSpace(content))
				{
					return ((TagName)(ref tag)).IsEmpty;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public static explicit operator YamlScalarNode(ValueDataNode node)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_0065: Expected O, but got Unknown
		if (!node.IsNull)
		{
			return new YamlScalarNode(node.Value)
			{
				Tag = TagName.op_Implicit(node.Tag),
				Style = (ScalarStyle)((IsNullLiteral(node.Value) || string.IsNullOrWhiteSpace(node.Value)) ? 3 : 0)
			};
		}
		return new YamlScalarNode("null")
		{
			Tag = TagName.op_Implicit(node.Tag)
		};
	}

	public static bool IsNullLiteral(string? value)
	{
		if (value != null)
		{
			return string.Equals(value.Trim(), "null", StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	public override ValueDataNode Copy()
	{
		return new ValueDataNode(Value)
		{
			Tag = Tag,
			Start = Start,
			End = End,
			IsNull = IsNull
		};
	}

	public override ValueDataNode? Except(ValueDataNode node)
	{
		if (!(node.Value == Value))
		{
			return Copy();
		}
		return null;
	}

	[Obsolete("Use SerializationManager.PushComposition()")]
	public override ValueDataNode PushInheritance(ValueDataNode node)
	{
		return Copy();
	}

	public override bool Equals(object? obj)
	{
		if (obj is ValueDataNode other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Value.GetHashCode();
	}

	public override string ToString()
	{
		return Value;
	}

	public int AsInt()
	{
		return int.Parse(Value, CultureInfo.InvariantCulture);
	}

	public uint AsUint()
	{
		return uint.Parse(Value, CultureInfo.InvariantCulture);
	}

	public float AsFloat()
	{
		return float.Parse(Value, CultureInfo.InvariantCulture);
	}

	public bool AsBool()
	{
		return bool.Parse(Value);
	}

	public bool Equals(ValueDataNode? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		return Value == other.Value;
	}
}
