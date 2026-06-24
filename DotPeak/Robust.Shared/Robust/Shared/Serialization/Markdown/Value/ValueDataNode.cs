// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Markdown.Value.ValueDataNode
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Robust.Shared.Serialization.Markdown.Value;

public sealed class ValueDataNode : DataNode<ValueDataNode>, IEquatable<ValueDataNode>
{
  public static ValueDataNode Null() => new ValueDataNode((string) null);

  public ValueDataNode()
    : this(string.Empty)
  {
  }

  public ValueDataNode(string? value)
    : base(NodeMark.Invalid, NodeMark.Invalid)
  {
    this.Value = value ?? string.Empty;
    this.IsNull = value == null;
  }

  public ValueDataNode(YamlScalarNode node)
    : base((NodeMark) ((YamlNode) node).Start, (NodeMark) ((YamlNode) node).End)
  {
    this.IsNull = this.CalculateIsNullValue(node.Value, ((YamlNode) node).Tag, node.Style);
    this.Value = node.Value ?? string.Empty;
    TagName tag1 = ((YamlNode) node).Tag;
    string str;
    if (!((TagName) ref tag1).IsEmpty)
    {
      TagName tag2 = ((YamlNode) node).Tag;
      str = ((TagName) ref tag2).Value;
    }
    else
      str = (string) null;
    this.Tag = str;
  }

  public ValueDataNode(Scalar scalar)
    : base((NodeMark) ((ParsingEvent) scalar).Start, (NodeMark) ((ParsingEvent) scalar).End)
  {
    this.IsNull = this.CalculateIsNullValue(scalar.Value, ((NodeEvent) scalar).Tag, scalar.Style);
    this.Value = scalar.Value;
    TagName tag1 = ((NodeEvent) scalar).Tag;
    string str;
    if (!((TagName) ref tag1).IsEmpty)
    {
      TagName tag2 = ((NodeEvent) scalar).Tag;
      str = ((TagName) ref tag2).Value;
    }
    else
      str = (string) null;
    this.Tag = str;
  }

  private bool CalculateIsNullValue(string? content, TagName tag, ScalarStyle style)
  {
    if (style == 3 || style == 2)
      return false;
    if (ValueDataNode.IsNullLiteral(content))
      return true;
    return string.IsNullOrWhiteSpace(content) && ((TagName) ref tag).IsEmpty;
  }

  public static explicit operator YamlScalarNode(ValueDataNode node)
  {
    if (node.IsNull)
    {
      YamlScalarNode yamlScalarNode = new YamlScalarNode("null");
      ((YamlNode) yamlScalarNode).Tag = TagName.op_Implicit(node.Tag);
      return yamlScalarNode;
    }
    YamlScalarNode yamlScalarNode1 = new YamlScalarNode(node.Value);
    ((YamlNode) yamlScalarNode1).Tag = TagName.op_Implicit(node.Tag);
    yamlScalarNode1.Style = ValueDataNode.IsNullLiteral(node.Value) || string.IsNullOrWhiteSpace(node.Value) ? (ScalarStyle) 3 : (ScalarStyle) 0;
    return yamlScalarNode1;
  }

  public string Value { get; set; }

  public override bool IsNull { get; init; }

  public override bool IsEmpty => string.IsNullOrWhiteSpace(this.Value);

  public static bool IsNullLiteral(string? value)
  {
    return value != null && string.Equals(value.Trim(), "null", StringComparison.OrdinalIgnoreCase);
  }

  public override ValueDataNode Copy()
  {
    ValueDataNode valueDataNode = new ValueDataNode(this.Value);
    valueDataNode.Tag = this.Tag;
    valueDataNode.Start = this.Start;
    valueDataNode.End = this.End;
    valueDataNode.IsNull = this.IsNull;
    return valueDataNode;
  }

  public override ValueDataNode? Except(ValueDataNode node)
  {
    return !(node.Value == this.Value) ? this.Copy() : (ValueDataNode) null;
  }

  [Obsolete("Use SerializationManager.PushComposition()")]
  public override ValueDataNode PushInheritance(ValueDataNode node) => this.Copy();

  public override bool Equals(object? obj) => obj is ValueDataNode other && this.Equals(other);

  public override int GetHashCode() => this.Value.GetHashCode();

  public override string ToString() => this.Value;

  public int AsInt() => int.Parse(this.Value, (IFormatProvider) CultureInfo.InvariantCulture);

  public uint AsUint() => uint.Parse(this.Value, (IFormatProvider) CultureInfo.InvariantCulture);

  public float AsFloat() => float.Parse(this.Value, (IFormatProvider) CultureInfo.InvariantCulture);

  public bool AsBool() => bool.Parse(this.Value);

  public bool Equals(ValueDataNode? other)
  {
    if (other == null)
      return false;
    return this == other || this.Value == other.Value;
  }
}
