using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.Console;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers.Tuples;

public abstract class BaseTupleTypeParser<TParses> : TypeParser<TParses> where TParses : ITuple
{
	public abstract IEnumerable<Type> Fields { get; }

	public abstract TParses Create(IReadOnlyList<object> values);

	public override bool TryParse(ParserContext parserContext, [NotNullWhen(true)] out TParses? result)
	{
		List<object> list = new List<object>();
		foreach (Type field in Fields)
		{
			if (!Toolshed.TryParse(parserContext, field, out object parsed))
			{
				result = default(TParses);
				return false;
			}
			list.Add(parsed);
		}
		result = Create(list);
		return true;
	}

	public override CompletionResult? TryAutocomplete(ParserContext parserContext, CommandArgument? arg)
	{
		foreach (Type field in Fields)
		{
			ParserRestorePoint point = parserContext.Save();
			if (!Toolshed.TryParse(parserContext, field, out object _) || !Rune.IsWhiteSpace(parserContext.PeekRune() ?? new Rune('.')))
			{
				parserContext.Restore(point);
				return Toolshed.TryAutocomplete(parserContext, field, null);
			}
		}
		return null;
	}
}
