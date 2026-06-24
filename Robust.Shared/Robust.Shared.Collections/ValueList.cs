using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Robust.Shared.Collections;

public struct ValueList<T> : IEnumerable<T>, IEnumerable
{
	public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
	{
		private readonly ValueList<T> _list;

		private int _index;

		public T Current => RefCurrent;

		public ref T RefCurrent => ref _list._items[_index];

		object? IEnumerator.Current => Current;

		internal Enumerator(ValueList<T> list)
		{
			_index = -1;
			_list = list;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			return ++_index < _list.Count;
		}

		void IEnumerator.Reset()
		{
			_index = -1;
		}
	}

	private const int DefaultCapacity = 4;

	private T[]? _items;

	public int Count { get; private set; }

	public readonly ref T this[int index]
	{
		get
		{
			if ((uint)index >= (uint)Count)
			{
				throw new IndexOutOfRangeException();
			}
			return ref _items[index];
		}
	}

	public int Capacity
	{
		readonly get
		{
			T[]? items = _items;
			if (items == null)
			{
				return 0;
			}
			return items.Length;
		}
		set
		{
			if (value < Count)
			{
				throw new ArgumentException("Cannot set capacity lower than contained count");
			}
			if (value == Capacity)
			{
				return;
			}
			if (value > 0)
			{
				T[] array = new T[value];
				if (Count > 0)
				{
					Array.Copy(_items, array, Count);
				}
				_items = array;
			}
			else
			{
				_items = null;
			}
		}
	}

	public readonly Span<T> Span => new Span<T>(_items, 0, Count);

	public ValueList(int capacity)
	{
		_items = ((capacity == 0) ? null : new T[capacity]);
		Count = 0;
	}

	public ValueList(List<T> list)
	{
		this = new ValueList<T>(list, 0, list.Count);
	}

	public ValueList(List<T> list, int start, int count)
	{
		_items = new T[count];
		Span<T> span = CollectionsMarshal.AsSpan(list);
		span.Slice(start, start + count - start).CopyTo(_items);
		Count = count;
	}

	public ValueList(IReadOnlyCollection<T> list)
	{
		Count = 0;
		int count = list.Count;
		_items = new T[count];
		foreach (T item in list)
		{
			int count2 = Count;
			AddNoResize(item, count2);
		}
	}

	public ValueList(IEnumerable<T> collection)
	{
		_items = collection.ToArray();
		Count = _items.Length;
	}

	public static ValueList<T> OwningArray(T[]? array)
	{
		ValueList<T> result = default(ValueList<T>);
		result._items = array;
		result.Count = result.Capacity;
		return result;
	}

	public static ValueList<T> OwningArray(T[]? array, int count)
	{
		ValueList<T> result = new ValueList<T>
		{
			_items = array
		};
		if (count < 0)
		{
			throw new ArgumentException("Count cannot be negative.");
		}
		if (count >= result.Capacity)
		{
			throw new ArgumentException("Count cannot be greater than the size of the array.");
		}
		result.Count = count;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(T item)
	{
		int count = Count;
		if ((uint)count < (uint)Capacity)
		{
			AddNoResize(item, count);
		}
		else
		{
			AddWithResize(item);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void AddNoResize(T item, int size)
	{
		T[]? items = _items;
		Count = size + 1;
		items[size] = item;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref T AddRef()
	{
		T[] items = _items;
		int count = Count;
		if ((uint)count < (uint)Capacity)
		{
			Count = count + 1;
			return ref items[count];
		}
		return ref AddRefWithResize();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void AddWithResize(T item)
	{
		int count = Count;
		Grow(count + 1);
		Count = count + 1;
		_items[count] = item;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private ref T AddRefWithResize()
	{
		int count = Count;
		Grow(count + 1);
		Count = count + 1;
		return ref _items[count];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear()
	{
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			int count = Count;
			Count = 0;
			if (count > 0)
			{
				Array.Clear(_items, 0, count);
			}
		}
		else
		{
			Count = 0;
		}
	}

	public readonly bool Contains(T item)
	{
		return IndexOf(item) >= 0;
	}

	public int EnsureCapacity(int capacity)
	{
		if (capacity < 0)
		{
			throw new ArgumentException("Capacity cannot be negative");
		}
		if (Capacity < capacity)
		{
			Grow(capacity);
		}
		if (capacity == 0)
		{
			return capacity;
		}
		return _items.Length;
	}

	private void Grow(int capacity)
	{
		int num = ((Capacity == 0) ? 4 : (2 * _items.Length));
		if ((uint)num > Array.MaxLength)
		{
			num = Array.MaxLength;
		}
		if (num < capacity)
		{
			num = capacity;
		}
		Capacity = num;
	}

	public readonly Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new Enumerator(this);
	}

	public readonly int IndexOf(T item)
	{
		if (_items != null)
		{
			return Array.IndexOf(_items, item, 0, Count);
		}
		return -1;
	}

	public readonly int IndexOf(T item, int index)
	{
		if (index > Count)
		{
			throw new ArgumentOutOfRangeException();
		}
		if (_items != null)
		{
			return Array.IndexOf(_items, item, index, Count - index);
		}
		return -1;
	}

	public readonly int IndexOf(T item, int index, int count)
	{
		if (index > Count)
		{
			throw new ArgumentException("Start index out of bounds");
		}
		if (count < 0 || index > Count - count)
		{
			throw new ArgumentException("Count out of range");
		}
		if (_items != null)
		{
			return Array.IndexOf(_items, item, index, count);
		}
		return -1;
	}

	public void Insert(int index, T item)
	{
		if ((uint)index > (uint)Count)
		{
			throw new ArgumentOutOfRangeException();
		}
		if (Count == _items.Length)
		{
			Grow(Count + 1);
		}
		if (index < Count)
		{
			Array.Copy(_items, index, _items, index + 1, Count - index);
		}
		_items[index] = item;
		Count++;
	}

	public readonly int LastIndexOf(T item)
	{
		if (Count == 0)
		{
			return -1;
		}
		return LastIndexOf(item, Count - 1, Count);
	}

	public readonly int LastIndexOf(T item, int index)
	{
		if (index >= Count)
		{
			throw new ArgumentOutOfRangeException("index", "Index out of range");
		}
		return LastIndexOf(item, index, index + 1);
	}

	public readonly int LastIndexOf(T item, int index, int count)
	{
		if (Count == 0)
		{
			return -1;
		}
		if (index < 0)
		{
			throw new ArgumentException("Index cannot be negative");
		}
		if (count < 0)
		{
			throw new ArgumentException("Count cannot be negative");
		}
		if (index >= Count)
		{
			throw new ArgumentException("Range outside of collection bounds");
		}
		if (count > index + 1)
		{
			throw new ArgumentException("Range outside of collection bounds");
		}
		return Array.LastIndexOf(_items, item, index, count);
	}

	public bool Remove(T item)
	{
		int num = IndexOf(item);
		if (num >= 0)
		{
			RemoveAt(num);
			return true;
		}
		return false;
	}

	public void RemoveAt(int index)
	{
		if ((uint)index >= (uint)Count)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		Count--;
		if (index < Count)
		{
			Array.Copy(_items, index + 1, _items, index, Count - index);
		}
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			_items[Count] = default(T);
		}
	}

	public void Sort()
	{
		Span.Sort();
	}

	public void Sort(IComparer<T>? comparer)
	{
		Span.Sort(comparer);
	}

	public void Sort(Comparison<T> comparison)
	{
		Span.Sort(comparison);
	}

	public readonly T[] ToArray()
	{
		return Span.ToArray();
	}

	public void TrimExcess()
	{
		int num = (int)((double)Capacity * 0.9);
		if (Count < num)
		{
			Capacity = Count;
		}
	}

	public T RemoveSwap(int index)
	{
		T result = this[index];
		T val = this[Count - 1];
		this[index] = val;
		RemoveAt(Count - 1);
		return result;
	}

	public void AddRange(ValueList<T> list)
	{
		Span<T> span = list.Span;
		AddRange(span);
	}

	public void AddRange(List<T> list)
	{
		Span<T> span = CollectionsMarshal.AsSpan(list);
		AddRange(span);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddRange(Span<T> span)
	{
		AddRange((ReadOnlySpan<T>)span);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddRange(ReadOnlySpan<T> span)
	{
		int length = span.Length;
		EnsureCapacity(Count + length);
		Span<T> destination = new Span<T>(_items, Count, length);
		span.CopyTo(destination);
		Count += length;
	}

	public void EnsureLength(int newCount)
	{
		if (Count <= newCount)
		{
			EnsureCapacity(newCount);
			new Span<T>(_items, Count, newCount - Count).Clear();
			Count = newCount;
		}
	}

	public void AddRange(IEnumerable<T> select)
	{
		foreach (T item in select)
		{
			Add(item);
		}
	}

	public void Push(T item)
	{
		Add(item);
	}

	public T Pop()
	{
		if (!TryPop(out var value))
		{
			throw new InvalidOperationException("List is empty");
		}
		return value;
	}

	public T Peek()
	{
		if (!TryPeek(out var value))
		{
			throw new InvalidOperationException("List is empty");
		}
		return value;
	}

	public bool TryPop([MaybeNullWhen(false)] out T value)
	{
		if (Count == 0)
		{
			value = default(T);
			return false;
		}
		value = _items[--Count];
		return true;
	}

	public bool TryPeek([MaybeNullWhen(false)] out T value)
	{
		if (Count == 0)
		{
			value = default(T);
			return false;
		}
		value = _items[Count];
		return true;
	}
}
