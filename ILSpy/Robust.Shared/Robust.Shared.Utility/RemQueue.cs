using System;
using System.Collections;
using System.Collections.Generic;

namespace Robust.Shared.Utility;

public struct RemQueue<T>
{
	public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
	{
		private readonly bool _filled;

		private List<T>.Enumerator _enumerator;

		public T Current => _enumerator.Current;

		object? IEnumerator.Current => Current;

		public Enumerator(List<T>? list)
		{
			if (list == null)
			{
				_filled = false;
				_enumerator = default(List<T>.Enumerator);
			}
			else
			{
				_filled = true;
				_enumerator = list.GetEnumerator();
			}
		}

		public bool MoveNext()
		{
			if (!_filled)
			{
				return false;
			}
			return _enumerator.MoveNext();
		}

		void IEnumerator.Reset()
		{
			if (_filled)
			{
				((IEnumerator)_enumerator).Reset();
			}
		}

		void IDisposable.Dispose()
		{
		}
	}

	public List<T>? List;

	public int Count => List?.Count ?? 0;

	public void Add(T t)
	{
		if (List == null)
		{
			List = new List<T>();
		}
		List.Add(t);
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(List);
	}

	public void Clear()
	{
		List?.Clear();
	}
}
