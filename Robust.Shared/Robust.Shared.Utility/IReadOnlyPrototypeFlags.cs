using System.Collections;
using System.Collections.Generic;
using Robust.Shared.Prototypes;

namespace Robust.Shared.Utility;

public interface IReadOnlyPrototypeFlags<T> : IEnumerable<string>, IEnumerable where T : class, IPrototype
{
	int Count { get; }

	bool Contains(string flag);

	bool ContainsAll(IEnumerable<string> flags);

	bool ContainsAll(params string[] flags);

	bool ContainsAny(IEnumerable<string> flags);

	bool ContainsAny(params string[] flags);

	IEnumerable<T> GetPrototypes(IPrototypeManager prototypeManager);
}
