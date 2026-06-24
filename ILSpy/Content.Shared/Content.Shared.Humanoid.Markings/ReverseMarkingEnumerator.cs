using System.Collections;
using System.Collections.Generic;

namespace Content.Shared.Humanoid.Markings;

public sealed class ReverseMarkingEnumerator : IEnumerable<Marking>, IEnumerable
{
	private List<Marking> _markings;

	public ReverseMarkingEnumerator(List<Marking> markings)
	{
		_markings = markings;
	}

	public IEnumerator<Marking> GetEnumerator()
	{
		return new MarkingsEnumerator(_markings, reverse: true);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
