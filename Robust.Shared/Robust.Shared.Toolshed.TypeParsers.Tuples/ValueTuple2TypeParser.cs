using System;
using System.Collections.Generic;

namespace Robust.Shared.Toolshed.TypeParsers.Tuples;

public sealed class ValueTuple2TypeParser<T1, T2> : BaseTupleTypeParser<(T1, T2)>
{
	public override IEnumerable<Type> Fields => new Type[2]
	{
		typeof(T1),
		typeof(T2)
	};

	public override (T1, T2) Create(IReadOnlyList<object> values)
	{
		return ((T1)values[0], (T2)values[1]);
	}
}
