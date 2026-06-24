using System;
using System.Collections.Generic;

namespace Robust.Shared.Toolshed.TypeParsers.Tuples;

public sealed class ValueTuple5TypeParser<T1, T2, T3, T4, T5> : BaseTupleTypeParser<(T1, T2, T3, T4, T5)>
{
	public override IEnumerable<Type> Fields => new Type[5]
	{
		typeof(T1),
		typeof(T2),
		typeof(T3),
		typeof(T4),
		typeof(T5)
	};

	public override (T1, T2, T3, T4, T5) Create(IReadOnlyList<object> values)
	{
		return ((T1)values[0], (T2)values[1], (T3)values[2], (T4)values[3], (T5)values[4]);
	}
}
