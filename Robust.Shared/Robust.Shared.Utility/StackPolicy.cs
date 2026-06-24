using System.Collections.Generic;
using Microsoft.Extensions.ObjectPool;

namespace Robust.Shared.Utility;

public sealed class StackPolicy<T> : PooledObjectPolicy<Stack<T>>
{
	public override Stack<T> Create()
	{
		return new Stack<T>();
	}

	public override bool Return(Stack<T> obj)
	{
		obj.Clear();
		return true;
	}
}
