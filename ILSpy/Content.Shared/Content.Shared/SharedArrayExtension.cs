using System;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Shared;

public static class SharedArrayExtension
{
	public static void Shuffle<T>(this Span<T> array, IRobustRandom? random = null)
	{
		int n = array.Length;
		if (n > 1)
		{
			IoCManager.Resolve<IRobustRandom>(ref random);
			while (n > 1)
			{
				n--;
				int k = random.Next(n + 1);
				ref T reference = ref array[k];
				ref T reference2 = ref array[n];
				T val = array[n];
				T val2 = array[k];
				reference = val;
				reference2 = val2;
			}
		}
	}
}
