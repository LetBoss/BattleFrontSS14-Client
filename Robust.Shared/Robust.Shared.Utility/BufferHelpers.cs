using System.Buffers;
using System.Numerics;

namespace Robust.Shared.Utility;

public static class BufferHelpers
{
	public static void EnsureBuffer<T>(ref T[] buf, int minimumLength)
	{
		if (buf.Length < minimumLength)
		{
			buf = new T[FittingPowerOfTwo(minimumLength)];
		}
	}

	public static void EnsurePooledBuffer<T>(ref T[] buf, ArrayPool<T> pool, int minimumLength)
	{
		if (buf.Length < minimumLength)
		{
			pool.Return(buf);
			buf = pool.Rent(minimumLength);
		}
	}

	public static int FittingPowerOfTwo(int size)
	{
		return 2 << BitOperations.Log2((uint)(size - 1));
	}
}
