// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Syntax.UnknownSubcommandError
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
namespace Robust.Shared.Toolshed.Syntax;

public record UnknownSubcommandError(string SubCmd, ToolshedCommand Command) : IConError
{
  public FormattedMessage DescribeInner()
  {
    FormattedMessage formattedMessage = new FormattedMessage();
    formattedMessage.AddText($"The command group {this.Command.Name} doesn't have command {this.SubCmd}.");
    formattedMessage.PushNewline();
    formattedMessage.AddText($"The valid commands are: {string.Join(", ", this.Command.Subcommands)}.");
    return formattedMessage;
  }

  public string? Expression { get; set; }

  public Vector2i? IssueSpan { get; set; }

  public StackTrace? Trace { get; set; }

  [CompilerGenerated]
  protected virtual bool PrintMembers(StringBuilder builder)
  {
    RuntimeHelpers.EnsureSufficientExecutionStack();
    builder.Append("SubCmd = ");
    builder.Append((object) this.SubCmd);
    builder.Append(", Command = ");
    builder.Append((object) this.Command);
    builder.Append(", Expression = ");
    builder.Append((object) this.Expression);
    builder.Append(", IssueSpan = ");
    builder.Append(this.IssueSpan.ToString());
    builder.Append(", Trace = ");
    builder.Append((object) this.Trace);
    return true;
  }
}
