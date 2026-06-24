using Robust.Shared.Console;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class QuantityParser : TypeParser<Quantity>
{
	public override bool TryParse(ParserContext ctx, out Quantity result)
	{
		result = default(Quantity);
		string word = ctx.GetWord(ParserContext.IsNumeric);
		string text = word?.TrimEnd('%');
		if (text == null || !float.TryParse(text, out var result2))
		{
			IConError error;
			if (word == null)
			{
				IConError conError = new OutOfInputError();
				error = conError;
			}
			else
			{
				IConError conError = new InvalidQuantity(word);
				error = conError;
			}
			ctx.Error = error;
			return false;
		}
		if ((double)result2 < 0.0)
		{
			ctx.Error = new InvalidQuantity(word);
			return false;
		}
		if (word.EndsWith('%'))
		{
			if ((double)result2 > 100.0)
			{
				ctx.Error = new InvalidQuantity(word);
				return false;
			}
			result = new Quantity(null, result2 / 100f);
			return true;
		}
		result = new Quantity(result2, null);
		return true;
	}

	public override CompletionResult? TryAutocomplete(ParserContext parserContext, CommandArgument? arg)
	{
		return CompletionResult.FromHint(GetArgHint(arg));
	}
}
