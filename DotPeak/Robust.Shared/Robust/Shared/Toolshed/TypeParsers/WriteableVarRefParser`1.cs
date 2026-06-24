// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.WriteableVarRefParser`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class WriteableVarRefParser<T> : TypeParser<WriteableVarRef<T>>
{
  public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out WriteableVarRef<T>? result)
  {
    int index = ctx.Index;
    result = (WriteableVarRef<T>) null;
    VarRef<T> parsed;
    if (!ctx.Toolshed.TryParse<VarRef<T>>(ctx, out parsed))
      return false;
    if (!ctx.VariableParser.IsReadonlyVar(parsed.VarName))
    {
      result = new WriteableVarRef<T>(parsed);
      return true;
    }
    if (ctx.GenerateCompletions)
      return false;
    ctx.Error = (IConError) new ReadonlyVariableError(parsed.VarName);
    ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index + 1)));
    return false;
  }

  public override CompletionResult? TryAutocomplete(
    ParserContext parserContext,
    CommandArgument? arg)
  {
    return parserContext.VariableParser.GenerateCompletions<T>(false);
  }
}
