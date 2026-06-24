using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Robust.Shared.Toolshed.Syntax;

public sealed class InvocationCtxVarParser(IInvocationContext ctx) : IVariableParser
{
	private readonly IInvocationContext _ctx = ctx;

	public bool TryParseVar(string name, [NotNullWhen(true)] out Type? type)
	{
		type = _ctx.ReadVar(name)?.GetType();
		return type != null;
	}

	public IEnumerable<(string, Type)> GetVars()
	{
		foreach (string var in _ctx.GetVars())
		{
			if (TryParseVar(var, out Type type))
			{
				yield return (var, type);
			}
		}
	}

	public bool IsReadonlyVar(string name)
	{
		return _ctx.IsReadonlyVar(name);
	}
}
