using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Console;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class CustomValueRefTypeParser<T, TParser> : CustomTypeParser<ValueRef<T>> where T : notnull where TParser : CustomTypeParser<T>, new()
{
	public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
	{
		TParser customParser = Toolshed.GetCustomParser<TParser, T>();
		return ValueRefTypeParser<T>.TryAutocomplete(Toolshed, ctx, arg, customParser);
	}

	public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out ValueRef<T>? result)
	{
		TParser customParser = Toolshed.GetCustomParser<TParser, T>();
		return ValueRefTypeParser<T>.TryParse(Toolshed, ctx, customParser, out result);
	}
}
