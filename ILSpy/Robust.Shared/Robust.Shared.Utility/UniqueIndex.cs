using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Robust.Shared.Utility;

public struct UniqueIndex<TKey, TValue> : IUniqueIndex<TKey, TValue>, IEnumerable<KeyValuePair<TKey, HashSet<TValue>>>, IEnumerable where TKey : notnull
{
	private ImmutableDictionary<TKey, HashSet<TValue>>? _index;

	public int KeyCount => _index?.Count ?? 0;

	public ISet<TValue> this[TKey key]
	{
		get
		{
			HashSet<TValue> value;
			if (_index == null)
			{
				_index = ImmutableDictionary<TKey, HashSet<TValue>>.Empty;
			}
			else if (_index.TryGetValue(key, out value))
			{
				return value;
			}
			_index = _index.Add(key, value = new HashSet<TValue>());
			return value;
		}
	}

	public bool Add(TKey key, TValue value)
	{
		HashSet<TValue> value2;
		if (_index == null)
		{
			value2 = new HashSet<TValue> { value };
			_index = ImmutableDictionary.CreateRange(new KeyValuePair<TKey, HashSet<TValue>>[1]
			{
				new KeyValuePair<TKey, HashSet<TValue>>(key, value2)
			});
			return true;
		}
		if (_index.TryGetValue(key, out value2))
		{
			return value2.Add(value);
		}
		_index = _index.Add(key, new HashSet<TValue> { value });
		return true;
	}

	public int AddRange(TKey key, IEnumerable<TValue> values)
	{
		HashSet<TValue> hashSet;
		if (_index == null)
		{
			hashSet = new HashSet<TValue>(values);
			_index = ImmutableDictionary.CreateRange(new KeyValuePair<TKey, HashSet<TValue>>[1]
			{
				new KeyValuePair<TKey, HashSet<TValue>>(key, hashSet)
			});
			return hashSet.Count;
		}
		if (_index.TryGetValue(key, out hashSet))
		{
			int count = hashSet.Count;
			hashSet.UnionWith(values);
			return hashSet.Count - count;
		}
		_index = _index.Add(key, hashSet = new HashSet<TValue>(values));
		return hashSet.Count;
	}

	public bool Remove(TKey key)
	{
		if (_index == null)
		{
			return false;
		}
		ImmutableDictionary<TKey, HashSet<TValue>> immutableDictionary = _index.Remove(key);
		if (_index != immutableDictionary)
		{
			return false;
		}
		_index = immutableDictionary;
		return true;
	}

	public bool Remove(TKey key, TValue value)
	{
		if (_index == null)
		{
			return false;
		}
		if (_index.TryGetValue(key, out HashSet<TValue> value2))
		{
			return value2.Remove(value);
		}
		return false;
	}

	public int RemoveRange(TKey key, IEnumerable<TValue> values)
	{
		if (_index == null)
		{
			return 0;
		}
		if (!_index.TryGetValue(key, out HashSet<TValue> value))
		{
			return 0;
		}
		int count = value.Count;
		value.ExceptWith(values);
		return count - value.Count;
	}

	public bool Replace(TKey key, TValue oldValue, TValue newValue)
	{
		if (_index == null)
		{
			return false;
		}
		if (!_index.TryGetValue(key, out HashSet<TValue> value))
		{
			return false;
		}
		if (value.Remove(oldValue))
		{
			return value.Add(newValue);
		}
		return false;
	}

	public void Touch(TKey key)
	{
		if (_index == null)
		{
			_index = ImmutableDictionary<TKey, HashSet<TValue>>.Empty;
		}
		if (!_index.ContainsKey(key))
		{
			_index = _index.Add(key, new HashSet<TValue>());
		}
	}

	public void Initialize(IEnumerable<TKey> keys)
	{
		Initialize(keys.Select((TKey k) => new KeyValuePair<TKey, HashSet<TValue>>(k, new HashSet<TValue>())));
	}

	public void Initialize(IEnumerable<KeyValuePair<TKey, HashSet<TValue>>> index)
	{
		if (_index != null)
		{
			throw new InvalidOperationException("Already initialized.");
		}
		_index = ImmutableDictionary.CreateRange(index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator()
	{
		if (_index != null)
		{
			return _index.GetEnumerator();
		}
		return Enumerable.Empty<KeyValuePair<TKey, HashSet<TValue>>>().GetEnumerator();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
