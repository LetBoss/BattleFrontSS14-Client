// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.ZStdCompressStream
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using SharpZstd.Interop;
using System;
using System.Buffers;
using System.IO;

#nullable enable
namespace Robust.Shared.Utility;

public sealed class ZStdCompressStream : Stream
{
  private readonly Stream _baseStream;
  private readonly bool _ownStream;
  private readonly byte[] _buffer;
  private int _bufferPos;
  private bool _disposed;
  private bool _hasSession;

  public ZStdCompressionContext Context { get; }

  public ZStdCompressStream(Stream baseStream, bool ownStream = true)
  {
    this.Context = new ZStdCompressionContext();
    this._baseStream = baseStream;
    this._ownStream = ownStream;
    this._buffer = ArrayPool<byte>.Shared.Rent((int) Zstd.ZSTD_CStreamOutSize());
  }

  public override void Flush() => this.FlushInternal((ZSTD_EndDirective) 1);

  public void FlushEnd()
  {
    this._hasSession = false;
    this.FlushInternal((ZSTD_EndDirective) 2);
  }

  private unsafe void FlushInternal(ZSTD_EndDirective directive)
  {
    fixed (byte* numPtr = this._buffer)
    {
      ZSTD_outBuffer zstdOutBuffer = new ZSTD_outBuffer();
      zstdOutBuffer.size = (UIntPtr) this._buffer.Length;
      zstdOutBuffer.pos = (UIntPtr) this._bufferPos;
      zstdOutBuffer.dst = (void*) numPtr;
      ZSTD_inBuffer zstdInBuffer = new ZSTD_inBuffer();
      UIntPtr code;
      do
      {
        code = Zstd.ZSTD_compressStream2(this.Context.Context, &zstdOutBuffer, &zstdInBuffer, directive);
        ZStdException.ThrowIfError(code);
        this._bufferPos = (int) zstdOutBuffer.pos;
        this._baseStream.Write((ReadOnlySpan<byte>) this._buffer.AsSpan<byte>(0, (int) zstdOutBuffer.pos));
        this._bufferPos = 0;
        zstdOutBuffer.pos = UIntPtr.Zero;
      }
      while ((IntPtr) code != IntPtr.Zero);
    }
    this._baseStream.Flush();
  }

  public override int Read(byte[] buffer, int offset, int count)
  {
    throw new NotSupportedException();
  }

  public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

  public override void SetLength(long value) => throw new NotSupportedException();

  public override void Write(byte[] buffer, int offset, int count)
  {
    this.Write((ReadOnlySpan<byte>) buffer.AsSpan<byte>(offset, count));
  }

  public override unsafe void Write(ReadOnlySpan<byte> buffer)
  {
    this.ThrowIfDisposed();
    this._hasSession = true;
    fixed (byte* numPtr1 = this._buffer)
      fixed (byte* numPtr2 = &buffer.GetPinnableReference())
      {
        ZSTD_outBuffer zstdOutBuffer = new ZSTD_outBuffer();
        zstdOutBuffer.size = (UIntPtr) this._buffer.Length;
        zstdOutBuffer.pos = (UIntPtr) this._bufferPos;
        zstdOutBuffer.dst = (void*) numPtr1;
        ZSTD_inBuffer zstdInBuffer = new ZSTD_inBuffer();
        zstdInBuffer.pos = UIntPtr.Zero;
        zstdInBuffer.size = (UIntPtr) buffer.Length;
        zstdInBuffer.src = (void*) numPtr2;
        while (true)
        {
          ZStdException.ThrowIfError(Zstd.ZSTD_compressStream2(this.Context.Context, &zstdOutBuffer, &zstdInBuffer, (ZSTD_EndDirective) 0));
          this._bufferPos = (int) zstdOutBuffer.pos;
          if (zstdInBuffer.pos < zstdInBuffer.size)
          {
            this._baseStream.Write((ReadOnlySpan<byte>) this._buffer.AsSpan<byte>(0, (int) zstdOutBuffer.pos));
            this._bufferPos = 0;
            zstdOutBuffer.pos = UIntPtr.Zero;
          }
          else
            break;
        }
      }
  }

  public override bool CanRead => false;

  public override bool CanSeek => false;

  public override bool CanWrite => true;

  public override long Length => throw new NotSupportedException();

  public override long Position
  {
    get => throw new NotSupportedException();
    set => throw new NotSupportedException();
  }

  protected override void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (this._disposed)
      return;
    if (disposing)
    {
      if (this._hasSession)
        this.FlushEnd();
      if (this._ownStream)
        this._baseStream.Dispose();
      ArrayPool<byte>.Shared.Return(this._buffer);
      this.Context.Dispose();
    }
    this._disposed = true;
  }

  private void ThrowIfDisposed()
  {
    if (this._disposed)
      throw new ObjectDisposedException(nameof (ZStdCompressStream));
  }
}
