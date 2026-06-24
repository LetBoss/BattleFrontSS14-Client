using System.Collections.Generic;
using Microsoft.Extensions.ObjectPool;

namespace Robust.Shared.Utility;

public sealed class DictPolicy<T1, T2> : PooledObjectPolicy<Dictionary<T1, T2>> where T1 : notnull
{
	public override Dictionary<T1, T2> Create()
	{
		return new Dictionary<T1, T2>();
	}

	public override bool Return(Dictionary<T1, T2> obj)
	{
		obj.Clear();
		return true;
	}
}
