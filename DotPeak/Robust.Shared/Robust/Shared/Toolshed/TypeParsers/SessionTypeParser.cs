// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.SessionTypeParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Utility;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class SessionTypeParser : TypeParser<ICommonSession>
{
  [Robust.Shared.IoC.Dependency]
  private ISharedPlayerManager _player;

  public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out ICommonSession? result)
  {
    int index = ctx.Index;
    string word = ctx.GetWord();
    result = (ICommonSession) null;
    if (word == null)
    {
      ctx.Error = (IConError) new OutOfInputError();
      return false;
    }
    ICommonSession session;
    if (this._player.TryGetSessionByUsername(word, out session))
    {
      result = session;
      return true;
    }
    ctx.Error = (IConError) new SessionTypeParser.InvalidUsername(this.Loc, word);
    ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index)));
    return false;
  }

  public override CompletionResult TryAutocomplete(
    ParserContext parserContext,
    CommandArgument? arg)
  {
    return CompletionResult.FromHintOptions(CompletionHelper.SessionNames(players: this._player), this.GetArgHint(arg));
  }

  public record InvalidUsername(ILocalizationManager Loc, string Username) : IConError
  {
    public FormattedMessage DescribeInner()
    {
      return FormattedMessage.FromUnformatted(this.Loc.GetString("cmd-parse-failure-session", ("username", (object) this.Username)));
    }

    public string? Expression { get; set; }

    public Vector2i? IssueSpan { get; set; }

    public StackTrace? Trace { get; set; }

    [CompilerGenerated]
    protected virtual bool PrintMembers(StringBuilder builder)
    {
      RuntimeHelpers.EnsureSufficientExecutionStack();
      builder.Append("Loc = ");
      builder.Append((object) this.Loc);
      builder.Append(", Username = ");
      builder.Append((object) this.Username);
      builder.Append(", Expression = ");
      builder.Append((object) this.Expression);
      builder.Append(", IssueSpan = ");
      builder.Append(this.IssueSpan.ToString());
      builder.Append(", Trace = ");
      builder.Append((object) this.Trace);
      return true;
    }
  }
}
