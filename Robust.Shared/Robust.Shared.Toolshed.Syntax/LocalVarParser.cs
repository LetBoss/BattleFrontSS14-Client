using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Robust.Shared.Toolshed.Syntax;

public sealed class LocalVarParser(IVariableParser inner) : IVariableParser
{
	public readonly IVariableParser Inner = inner;

	public Dictionary<string, Type>? Variables;

	public HashSet<string>? ReadonlyVariables;

	public void SetLocalType(string name, Type? type, bool @readonly)
	{
		if (type == null)
		{
			Variables?.Remove(name);
			return;
		}
		if (Variables == null)
		{
			Variables = new Dictionary<string, Type>();
		}
		Variables[name] = type;
		if (@readonly)
		{
			if (ReadonlyVariables == null)
			{
				ReadonlyVariables = new HashSet<string>();
			}
			ReadonlyVariables.Add(name);
		}
	}

	public bool TryParseVar(string name, [NotNullWhen(true)] out Type? type)
	{
		if (Variables != null && Variables.TryGetValue(name, out type))
		{
			return true;
		}
		return Inner.TryParseVar(name, out type);
	}

	public IEnumerable<(string, Type)> GetVars()
	{
		foreach (var (item, item2) in Variables)
		{
			yield return (item, item2);
		}
		foreach (var (text2, item3) in Inner.GetVars())
		{
			if (!Variables.ContainsKey(text2))
			{
				yield return (text2, item3);
			}
		}
	}

	public bool IsReadonlyVar(string name)
	{
		if (ReadonlyVariables != null)
		{
			return ReadonlyVariables.Contains(name);
		}
		return false;
	}
}
