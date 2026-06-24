using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Robust.Shared.Utility;

internal static class HeapMethods
{
	internal static void Swap<T>(this T[] array, int i, int j)
	{
		T val = array[i];
		array[i] = array[j];
		array[j] = val;
	}

	internal static bool GreaterOrEqual<T>(this IComparer<T> comparer, [AllowNull] T x, [AllowNull] T y)
	{
		return comparer.Compare(x, y) >= 0;
	}

	internal static void Sink<T>(this T[] heap, int i, int count, IComparer<T> comparer, int shift)
	{
		int num = count + shift;
		while (true)
		{
			int num2 = i + shift;
			int num3 = 2 * i + shift;
			if (num3 > num)
			{
				break;
			}
			int num4 = num3 + 1;
			bool flag = num4 <= num;
			T x = heap[num2];
			T val = heap[num3];
			T y = (T)(flag ? ((object)heap[num4]) : ((object)default(T)));
			if (comparer.GreaterOrEqual(x, val) && (!flag || comparer.GreaterOrEqual(x, y)))
			{
				break;
			}
			int num5 = ((!flag || comparer.GreaterOrEqual(val, y)) ? num3 : num4);
			heap.Swap(num2, num5);
			i = num5 - shift;
		}
	}

	internal static void Sift<T>(this T[] heap, int i, IComparer<T> comparer, int shift)
	{
		while (i > 1)
		{
			int num = i / 2 + shift;
			int num2 = i + shift;
			if (comparer.GreaterOrEqual(heap[num], heap[num2]))
			{
				break;
			}
			heap.Swap(num, num2);
			i = num - shift;
		}
	}

	internal static void HeapSort<T>(this T[] heap, int startIndex, int count, IComparer<T> comparer)
	{
		int shift = startIndex - 1;
		int num = startIndex + count;
		int num2 = count;
		while (num > startIndex)
		{
			num--;
			num2--;
			heap.Swap(startIndex, num);
			heap.Sink(1, num2, comparer, shift);
		}
		Array.Reverse(heap, startIndex, count);
	}
}
