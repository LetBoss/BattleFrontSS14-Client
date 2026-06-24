// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.UnparseableValueError
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed;

public record UnparseableValueError(Type T) : IConError
{
  public FormattedMessage DescribeInner()
  {
    if (!this.T.Constructable())
      return FormattedMessage.FromUnformatted($"The type {this.T.PrettyName()} cannot be parsed, as it cannot be constructed.");
    FormattedMessage formattedMessage = FormattedMessage.FromUnformatted($"The type {this.T.PrettyName()} has no parser available and cannot be parsed.");
    formattedMessage.PushNewline();
    formattedMessage.AddText("Please contact a programmer with this error, they'd probably like to see it.");
    formattedMessage.PushNewline();
    formattedMessage.AddMarkupOrThrow("[bold][color=red]THIS IS A BUG.[/color][/bold]");
    return formattedMessage;
  }

  public string? Expression { get; set; }

  public Vector2i? IssueSpan { get; set; }

  public StackTrace? Trace { get; set; }

  [CompilerGenerated]
  protected virtual bool PrintMembers(StringBuilder builder)
  {
    RuntimeHelpers.EnsureSufficientExecutionStack();
    builder.Append("T = ");
    builder.Append((object) this.T);
    builder.Append(", Expression = ");
    builder.Append((object) this.Expression);
    builder.Append(", IssueSpan = ");
    builder.Append(this.IssueSpan.ToString());
    builder.Append(", Trace = ");
    builder.Append((object) this.Trace);
    return true;
  }
}
