// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.BoolTypeParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class BoolTypeParser : TypeParser<bool>
{
  public override bool TryParse(ParserContext ctx, out bool result)
  {
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    string lowerInvariant = ctx.GetWord(BoolTypeParser.\u003C\u003EO.\u003C0\u003E__IsToken ?? (BoolTypeParser.\u003C\u003EO.\u003C0\u003E__IsToken = new Func<Rune, bool>(ParserContext.IsToken)))?.ToLowerInvariant();
    switch (lowerInvariant)
    {
      case null:
        if (!ctx.PeekRune().HasValue)
        {
          ctx.Error = (IConError) new OutOfInputError();
          result = false;
          return false;
        }
        ctx.Error = (IConError) new InvalidBool(ctx.GetWord());
        result = false;
        return false;
      case "true":
      case "t":
      case "1":
        result = true;
        return true;
      case "false":
      case "f":
      case "0":
        result = false;
        return true;
      default:
        ctx.Error = (IConError) new InvalidBool(lowerInvariant);
        result = false;
        return false;
    }
  }

  public override CompletionResult TryAutocomplete(
    ParserContext parserContext,
    CommandArgument? arg)
  {
    return CompletionResult.FromHintOptions((IEnumerable<string>) new string[2]
    {
      "true",
      "false"
    }, this.GetArgHint(arg));
  }
}
