using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Robust.Shared.Utility;

internal static class StringEnumerateHelpers
{
	internal struct SubstringRuneEnumerator(string source, int firstChar) : IEnumerable<Rune>, IEnumerable, IEnumerator<Rune>, IEnumerator, IDisposable
	{
		private readonly string _source = source;

		private int _nextChar = firstChar;

		private Rune _current = default(Rune);

		public readonly Rune Current => _current;

		object IEnumerator.Current => Current;

		public bool MoveNext()
		{
			if (_nextChar >= _source.Length)
			{
				return false;
			}
			if (!Rune.TryGetRuneAt(_source, _nextChar, out _current))
			{
				_current = Rune.ReplacementChar;
			}
			_nextChar += _current.Utf16SequenceLength;
			return true;
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}

		public void Dispose()
		{
		}

		public SubstringRuneEnumerator GetEnumerator()
		{
			return this;
		}

		IEnumerator<Rune> IEnumerable<Rune>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	internal struct SubstringReverseRuneEnumerator(string source, int startChar) : IEnumerator<Rune>, IEnumerator, IDisposable, IEnumerable<Rune>, IEnumerable
	{
		private string _source = source;

		private int _nextChar = startChar - 1;

		private Rune _current = default(Rune);

		public Rune Current => _current;

		object IEnumerator.Current => Current;

		public bool MoveNext()
		{
			if (_nextChar < 0)
			{
				return false;
			}
			char c = _source[_nextChar];
			if (!char.IsSurrogate(c))
			{
				_current = new Rune(c);
			}
			else if (char.IsLowSurrogate(c) && _nextChar >= 1)
			{
				char c2 = _source[_nextChar - 1];
				if (char.IsHighSurrogate(c2))
				{
					_current = new Rune(c2, c);
				}
				else
				{
					_current = Rune.ReplacementChar;
				}
			}
			else
			{
				_current = Rune.ReplacementChar;
			}
			_nextChar -= _current.Utf16SequenceLength;
			return true;
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}

		public void Dispose()
		{
		}

		public SubstringReverseRuneEnumerator GetEnumerator()
		{
			return this;
		}

		IEnumerator<Rune> IEnumerable<Rune>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
