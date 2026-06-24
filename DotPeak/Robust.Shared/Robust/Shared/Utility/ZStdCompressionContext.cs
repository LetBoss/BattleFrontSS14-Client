// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.ZStdCompressionContext
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using SharpZstd.Interop;
using System;

#nullable disable
namespace Robust.Shared.Utility;

public sealed class ZStdCompressionContext : IDisposable
{
  public unsafe ZSTD_CCtx* Context { get; private set; }

  private unsafe bool Disposed => (IntPtr) this.Context == IntPtr.Zero;

  public unsafe ZStdCompressionContext() => this.Context = Zstd.ZSTD_createCCtx();

  public unsafe void SetParameter(ZSTD_cParameter parameter, int value)
  {
    this.CheckDisposed();
    IntPtr num = (IntPtr) Zstd.ZSTD_CCtx_setParameter(this.Context, parameter, value);
  }

  public unsafe int Compress(
    Span<byte> destination,
    ReadOnlySpan<byte> source,
    int compressionLevel = 3)
  {
    this.CheckDisposed();
    fixed (byte* numPtr1 = &destination.GetPinnableReference())
      fixed (byte* numPtr2 = &source.GetPinnableReference())
      {
        UIntPtr code = Zstd.ZSTD_compressCCtx(this.Context, (void*) numPtr1, (UIntPtr) destination.Length, (void*) numPtr2, (UIntPtr) source.Length, compressionLevel);
        ZStdException.ThrowIfError(code);
        return (int) code;
      }
  }

  public unsafe int Compress2(Span<byte> destination, ReadOnlySpan<byte> source)
  {
    this.CheckDisposed();
    fixed (byte* numPtr1 = &destination.GetPinnableReference())
      fixed (byte* numPtr2 = &source.GetPinnableReference())
      {
        UIntPtr code = Zstd.ZSTD_compress2(this.Context, (void*) numPtr1, (UIntPtr) destination.Length, (void*) numPtr2, (UIntPtr) source.Length);
        ZStdException.ThrowIfError(code);
        return (int) code;
      }
  }

  ~ZStdCompressionContext() => this.Dispose();

  public unsafe void Dispose()
  {
    if (this.Disposed)
      return;
    IntPtr num = (IntPtr) Zstd.ZSTD_freeCCtx(this.Context);
    this.Context = (ZSTD_CCtx*) null;
    GC.SuppressFinalize((object) this);
  }

  private void CheckDisposed()
  {
    if (this.Disposed)
      throw new ObjectDisposedException(nameof (ZStdCompressionContext));
  }
}
