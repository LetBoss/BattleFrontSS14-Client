using System;
using System.Collections.Generic;

namespace Robust.Shared.GameObjects;

public struct TransformChildrenEnumerator(HashSet<EntityUid>.Enumerator children) : IDisposable
{
	private HashSet<EntityUid>.Enumerator _children = children;

	public bool MoveNext(out EntityUid child)
	{
		if (!_children.MoveNext())
		{
			child = default(EntityUid);
			return false;
		}
		child = _children.Current;
		return true;
	}

	public void Dispose()
	{
		_children.Dispose();
	}
}
