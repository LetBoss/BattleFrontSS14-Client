using System;
using SharpZstd.Interop;

namespace Robust.Shared.Utility;

public sealed class ZStdCompressionContext : IDisposable
{
	public unsafe ZSTD_CCtx* Context { get; private set; }

	private unsafe bool Disposed => Context == null;

	public unsafe ZStdCompressionContext()
	{
		Context = Zstd.ZSTD_createCCtx();
	}

	public unsafe void SetParameter(ZSTD_cParameter parameter, int value)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		CheckDisposed();
		Zstd.ZSTD_CCtx_setParameter(Context, parameter, value);
	}

	public unsafe int Compress(Span<byte> destination, ReadOnlySpan<byte> source, int compressionLevel = 3)
	{
		CheckDisposed();
		fixed (byte* ptr = destination)
		{
			fixed (byte* ptr2 = source)
			{
				UIntPtr num = Zstd.ZSTD_compressCCtx(Context, (void*)ptr, (UIntPtr)(nuint)destination.Length, (void*)ptr2, (UIntPtr)(nuint)source.Length, compressionLevel);
				ZStdException.ThrowIfError(num);
				return (int)(nuint)num;
			}
		}
	}

	public unsafe int Compress2(Span<byte> destination, ReadOnlySpan<byte> source)
	{
		CheckDisposed();
		fixed (byte* ptr = destination)
		{
			fixed (byte* ptr2 = source)
			{
				UIntPtr num = Zstd.ZSTD_compress2(Context, (void*)ptr, (UIntPtr)(nuint)destination.Length, (void*)ptr2, (UIntPtr)(nuint)source.Length);
				ZStdException.ThrowIfError(num);
				return (int)(nuint)num;
			}
		}
	}

	~ZStdCompressionContext()
	{
		Dispose();
	}

	public unsafe void Dispose()
	{
		if (!Disposed)
		{
			Zstd.ZSTD_freeCCtx(Context);
			Context = null;
			GC.SuppressFinalize(this);
		}
	}

	private void CheckDisposed()
	{
		if (Disposed)
		{
			throw new ObjectDisposedException("ZStdCompressionContext");
		}
	}
}
