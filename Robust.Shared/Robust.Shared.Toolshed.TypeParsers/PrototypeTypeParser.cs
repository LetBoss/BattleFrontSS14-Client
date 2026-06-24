using System;
using System.Collections.Generic;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

[Obsolete]
internal sealed class PrototypeTypeParser<T> : TypeParser<Prototype<T>> where T : class, IPrototype
{
	[Dependency]
	private readonly IPrototypeManager _prototype;

	public override bool TryParse(ParserContext ctx, out Prototype<T> result)
	{
		if (!Toolshed.TryParse<string>(ctx, out string parsed))
		{
			parsed = ctx.GetWord(ParserContext.IsToken);
		}
		if (parsed == null || !_prototype.TryIndex(parsed, out T prototype))
		{
			_prototype.TryGetKindFrom<T>(out string kind);
			ctx.Error = new NotAValidPrototype(parsed ?? "[null]", kind);
			result = default(Prototype<T>);
			return false;
		}
		result = new Prototype<T>(prototype);
		return true;
	}

	public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
	{
		IEnumerable<CompletionOption> options = ((!(typeof(T) != typeof(EntityPrototype))) ? Array.Empty<CompletionOption>() : CompletionHelper.PrototypeIDs<T>());
		_prototype.TryGetKindFrom<T>(out string kind);
		return CompletionResult.FromHintOptions(options, "<" + kind + " prototype>");
	}
}
