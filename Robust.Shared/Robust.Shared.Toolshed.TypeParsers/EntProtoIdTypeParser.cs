using Robust.Shared.Console;
using Robust.Shared.Prototypes;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class EntProtoIdTypeParser : TypeParser<EntProtoId>
{
	public override bool TryParse(ParserContext ctx, out EntProtoId result)
	{
		result = default(EntProtoId);
		if (!Toolshed.TryParse<ProtoId<EntityPrototype>>(ctx, out var parsed))
		{
			return false;
		}
		result = new EntProtoId(parsed.Id);
		return true;
	}

	public override CompletionResult? TryAutocomplete(ParserContext parserContext, CommandArgument? arg)
	{
		return CompletionResult.FromHint(GetArgHint(arg));
	}
}
