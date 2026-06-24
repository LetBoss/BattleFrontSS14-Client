// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.Math.AngleTypeParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Globalization;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers.Math;

public sealed class AngleTypeParser : TypeParser<Angle>
{
  public override bool TryParse(ParserContext ctx, out Angle result)
  {
    result = new Angle();
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    string lowerInvariant = ctx.GetWord(AngleTypeParser.\u003C\u003EO.\u003C0\u003E__IsNumeric ?? (AngleTypeParser.\u003C\u003EO.\u003C0\u003E__IsNumeric = new Func<Rune, bool>(ParserContext.IsNumeric)))?.ToLowerInvariant();
    if (lowerInvariant == null)
    {
      if (!ctx.PeekRune().HasValue)
      {
        ctx.Error = (IConError) new OutOfInputError();
        return false;
      }
      ctx.Error = (IConError) new InvalidAngle(ctx.GetWord());
      return false;
    }
    if (lowerInvariant.EndsWith("deg"))
    {
      string str = lowerInvariant;
      float result1;
      if (!float.TryParse(str.Substring(0, str.Length - 3), (IFormatProvider) CultureInfo.InvariantCulture, out result1))
      {
        ctx.Error = (IConError) new InvalidAngle(lowerInvariant);
        return false;
      }
      result = Angle.FromDegrees((double) result1);
      return true;
    }
    float result2;
    if (!float.TryParse(lowerInvariant, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
    {
      ctx.Error = (IConError) new InvalidAngle(lowerInvariant);
      result = new Angle();
      return false;
    }
    result = new Angle((double) result2);
    return true;
  }

  public override CompletionResult? TryAutocomplete(
    ParserContext parserContext,
    CommandArgument? arg)
  {
    return CompletionResult.FromHint(this.GetArgHint(arg) + "\nAppend \"deg\" for degrees");
  }
}
