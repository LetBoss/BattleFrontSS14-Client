using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

namespace Robust.Shared.Utility;

public static class Extensions
{
	public static void EnsureLength<T>(ref T[] array, int length)
	{
		if (array.Length <= length)
		{
			Array.Resize(ref array, length);
		}
	}

	public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
	{
		List<T> list = new List<T>(listToClone.Count);
		foreach (T item in listToClone)
		{
			list.Add((T)item.Clone());
		}
		return list;
	}

	public static List<T> ShallowClone<T>(this List<T> self)
	{
		List<T> list = new List<T>(self.Count);
		list.AddRange(self);
		return list;
	}

	public static Dictionary<TKey, TValue> ShallowClone<TKey, TValue>(this Dictionary<TKey, TValue> self) where TKey : notnull
	{
		Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(self.Count);
		foreach (KeyValuePair<TKey, TValue> item in self)
		{
			dictionary[item.Key] = item.Value;
		}
		return dictionary;
	}

	public static bool TryGetValue<T>(this IList<T> list, int index, out T value)
	{
		if (index >= 0 && list.Count > index)
		{
			value = list[index];
			return true;
		}
		value = default(T);
		return false;
	}

	public static T RemoveSwap<T>(this IList<T> list, int index)
	{
		T result = list[index];
		T value = list[list.Count - 1];
		list[index] = value;
		list.RemoveAt(list.Count - 1);
		return result;
	}

	public static T Pop<T>(this IList<T> list)
	{
		if (list.Count == 0)
		{
			throw new InvalidOperationException();
		}
		T result = list[list.Count - 1];
		list.RemoveAt(list.Count - 1);
		return result;
	}

	public static TSource? FirstOrNull<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) where TSource : struct
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		foreach (TSource item in source)
		{
			if (predicate(item))
			{
				return item;
			}
		}
		return null;
	}

	public static TSource? FirstOrNull<TSource>(this IEnumerable<TSource> source) where TSource : struct
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			return null;
		}
		return enumerator.Current;
	}

	public static bool TryFirstOrNull<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, [NotNullWhen(true)] out TSource? element) where TSource : struct
	{
		element = source.FirstOrNull(predicate);
		return element.HasValue;
	}

	public static bool TryFirstOrNull<TSource>(this IEnumerable<TSource> source, [NotNullWhen(true)] out TSource? element) where TSource : struct
	{
		return source.TryFirstOrNull((TSource _) => true, out element);
	}

	public static bool TryFirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, [NotNullWhen(true)] out TSource? element) where TSource : class
	{
		element = source.FirstOrDefault(predicate);
		return element != null;
	}

	public static bool TryFirstOrDefault<TSource>(this IEnumerable<TSource> source, [NotNullWhen(true)] out TSource? element) where TSource : class
	{
		return source.TryFirstOrDefault((TSource _) => true, out element);
	}

	public static TValue GetOrNew<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TKey : notnull where TValue : new()
	{
		if (!dict.TryGetValue(key, out TValue value))
		{
			value = new TValue();
			dict.Add(key, value);
		}
		return value;
	}

	public static TValue GetOrNew<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) where TKey : notnull where TValue : new()
	{
		bool exists;
		ref TValue valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out exists);
		if (!exists)
		{
			valueRefOrAddDefault = new TValue();
		}
		return valueRefOrAddDefault;
	}

	public static TValue GetOrNew<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, out bool exists) where TKey : notnull where TValue : new()
	{
		ref TValue valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out exists);
		if (!exists)
		{
			valueRefOrAddDefault = new TValue();
		}
		return valueRefOrAddDefault;
	}

	public static KeyValuePair<TKey, TValue>[] ToArray<TKey, TValue>(this Dictionary<TKey, TValue> dict) where TKey : notnull
	{
		KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[dict.Count];
		int num = 0;
		foreach (KeyValuePair<TKey, TValue> item in dict)
		{
			array[num] = item;
			num++;
		}
		return array;
	}

	public static bool TryCastValue<T, TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, [NotNullWhen(true)] out T? value) where TKey : notnull
	{
		if (dict.TryGetValue(key, out TValue value2) && value2 is T val)
		{
			value = val;
			return true;
		}
		value = default(T);
		return false;
	}
}
