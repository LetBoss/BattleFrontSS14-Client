// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.StringTypeParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class StringTypeParser : TypeParser<string>
{
  private static readonly CompletionOption[] Option = new CompletionOption[1]
  {
    new CompletionOption("\"", Flags: CompletionOptionFlags.PartialCompletion | CompletionOptionFlags.NoEscape)
  };

  public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out string? result)
  {
    ctx.ConsumeWhitespace();
    if (!ctx.EatMatch('"'))
    {
      if (!ctx.PeekRune().HasValue)
      {
        ctx.Error = (IConError) new OutOfInputError();
        result = (string) null;
        return false;
      }
      ctx.Error = (IConError) new StringMustStartWithQuote();
      ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Index, ctx.Index + 1)));
      result = (string) null;
      return false;
    }
    StringBuilder stringBuilder = new StringBuilder();
    Rune? rune1;
    while (true)
    {
      Rune valueOrDefault;
      do
      {
        Rune? rune2 = ctx.GetRune();
        if (rune2.HasValue)
        {
          valueOrDefault = rune2.GetValueOrDefault();
          if (valueOrDefault == new Rune('"'))
          {
            result = stringBuilder.ToString();
            return true;
          }
          if (valueOrDefault != new Rune('\\'))
            stringBuilder.Append((object) valueOrDefault);
          else
            rune1 = ctx.GetRune();
        }
        else
          goto label_14;
      }
      while (!rune1.HasValue);
      if (valueOrDefault == new Rune('"') || valueOrDefault == new Rune('n') || valueOrDefault == new Rune('\\'))
        stringBuilder.Append((object) rune1);
      else
        break;
    }
    ctx.Error = (IConError) new UnknownEscapeSequence(rune1.Value);
    result = (string) null;
    return false;
label_14:
    if (!ctx.GenerateCompletions)
      ctx.Error = (IConError) new StringMustEndWithQuote();
    result = (string) null;
    return false;
  }

  public override CompletionResult? TryAutocomplete(
    ParserContext parserContext,
    CommandArgument? arg)
  {
    string argHint = this.GetArgHint(arg);
    parserContext.ConsumeWhitespace();
    Rune? nullable = parserContext.PeekRune();
    Rune rune = new Rune('"');
    return (nullable.HasValue ? (nullable.GetValueOrDefault() == rune ? 1 : 0) : 0) == 0 ? CompletionResult.FromHintOptions((IEnumerable<CompletionOption>) StringTypeParser.Option, argHint) : CompletionResult.FromHint(argHint);
  }
}
