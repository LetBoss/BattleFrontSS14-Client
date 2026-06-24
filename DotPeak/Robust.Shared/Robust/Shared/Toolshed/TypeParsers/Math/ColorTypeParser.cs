// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.Math.ColorTypeParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers.Math;

public sealed class ColorTypeParser : TypeParser<Color>
{
  public override bool TryParse(ParserContext ctx, out Color result)
  {
    result = new Color();
    string lowerInvariant = ctx.GetWord((Func<Rune, bool>) (x => ParserContext.IsToken(x) || x == new Rune('#')))?.ToLowerInvariant();
    if (lowerInvariant == null)
    {
      if (!ctx.PeekRune().HasValue)
      {
        ctx.Error = (IConError) new OutOfInputError();
        return false;
      }
      ctx.Error = (IConError) new InvalidColor(ctx.GetWord());
      result = new Color();
      return false;
    }
    if (Color.TryParse(lowerInvariant, ref result))
      return true;
    ctx.Error = (IConError) new InvalidColor(lowerInvariant);
    return false;
  }

  public override CompletionResult TryAutocomplete(
    ParserContext parserContext,
    CommandArgument? arg)
  {
    return CompletionResult.FromHintOptions(Color.GetAllDefaultColors().Select<KeyValuePair<string, Color>, string>((Func<KeyValuePair<string, Color>, string>) (x => x.Key)), this.GetArgHint(arg) + "\nHex code or color name.");
  }
}
