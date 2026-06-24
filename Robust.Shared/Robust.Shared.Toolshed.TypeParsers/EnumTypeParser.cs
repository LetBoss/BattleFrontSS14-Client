using System;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Console;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class EnumTypeParser<T> : TypeParser<T> where T : unmanaged, Enum
{
	public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out T result)
	{
		string word = ctx.GetWord(ParserContext.IsToken);
		if (word == null)
		{
			if (!ctx.PeekRune().HasValue)
			{
				ctx.Error = new OutOfInputError();
				result = default(T);
				return false;
			}
			ctx.Error = new InvalidEnum<T>(ctx.GetWord());
			result = default(T);
			return false;
		}
		if (!Enum.TryParse<T>(word, ignoreCase: true, out var result2))
		{
			result = default(T);
			ctx.Error = new InvalidEnum<T>(word);
			return false;
		}
		result = result2;
		return true;
	}

	public override CompletionResult? TryAutocomplete(ParserContext parserContext, CommandArgument? arg)
	{
		return CompletionResult.FromHintOptions(Enum.GetNames<T>(), GetArgHint(arg));
	}
}
