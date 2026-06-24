// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.InvalidBool
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

public record InvalidBool(string Value) : IConError
{
  public FormattedMessage DescribeInner()
  {
    return FormattedMessage.FromUnformatted($"The value {this.Value} is not a valid boolean.");
  }

  public string? Expression { get; set; }

  public Vector2i? IssueSpan { get; set; }

  public StackTrace? Trace { get; set; }

  [CompilerGenerated]
  protected virtual bool PrintMembers(StringBuilder builder)
  {
    RuntimeHelpers.EnsureSufficientExecutionStack();
    builder.Append("Value = ");
    builder.Append((object) this.Value);
    builder.Append(", Expression = ");
    builder.Append((object) this.Expression);
    builder.Append(", IssueSpan = ");
    builder.Append(this.IssueSpan.ToString());
    builder.Append(", Trace = ");
    builder.Append((object) this.Trace);
    return true;
  }
}
