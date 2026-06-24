using System.Text;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class ProtoIdTypeParser<T> : TypeParser<ProtoId<T>> where T : class, IPrototype
{
	[Dependency]
	private readonly IConfigurationManager _config;

	[Dependency]
	private readonly IPrototypeManager _proto;

	public override bool TryParse(ParserContext ctx, out ProtoId<T> result)
	{
		result = default(ProtoId<T>);
		string parsed;
		if (ctx.PeekRune() == new Rune('"'))
		{
			if (!Toolshed.TryParse<string>(ctx, out parsed))
			{
				return false;
			}
		}
		else
		{
			parsed = ctx.GetWord(ParserContext.IsToken);
		}
		if (parsed == null || !_proto.HasIndex<T>(parsed))
		{
			_proto.TryGetKindFrom<T>(out string kind);
			ctx.Error = new NotAValidPrototype(parsed ?? "[null]", kind);
			result = default(ProtoId<T>);
			return false;
		}
		result = new ProtoId<T>(parsed);
		return true;
	}

	public override CompletionResult TryAutocomplete(ParserContext ctx, CommandArgument? arg)
	{
		if (typeof(T) == typeof(EntityPrototype))
		{
			return CompletionResult.FromHint(GetArgHint(arg));
		}
		string argHint = ToolshedCommand.GetArgHint(arg, typeof(ProtoId<T>));
		int cVar = _config.GetCVar(CVars.ToolshedPrototypesAutocompleteLimit);
		string input = ctx.Input;
		int index = ctx.Index;
		return CompletionResult.FromHintOptions(CompletionHelper.PrototypeIdsLimited<T>(input.Substring(index, input.Length - index), _proto, sorted: true, cVar), argHint);
	}
}
