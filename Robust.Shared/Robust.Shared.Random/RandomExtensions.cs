using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Robust.Shared.Collections;
using Robust.Shared.Maths;

namespace Robust.Shared.Random;

public static class RandomExtensions
{
	public static double NextGaussian(this IRobustRandom random, double μ = 0.0, double σ = 1.0)
	{
		return random.GetRandom().NextGaussian(μ, σ);
	}

	public static T Pick<T>(this IRobustRandom random, IReadOnlyList<T> list)
	{
		int index = random.Next(list.Count);
		return list[index];
	}

	public static ref T Pick<T>(this IRobustRandom random, ValueList<T> list)
	{
		int index = random.Next(list.Count);
		return ref list[index];
	}

	public static ref T Pick<T>(this System.Random random, ValueList<T> list)
	{
		int index = random.Next(list.Count);
		return ref list[index];
	}

	public static T Pick<T>(this IRobustRandom random, IReadOnlyCollection<T> collection)
	{
		int num = random.Next(collection.Count);
		int num2 = 0;
		foreach (T item in collection)
		{
			if (num2++ == num)
			{
				return item;
			}
		}
		throw new UnreachableException("This should be unreachable!");
	}

	public static T PickAndTake<T>(this IRobustRandom random, IList<T> list)
	{
		int index = random.Next(list.Count);
		T result = list[index];
		list.RemoveAt(index);
		return result;
	}

	public static T Pick<T>(this System.Random random, ICollection<T> collection)
	{
		int num = random.Next(collection.Count);
		int num2 = 0;
		foreach (T item in collection)
		{
			if (num2++ == num)
			{
				return item;
			}
		}
		throw new UnreachableException("This should be unreachable!");
	}

	public static T PickAndTake<T>(this System.Random random, ICollection<T> set)
	{
		T val = random.Pick(set);
		set.Remove(val);
		return val;
	}

	public static double NextGaussian(this System.Random random, double μ = 0.0, double σ = 1.0)
	{
		double d = random.NextDouble();
		double num = random.NextDouble();
		double num2 = Math.Sqrt(-2.0 * Math.Log(d)) * Math.Sin(Math.PI * 2.0 * num);
		return μ + σ * num2;
	}

	public static Angle NextAngle(this System.Random random)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Angle.op_Implicit(random.NextFloat() * ((float)Math.PI * 2f));
	}

	public static Angle NextAngle(this System.Random random, Angle minAngle, Angle maxAngle)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		return minAngle + Angle.op_Implicit(Angle.op_Implicit(maxAngle - minAngle) * random.NextDouble());
	}

	public static Vector2 NextPolarVector2(this System.Random random, float minMagnitude, float maxMagnitude)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Angle val = random.NextAngle();
		Vector2 vector = new Vector2(random.NextFloat(minMagnitude, maxMagnitude), 0f);
		return ((Angle)(ref val)).RotateVec(ref vector);
	}

	public static float NextFloat(this IRobustRandom random)
	{
		return (float)random.Next() * 4.656613E-10f;
	}

	public static float NextFloat(this System.Random random)
	{
		return (float)random.Next() * 4.656613E-10f;
	}

	public static float NextFloat(this System.Random random, float minValue, float maxValue)
	{
		return random.NextFloat() * (maxValue - minValue) + minValue;
	}

	public static bool Prob(this IRobustRandom random, float chance)
	{
		return random.NextDouble() < (double)chance;
	}

	public static T[] GetItems<T>(this IRobustRandom random, IList<T> source, int count, bool allowDuplicates = true)
	{
		if (source.Count == 0 || count <= 0)
		{
			return Array.Empty<T>();
		}
		if (!allowDuplicates && count >= source.Count)
		{
			T[] array = source.ToArray();
			random.Shuffle((IList<T>)array);
			return array;
		}
		int count2 = source.Count;
		T[] array2 = new T[count];
		if (allowDuplicates)
		{
			for (int i = 0; i < count; i++)
			{
				array2[i] = source[random.Next(count2)];
			}
			return array2;
		}
		Span<int> span = ((count2 > 1024) ? ((Span<int>)new int[count2]) : stackalloc int[count2]);
		Span<int> span2 = span;
		for (int j = 0; j < count2; j++)
		{
			span2[j] = j;
		}
		for (int k = 0; k < count; k++)
		{
			int index = random.Next(count2 - k);
			array2[k] = source[span2[index]];
			span2[index] = span2[count2 - k - 1];
		}
		return array2;
	}

	public static T[] GetItems<T>(this IRobustRandom random, ValueList<T> source, int count, bool allowDuplicates = true)
	{
		return random.GetItems(source.Span, count, allowDuplicates);
	}

	public static T[] GetItems<T>(this IRobustRandom random, T[] source, int count, bool allowDuplicates = true)
	{
		return random.GetItems(source.AsSpan(), count, allowDuplicates);
	}

	public static T[] GetItems<T>(this IRobustRandom random, Span<T> source, int count, bool allowDuplicates = true)
	{
		if (source.Length == 0 || count <= 0)
		{
			return Array.Empty<T>();
		}
		if (!allowDuplicates && count >= source.Length)
		{
			T[] array = source.ToArray();
			random.Shuffle((IList<T>)array);
			return array;
		}
		int length = source.Length;
		T[] array2 = new T[count];
		if (allowDuplicates)
		{
			for (int i = 0; i < count; i++)
			{
				array2[i] = source[random.Next(length)];
			}
			return array2;
		}
		Span<int> span = ((length > 1024) ? ((Span<int>)new int[length]) : stackalloc int[length]);
		Span<int> span2 = span;
		for (int j = 0; j < length; j++)
		{
			span2[j] = j;
		}
		for (int k = 0; k < count; k++)
		{
			int index = random.Next(length - k);
			array2[k] = source[span2[index]];
			span2[index] = span2[length - k - 1];
		}
		return array2;
	}
}
