using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Console;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;

namespace Robust.Shared.Toolshed.Commands.Generic;

[ToolshedCommand]
public sealed class ReduceCommand : ToolshedCommand
{
	private sealed class ReduceBlockParser : CustomTypeParser<Block>
	{
		public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Block? result)
		{
			result = null;
			Type pipedType = ctx.Bundle.PipedType;
			if ((object)pipedType == null || !pipedType.IsGenericType)
			{
				return false;
			}
			LocalVarParser localVarParser = new LocalVarParser(ctx.VariableParser);
			Type type = ctx.Bundle.PipedType.GetGenericArguments()[0];
			localVarParser.SetLocalType("value", type, @readonly: false);
			ctx.VariableParser = localVarParser;
			if (!Block.TryParseBlock(ctx, type, type, out CommandRun run))
			{
				result = null;
				ctx.VariableParser = localVarParser.Inner;
				return false;
			}
			ctx.VariableParser = localVarParser.Inner;
			result = new Block(run);
			return true;
		}

		public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
		{
			Type pipedType = ctx.Bundle.PipedType;
			if ((object)pipedType == null || !pipedType.IsGenericType)
			{
				return null;
			}
			LocalVarParser localVarParser = new LocalVarParser(ctx.VariableParser);
			Type type = ctx.Bundle.PipedType.GetGenericArguments()[0];
			localVarParser.SetLocalType("value", type, @readonly: false);
			ctx.VariableParser = localVarParser;
			Block.TryParseBlock(ctx, type, type, out CommandRun _);
			ctx.VariableParser = localVarParser.Inner;
			return ctx.Completions;
		}
	}

	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public T Reduce<T>(IInvocationContext ctx, [PipedArgument] IEnumerable<T> input, [CommandArgument(typeof(ReduceBlockParser), false)] Block reducer)
	{
		LocalVarInvocationContext localVarInvocationContext = new LocalVarInvocationContext(ctx);
		localVarInvocationContext.SetLocal("value", default(T));
		using IEnumerator<T> enumerator = input.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			throw new InvalidOperationException("Input contains no elements");
		}
		T val = enumerator.Current;
		while (enumerator.MoveNext())
		{
			localVarInvocationContext.SetLocal("value", enumerator.Current);
			val = (T)reducer.Invoke(val, localVarInvocationContext);
			if (ctx.HasErrors)
			{
				break;
			}
		}
		return val;
	}
}
