using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Robust.Shared.Console;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class MapBlockOutputParser : CustomTypeParser<Type>
{
	public override bool ShowTypeArgSignature => false;

	public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Type? result)
	{
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		result = null;
		Type type = ctx.Bundle.PipedType;
		if (type != null)
		{
			if (type.IsGenericType(typeof(IEnumerable<>)))
			{
				type = type.GetGenericArguments()[0];
			}
			else if (type.IsGenericType(typeof(List<>)))
			{
				type = type.GetGenericArguments()[0];
			}
			else if (type.IsArray)
			{
				type = type.GetElementType();
			}
			else
			{
				Type? type2 = type.GetInterfaces().FirstOrDefault((Type x) => x.IsGenericType(typeof(IEnumerable<>)));
				type = (((object)type2 != null) ? type2.GetGenericArguments()[0] : null) ?? type;
			}
		}
		ParserRestorePoint point = ctx.Save();
		int index = ctx.Index;
		if (!Block.TryParseBlock(ctx, type, null, out CommandRun run))
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
		Type type = ctx.Bundle.PipedType;
		if (type != null && type.IsGenericType(typeof(IEnumerable<>)))
		{
			type = type.GetGenericArguments()[0];
		}
		Block.TryParseBlock(ctx, type, null, out CommandRun _);
		return ctx.Completions;
	}
}
