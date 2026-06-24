using System.Collections.Generic;
using Microsoft.Extensions.ObjectPool;

namespace Robust.Shared.Utility;

public sealed class SetPolicy<T> : PooledObjectPolicy<HashSet<T>>
{
	public override HashSet<T> Create()
	{
		return new HashSet<T>();
	}

	public override bool Return(HashSet<T> obj)
	{
		obj.Clear();
		return true;
	}
}
