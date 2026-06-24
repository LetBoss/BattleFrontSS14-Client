// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Markdown.Validation.ErrorNode
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Serialization.Markdown.Validation;

[Virtual]
public class ErrorNode : ValidationNode, IEquatable<ErrorNode>
{
  public ErrorNode(DataNode node, string errorReason, bool alwaysRelevant = true)
  {
    this.Node = node;
    this.ErrorReason = errorReason;
    this.AlwaysRelevant = alwaysRelevant;
  }

  public DataNode Node { get; }

  public string ErrorReason { get; }

  public bool AlwaysRelevant { get; }

  public override bool Valid => false;

  public override IEnumerable<ErrorNode> GetErrors()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    ErrorNode errorNode = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = errorNode;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public override int GetHashCode()
  {
    return HashCode.Combine<DataNode, string, bool>(this.Node, this.ErrorReason, this.AlwaysRelevant);
  }

  public bool Equals(ErrorNode? other)
  {
    if ((object) other == null)
      return false;
    if ((object) this == (object) other)
      return true;
    return this.Node.Equals((object) other.Node) && this.ErrorReason == other.ErrorReason && this.AlwaysRelevant == other.AlwaysRelevant;
  }

  public override bool Equals(object? obj)
  {
    if (obj == null)
      return false;
    if ((object) this == obj)
      return true;
    return !(obj.GetType() != this.GetType()) && this.Equals((ErrorNode) obj);
  }

  public static bool operator ==(ErrorNode? left, ErrorNode? right)
  {
    return object.Equals((object) left, (object) right);
  }

  public static bool operator !=(ErrorNode? left, ErrorNode? right)
  {
    return !object.Equals((object) left, (object) right);
  }
}
