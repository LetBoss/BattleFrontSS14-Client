using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Robust.Shared.Console;

namespace Robust.Shared.Toolshed.Syntax;

public interface IVariableParser
{
	private sealed class EmptyVarParser : IVariableParser
	{
		public bool TryParseVar(string name, [NotNullWhen(true)] out Type? type)
		{
			type = null;
			return false;
		}

		public IEnumerable<(string, Type)> GetVars()
		{
			yield break;
		}
	}

	static readonly IVariableParser Empty = new EmptyVarParser();

	bool TryParseVar(string name, [NotNullWhen(true)] out Type? type);

	CompletionResult GenerateCompletions(bool includeReadonly = true)
	{
		return CompletionResult.FromHintOptions(from x in GetVars()
			where includeReadonly || !IsReadonlyVar(x.Item1)
			select new CompletionOption("$" + x.Item1, x.Item2.PrettyName() ?? ""), "<variable name>");
	}

	CompletionResult GenerateCompletions<T>(bool includeReadonly = true)
	{
		return CompletionResult.FromHintOptions(from x in GetVars()
			where x.Item2 == typeof(T)
			where includeReadonly || !IsReadonlyVar(x.Item1)
			select new CompletionOption("$" + x.Item1), "<Variable of type " + typeof(T).PrettyName() + ">");
	}

	bool IsReadonlyVar(string name)
	{
		return false;
	}

	IEnumerable<(string, Type)> GetVars();
}
