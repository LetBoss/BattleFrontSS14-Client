using System.Collections.Generic;

namespace Content.Shared.FixedPoint;

public static class FixedPoint4EnumerableExt
{
	public static FixedPoint4 Sum(this IEnumerable<FixedPoint4> source)
	{
		FixedPoint4 acc = FixedPoint4.Zero;
		foreach (FixedPoint4 n in source)
		{
			acc += n;
		}
		return acc;
	}
}
