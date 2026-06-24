using System;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Console;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers.Math;

public abstract class SpanLikeTypeParser<T, TElem> : TypeParser<T> where T : notnull where TElem : unmanaged
{
	public abstract int Elements { get; }

	public abstract T Create(Span<TElem> elements);

	public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out T? result)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		if (!ctx.EatMatch('['))
		{
			ctx.Error = new ExpectedOpenBrace();
			result = default(T);
			return false;
		}
		ctx.ConsumeWhitespace();
		ctx.PushBlockTerminator(']');
		Span<TElem> elements = stackalloc TElem[Elements];
		for (int i = 0; i < Elements; i++)
		{
			ParserRestorePoint point = ctx.Save();
			if (!Toolshed.TryParse<TElem>(ctx, out TElem parsed))
			{
				ctx.Restore(point);
				int index = ctx.Index;
				if (ctx.EatBlockTerminator())
				{
					ctx.Error = new UnexpectedCloseBrace();
					ctx.Error.Contextualize(ctx.Input, new Vector2i(index, ctx.Index));
				}
				result = default(T);
				return false;
			}
			ctx.ConsumeWhitespace();
			if (i + 1 < Elements && ctx.EatBlockTerminator())
			{
				ctx.Error = new UnexpectedCloseBrace();
				result = default(T);
				return false;
			}
			if (i + 1 < Elements && !ctx.EatMatch(','))
			{
				ctx.Error = new ExpectedComma();
				result = default(T);
				return false;
			}
			elements[i] = parsed;
			ctx.ConsumeWhitespace();
		}
		if (!ctx.EatBlockTerminator())
		{
			ctx.Error = new ExpectedCloseBrace();
			result = default(T);
			return false;
		}
		result = Create(elements);
		return true;
	}

	public override CompletionResult? TryAutocomplete(ParserContext parserContext, CommandArgument? arg)
	{
		return CompletionResult.FromHint(GetArgHint(arg));
	}
}
