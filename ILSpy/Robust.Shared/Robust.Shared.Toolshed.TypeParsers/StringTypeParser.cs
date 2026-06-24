using System.Diagnostics.CodeAnalysis;
using System.Text;
using Robust.Shared.Console;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class StringTypeParser : TypeParser<string>
{
	private static readonly CompletionOption[] Option = new CompletionOption[1]
	{
		new CompletionOption("\"", null, CompletionOptionFlags.PartialCompletion | CompletionOptionFlags.NoEscape)
	};

	public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out string? result)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		ctx.ConsumeWhitespace();
		if (!ctx.EatMatch('"'))
		{
			if (!ctx.PeekRune().HasValue)
			{
				ctx.Error = new OutOfInputError();
				result = null;
				return false;
			}
			ctx.Error = default(StringMustStartWithQuote);
			ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Index, ctx.Index + 1)));
			result = null;
			return false;
		}
		StringBuilder stringBuilder = new StringBuilder();
		while (true)
		{
			Rune? rune = ctx.GetRune();
			if (!rune.HasValue)
			{
				break;
			}
			Rune valueOrDefault = rune.GetValueOrDefault();
			if (valueOrDefault == new Rune('"'))
			{
				result = stringBuilder.ToString();
				return true;
			}
			if (valueOrDefault != new Rune('\\'))
			{
				stringBuilder.Append(valueOrDefault);
				continue;
			}
			Rune? rune2 = ctx.GetRune();
			if (rune2.HasValue)
			{
				if (!(valueOrDefault == new Rune('"')) && !(valueOrDefault == new Rune('n')) && !(valueOrDefault == new Rune('\\')))
				{
					ctx.Error = new UnknownEscapeSequence(rune2.Value);
					result = null;
					return false;
				}
				stringBuilder.Append(rune2);
			}
		}
		if (!ctx.GenerateCompletions)
		{
			ctx.Error = new StringMustEndWithQuote();
		}
		result = null;
		return false;
	}

	public override CompletionResult? TryAutocomplete(ParserContext parserContext, CommandArgument? arg)
	{
		string argHint = GetArgHint(arg);
		parserContext.ConsumeWhitespace();
		if (!(parserContext.PeekRune() == new Rune('"')))
		{
			return CompletionResult.FromHintOptions(Option, argHint);
		}
		return CompletionResult.FromHint(argHint);
	}
}
