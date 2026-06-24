// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Generic.ReduceCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Generic;

[ToolshedCommand]
public sealed class ReduceCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public T Reduce<T>(IInvocationContext ctx, [PipedArgument] IEnumerable<T> input, [CommandArgument(typeof (ReduceCommand.ReduceBlockParser), false)] Block reducer)
  {
    LocalVarInvocationContext ctx1 = new LocalVarInvocationContext(ctx);
    ctx1.SetLocal("value", (object) default (T));
    using (IEnumerator<T> enumerator = input.GetEnumerator())
    {
      T input1 = enumerator.MoveNext() ? enumerator.Current : throw new InvalidOperationException("Input contains no elements");
      while (enumerator.MoveNext())
      {
        ctx1.SetLocal("value", (object) enumerator.Current);
        input1 = (T) reducer.Invoke((object) input1, (IInvocationContext) ctx1);
        if (ctx.HasErrors)
          break;
      }
      return input1;
    }
  }

  private sealed class ReduceBlockParser : CustomTypeParser<Block>
  {
    public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Block? result)
    {
      result = (Block) null;
      Type pipedType = ctx.Bundle.PipedType;
      if ((object) pipedType == null || !pipedType.IsGenericType)
        return false;
      LocalVarParser localVarParser = new LocalVarParser(ctx.VariableParser);
      Type genericArgument = ctx.Bundle.PipedType.GetGenericArguments()[0];
      localVarParser.SetLocalType("value", genericArgument, false);
      ctx.VariableParser = (IVariableParser) localVarParser;
      CommandRun run;
      if (!Block.TryParseBlock(ctx, genericArgument, genericArgument, out run))
      {
        result = (Block) null;
        ctx.VariableParser = localVarParser.Inner;
        return false;
      }
      ctx.VariableParser = localVarParser.Inner;
      result = new Block(run);
      return true;
    }

    public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
    {
      Type pipedType = ctx.Bundle.PipedType;
      if ((object) pipedType == null || !pipedType.IsGenericType)
        return (CompletionResult) null;
      LocalVarParser localVarParser = new LocalVarParser(ctx.VariableParser);
      Type genericArgument = ctx.Bundle.PipedType.GetGenericArguments()[0];
      localVarParser.SetLocalType("value", genericArgument, false);
      ctx.VariableParser = (IVariableParser) localVarParser;
      Block.TryParseBlock(ctx, genericArgument, genericArgument, out CommandRun _);
      ctx.VariableParser = localVarParser.Inner;
      return ctx.Completions;
    }
  }
}
