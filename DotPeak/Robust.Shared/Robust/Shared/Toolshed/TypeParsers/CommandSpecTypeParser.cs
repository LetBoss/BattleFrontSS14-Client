// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.CommandSpecTypeParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Linq;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class CommandSpecTypeParser : TypeParser<CommandSpec>
{
  public override bool TryParse(ParserContext ctx, out CommandSpec result)
  {
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    string word1 = ctx.GetWord(CommandSpecTypeParser.\u003C\u003EO.\u003C0\u003E__IsCommandToken ?? (CommandSpecTypeParser.\u003C\u003EO.\u003C0\u003E__IsCommandToken = new Func<Rune, bool>(ParserContext.IsCommandToken)));
    int index1 = ctx.Index;
    string SubCommand = (string) null;
    if (word1 == null)
    {
      if (!ctx.PeekRune().HasValue)
      {
        ctx.Error = (IConError) new OutOfInputError();
        ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Index, ctx.Index)));
        result = new CommandSpec();
        return false;
      }
      ctx.Error = (IConError) new NotValidCommandError();
      ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index1, ctx.Index + 1)));
      result = new CommandSpec();
      return false;
    }
    ToolshedCommand command;
    if (!ctx.Environment.TryGetCommand(word1, out command))
    {
      ctx.Error = (IConError) new UnknownCommandError(word1);
      ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index1, ctx.Index)));
      result = new CommandSpec();
      return false;
    }
    if (command.HasSubCommands)
    {
      if (!ctx.EatMatch(':'))
      {
        ctx.Error = ctx.OutOfInput ? (IConError) new OutOfInputError() : (IConError) new ExpectedSubCommand();
        ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Index, ctx.Index + 1)));
        result = new CommandSpec();
        return false;
      }
      int index2 = ctx.Index;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      string word2 = ctx.GetWord(CommandSpecTypeParser.\u003C\u003EO.\u003C1\u003E__IsToken ?? (CommandSpecTypeParser.\u003C\u003EO.\u003C1\u003E__IsToken = new Func<Rune, bool>(ParserContext.IsToken)));
      if (word2 == null)
      {
        ctx.Error = (IConError) new ExpectedSubCommand();
        ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Index, ctx.Index)));
        result = new CommandSpec();
        return false;
      }
      if (!command.Subcommands.Contains<string>(word2))
      {
        ctx.Error = (IConError) new UnknownSubcommandError(word2, command);
        ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index2, ctx.Index)));
        result = new CommandSpec();
        return false;
      }
      SubCommand = word2;
    }
    result = new CommandSpec(command, SubCommand);
    return true;
  }

  public override CompletionResult? TryAutocomplete(
    ParserContext parserContext,
    CommandArgument? arg)
  {
    return CompletionResult.FromHintOptions(parserContext.Environment.AllCommands().Select<CommandSpec, CompletionOption>((Func<CommandSpec, CompletionOption>) (x => x.AsCompletion())), "<command name>");
  }
}
