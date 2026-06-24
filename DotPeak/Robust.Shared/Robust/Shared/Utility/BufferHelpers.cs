// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.BufferHelpers
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Buffers;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Utility;

public static class BufferHelpers
{
  public static void EnsureBuffer<T>(ref T[] buf, int minimumLength)
  {
    if (buf.Length >= minimumLength)
      return;
    buf = new T[BufferHelpers.FittingPowerOfTwo(minimumLength)];
  }

  public static void EnsurePooledBuffer<T>(ref T[] buf, ArrayPool<T> pool, int minimumLength)
  {
    if (buf.Length >= minimumLength)
      return;
    pool.Return(buf);
    buf = pool.Rent(minimumLength);
  }

  public static int FittingPowerOfTwo(int size) => 2 << BitOperations.Log2((uint) (size - 1));
}
