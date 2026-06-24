using System;

namespace Robust.Shared.Utility;

internal ref struct GrowableStack<T>(Span<T> stackSpace) where T : unmanaged
{
	private Span<T> _stack = stackSpace;

	private int _count = 0;

	private int _capacity = stackSpace.Length;

	internal ref T this[int index] => ref _stack[index];

	public void Push(in T element)
	{
		if (_count == _capacity)
		{
			_capacity *= 2;
			Span<T> stack = _stack;
			_stack = GC.AllocateUninitializedArray<T>(_capacity);
			stack.CopyTo(_stack);
		}
		_stack[_count] = element;
		_count++;
	}

	public T Pop()
	{
		_count--;
		return _stack[_count];
	}

	public int GetCount()
	{
		return _count;
	}
}
