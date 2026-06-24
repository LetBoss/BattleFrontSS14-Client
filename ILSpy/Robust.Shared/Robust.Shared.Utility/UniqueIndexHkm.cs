using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Robust.Shared.Utility;

public struct UniqueIndexHkm<TKey, TValue>(int capacity) : IUniqueIndex<TKey, TValue>, IEnumerable<KeyValuePair<TKey, HashSet<TValue>>>, IEnumerable where TKey : notnull
{
	private readonly Dictionary<TKey, HashSet<TValue>> _index = new Dictionary<TKey, HashSet<TValue>>(capacity);

	public int KeyCount => _index.Count;

	public HashSet<TValue> this[TKey key]
	{
		get
		{
			InitializedCheck();
			if (_index.TryGetValue(key, out HashSet<TValue> value))
			{
				return value;
			}
			_index.Add(key, value = new HashSet<TValue>());
			return value;
		}
	}

	private void InitializedCheck()
	{
		if (_index == null)
		{
			throw new NotSupportedException("UniqueIndexHkm instances must use the non-default constructor.");
		}
	}

	public bool Add(TKey key, TValue value)
	{
		InitializedCheck();
		return _index.GetOrNew(key).Add(value);
	}

	public int AddRange(TKey key, IEnumerable<TValue> values)
	{
		InitializedCheck();
		if (_index.TryGetValue(key, out HashSet<TValue> value))
		{
			int count = value.Count;
			value.UnionWith(values);
			return value.Count - count;
		}
		_index.Add(key, value = new HashSet<TValue>(values));
		return value.Count;
	}

	public bool Remove(TKey key)
	{
		InitializedCheck();
		return _index.Remove(key);
	}

	public bool Remove(TKey key, TValue value)
	{
		InitializedCheck();
		if (_index.Count == 0)
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
		InitializedCheck();
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
		InitializedCheck();
		if (_index.Count == 0)
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
		InitializedCheck();
		if (!_index.ContainsKey(key))
		{
			_index.Add(key, new HashSet<TValue>());
		}
	}

	public void Initialize(IEnumerable<TKey> keys)
	{
		Initialize(keys.Select((TKey k) => new KeyValuePair<TKey, HashSet<TValue>>(k, new HashSet<TValue>())));
	}

	public void Initialize(IEnumerable<KeyValuePair<TKey, HashSet<TValue>>> index)
	{
		InitializedCheck();
		if (_index.Count != 0)
		{
			throw new InvalidOperationException("Already initialized.");
		}
		foreach (var (key, value) in index)
		{
			_index.Add(key, value);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator()
	{
		InitializedCheck();
		return _index.GetEnumerator();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void Clear()
	{
		InitializedCheck();
		_index.Clear();
	}
}
