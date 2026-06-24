using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Console;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class WriteableVarRefParser<T> : TypeParser<WriteableVarRef<T>>
{
	public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out WriteableVarRef<T>? result)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		int index = ctx.Index;
		result = null;
		if (!ctx.Toolshed.TryParse<VarRef<T>>(ctx, out VarRef<T> parsed))
		{
			return false;
		}
		if (!ctx.VariableParser.IsReadonlyVar(parsed.VarName))
		{
			result = new WriteableVarRef<T>(parsed);
			return true;
		}
		if (ctx.GenerateCompletions)
		{
			return false;
		}
		ctx.Error = new ReadonlyVariableError(parsed.VarName);
		ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index + 1)));
		return false;
	}

	public override CompletionResult? TryAutocomplete(ParserContext parserContext, CommandArgument? arg)
	{
		return parserContext.VariableParser.GenerateCompletions<T>(includeReadonly: false);
	}
}
