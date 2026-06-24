using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Robust.Shared.Collections;

public sealed class OverflowDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDisposable where TKey : notnull
{
	private TKey[] _insertionQueue;

	private int _currentIndex;

	private IDictionary<TKey, TValue> _dict;

	private Action<TValue>? _valueDisposer;

	public int Capacity => _insertionQueue.Length;

	public ICollection<TKey> Keys => _dict.Keys;

	public ICollection<TValue> Values => _dict.Values;

	public int Count => _dict.Count;

	public bool IsReadOnly => false;

	public TValue this[TKey key]
	{
		get
		{
			return _dict[key];
		}
		set
		{
			_dict[key] = value;
		}
	}

	public OverflowDictionary(int capacity, Action<TValue>? valueDisposer = null)
	{
		if (capacity <= 0)
		{
			throw new InvalidOperationException("Cannot create an OverflowDictionary with a capacity of less than 1.");
		}
		_valueDisposer = valueDisposer;
		_dict = new Dictionary<TKey, TValue>(capacity);
		_insertionQueue = new TKey[capacity];
	}

	private int GetArrayStartIndex()
	{
		return _currentIndex % Capacity;
	}

	public void Clear()
	{
		_dict.Clear();
		Array.Clear(_insertionQueue);
	}

	public void Add(TKey key, TValue value)
	{
		if (_dict.ContainsKey(key))
		{
			throw new InvalidOperationException("Tried inserting duplicate key.");
		}
		if (Count == Capacity)
		{
			int arrayStartIndex = GetArrayStartIndex();
			TKey key2 = _insertionQueue[arrayStartIndex];
			Array.Clear(_insertionQueue, arrayStartIndex, 1);
			_valueDisposer?.Invoke(_dict[key2]);
			_dict.Remove(key2);
		}
		_dict.Add(key, value);
		_insertionQueue[_currentIndex++] = key;
		if (_currentIndex == Capacity)
		{
			_currentIndex = 0;
		}
	}

	public bool Add(TKey key, TValue value, [NotNullWhen(true)] out (TKey Key, TValue Value)? old)
	{
		if (_dict.ContainsKey(key))
		{
			throw new InvalidOperationException("Tried inserting duplicate key.");
		}
		if (Count == Capacity)
		{
			int arrayStartIndex = GetArrayStartIndex();
			TKey val = _insertionQueue[arrayStartIndex];
			_dict.Remove(val, out var value2);
			Array.Clear(_insertionQueue, arrayStartIndex, 1);
			_valueDisposer?.Invoke(value2);
			old = (val, value2);
		}
		else
		{
			old = null;
		}
		_dict.Add(key, value);
		_insertionQueue[_currentIndex++] = key;
		if (_currentIndex == Capacity)
		{
			_currentIndex = 0;
		}
		return old.HasValue;
	}

	public bool Remove(TKey key)
	{
		throw new NotImplementedException("Removing from an Overflowdictionary is not yet supported");
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		return _dict.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void Add(KeyValuePair<TKey, TValue> item)
	{
		Add(item.Key, item.Value);
	}

	public bool Contains(KeyValuePair<TKey, TValue> item)
	{
		return _dict.Contains(item);
	}

	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		_dict.CopyTo(array, arrayIndex);
	}

	public bool Remove(KeyValuePair<TKey, TValue> item)
	{
		return Remove(item.Key);
	}

	public bool ContainsKey(TKey key)
	{
		return _dict.ContainsKey(key);
	}

	public bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? value)
	{
		return _dict.TryGetValue(key, out value);
	}

	public void Dispose()
	{
		foreach (TValue value in _dict.Values)
		{
			_valueDisposer?.Invoke(value);
		}
	}
}
