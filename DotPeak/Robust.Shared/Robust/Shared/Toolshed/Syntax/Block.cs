// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Syntax.Block
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Console;
using Robust.Shared.Toolshed.Errors;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Toolshed.Syntax;

[Virtual]
public class Block(CommandRun expr)
{
  public readonly CommandRun Run = expr;

  public static bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Block? block)
  {
    block = (Block) null;
    CommandRun run;
    if (!Block.TryParseBlock(ctx, (Type) null, (Type) null, out run))
      return false;
    block = new Block(run);
    return true;
  }

  public object? Invoke(object? input, IInvocationContext ctx) => this.Run.Invoke(input, ctx);

  public static bool TryParseBlock(
    ParserContext ctx,
    Type? pipedType,
    Type? targetOutput,
    [NotNullWhen(true)] out CommandRun? run)
  {
    run = (CommandRun) null;
    ctx.ConsumeWhitespace();
    if (!ctx.EatMatch('{'))
    {
      if (ctx.GenerateCompletions)
      {
        // ISSUE: object of a compiler-generated type is created
        ctx.Completions = CompletionResult.FromOptions((IEnumerable<CompletionOption>) new \u003C\u003Ez__ReadOnlySingleElementList<CompletionOption>(new CompletionOption("{")));
      }
      else
        ctx.Error = (IConError) new MissingOpeningBrace();
      return false;
    }
    ctx.PushBlockTerminator('}');
    if (!CommandRun.TryParse(ctx, pipedType, targetOutput, out run))
      return false;
    if (ctx.EatBlockTerminator())
      return true;
    ctx.ConsumeWhitespace();
    if (!ctx.GenerateCompletions)
    {
      ctx.Error = (IConError) new MissingClosingBrace();
      return false;
    }
    if (ctx.OutOfInput)
    {
      // ISSUE: object of a compiler-generated type is created
      ctx.Completions = CompletionResult.FromOptions((IEnumerable<CompletionOption>) new \u003C\u003Ez__ReadOnlySingleElementList<CompletionOption>(new CompletionOption("}")));
    }
    return false;
  }

  public override string ToString() => $"{{ {this.Run} }}";
}
