using System;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Analyzers;
using Robust.Shared.Console;

namespace Robust.Shared.Toolshed.Syntax;

[Virtual]
public class Block(CommandRun expr)
{
	public readonly CommandRun Run = expr;

	public static bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Block? block)
	{
		block = null;
		if (!TryParseBlock(ctx, null, null, out CommandRun run))
		{
			return false;
		}
		block = new Block(run);
		return true;
	}

	public object? Invoke(object? input, IInvocationContext ctx)
	{
		return Run.Invoke(input, ctx);
	}

	public static bool TryParseBlock(ParserContext ctx, Type? pipedType, Type? targetOutput, [NotNullWhen(true)] out CommandRun? run)
	{
		run = null;
		ctx.ConsumeWhitespace();
		if (!ctx.EatMatch('{'))
		{
			if (ctx.GenerateCompletions)
			{
				ctx.Completions = CompletionResult.FromOptions(new _003C_003Ez__ReadOnlySingleElementList<CompletionOption>(new CompletionOption("{")));
			}
			else
			{
				ctx.Error = default(MissingOpeningBrace);
			}
			return false;
		}
		ctx.PushBlockTerminator('}');
		if (!CommandRun.TryParse(ctx, pipedType, targetOutput, out run))
		{
			return false;
		}
		if (ctx.EatBlockTerminator())
		{
			return true;
		}
		ctx.ConsumeWhitespace();
		if (!ctx.GenerateCompletions)
		{
			ctx.Error = default(MissingClosingBrace);
			return false;
		}
		if (ctx.OutOfInput)
		{
			ctx.Completions = CompletionResult.FromOptions(new _003C_003Ez__ReadOnlySingleElementList<CompletionOption>(new CompletionOption("}")));
		}
		return false;
	}

	public override string ToString()
	{
		return $"{{ {Run} }}";
	}
}
[Virtual]
public class Block<T> : Block
{
	public Block(CommandRun expr)
		: base(expr)
	{
	}

	public static bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Block<T>? block)
	{
		block = null;
		if (!Block.TryParseBlock(ctx, null, typeof(T), out CommandRun run))
		{
			return false;
		}
		block = new Block<T>(run);
		return true;
	}

	public T? Invoke(IInvocationContext ctx)
	{
		object obj = Run.Invoke(null, ctx);
		if (obj == null)
		{
			return default(T);
		}
		return (T)obj;
	}
}
[Virtual]
public class Block<TIn, TOut> : Block
{
	public Block(CommandRun expr)
		: base(expr)
	{
	}

	public static bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Block<TIn, TOut>? block)
	{
		block = null;
		if (!Block.TryParseBlock(ctx, typeof(TIn), typeof(TOut), out CommandRun run))
		{
			return false;
		}
		block = new Block<TIn, TOut>(run);
		return true;
	}

	public TOut? Invoke(TIn? input, IInvocationContext ctx)
	{
		object obj = Run.Invoke(input, ctx);
		if (obj == null)
		{
			return default(TOut);
		}
		return (TOut)obj;
	}
}
