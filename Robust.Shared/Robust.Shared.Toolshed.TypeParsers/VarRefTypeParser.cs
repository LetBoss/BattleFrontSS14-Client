using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Console;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class VarRefTypeParser<T> : TypeParser<VarRef<T>>
{
	public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out VarRef<T>? result)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		result = null;
		ctx.ConsumeWhitespace();
		int index = ctx.Index;
		if (!ctx.EatMatch('$'))
		{
			if (ctx.GenerateCompletions)
			{
				return false;
			}
			ctx.Error = new ExpectedDollarydoo();
			ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index + 1)));
			return false;
		}
		index = ctx.Index;
		string word = ctx.GetWord(ParserContext.IsToken);
		if (string.IsNullOrEmpty(word))
		{
			if (ctx.GenerateCompletions)
			{
				return false;
			}
			ctx.Error = new ExpectedVariableName();
			ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index + 1)));
			return false;
		}
		result = new VarRef<T>(word);
		return true;
	}

	public override CompletionResult TryAutocomplete(ParserContext parserContext, CommandArgument? arg)
	{
		return parserContext.VariableParser.GenerateCompletions<T>();
	}
}
