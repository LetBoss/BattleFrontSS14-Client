using System;
using System.Collections;
using System.Collections.Generic;

namespace Robust.Shared.Utility;

public sealed class PriorityQueue<T> : ICollection<T>, IEnumerable<T>, IEnumerable
{
	private readonly IComparer<T> _comparer;

	private T[] _heap;

	private const int DEFAULT_CAPACITY = 10;

	private const int SHRINK_RATIO = 4;

	private const int RESIZE_FACTOR = 2;

	private int _shrinkBound;

	private static readonly InvalidOperationException EmptyCollectionException = new InvalidOperationException("Collection is empty.");

	public int Capacity => _heap.Length;

	public int Count { get; private set; }

	public bool IsReadOnly => false;

	public PriorityQueue(IComparer<T>? comparer = null)
		: this(10, comparer)
	{
	}

	public PriorityQueue(int capacity, IComparer<T>? comparer = null)
	{
		if (capacity <= 0)
		{
			throw new ArgumentOutOfRangeException("capacity", "Expected capacity greater than zero.");
		}
		if (comparer == null && !typeof(IComparable).IsAssignableFrom(typeof(T)) && !typeof(IComparable<T>).IsAssignableFrom(typeof(T)))
		{
			throw new ArgumentException("Expected a comparer for types, which do not implement IComparable.", "comparer");
		}
		_comparer = comparer ?? Comparer<T>.Default;
		_shrinkBound = capacity / 4;
		_heap = new T[capacity];
	}

	public IEnumerator<T> GetEnumerator()
	{
		T[] array = new T[Count];
		CopyTo(array, 0);
		return ((IEnumerable<T>)array).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void Add(T item)
	{
		if (Count == Capacity)
		{
			GrowCapacity();
		}
		_heap[Count++] = item;
		_heap.Sift(Count, _comparer, -1);
	}

	public T Take()
	{
		if (Count == 0)
		{
			throw EmptyCollectionException;
		}
		T result = _heap[0];
		Count--;
		_heap.Swap(0, Count);
		_heap[Count] = default(T);
		_heap.Sink(1, Count, _comparer, -1);
		if (Count <= _shrinkBound && Count > 10)
		{
			ShrinkCapacity();
		}
		return result;
	}

	public T Peek()
	{
		if (Count == 0)
		{
			throw EmptyCollectionException;
		}
		return _heap[0];
	}

	public void Clear()
	{
		_heap = new T[10];
		Count = 0;
	}

	public bool Contains(T item)
	{
		return GetItemIndex(item) >= 0;
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex");
		}
		if (array.Length - arrayIndex < Count)
		{
			throw new ArgumentException("Insufficient space in destination array.");
		}
		Array.Copy(_heap, 0, array, arrayIndex, Count);
		array.HeapSort(arrayIndex, Count, _comparer);
	}

	public bool Remove(T item)
	{
		int itemIndex = GetItemIndex(item);
		switch (itemIndex)
		{
		case -1:
			return false;
		case 0:
			Take();
			break;
		default:
			RemoveAt(itemIndex + 1, -1);
			break;
		}
		return true;
	}

	private void RemoveAt(int index, int shift)
	{
		int num = index + shift;
		Count--;
		_heap.Swap(num, Count);
		_heap[Count] = default(T);
		int num2 = index / 2 + shift;
		if (_comparer.GreaterOrEqual(_heap[num], _heap[num2]))
		{
			_heap.Sift(index, _comparer, shift);
		}
		else
		{
			_heap.Sink(index, Count, _comparer, shift);
		}
	}

	private int GetItemIndex(T item)
	{
		for (int i = 0; i < Count; i++)
		{
			if (_comparer.Compare(_heap[i], item) == 0)
			{
				return i;
			}
		}
		return -1;
	}

	private void GrowCapacity()
	{
		int num = Capacity * 2;
		Array.Resize(ref _heap, num);
		_shrinkBound = num / 4;
	}

	private void ShrinkCapacity()
	{
		int num = Capacity / 2;
		Array.Resize(ref _heap, num);
		_shrinkBound = num / 4;
	}
}
