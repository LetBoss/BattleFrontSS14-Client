using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using Robust.Shared.Console;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers.Math;

internal sealed class NumberBaseTypeParser<T> : TypeParser<T> where T : INumberBase<T>
{
	public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out T? result)
	{
		result = default(T);
		string word = ctx.GetWord(ParserContext.IsNumeric);
		if (string.IsNullOrEmpty(word))
		{
			ctx.Error = new ExpectedNumericError();
			return false;
		}
		if (T.TryParse(word, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
		{
			return true;
		}
		ctx.Error = new InvalidNumber<T>(word);
		return false;
	}

	public override CompletionResult? TryAutocomplete(ParserContext parserContext, CommandArgument? arg)
	{
		return CompletionResult.FromHint(GetArgHint(arg));
	}
}
