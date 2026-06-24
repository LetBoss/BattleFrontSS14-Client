using System;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Console;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class VarTypeParser : CustomTypeParser<Type>
{
	public override bool ShowTypeArgSignature => false;

	public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Type? result)
	{
		result = null;
		ParserRestorePoint point = ctx.Save();
		ctx.ConsumeWhitespace();
		if (!ctx.EatMatch('$'))
		{
			return false;
		}
		string word = ctx.GetWord(ParserContext.IsToken);
		if (string.IsNullOrEmpty(word))
		{
			if (!ctx.GenerateCompletions)
			{
				ctx.Error = new OutOfInputError();
			}
			return false;
		}
		if (ctx.VariableParser.TryParseVar(word, out result))
		{
			ctx.Restore(point);
			return true;
		}
		if (!ctx.GenerateCompletions)
		{
			ctx.Error = new UnknownVariableError(word);
		}
		return false;
	}

	public override CompletionResult TryAutocomplete(ParserContext ctx, CommandArgument? arg)
	{
		return ctx.VariableParser.GenerateCompletions();
	}
}
