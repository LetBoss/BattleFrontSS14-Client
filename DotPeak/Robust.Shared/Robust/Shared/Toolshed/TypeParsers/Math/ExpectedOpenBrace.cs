// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.Math.ExpectedOpenBrace
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers.Math;

public record ExpectedOpenBrace() : IConError
{
  public FormattedMessage DescribeInner()
  {
    return FormattedMessage.FromUnformatted("Expected an opening brace, [");
  }

  public string? Expression { get; set; }

  public Vector2i? IssueSpan { get; set; }

  public StackTrace? Trace { get; set; }

  [CompilerGenerated]
  protected virtual bool PrintMembers(StringBuilder builder)
  {
    RuntimeHelpers.EnsureSufficientExecutionStack();
    builder.Append("Expression = ");
    builder.Append((object) this.Expression);
    builder.Append(", IssueSpan = ");
    builder.Append(this.IssueSpan.ToString());
    builder.Append(", Trace = ");
    builder.Append((object) this.Trace);
    return true;
  }

  [CompilerGenerated]
  public override int GetHashCode()
  {
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    return ((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.\u003CExpression\u003Ek__BackingField)) * -1521134295 + EqualityComparer<Vector2i?>.Default.GetHashCode(this.\u003CIssueSpan\u003Ek__BackingField)) * -1521134295 + EqualityComparer<StackTrace>.Default.GetHashCode(this.\u003CTrace\u003Ek__BackingField);
  }

  [CompilerGenerated]
  public virtual bool Equals(ExpectedOpenBrace? other)
  {
    if ((object) this == (object) other)
      return true;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(this.\u003CExpression\u003Ek__BackingField, other.\u003CExpression\u003Ek__BackingField) && EqualityComparer<Vector2i?>.Default.Equals(this.\u003CIssueSpan\u003Ek__BackingField, other.\u003CIssueSpan\u003Ek__BackingField) && EqualityComparer<StackTrace>.Default.Equals(this.\u003CTrace\u003Ek__BackingField, other.\u003CTrace\u003Ek__BackingField);
  }

  [CompilerGenerated]
  protected ExpectedOpenBrace(ExpectedOpenBrace original)
  {
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.\u003CExpression\u003Ek__BackingField = original.\u003CExpression\u003Ek__BackingField;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.\u003CIssueSpan\u003Ek__BackingField = original.\u003CIssueSpan\u003Ek__BackingField;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.\u003CTrace\u003Ek__BackingField = original.\u003CTrace\u003Ek__BackingField;
  }
}
