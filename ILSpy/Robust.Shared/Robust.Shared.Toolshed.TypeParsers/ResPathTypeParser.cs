using Robust.Shared.Console;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class ResPathTypeParser : TypeParser<ResPath>
{
	public override bool TryParse(ParserContext parserContext, out ResPath result)
	{
		result = default(ResPath);
		if (!Toolshed.TryParse<string>(parserContext, out string parsed))
		{
			return false;
		}
		result = new ResPath(parsed);
		return true;
	}

	public override CompletionResult? TryAutocomplete(ParserContext parserContext, CommandArgument? arg)
	{
		return CompletionResult.FromHint(GetArgHint(arg));
	}
}
