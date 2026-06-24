using Robust.Shared.Console;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class BoolTypeParser : TypeParser<bool>
{
	public override bool TryParse(ParserContext ctx, out bool result)
	{
		string text = ctx.GetWord(ParserContext.IsToken)?.ToLowerInvariant();
		if (text == null)
		{
			if (!ctx.PeekRune().HasValue)
			{
				ctx.Error = new OutOfInputError();
				result = false;
				return false;
			}
			ctx.Error = new InvalidBool(ctx.GetWord());
			result = false;
			return false;
		}
		switch (text)
		{
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
			ctx.Error = new InvalidBool(text);
			result = false;
			return false;
		}
	}

	public override CompletionResult TryAutocomplete(ParserContext parserContext, CommandArgument? arg)
	{
		return CompletionResult.FromHintOptions(new string[2] { "true", "false" }, GetArgHint(arg));
	}
}
