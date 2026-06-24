using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Robust.Shared.Maths;
using TerraFX.Interop.Windows;

namespace Robust.Shared.Utility;

internal sealed class ResizableMemoryRegion<T> : IDisposable where T : unmanaged
{
	private static readonly KeyValuePair<string, object?>[] MetricTags;

	private static long _memoryUsed;

	public unsafe T* BaseAddress { get; private set; }

	public int MaxSize { get; }

	public int CurrentSize { get; private set; }

	static ResizableMemoryRegion()
	{
		MetricTags = new KeyValuePair<string, object>[1]
		{
			new KeyValuePair<string, object>("type", typeof(T).FullName)
		};
		ResizableMemoryRegionMetrics.Meter.CreateObservableUpDownCounter("used_bytes", () => new Measurement<long>(_memoryUsed, MetricTags), "bytes", "The amount of committed memory used by ResizableMemoryRegion<T> instances.");
	}

	public unsafe ResizableMemoryRegion(int maxElementSize, int initialElementSize = 0)
	{
		if (BaseAddress != null)
		{
			throw new InvalidOperationException("Memory region is already initialized!");
		}
		if (initialElementSize > maxElementSize)
		{
			throw new ArgumentException("initialSize must be smaller than maxSize");
		}
		if (maxElementSize == 0)
		{
			throw new ArgumentException("Cannot allocate a 0-byte memory region!");
		}
		nuint num = checked((nuint)sizeof(T) * (nuint)maxElementSize);
		if (OperatingSystem.IsWindows())
		{
			BaseAddress = (T*)Windows.VirtualAlloc((void*)null, (UIntPtr)num, 8192u, 1u);
			if (BaseAddress == null)
			{
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
		}
		else
		{
			BaseAddress = (T*)NativeMemory.AllocZeroed(num);
		}
		MaxSize = maxElementSize;
		Expand(initialElementSize);
	}

	public unsafe void Expand(int newElementSize)
	{
		ThrowIfDisposed();
		if (newElementSize > MaxSize)
		{
			throw new ArgumentException("Cannot expand memory region past max size.", "newElementSize");
		}
		if (newElementSize > CurrentSize)
		{
			int currentSize = CurrentSize;
			nuint num = (nuint)sizeof(T) * (nuint)newElementSize;
			if (OperatingSystem.IsWindows() && Windows.VirtualAlloc((void*)BaseAddress, (UIntPtr)num, 4096u, 4u) == null)
			{
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
			CurrentSize = newElementSize;
			Interlocked.Add(ref _memoryUsed, (newElementSize - currentSize) * sizeof(T));
		}
	}

	public unsafe void Shrink(int newElementSize)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		ThrowIfDisposed();
		if (newElementSize > CurrentSize)
		{
			throw new ArgumentException("Cannot shrink to a larger size!", "newElementSize");
		}
		if (newElementSize < 0)
		{
			throw new ArgumentException("Cannot shrink to a negative size!", "newElementSize");
		}
		nuint num = (nuint)sizeof(T) * (nuint)CurrentSize;
		nuint num2 = (nuint)sizeof(T) * (nuint)newElementSize;
		nuint num3 = MathHelper.CeilMultipleOfPowerOfTwo<nuint>(num2, (nuint)Environment.SystemPageSize);
		if (OperatingSystem.IsWindows())
		{
			byte* num4 = (byte*)BaseAddress + num3;
			nuint num5 = num - num3;
			if (!BOOL.op_Implicit(Windows.VirtualFree((void*)num4, (UIntPtr)num5, 16384u)))
			{
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
		}
		CurrentSize = newElementSize;
		Interlocked.Add(ref _memoryUsed, (long)(num2 - num));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Span<T> GetSpan()
	{
		return new Span<T>(BaseAddress, CurrentSize);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<TCast> GetSpan<TCast>() where TCast : unmanaged
	{
		return MemoryMarshal.Cast<T, TCast>(GetSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe ref T GetRef(int index)
	{
		if (index >= CurrentSize || index < 0)
		{
			ThrowIndexOutOfRangeException(CurrentSize, index);
		}
		return ref BaseAddress[index];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref TCast GetRef<TCast>(int index) where TCast : unmanaged
	{
		return ref Unsafe.As<T, TCast>(ref GetRef(index));
	}

	public void Clear()
	{
		GetSpan().Clear();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe void ThrowIfDisposed()
	{
		if (BaseAddress == null)
		{
			ThrowNotInitialized();
		}
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
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (BaseAddress == null)
		{
			return;
		}
		if (OperatingSystem.IsWindows())
		{
			if (!BOOL.op_Implicit(Windows.VirtualFree((void*)BaseAddress, (UIntPtr)(nuint)0u, 32768u)))
			{
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
		}
		else
		{
			NativeMemory.Free(BaseAddress);
		}
		Interlocked.Add(ref _memoryUsed, -CurrentSize * sizeof(T));
		BaseAddress = null;
		CurrentSize = 0;
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~ResizableMemoryRegion()
	{
		ReleaseUnmanagedResources();
	}
}
