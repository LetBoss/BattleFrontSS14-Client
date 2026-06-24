using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Console;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class CommandRunTypeParser : TypeParser<CommandRun>
{
	public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out CommandRun? result)
	{
		return CommandRun.TryParse(ctx, null, null, out result);
	}

	public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
	{
		CommandRun.TryParse(ctx, null, null, out CommandRun _);
		return ctx.Completions;
	}
}
