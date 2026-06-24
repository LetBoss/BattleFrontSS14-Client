using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Robust.Shared.Utility;

public sealed class RefList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	public struct Enumerator(RefList<T> owner) : IEnumerator<T>, IEnumerator, IDisposable
	{
		private readonly RefList<T> _owner = owner;

		private int _position = -1;

		public ref T Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return ref _owner._array[_position];
			}
		}

		T IEnumerator<T>.Current => Current;

		object? IEnumerator.Current => Current;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			return ++_position < _owner._size;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			_position = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
		}
	}

	private T[] _array;

	private int _size;

	public int Count => _size;

	public bool IsReadOnly => false;

	public int Capacity => _array.Length;

	public ref T this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return ref _array[index];
		}
	}

	T IList<T>.this[int index]
	{
		get
		{
			return _array[index];
		}
		set
		{
			_array[index] = value;
		}
	}

	public RefList()
		: this(1)
	{
	}

	public RefList(int initialCapacity)
	{
		_array = new T[initialCapacity];
		_size = 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

	public ref T AllocAdd()
	{
		_ensureCapacity(_size + 1);
		return ref _array[_size++];
	}

	public void Add(T item)
	{
		_ensureCapacity(_size + 1);
		_array[_size++] = item;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear()
	{
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			Array.Clear(_array, 0, _size);
		}
		_size = 0;
	}

	public bool Contains(T item)
	{
		return IndexOf(item) != -1;
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		Array.Copy(_array, 0, array, arrayIndex, _size);
	}

	public bool Remove(T item)
	{
		int num = IndexOf(item);
		if (num == -1)
		{
			return false;
		}
		RemoveAt(num);
		return true;
	}

	public int IndexOf(T item)
	{
		return Array.IndexOf(_array, item, 0, _size);
	}

	public void Insert(int index, T item)
	{
		_ensureCapacity(_size + 1);
		if (index < _size)
		{
			Array.Copy(_array, index, _array, index + 1, _size - index);
		}
		_array[index] = item;
		_size++;
	}

	public void TrimCapacity(int capacity)
	{
		if (Count > capacity)
		{
			throw new ArgumentException("Cannot trim past list contents");
		}
		T[] array = _array;
		_array = new T[capacity];
		array.AsSpan(0, _size).CopyTo(_array);
	}

	public void RemoveAt(int index)
	{
		if (index >= _size)
		{
			throw new ArgumentOutOfRangeException("index", index, "Index must fit into list.");
		}
		_size--;
		if (index < _size)
		{
			Array.Copy(_array, index + 1, _array, index, _size - index);
		}
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			_array[_size] = default(T);
		}
	}

	public void Sort(IComparer<T> comparer)
	{
		Array.Sort(_array, 0, _size, comparer);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<T> GetSpan()
	{
		return new Span<T>(_array, 0, _size);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void _ensureCapacity(int newCapacity)
	{
		if (newCapacity >= _array.Length)
		{
			T[] array = _array;
			_array = new T[array.Length * 2];
			Array.Copy(array, 0, _array, 0, _size);
		}
	}
}
