// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Syntax.ParsedCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.Syntax;

public sealed class ParsedCommand
{
  internal readonly ToolshedCommandImplementor Implementor;
  internal readonly ConcreteCommandMethod Method;
  private bool _passedInvokeTest;

  public ToolshedCommand Command => this.Implementor.Owner;

  public Type ReturnType => this.Method.Info.ReturnType;

  public Type? PipedType => this.Bundle.PipedType;

  public string? SubCommand => this.Bundle.SubCommand;

  internal Func<CommandInvocationArguments, object?> Invocable { get; }

  internal CommandArgumentBundle Bundle { get; }

  public static bool TryParse(ParserContext ctx, Type? piped, [NotNullWhen(true)] out ParsedCommand? result)
  {
    ParserRestorePoint point = ctx.Save();
    CommandArgumentBundle bundle = ctx.Bundle;
    ctx.Bundle = new CommandArgumentBundle()
    {
      Inverted = false,
      PipedType = piped
    };
    ctx.ConsumeWhitespace();
    if (!ParsedCommand.TryDigestModifiers(ctx))
    {
      result = (ParsedCommand) null;
      ctx.Restore(point);
      return false;
    }
    Func<CommandInvocationArguments, object> invocable;
    ConcreteCommandMethod? method;
    ToolshedCommandImplementor implementor;
    if (!ParsedCommand.TryParseCommand(ctx, out invocable, out method, out implementor))
    {
      result = (ParsedCommand) null;
      ctx.Restore(point);
      return false;
    }
    result = new ParsedCommand(ctx.Bundle, invocable, method.Value, implementor);
    ctx.Bundle = bundle;
    return true;
  }

  private ParsedCommand(
    CommandArgumentBundle bundle,
    Func<CommandInvocationArguments, object?> invocable,
    ConcreteCommandMethod method,
    ToolshedCommandImplementor implementor)
  {
    this.Invocable = invocable;
    this.Bundle = bundle;
    this.Implementor = implementor;
    this.Method = method;
  }

  private static bool TryDigestModifiers(ParserContext ctx)
  {
    if (ctx.EatMatch("not"))
    {
      ctx.ConsumeWhitespace();
      ctx.Bundle.Inverted = true;
    }
    return true;
  }

  private static bool TryParseCommand(
    ParserContext ctx,
    [NotNullWhen(true)] out Func<CommandInvocationArguments, object?>? invocable,
    [NotNullWhen(true)] out ConcreteCommandMethod? method,
    [NotNullWhen(true)] out ToolshedCommandImplementor? implementor)
  {
    invocable = (Func<CommandInvocationArguments, object>) null;
    implementor = (ToolshedCommandImplementor) null;
    method = new ConcreteCommandMethod?();
    int index = ctx.Index;
    string name;
    if (!ParsedCommand.TryParseCommandName(ctx, out name))
      return false;
    ToolshedCommand command;
    if (!ctx.Environment.TryGetCommand(name, out command))
    {
      if (ctx.GenerateCompletions)
      {
        if (ctx.OutOfInput)
          ctx.Completions = ctx.Environment.CommandCompletionsForType(ctx.Bundle.PipedType);
        return false;
      }
      ParserContext parserContext = ctx;
      if (parserContext.Error == null)
        parserContext.Error = (IConError) new UnknownCommandError(name);
      ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index)));
      return false;
    }
    if (!ParsedCommand.TryParseImplementor(ctx, command, out implementor))
      return false;
    if (!ctx.CheckInvokable(implementor.Spec))
    {
      if (ctx.GenerateCompletions)
        ctx.Completions = CompletionResult.FromHint("Insufficient permissions for command: " + implementor.FullName);
      return false;
    }
    if (!ctx.GenerateCompletions || !ctx.OutOfInput)
      return implementor.TryParse(ctx, out invocable, out method);
    ctx.Completions = ctx.Bundle.SubCommand == null ? ctx.Environment.CommandCompletionsForType(ctx.Bundle.PipedType) : ctx.Environment.SubCommandCompletionsForType(ctx.Bundle.PipedType, command);
    return false;
  }

  private static bool TryParseCommandName(ParserContext ctx, [NotNullWhen(true)] out string? name)
  {
    ctx.Bundle.NameStart = ctx.Index;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    name = ctx.GetWord(ParsedCommand.\u003C\u003EO.\u003C0\u003E__IsCommandToken ?? (ParsedCommand.\u003C\u003EO.\u003C0\u003E__IsCommandToken = new Func<Rune, bool>(ParserContext.IsCommandToken)));
    if (name != null)
    {
      ctx.Bundle.Command = name;
      ctx.Bundle.NameEnd = ctx.Index;
      return true;
    }
    if (ctx.OutOfInput)
    {
      if (ctx.GenerateCompletions)
      {
        ctx.Completions = ctx.Environment.CommandCompletionsForType(ctx.Bundle.PipedType);
      }
      else
      {
        ctx.Error = (IConError) new OutOfInputError();
        ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Index, ctx.Index)));
      }
      return false;
    }
    if (ctx.GenerateCompletions)
      return false;
    ctx.Error = (IConError) new NotValidCommandError();
    ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Bundle.NameStart, ctx.Index + 1)));
    return false;
  }

  private static bool TryParseImplementor(
    ParserContext ctx,
    ToolshedCommand cmd,
    [NotNullWhen(true)] out ToolshedCommandImplementor? impl)
  {
    if (!cmd.HasSubCommands)
    {
      impl = cmd.CommandImplementors[string.Empty];
      return true;
    }
    impl = (ToolshedCommandImplementor) null;
    if (!ctx.EatMatch(':'))
    {
      if (ctx.GenerateCompletions)
      {
        ctx.Completions = ctx.Environment.SubCommandCompletionsForType(ctx.Bundle.PipedType, cmd);
        return false;
      }
      ctx.Error = (IConError) new OutOfInputError();
      ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Index, ctx.Index)));
      return false;
    }
    int index = ctx.Index;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    string word = ctx.GetWord(ParsedCommand.\u003C\u003EO.\u003C1\u003E__IsToken ?? (ParsedCommand.\u003C\u003EO.\u003C1\u003E__IsToken = new Func<Rune, bool>(ParserContext.IsToken)));
    if (word == null)
    {
      if (ctx.GenerateCompletions)
      {
        ctx.Completions = ctx.Environment.SubCommandCompletionsForType(ctx.Bundle.PipedType, cmd);
        return false;
      }
      ctx.Error = (IConError) new OutOfInputError();
      ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Index, ctx.Index)));
      return false;
    }
    if (!cmd.CommandImplementors.TryGetValue(word, out impl))
    {
      if (ctx.GenerateCompletions)
      {
        ctx.Completions = ctx.Environment.SubCommandCompletionsForType(ctx.Bundle.PipedType, cmd);
        return false;
      }
      ctx.Error = (IConError) new UnknownSubcommandError(word, cmd);
      ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index)));
      return false;
    }
    ctx.Bundle.NameEnd = ctx.Index;
    ctx.Bundle.SubCommand = word;
    return true;
  }

  public object? Invoke(object? pipedIn, IInvocationContext ctx)
  {
    IConError error;
    if (!this._passedInvokeTest && !ctx.CheckInvokable(new CommandSpec(this.Command, this.SubCommand), out error))
    {
      if (error != null)
        ctx.ReportError(error);
      return (object) null;
    }
    this._passedInvokeTest = true;
    try
    {
      return this.Invocable(new CommandInvocationArguments()
      {
        Bundle = this.Bundle,
        PipedArgument = pipedIn,
        Context = ctx
      });
    }
    catch (Exception ex)
    {
      ctx.ReportError((IConError) new UnhandledExceptionError(ex));
      return (object) null;
    }
  }
}
