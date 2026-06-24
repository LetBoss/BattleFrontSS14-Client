using System;

namespace Robust.Shared.Utility;

public static class HashCodeHelpers
{
	public static void AddArray<T>(this ref HashCode hc, T[]? array)
	{
		if (array == null)
		{
			hc.Add(0);
			return;
		}
		hc.Add(array.Length);
		foreach (T value in array)
		{
			hc.Add(value);
		}
	}
}
