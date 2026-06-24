using System.Collections;
using System.Collections.Generic;

namespace Content.Shared.Humanoid.Markings;

public sealed class ForwardMarkingEnumerator : IEnumerable<Marking>, IEnumerable
{
	private List<Marking> _markings;

	public ForwardMarkingEnumerator(List<Marking> markings)
	{
		_markings = markings;
	}

	public IEnumerator<Marking> GetEnumerator()
	{
		return new MarkingsEnumerator(_markings, reverse: false);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
