// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.PoolHelpers
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Buffers;

#nullable enable
namespace Robust.Shared.Utility;

public static class PoolHelpers
{
  public static PoolHelpers.PoolReturnGuard<T> ReturnGuard<T>(this ArrayPool<T> pool, T[] buf)
  {
    return new PoolHelpers.PoolReturnGuard<T>(pool, buf);
  }

  public readonly struct PoolReturnGuard<T>(ArrayPool<T> pool, T[] array) : IDisposable
  {
    private readonly ArrayPool<T> _pool = pool;
    private readonly T[] _array = array;

    public void Dispose() => this._pool.Return(this._array);
  }
}
