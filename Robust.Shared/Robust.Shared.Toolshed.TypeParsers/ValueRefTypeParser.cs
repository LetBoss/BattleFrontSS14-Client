using System.Diagnostics.CodeAnalysis;
using System.Text;
using Robust.Shared.Console;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class ValueRefTypeParser<T, TAuto> : TypeParser<ValueRef<T, TAuto>>
{
	public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out ValueRef<T, TAuto>? result)
	{
		ValueRef<T> parsed;
		bool num = Toolshed.TryParse<ValueRef<T>>(ctx, out parsed);
		result = null;
		if (num)
		{
			result = new ValueRef<T, TAuto>(parsed);
		}
		return num;
	}

	public override CompletionResult? TryAutocomplete(ParserContext parserContext, CommandArgument? arg)
	{
		return Toolshed.TryAutocomplete(parserContext, typeof(ValueRef<T, T>), arg);
	}
}
internal sealed class ValueRefTypeParser<T> : TypeParser<ValueRef<T>>
{
	internal static bool TryParse(ToolshedManager shed, ParserContext ctx, ITypeParser? parser, [NotNullWhen(true)] out ValueRef<T>? result)
	{
		result = null;
		ctx.ConsumeWhitespace();
		Rune? rune = ctx.PeekRune();
		if (rune == new Rune('$'))
		{
			if (!shed.TryParse<VarRef<T>>(ctx, out VarRef<T> parsed))
			{
				return false;
			}
			result = parsed;
			return true;
		}
		if (rune == new Rune('{'))
		{
			if (!shed.TryParse<Block<T>>(ctx, out Block<T> parsed2))
			{
				return false;
			}
			result = new BlockRef<T>(parsed2);
			return true;
		}
		if (parser == null)
		{
			parser = shed.GetParserForType(typeof(T));
		}
		if (parser == null)
		{
			if (!ctx.GenerateCompletions)
			{
				ctx.Error = new MustBeVarOrBlock(typeof(T));
			}
			return false;
		}
		if (!parser.TryParse(ctx, out object result2))
		{
			return false;
		}
		if (!(result2 is T value))
		{
			return false;
		}
		result = new ParsedValueRef<T>(value);
		return true;
	}

	public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out ValueRef<T>? result)
	{
		return TryParse(Toolshed, ctx, null, out result);
	}

	public static CompletionResult? TryAutocomplete(ToolshedManager shed, ParserContext ctx, CommandArgument? arg, ITypeParser? parser)
	{
		ctx.ConsumeWhitespace();
		Rune? rune = ctx.PeekRune();
		if (rune == new Rune('$'))
		{
			return shed.TryAutocomplete(ctx, typeof(VarRef<T>), arg);
		}
		if (rune == new Rune('{'))
		{
			Block<T>.TryParse(ctx, out Block<T> _);
			return ctx.Completions;
		}
		if (parser == null)
		{
			parser = shed.GetParserForType(typeof(T));
		}
		if (parser == null)
		{
			return CompletionResult.FromHint("<variable or block of type " + typeof(T).PrettyName() + ">");
		}
		return parser.TryAutocomplete(ctx, arg) ?? CompletionResult.FromHint("<variable, block, or value of type " + typeof(T).PrettyName() + ">");
	}

	public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
	{
		return TryAutocomplete(Toolshed, ctx, arg, null);
	}
}
