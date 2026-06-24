// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.EnumTypeParser`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class EnumTypeParser<T> : TypeParser<T> where T : unmanaged, Enum
{
  public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out T result)
  {
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    string word = ctx.GetWord(EnumTypeParser<T>.\u003C\u003EO.\u003C0\u003E__IsToken ?? (EnumTypeParser<T>.\u003C\u003EO.\u003C0\u003E__IsToken = new Func<Rune, bool>(ParserContext.IsToken)));
    if (word == null)
    {
      if (!ctx.PeekRune().HasValue)
      {
        ctx.Error = (IConError) new OutOfInputError();
        result = default (T);
        return false;
      }
      ctx.Error = (IConError) new InvalidEnum<T>(ctx.GetWord());
      result = default (T);
      return false;
    }
    T result1;
    if (!Enum.TryParse<T>(word, true, out result1))
    {
      result = default (T);
      ctx.Error = (IConError) new InvalidEnum<T>(word);
      return false;
    }
    result = result1;
    return true;
  }

  public override CompletionResult? TryAutocomplete(
    ParserContext parserContext,
    CommandArgument? arg)
  {
    return CompletionResult.FromHintOptions((IEnumerable<string>) Enum.GetNames<T>(), this.GetArgHint(arg));
  }
}
