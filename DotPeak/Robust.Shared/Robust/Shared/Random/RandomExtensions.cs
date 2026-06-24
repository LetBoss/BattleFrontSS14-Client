// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Random.RandomExtensions
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Collections;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

#nullable enable
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
    int num1 = random.Next(collection.Count);
    int num2 = 0;
    foreach (T obj in (IEnumerable<T>) collection)
    {
      if (num2++ == num1)
        return obj;
    }
    throw new UnreachableException("This should be unreachable!");
  }

  public static T PickAndTake<T>(this IRobustRandom random, IList<T> list)
  {
    int index = random.Next(list.Count);
    T obj = list[index];
    list.RemoveAt(index);
    return obj;
  }

  public static T Pick<T>(this System.Random random, ICollection<T> collection)
  {
    int num1 = random.Next(collection.Count);
    int num2 = 0;
    foreach (T obj in (IEnumerable<T>) collection)
    {
      if (num2++ == num1)
        return obj;
    }
    throw new UnreachableException("This should be unreachable!");
  }

  public static T PickAndTake<T>(this System.Random random, ICollection<T> set)
  {
    T obj = random.Pick<T>(set);
    set.Remove(obj);
    return obj;
  }

  public static double NextGaussian(this System.Random random, double μ = 0.0, double σ = 1.0)
  {
    double d = random.NextDouble();
    double num1 = random.NextDouble();
    double num2 = Math.Sqrt(-2.0 * Math.Log(d)) * Math.Sin(2.0 * Math.PI * num1);
    return μ + σ * num2;
  }

  public static Angle NextAngle(this System.Random random)
  {
    return Angle.op_Implicit(random.NextFloat() * 6.28318548f);
  }

  public static Angle NextAngle(this System.Random random, Angle minAngle, Angle maxAngle)
  {
    return Angle.op_Addition(minAngle, Angle.op_Implicit(Angle.op_Implicit(Angle.op_Subtraction(maxAngle, minAngle)) * random.NextDouble()));
  }

  public static Vector2 NextPolarVector2(
    this System.Random random,
    float minMagnitude,
    float maxMagnitude)
  {
    Angle angle = random.NextAngle();
    ref Angle local1 = ref angle;
    Vector2 vector2 = new Vector2(random.NextFloat(minMagnitude, maxMagnitude), 0.0f);
    ref Vector2 local2 = ref vector2;
    return ((Angle) ref local1).RotateVec(ref local2);
  }

  public static float NextFloat(this IRobustRandom random) => (float) random.Next() * 4.656613E-10f;

  public static float NextFloat(this System.Random random) => (float) random.Next() * 4.656613E-10f;

  public static float NextFloat(this System.Random random, float minValue, float maxValue)
  {
    return random.NextFloat() * (maxValue - minValue) + minValue;
  }

  public static bool Prob(this IRobustRandom random, float chance)
  {
    return random.NextDouble() < (double) chance;
  }

  public static T[] GetItems<T>(
    this IRobustRandom random,
    IList<T> source,
    int count,
    bool allowDuplicates = true)
  {
    if (source.Count == 0 || count <= 0)
      return Array.Empty<T>();
    if (!allowDuplicates && count >= source.Count)
    {
      T[] array = source.ToArray<T>();
      random.Shuffle<T>((IList<T>) array);
      return array;
    }
    int count1 = source.Count;
    T[] items = new T[count];
    if (allowDuplicates)
    {
      for (int index = 0; index < count; ++index)
        items[index] = source[random.Next(count1)];
      return items;
    }
    Span<int> span = count1 > 1024 /*0x0400*/ ? (Span<int>) new int[count1] : stackalloc int[count1];
    for (int index = 0; index < count1; ++index)
      span[index] = index;
    for (int index1 = 0; index1 < count; ++index1)
    {
      int index2 = random.Next(count1 - index1);
      items[index1] = source[span[index2]];
      span[index2] = span[count1 - index1 - 1];
    }
    return items;
  }

  public static T[] GetItems<T>(
    this IRobustRandom random,
    ValueList<T> source,
    int count,
    bool allowDuplicates = true)
  {
    return random.GetItems<T>(source.Span, count, allowDuplicates);
  }

  public static T[] GetItems<T>(
    this IRobustRandom random,
    T[] source,
    int count,
    bool allowDuplicates = true)
  {
    return random.GetItems<T>(source.AsSpan<T>(), count, allowDuplicates);
  }

  public static T[] GetItems<T>(
    this IRobustRandom random,
    Span<T> source,
    int count,
    bool allowDuplicates = true)
  {
    if (source.Length == 0 || count <= 0)
      return Array.Empty<T>();
    if (!allowDuplicates && count >= source.Length)
    {
      T[] array = source.ToArray();
      random.Shuffle<T>((IList<T>) array);
      return array;
    }
    int length = source.Length;
    T[] items = new T[count];
    if (allowDuplicates)
    {
      for (int index = 0; index < count; ++index)
        items[index] = source[random.Next(length)];
      return items;
    }
    Span<int> span = length > 1024 /*0x0400*/ ? (Span<int>) new int[length] : stackalloc int[length];
    for (int index = 0; index < length; ++index)
      span[index] = index;
    for (int index1 = 0; index1 < count; ++index1)
    {
      int index2 = random.Next(length - index1);
      items[index1] = source[span[index2]];
      span[index2] = span[length - index1 - 1];
    }
    return items;
  }
}
