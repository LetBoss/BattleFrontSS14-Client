// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.MarkupNode
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Utility;

[NetSerializable]
[Serializable]
public sealed class MarkupNode : IComparable<MarkupNode>, IEquatable<MarkupNode>
{
  public readonly string? Name;
  public readonly MarkupParameter Value;
  public readonly Dictionary<string, MarkupParameter> Attributes;
  public readonly bool Closing;

  public MarkupNode(string text)
  {
    this.Attributes = new Dictionary<string, MarkupParameter>();
    this.Value = new MarkupParameter(text);
  }

  public MarkupNode(
    string? name,
    MarkupParameter? value,
    Dictionary<string, MarkupParameter>? attributes,
    bool closing = false)
  {
    this.Name = name;
    this.Value = value.GetValueOrDefault();
    this.Attributes = attributes ?? new Dictionary<string, MarkupParameter>();
    this.Closing = closing;
  }

  public override string ToString()
  {
    if (this.Name == null)
      return FormattedMessage.EscapeText(this.Value.StringValue ?? "");
    string str = "";
    foreach ((string key, MarkupParameter markupParameter) in this.Attributes)
      str += $" {key}{markupParameter}";
    return $"[{(this.Closing ? "/" : "")}{this.Name}{this.Value.ToString().ReplaceLineEndings("\\n")}{str}]";
  }

  public override bool Equals(object? obj) => obj is MarkupNode node && this.Equals(node);

  public bool Equals(MarkupNode? node)
  {
    if (node == null || this.Name != node.Name || !this.Value.Equals(node.Value) || this.Closing != node.Closing || this.Attributes.Count != node.Attributes.Count)
      return false;
    foreach ((string key, MarkupParameter markupParameter) in this.Attributes)
    {
      MarkupParameter other;
      if (!node.Attributes.TryGetValue(key, out other) || !markupParameter.Equals(other))
        return false;
    }
    return true;
  }

  public override int GetHashCode()
  {
    return HashCode.Combine<string, MarkupParameter, bool>(this.Name, this.Value, this.Closing);
  }

  public int CompareTo(MarkupNode? other)
  {
    if (this == other)
      return 0;
    if (other == null)
      return 1;
    int num = string.Compare(this.Name, other.Name, StringComparison.Ordinal);
    return num != 0 ? num : this.Closing.CompareTo(other.Closing);
  }
}
