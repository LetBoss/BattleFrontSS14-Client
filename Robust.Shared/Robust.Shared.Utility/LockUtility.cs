using System;
using System.Threading;
using System.Threading.Tasks;

namespace Robust.Shared.Utility;

public static class LockUtility
{
	public struct RWReadGuard : IDisposable
	{
		public readonly ReaderWriterLockSlim RwLock;

		public bool Disposed { get; private set; }

		public RWReadGuard(ReaderWriterLockSlim rwLock)
		{
			RwLock = rwLock;
			Disposed = false;
		}

		public void Dispose()
		{
			if (Disposed)
			{
				throw new InvalidOperationException("Double dispose of RWReadGuard");
			}
			Disposed = true;
			RwLock.ExitReadLock();
		}
	}

	public struct RWWriteGuard : IDisposable
	{
		public readonly ReaderWriterLockSlim RwLock;

		public bool Disposed { get; private set; }

		public RWWriteGuard(ReaderWriterLockSlim rwLock)
		{
			RwLock = rwLock;
			Disposed = false;
		}

		public void Dispose()
		{
			if (Disposed)
			{
				throw new InvalidOperationException("Double dispose of RWWriteGuard");
			}
			Disposed = true;
			RwLock.ExitWriteLock();
		}
	}

	public struct SemaphoreGuard : IDisposable
	{
		public readonly SemaphoreSlim Semaphore;

		public bool Disposed { get; private set; }

		public SemaphoreGuard(SemaphoreSlim semaphore)
		{
			Semaphore = semaphore;
			Disposed = false;
		}

		public void Dispose()
		{
			if (Disposed)
			{
				throw new InvalidOperationException("Double dispose of SemaphoreGuard");
			}
			Disposed = true;
			Semaphore.Release();
		}
	}

	public static RWReadGuard ReadGuard(this ReaderWriterLockSlim rwLock)
	{
		rwLock.EnterReadLock();
		return new RWReadGuard(rwLock);
	}

	public static RWWriteGuard WriteGuard(this ReaderWriterLockSlim rwLock)
	{
		rwLock.EnterWriteLock();
		return new RWWriteGuard(rwLock);
	}

	public static SemaphoreGuard WaitGuard(this SemaphoreSlim semaphore)
	{
		semaphore.Wait();
		return new SemaphoreGuard(semaphore);
	}

	public static async ValueTask<SemaphoreGuard> WaitGuardAsync(this SemaphoreSlim semaphore)
	{
		await semaphore.WaitAsync();
		return new SemaphoreGuard(semaphore);
	}
}
