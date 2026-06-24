using System.Collections.Generic;
using System.Linq;
using System.Text;
using Robust.Shared.Console;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers.Math;

public sealed class ColorTypeParser : TypeParser<Color>
{
	public override bool TryParse(ParserContext ctx, out Color result)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		result = default(Color);
		string text = ctx.GetWord((Rune x) => ParserContext.IsToken(x) || x == new Rune('#'))?.ToLowerInvariant();
		if (text == null)
		{
			if (!ctx.PeekRune().HasValue)
			{
				ctx.Error = new OutOfInputError();
				return false;
			}
			ctx.Error = new InvalidColor(ctx.GetWord());
			result = default(Color);
			return false;
		}
		if (Color.TryParse(text, ref result))
		{
			return true;
		}
		ctx.Error = new InvalidColor(text);
		return false;
	}

	public override CompletionResult TryAutocomplete(ParserContext parserContext, CommandArgument? arg)
	{
		return CompletionResult.FromHintOptions(from x in Color.GetAllDefaultColors()
			select x.Key, GetArgHint(arg) + "\nHex code or color name.");
	}
}
