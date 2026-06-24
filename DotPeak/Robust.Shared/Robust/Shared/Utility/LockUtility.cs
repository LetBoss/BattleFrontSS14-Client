// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.LockUtility
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Utility;

public static class LockUtility
{
  public static LockUtility.RWReadGuard ReadGuard(this ReaderWriterLockSlim rwLock)
  {
    rwLock.EnterReadLock();
    return new LockUtility.RWReadGuard(rwLock);
  }

  public static LockUtility.RWWriteGuard WriteGuard(this ReaderWriterLockSlim rwLock)
  {
    rwLock.EnterWriteLock();
    return new LockUtility.RWWriteGuard(rwLock);
  }

  public static LockUtility.SemaphoreGuard WaitGuard(this SemaphoreSlim semaphore)
  {
    semaphore.Wait();
    return new LockUtility.SemaphoreGuard(semaphore);
  }

  public static async ValueTask<LockUtility.SemaphoreGuard> WaitGuardAsync(
    this SemaphoreSlim semaphore)
  {
    await semaphore.WaitAsync();
    return new LockUtility.SemaphoreGuard(semaphore);
  }

  public struct RWReadGuard(ReaderWriterLockSlim rwLock) : IDisposable
  {
    public readonly ReaderWriterLockSlim RwLock = rwLock;

    public bool Disposed { get; private set; } = false;

    public void Dispose()
    {
      this.Disposed = !this.Disposed ? true : throw new InvalidOperationException("Double dispose of RWReadGuard");
      this.RwLock.ExitReadLock();
    }
  }

  public struct RWWriteGuard(ReaderWriterLockSlim rwLock) : IDisposable
  {
    public readonly ReaderWriterLockSlim RwLock = rwLock;

    public bool Disposed { get; private set; } = false;

    public void Dispose()
    {
      this.Disposed = !this.Disposed ? true : throw new InvalidOperationException("Double dispose of RWWriteGuard");
      this.RwLock.ExitWriteLock();
    }
  }

  public struct SemaphoreGuard(SemaphoreSlim semaphore) : IDisposable
  {
    public readonly SemaphoreSlim Semaphore = semaphore;

    public bool Disposed { get; private set; } = false;

    public void Dispose()
    {
      this.Disposed = !this.Disposed ? true : throw new InvalidOperationException("Double dispose of SemaphoreGuard");
      this.Semaphore.Release();
    }
  }
}
