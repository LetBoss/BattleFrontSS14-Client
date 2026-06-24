using System;
using System.Buffers;

namespace Robust.Shared.Utility;

public static class PoolHelpers
{
	public readonly struct PoolReturnGuard<T>(ArrayPool<T> pool, T[] array) : IDisposable
	{
		private readonly ArrayPool<T> _pool = pool;

		private readonly T[] _array = array;

		public void Dispose()
		{
			_pool.Return(_array);
		}
	}

	public static PoolReturnGuard<T> ReturnGuard<T>(this ArrayPool<T> pool, T[] buf)
	{
		return new PoolReturnGuard<T>(pool, buf);
	}
}
