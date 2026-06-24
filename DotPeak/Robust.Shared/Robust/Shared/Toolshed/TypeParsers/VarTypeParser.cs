// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.VarTypeParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class VarTypeParser : CustomTypeParser<Type>
{
  public override bool ShowTypeArgSignature => false;

  public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Type? result)
  {
    result = (Type) null;
    ParserRestorePoint point = ctx.Save();
    ctx.ConsumeWhitespace();
    if (!ctx.EatMatch('$'))
      return false;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    string word = ctx.GetWord(VarTypeParser.\u003C\u003EO.\u003C0\u003E__IsToken ?? (VarTypeParser.\u003C\u003EO.\u003C0\u003E__IsToken = new Func<Rune, bool>(ParserContext.IsToken)));
    if (string.IsNullOrEmpty(word))
    {
      if (!ctx.GenerateCompletions)
        ctx.Error = (IConError) new OutOfInputError();
      return false;
    }
    if (ctx.VariableParser.TryParseVar(word, out result))
    {
      ctx.Restore(point);
      return true;
    }
    if (!ctx.GenerateCompletions)
      ctx.Error = (IConError) new UnknownVariableError(word);
    return false;
  }

  public override CompletionResult TryAutocomplete(ParserContext ctx, CommandArgument? arg)
  {
    return ctx.VariableParser.GenerateCompletions();
  }
}
