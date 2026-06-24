using System;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Console;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class BlockOutputParser : CustomTypeParser<Type>
{
	public override bool ShowTypeArgSignature => false;

	public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Type? result)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		result = null;
		ParserRestorePoint point = ctx.Save();
		int index = ctx.Index;
		if (!Block.TryParseBlock(ctx, null, null, out CommandRun run))
		{
			ctx.Error?.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index)));
			return false;
		}
		ctx.Restore(point);
		if (run.ReturnType == null)
		{
			return false;
		}
		result = run.ReturnType;
		return true;
	}

	public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
	{
		Block.TryParseBlock(ctx, null, null, out CommandRun _);
		return ctx.Completions;
	}
}
