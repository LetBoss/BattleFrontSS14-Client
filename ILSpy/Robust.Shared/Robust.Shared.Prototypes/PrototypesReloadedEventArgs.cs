using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Robust.Shared.Prototypes;

public sealed record PrototypesReloadedEventArgs(HashSet<Type> Modified, IReadOnlyDictionary<Type, PrototypesReloadedEventArgs.PrototypeChangeSet> ByType, IReadOnlyDictionary<Type, HashSet<string>>? Removed = null)
{
	public sealed record PrototypeChangeSet(IReadOnlyDictionary<string, IPrototype> Modified);

	public bool WasModified<T>() where T : IPrototype
	{
		return Modified.Contains(typeof(T));
	}

	public bool TryGetModified<T>([NotNullWhen(true)] out HashSet<string>? modified) where T : IPrototype
	{
		modified = null;
		if (!WasModified<T>())
		{
			return false;
		}
		modified = new HashSet<string>();
		if (ByType.TryGetValue(typeof(T), out PrototypeChangeSet value))
		{
			modified.UnionWith(value.Modified.Keys);
		}
		if (Removed != null && Removed.TryGetValue(typeof(T), out HashSet<string> value2))
		{
			modified.UnionWith(value2);
		}
		return true;
	}
}
