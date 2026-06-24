using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Prototypes;

public static class CMPrototypeExtensions
{
	public static bool FilterCM = true;

	public static IEnumerable<T> EnumerateCM<T>(this IPrototypeManager prototypes) where T : class, IPrototype, ICMSpecific
	{
		IEnumerable<T> protos = prototypes.EnumeratePrototypes<T>();
		if (FilterCM)
		{
			protos = protos.Where((T p) => p.IsCM);
		}
		return protos;
	}

	public static bool TryCM<T>(this IPrototypeManager prototypes, string id, [NotNullWhen(true)] out T? prototype) where T : class, IPrototype, ICMSpecific
	{
		prototype = null;
		T proto = default(T);
		if (!prototypes.TryIndex<T>(id, ref proto))
		{
			return false;
		}
		if (FilterCM && !proto.IsCM)
		{
			return false;
		}
		prototype = proto;
		return true;
	}
}
