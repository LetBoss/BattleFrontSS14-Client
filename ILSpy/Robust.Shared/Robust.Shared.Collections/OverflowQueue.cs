using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Robust.Shared.Collections;

public sealed class OverflowQueue<T>
{
	private readonly T[] _queue;

	private int _currentIdx;

	private int _length;

	public int Size => _queue.Length;

	public OverflowQueue(int size)
	{
		_queue = new T[size];
	}

	public void Enqueue(T item)
	{
		_queue[_currentIdx++] = item;
		if (_length < Size)
		{
			_length++;
		}
		if (_currentIdx == Size)
		{
			_currentIdx = 0;
		}
	}

	public T Dequeue()
	{
		if (!TryDequeue(out T item))
		{
			throw new InvalidOperationException("OverflowQueue has no more items to dequeue.");
		}
		return item;
	}

	public bool TryDequeue([NotNullWhen(true)] out T? item)
	{
		if (_length == 0)
		{
			item = default(T);
			return false;
		}
		item = _queue[GetCurrentIndex()];
		_length--;
		return true;
	}

	private int GetCurrentIndex()
	{
		int num = _currentIdx - _length;
		if (num < 0)
		{
			return num + Size;
		}
		return num;
	}

	public T Peek()
	{
		if (_length == 0)
		{
			throw new InvalidOperationException("OverflowQueue has no more items to dequeue.");
		}
		return _queue[GetCurrentIndex()];
	}

	public bool Contains(T item)
	{
		for (int i = 0; i < _length; i++)
		{
			int num = _currentIdx + i;
			if (num >= _length)
			{
				num -= _length;
			}
			if (EqualityComparer<T>.Default.Equals(item, _queue[num]))
			{
				return true;
			}
		}
		return false;
	}

	public T[] ToArray()
	{
		if (_length == 0)
		{
			return Array.Empty<T>();
		}
		T[] array = new T[_length];
		int num = _currentIdx - _length;
		if (num < 0)
		{
			Array.Copy(_queue, num + Size, array, 0, -1 * num);
			Array.Copy(_queue, 0, array, -1 * num, num + Size);
		}
		else
		{
			Array.Copy(_queue, num, array, 0, _length);
		}
		return array;
	}
}
