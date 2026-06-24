// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.Math.NumberBaseTypeParser`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers.Math;

internal sealed class NumberBaseTypeParser<T> : TypeParser<T> where T : INumberBase<T>
{
  public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out T? result)
  {
    result = default (T);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    string word = ctx.GetWord(NumberBaseTypeParser<T>.\u003C\u003EO.\u003C0\u003E__IsNumeric ?? (NumberBaseTypeParser<T>.\u003C\u003EO.\u003C0\u003E__IsNumeric = new Func<Rune, bool>(ParserContext.IsNumeric)));
    if (string.IsNullOrEmpty(word))
    {
      ctx.Error = (IConError) new ExpectedNumericError();
      return false;
    }
    if (T.TryParse(word, NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture, out result))
      return true;
    ctx.Error = (IConError) new InvalidNumber<T>(word);
    return false;
  }

  public override CompletionResult? TryAutocomplete(
    ParserContext parserContext,
    CommandArgument? arg)
  {
    return CompletionResult.FromHint(this.GetArgHint(arg));
  }
}
