// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.Extensions
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

#nullable enable
namespace Robust.Shared.Utility;

public static class Extensions
{
  public static void EnsureLength<T>(ref T[] array, int length)
  {
    if (array.Length > length)
      return;
    Array.Resize<T>(ref array, length);
  }

  public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
  {
    List<T> objList = new List<T>(listToClone.Count);
    foreach (T obj in (IEnumerable<T>) listToClone)
      objList.Add((T) obj.Clone());
    return (IList<T>) objList;
  }

  public static List<T> ShallowClone<T>(this List<T> self)
  {
    List<T> objList = new List<T>(self.Count);
    objList.AddRange((IEnumerable<T>) self);
    return objList;
  }

  public static Dictionary<TKey, TValue> ShallowClone<TKey, TValue>(
    this Dictionary<TKey, TValue> self)
    where TKey : notnull
  {
    Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(self.Count);
    foreach (KeyValuePair<TKey, TValue> keyValuePair in self)
      dictionary[keyValuePair.Key] = keyValuePair.Value;
    return dictionary;
  }

  public static bool TryGetValue<T>(this IList<T> list, int index, out T value)
  {
    if (index >= 0 && list.Count > index)
    {
      value = list[index];
      return true;
    }
    value = default (T);
    return false;
  }

  public static T RemoveSwap<T>(this IList<T> list, int index)
  {
    T obj1 = list[index];
    T obj2 = list[list.Count - 1];
    list[index] = obj2;
    list.RemoveAt(list.Count - 1);
    return obj1;
  }

  public static T Pop<T>(this IList<T> list)
  {
    T obj = list.Count != 0 ? list[list.Count - 1] : throw new InvalidOperationException();
    list.RemoveAt(list.Count - 1);
    return obj;
  }

  public static TSource? FirstOrNull<TSource>(
    this IEnumerable<TSource> source,
    Func<TSource, bool> predicate)
    where TSource : struct
  {
    if (source == null)
      throw new ArgumentNullException(nameof (source));
    if (predicate == null)
      throw new ArgumentNullException(nameof (predicate));
    foreach (TSource source1 in source)
    {
      if (predicate(source1))
        return new TSource?(source1);
    }
    return new TSource?();
  }

  public static TSource? FirstOrNull<TSource>(this IEnumerable<TSource> source) where TSource : struct
  {
    if (source == null)
      throw new ArgumentNullException(nameof (source));
    using (IEnumerator<TSource> enumerator = source.GetEnumerator())
      return !enumerator.MoveNext() ? new TSource?() : new TSource?(enumerator.Current);
  }

  public static bool TryFirstOrNull<TSource>(
    this IEnumerable<TSource> source,
    Func<TSource, bool> predicate,
    [NotNullWhen(true)] out TSource? element)
    where TSource : struct
  {
    element = source.FirstOrNull<TSource>(predicate);
    return element.HasValue;
  }

  public static bool TryFirstOrNull<TSource>(this IEnumerable<TSource> source, [NotNullWhen(true)] out TSource? element) where TSource : struct
  {
    return source.TryFirstOrNull<TSource>((Func<TSource, bool>) (_ => true), out element);
  }

  public static bool TryFirstOrDefault<TSource>(
    this IEnumerable<TSource> source,
    Func<TSource, bool> predicate,
    [NotNullWhen(true)] out TSource? element)
    where TSource : class
  {
    element = source.FirstOrDefault<TSource>(predicate);
    return (object) element != null;
  }

  public static bool TryFirstOrDefault<TSource>(
    this IEnumerable<TSource> source,
    [NotNullWhen(true)] out TSource? element)
    where TSource : class
  {
    return source.TryFirstOrDefault<TSource>((Func<TSource, bool>) (_ => true), out element);
  }

  public static TValue GetOrNew<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
    where TKey : notnull
    where TValue : new()
  {
    TValue orNew;
    if (!dict.TryGetValue(key, out orNew))
    {
      orNew = new TValue();
      dict.Add(key, orNew);
    }
    return orNew;
  }

  public static TValue GetOrNew<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
    where TKey : notnull
    where TValue : new()
  {
    bool exists;
    ref TValue local = ref CollectionsMarshal.GetValueRefOrAddDefault<TKey, TValue>(dict, key, out exists);
    if (!exists)
      local = new TValue();
    return local;
  }

  public static TValue GetOrNew<TKey, TValue>(
    this Dictionary<TKey, TValue> dict,
    TKey key,
    out bool exists)
    where TKey : notnull
    where TValue : new()
  {
    ref TValue local = ref CollectionsMarshal.GetValueRefOrAddDefault<TKey, TValue>(dict, key, out exists);
    if (!exists)
      local = new TValue();
    return local;
  }

  public static KeyValuePair<TKey, TValue>[] ToArray<TKey, TValue>(
    this Dictionary<TKey, TValue> dict)
    where TKey : notnull
  {
    KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[dict.Count];
    int index = 0;
    foreach (KeyValuePair<TKey, TValue> keyValuePair in dict)
    {
      array[index] = keyValuePair;
      ++index;
    }
    return array;
  }

  public static bool TryCastValue<T, TKey, TValue>(
    this Dictionary<TKey, TValue> dict,
    TKey key,
    [NotNullWhen(true)] out T? value)
    where TKey : notnull
  {
    TValue obj1;
    if (dict.TryGetValue(key, out obj1) && obj1 is T obj2)
    {
      value = obj2;
      return true;
    }
    value = default (T);
    return false;
  }
}
