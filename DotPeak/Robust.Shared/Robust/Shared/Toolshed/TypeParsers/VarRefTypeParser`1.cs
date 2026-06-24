// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.VarRefTypeParser`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class VarRefTypeParser<T> : TypeParser<VarRef<T>>
{
  public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out VarRef<T>? result)
  {
    result = (VarRef<T>) null;
    ctx.ConsumeWhitespace();
    int index1 = ctx.Index;
    if (!ctx.EatMatch('$'))
    {
      if (ctx.GenerateCompletions)
        return false;
      ctx.Error = (IConError) new ExpectedDollarydoo();
      ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index1, ctx.Index + 1)));
      return false;
    }
    int index2 = ctx.Index;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    string word = ctx.GetWord(VarRefTypeParser<T>.\u003C\u003EO.\u003C0\u003E__IsToken ?? (VarRefTypeParser<T>.\u003C\u003EO.\u003C0\u003E__IsToken = new Func<Rune, bool>(ParserContext.IsToken)));
    if (string.IsNullOrEmpty(word))
    {
      if (ctx.GenerateCompletions)
        return false;
      ctx.Error = (IConError) new ExpectedVariableName();
      ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index2, ctx.Index + 1)));
      return false;
    }
    result = new VarRef<T>(word);
    return true;
  }

  public override CompletionResult TryAutocomplete(
    ParserContext parserContext,
    CommandArgument? arg)
  {
    return parserContext.VariableParser.GenerateCompletions<T>();
  }
}
