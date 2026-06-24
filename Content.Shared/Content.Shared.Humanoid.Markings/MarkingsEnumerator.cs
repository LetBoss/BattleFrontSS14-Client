using System;
using System.Collections;
using System.Collections.Generic;

namespace Content.Shared.Humanoid.Markings;

public sealed class MarkingsEnumerator : IEnumerator<Marking>, IEnumerator, IDisposable
{
	private List<Marking> _markings;

	private bool _reverse;

	private int position;

	object IEnumerator.Current => _markings[position];

	public Marking Current => _markings[position];

	public MarkingsEnumerator(List<Marking> markings, bool reverse)
	{
		_markings = markings;
		_reverse = reverse;
		if (_reverse)
		{
			position = _markings.Count;
		}
		else
		{
			position = -1;
		}
	}

	public bool MoveNext()
	{
		if (_reverse)
		{
			position--;
			return position >= 0;
		}
		position++;
		return position < _markings.Count;
	}

	public void Reset()
	{
		if (_reverse)
		{
			position = _markings.Count;
		}
		else
		{
			position = -1;
		}
	}

	public void Dispose()
	{
	}
}
