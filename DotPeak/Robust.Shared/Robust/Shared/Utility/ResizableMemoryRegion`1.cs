// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.ResizableMemoryRegion`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

#nullable enable
namespace Robust.Shared.Utility;

internal sealed class ResizableMemoryRegion<T> : IDisposable where T : unmanaged
{
  private static readonly KeyValuePair<string, object?>[] MetricTags = new KeyValuePair<string, object>[1]
  {
    new KeyValuePair<string, object>("type", (object) typeof (T).FullName)
  };
  private static long _memoryUsed;

  static ResizableMemoryRegion()
  {
    ResizableMemoryRegionMetrics.Meter.CreateObservableUpDownCounter<long>("used_bytes", (Func<Measurement<long>>) (() => new Measurement<long>(ResizableMemoryRegion<T>._memoryUsed, ResizableMemoryRegion<T>.MetricTags)), "bytes", "The amount of committed memory used by ResizableMemoryRegion<T> instances.");
  }

  public unsafe T* BaseAddress { get; private set; }

  public int MaxSize { get; }

  public int CurrentSize { get; private set; }

  public unsafe ResizableMemoryRegion(int maxElementSize, int initialElementSize = 0)
  {
    if ((IntPtr) this.BaseAddress != IntPtr.Zero)
      throw new InvalidOperationException("Memory region is already initialized!");
    if (initialElementSize > maxElementSize)
      throw new ArgumentException("initialSize must be smaller than maxSize");
    if (maxElementSize == 0)
      throw new ArgumentException("Cannot allocate a 0-byte memory region!");
    UIntPtr num = checked ((UIntPtr) (uint) sizeof (T) * (UIntPtr) (uint) maxElementSize);
    if (OperatingSystem.IsWindows())
    {
      this.BaseAddress = (T*) TerraFX.Interop.Windows.Windows.VirtualAlloc((void*) null, num, 8192U /*0x2000*/, 1U);
      if ((IntPtr) this.BaseAddress == IntPtr.Zero)
        Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
    }
    else
      this.BaseAddress = (T*) NativeMemory.AllocZeroed(num);
    this.MaxSize = maxElementSize;
    this.Expand(initialElementSize);
  }

  public unsafe void Expand(int newElementSize)
  {
    this.ThrowIfDisposed();
    if (newElementSize > this.MaxSize)
      throw new ArgumentException("Cannot expand memory region past max size.", nameof (newElementSize));
    if (newElementSize <= this.CurrentSize)
      return;
    int currentSize = this.CurrentSize;
    UIntPtr dwSize = (UIntPtr) sizeof (T) * (UIntPtr) newElementSize;
    if (OperatingSystem.IsWindows() && (IntPtr) TerraFX.Interop.Windows.Windows.VirtualAlloc((void*) this.BaseAddress, dwSize, 4096U /*0x1000*/, 4U) == IntPtr.Zero)
      Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
    this.CurrentSize = newElementSize;
    Interlocked.Add(ref ResizableMemoryRegion<T>._memoryUsed, (long) ((newElementSize - currentSize) * sizeof (T)));
  }

  public unsafe void Shrink(int newElementSize)
  {
    this.ThrowIfDisposed();
    if (newElementSize > this.CurrentSize)
      throw new ArgumentException("Cannot shrink to a larger size!", nameof (newElementSize));
    if (newElementSize < 0)
      throw new ArgumentException("Cannot shrink to a negative size!", nameof (newElementSize));
    UIntPtr num1 = (UIntPtr) sizeof (T) * (UIntPtr) this.CurrentSize;
    UIntPtr num2 = (UIntPtr) sizeof (T) * (UIntPtr) newElementSize;
    UIntPtr num3 = MathHelper.CeilMultipleOfPowerOfTwo<UIntPtr>(num2, (UIntPtr) Environment.SystemPageSize);
    if (OperatingSystem.IsWindows() && !(bool) TerraFX.Interop.Windows.Windows.VirtualFree((void*) ((IntPtr) this.BaseAddress + (IntPtr) (ulong) num3), num1 - num3, 16384U /*0x4000*/))
      Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
    this.CurrentSize = newElementSize;
    Interlocked.Add(ref ResizableMemoryRegion<T>._memoryUsed, (long) (ulong) (num2 - num1));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public unsafe Span<T> GetSpan() => new Span<T>((void*) this.BaseAddress, this.CurrentSize);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Span<TCast> GetSpan<TCast>() where TCast : unmanaged
  {
    return MemoryMarshal.Cast<T, TCast>(this.GetSpan());
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public unsafe ref T GetRef(int index)
  {
    if (index >= this.CurrentSize || index < 0)
      ResizableMemoryRegion<T>.ThrowIndexOutOfRangeException(this.CurrentSize, index);
    // ISSUE: explicit reference operation
    return @this.BaseAddress[index];
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ref TCast GetRef<TCast>(int index) where TCast : unmanaged
  {
    return ref Unsafe.As<T, TCast>(ref this.GetRef(index));
  }

  public void Clear() => this.GetSpan().Clear();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private unsafe void ThrowIfDisposed()
  {
    if ((IntPtr) this.BaseAddress != IntPtr.Zero)
      return;
    ResizableMemoryRegion<T>.ThrowNotInitialized();
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private static void ThrowNotInitialized()
  {
    throw new InvalidOperationException("Memory region is not initialized!");
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private static void ThrowIndexOutOfRangeException(int size, int index)
  {
    throw new IndexOutOfRangeException($"Index was outside the bounds of the memory region. Size: {size}, Index: {index}");
  }

  private unsafe void ReleaseUnmanagedResources()
  {
    if ((IntPtr) this.BaseAddress == IntPtr.Zero)
      return;
    if (OperatingSystem.IsWindows())
    {
      if (!(bool) TerraFX.Interop.Windows.Windows.VirtualFree((void*) this.BaseAddress, UIntPtr.Zero, 32768U /*0x8000*/))
        Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
    }
    else
      NativeMemory.Free((void*) this.BaseAddress);
    Interlocked.Add(ref ResizableMemoryRegion<T>._memoryUsed, (long) (-this.CurrentSize * sizeof (T)));
    this.BaseAddress = (T*) null;
    this.CurrentSize = 0;
  }

  public void Dispose()
  {
    this.ReleaseUnmanagedResources();
    GC.SuppressFinalize((object) this);
  }

  ~ResizableMemoryRegion() => this.ReleaseUnmanagedResources();
}
