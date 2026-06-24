using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Robust.Shared.Random;

public static class RandomHelpers
{
	public static void Shuffle<T>(this System.Random random, IList<T> list)
	{
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int num2 = random.Next(num + 1);
			int index = num2;
			int index2 = num;
			T value = list[num];
			T value2 = list[num2];
			list[index] = value;
			list[index2] = value2;
		}
	}

	public static bool Prob(this System.Random random, double chance)
	{
		return random.NextDouble() < chance;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte NextByte(this System.Random random, byte maxValue)
	{
		return random.NextByte(0, maxValue);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte NextByte(this System.Random random)
	{
		return random.NextByte(byte.MaxValue);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte NextByte(this System.Random random, byte minValue, byte maxValue)
	{
		return (byte)random.Next(minValue, maxValue);
	}
}
