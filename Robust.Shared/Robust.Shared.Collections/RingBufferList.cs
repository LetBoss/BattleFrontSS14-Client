using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Robust.Shared.Collections;

internal sealed class RingBufferList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
	{
		private readonly RingBufferList<T> _ringBufferList;

		private int _readPos;

		public ref T Current => ref _ringBufferList._items[_readPos];

		T IEnumerator<T>.Current => Current;

		object? IEnumerator.Current => Current;

		internal Enumerator(RingBufferList<T> ringBufferList)
		{
			_ringBufferList = ringBufferList;
			_readPos = _ringBufferList._read - 1;
		}

		public bool MoveNext()
		{
			_readPos = _ringBufferList.NextIndex(_readPos);
			return _readPos != _ringBufferList._write;
		}

		public void Reset()
		{
			this = new Enumerator(_ringBufferList);
		}

		void IDisposable.Dispose()
		{
		}
	}

	private T[] _items;

	private int _read;

	private int _write;

	public int Capacity => _items.Length;

	private bool IsFull
	{
		get
		{
			if (_items.Length != 0)
			{
				return NextIndex(_write) == _read;
			}
			return true;
		}
	}

	public int Count
	{
		get
		{
			int num = _write - _read;
			if (num >= 0)
			{
				return num;
			}
			return num + _items.Length;
		}
	}

	public bool IsReadOnly => false;

	public T this[int index]
	{
		get
		{
			return GetSlot(index);
		}
		set
		{
			GetSlot(index) = value;
		}
	}

	public RingBufferList(int capacity)
	{
		_items = new T[capacity];
	}

	public RingBufferList()
	{
		_items = Array.Empty<T>();
	}

	public void Add(T item)
	{
		if (IsFull)
		{
			Expand();
		}
		_items[_write] = item;
		_write = NextIndex(_write);
	}

	public void Clear()
	{
		_read = 0;
		_write = 0;
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			Array.Clear(_items);
		}
	}

	public bool Contains(T item)
	{
		return IndexOf(item) >= 0;
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		ArgumentNullException.ThrowIfNull(array, "array");
		ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex, "arrayIndex");
		CopyTo(array.AsSpan(arrayIndex));
	}

	private void CopyTo(Span<T> dest)
	{
		if (dest.Length < Count)
		{
			throw new ArgumentException("Not enough elements in destination!");
		}
		int num = 0;
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			dest[num++] = current;
		}
	}

	public bool Remove(T item)
	{
		int num = IndexOf(item);
		if (num < 0)
		{
			return false;
		}
		RemoveAt(num);
		return true;
	}

	public int IndexOf(T item)
	{
		int num = 0;
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				T current = enumerator.Current;
				if (EqualityComparer<T>.Default.Equals(item, current))
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	public void Insert(int index, T item)
	{
		throw new NotSupportedException();
	}

	public void RemoveAt(int index)
	{
		int count = Count;
		ArgumentOutOfRangeException.ThrowIfNegative(index, "index");
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, count, "index");
		if (index == 0)
		{
			if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			{
				_items[_read] = default(T);
			}
			_read = NextIndex(_read);
			return;
		}
		if (index == count - 1)
		{
			_write = WrapInv(_write - 1);
			if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			{
				_items[_write] = default(T);
			}
			return;
		}
		int num = RealIndex(index);
		_ = ref _items[num];
		Span<T> span;
		if (num < _read)
		{
			span = _items.AsSpan();
			int num2 = num;
			ShiftDown(span.Slice(num2, _write - num2), default(T));
		}
		else if (_write < _read)
		{
			T substitution = ShiftDown(_items.AsSpan(0, _write), default(T));
			ShiftDown(_items.AsSpan(num), substitution);
		}
		else
		{
			span = _items.AsSpan();
			int num2 = num;
			ShiftDown(span.Slice(num2, _write - num2), default(T));
		}
		_write = WrapInv(_write - 1);
	}

	private static T ShiftDown(Span<T> span, T substitution)
	{
		if (span.Length == 0)
		{
			return substitution;
		}
		T result = span[0];
		Span<T> span2 = span.Slice(1, span.Length - 1);
		span2.CopyTo(span.Slice(0, span.Length - 1));
		span[span.Length - 1] = substitution;
		return result;
	}

	private ref T GetSlot(int index)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index, "index");
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count, "index");
		return ref _items[RealIndex(index)];
	}

	private int RealIndex(int index)
	{
		return Wrap(index + _read);
	}

	private int NextIndex(int index)
	{
		return Wrap(index + 1);
	}

	private int Wrap(int index)
	{
		if (index >= _items.Length)
		{
			index -= _items.Length;
		}
		return index;
	}

	private int WrapInv(int index)
	{
		if (index < 0)
		{
			index = _items.Length - 1;
		}
		return index;
	}

	private void Expand()
	{
		int num = _items.Length;
		int newSize = Math.Max(4, num * 2);
		Array.Resize(ref _items, newSize);
		if (_write < _read)
		{
			Span<T> span = _items.AsSpan(0, _write);
			Span<T> destination = _items.AsSpan(num);
			span.CopyTo(destination);
			if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			{
				span.Clear();
			}
			_write += num;
		}
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
