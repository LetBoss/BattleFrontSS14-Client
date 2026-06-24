using System.Collections.Generic;

namespace Content.Client.UserInterface.Controls;

public sealed class TabIdPool
{
	private readonly Queue<TabId> _freeIds = new Queue<TabId>();

	private int _nextId = 1;

	public TabId Take()
	{
		if (_freeIds.Count > 0)
		{
			return _freeIds.Dequeue();
		}
		return new TabId(_nextId++);
	}

	public void Free(TabId id)
	{
		if (!_freeIds.Contains(id))
		{
			_freeIds.Enqueue(id);
		}
	}
}
