using System.Collections.Generic;

namespace Content.Shared.FixedPoint;

public static class FixedPoint2EnumerableExt
{
	public static FixedPoint2 Sum(this IEnumerable<FixedPoint2> source)
	{
		FixedPoint2 acc = FixedPoint2.Zero;
		foreach (FixedPoint2 n in source)
		{
			acc += n;
		}
		return acc;
	}
}
