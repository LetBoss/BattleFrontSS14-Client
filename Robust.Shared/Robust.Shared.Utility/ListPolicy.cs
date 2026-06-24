using System.Collections.Generic;
using Microsoft.Extensions.ObjectPool;

namespace Robust.Shared.Utility;

public sealed class ListPolicy<T> : PooledObjectPolicy<List<T>>
{
	public override List<T> Create()
	{
		return new List<T>();
	}

	public override bool Return(List<T> obj)
	{
		obj.Clear();
		return true;
	}
}
