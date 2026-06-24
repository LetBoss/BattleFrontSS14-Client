// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.QuantityParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class QuantityParser : TypeParser<Quantity>
{
  public override bool TryParse(ParserContext ctx, out Quantity result)
  {
    result = new Quantity();
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    string word = ctx.GetWord(QuantityParser.\u003C\u003EO.\u003C0\u003E__IsNumeric ?? (QuantityParser.\u003C\u003EO.\u003C0\u003E__IsNumeric = new Func<Rune, bool>(ParserContext.IsNumeric)));
    string s = word?.TrimEnd('%');
    float result1;
    if (s == null || !float.TryParse(s, out result1))
    {
      ctx.Error = word != null ? (IConError) new InvalidQuantity(word) : (IConError) new OutOfInputError();
      return false;
    }
    if ((double) result1 < 0.0)
    {
      ctx.Error = (IConError) new InvalidQuantity(word);
      return false;
    }
    if (word.EndsWith('%'))
    {
      if ((double) result1 > 100.0)
      {
        ctx.Error = (IConError) new InvalidQuantity(word);
        return false;
      }
      result = new Quantity(new float?(), new float?(result1 / 100f));
      return true;
    }
    result = new Quantity(new float?(result1), new float?());
    return true;
  }

  public override CompletionResult? TryAutocomplete(
    ParserContext parserContext,
    CommandArgument? arg)
  {
    return CompletionResult.FromHint(this.GetArgHint(arg));
  }
}
