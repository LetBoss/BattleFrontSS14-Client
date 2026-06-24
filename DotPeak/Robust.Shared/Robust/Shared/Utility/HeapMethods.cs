// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.HeapMethods
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Utility;

internal static class HeapMethods
{
  internal static void Swap<T>(this T[] array, int i, int j)
  {
    T obj = array[i];
    array[i] = array[j];
    array[j] = obj;
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
      int i1 = i + shift;
      int index1 = 2 * i + shift;
      if (index1 <= num)
      {
        int index2 = index1 + 1;
        bool flag = index2 <= num;
        T x = heap[i1];
        T obj = heap[index1];
        T y = flag ? heap[index2] : default (T);
        if (!comparer.GreaterOrEqual<T>(x, obj) || flag && !comparer.GreaterOrEqual<T>(x, y))
        {
          int j = !flag || comparer.GreaterOrEqual<T>(obj, y) ? index1 : index2;
          heap.Swap<T>(i1, j);
          i = j - shift;
        }
        else
          goto label_2;
      }
      else
        break;
    }
    return;
label_2:;
  }

  internal static void Sift<T>(this T[] heap, int i, IComparer<T> comparer, int shift)
  {
    int i1;
    for (; i > 1; i = i1 - shift)
    {
      i1 = i / 2 + shift;
      int j = i + shift;
      if (comparer.GreaterOrEqual<T>(heap[i1], heap[j]))
        break;
      heap.Swap<T>(i1, j);
    }
  }

  internal static void HeapSort<T>(
    this T[] heap,
    int startIndex,
    int count,
    IComparer<T> comparer)
  {
    int shift = startIndex - 1;
    int j = startIndex + count;
    int count1 = count;
    while (j > startIndex)
    {
      --j;
      --count1;
      heap.Swap<T>(startIndex, j);
      heap.Sink<T>(1, count1, comparer, shift);
    }
    Array.Reverse<T>(heap, startIndex, count);
  }
}
