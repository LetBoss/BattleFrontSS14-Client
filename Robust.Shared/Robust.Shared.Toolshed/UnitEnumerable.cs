using System;
using System.Collections;
using System.Collections.Generic;

namespace Robust.Shared.Toolshed;

internal sealed record UnitEnumerable<T>(T Value) : IEnumerable<T>, IEnumerable
{
	internal record struct UnitEnumerator(T Value) : IEnumerator<T>, IEnumerator, IDisposable
	{
		public T Current => Value;

		object IEnumerator.Current => Current;

		private bool _taken;

		public bool MoveNext()
		{
			if (_taken)
			{
				return false;
			}
			_taken = true;
			return true;
		}

		public void Reset()
		{
			_taken = false;
		}

		public void Dispose()
		{
		}
	}

	public IEnumerator<T> GetEnumerator()
	{
		return new UnitEnumerator(Value);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
