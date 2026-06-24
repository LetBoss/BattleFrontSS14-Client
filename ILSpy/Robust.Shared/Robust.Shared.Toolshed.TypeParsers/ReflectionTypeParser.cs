using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Reflection;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class ReflectionTypeParser<TBase> : CustomTypeParser<Type> where TBase : class
{
	[Dependency]
	private readonly IReflectionManager _reflection;

	private Dictionary<string, Type>? _cache;

	private CompletionOption[]? _options;

	[MemberNotNull("_cache")]
	[MemberNotNull("_options")]
	private void InitCache()
	{
		if (_cache == null || _options == null)
		{
			_cache = (from x in _reflection.GetAllChildren(typeof(TBase))
				where x.HasParameterlessConstructor()
				select x).ToDictionary((Type x) => x.Name, (Type x) => x);
			_options = _cache.Keys.Select((string x) => new CompletionOption(x)).ToArray();
		}
	}

	public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Type? result)
	{
		InitCache();
		string word = ctx.GetWord();
		if (word == null)
		{
			ctx.Error = new OutOfInputError();
			result = null;
			return false;
		}
		if (_cache.TryGetValue(word, out result))
		{
			return true;
		}
		ctx.Error = new UnknownType(word);
		result = null;
		return false;
	}

	public override CompletionResult? TryAutocomplete(ParserContext parserContext, CommandArgument? arg)
	{
		InitCache();
		return CompletionResult.FromHintOptions(_options, GetArgHint(arg));
	}
}
