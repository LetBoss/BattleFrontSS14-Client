using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class PrototypeInstanceTypeParser<T> : TypeParser<T> where T : class, IPrototype
{
	[Dependency]
	private readonly IPrototypeManager _proto;

	public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out T? result)
	{
		if (!Toolshed.TryParse<string>(ctx, out string parsed))
		{
			parsed = ctx.GetWord(ParserContext.IsToken);
		}
		if (parsed != null && _proto.TryIndex(parsed, out result))
		{
			return true;
		}
		_proto.TryGetKindFrom<T>(out string kind);
		ctx.Error = new NotAValidPrototype(parsed ?? "[null]", kind);
		result = null;
		return false;
	}

	public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
	{
		return Toolshed.TryAutocomplete(ctx, typeof(ProtoId<T>), arg);
	}
}
