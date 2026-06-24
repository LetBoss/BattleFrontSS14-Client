using System;
using System.Threading;

namespace Robust.Shared.Utility;

internal static class InterlockedHelper
{
	public static void Min(ref uint a, uint b)
	{
		uint num;
		uint value;
		do
		{
			num = a;
			value = Math.Min(num, b);
		}
		while (Interlocked.CompareExchange(ref a, value, num) != num);
	}
}
