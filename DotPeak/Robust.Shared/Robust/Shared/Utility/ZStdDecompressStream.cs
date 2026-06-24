// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.ZStdDecompressStream
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using SharpZstd.Interop;
using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Utility;

public sealed class ZStdDecompressStream : Stream
{
  private readonly Stream _baseStream;
  private readonly bool _ownStream;
  private readonly unsafe ZSTD_DCtx* _ctx;
  private readonly byte[] _buffer;
  private int _bufferPos;
  private int _bufferSize;
  private bool _disposed;

  public unsafe ZStdDecompressStream(Stream baseStream, bool ownStream = true)
  {
    this._baseStream = baseStream;
    this._ownStream = ownStream;
    this._ctx = Zstd.ZSTD_createDCtx();
    this._buffer = ArrayPool<byte>.Shared.Rent((int) Zstd.ZSTD_DStreamInSize());
  }

  protected override unsafe void Dispose(bool disposing)
  {
    if (this._disposed)
      return;
    this._disposed = true;
    IntPtr num = (IntPtr) Zstd.ZSTD_freeDCtx(this._ctx);
    if (!disposing)
      return;
    if (this._ownStream)
      this._baseStream.Dispose();
    ArrayPool<byte>.Shared.Return(this._buffer);
  }

  public override void Flush()
  {
    this.ThrowIfDisposed();
    this._baseStream.Flush();
  }

  public override int Read(byte[] buffer, int offset, int count)
  {
    return this.Read(buffer.AsSpan<byte>(offset, count));
  }

  public override int ReadByte()
  {
    Span<byte> buffer = stackalloc byte[1];
    return this.Read(buffer) != 0 ? (int) buffer[0] : -1;
  }

  public override unsafe int Read(Span<byte> buffer)
  {
    this.ThrowIfDisposed();
    ZSTD_outBuffer zstdOutBuffer;
    while (true)
    {
      if (this._bufferSize == 0 || this._bufferPos == this._bufferSize)
      {
        this._bufferPos = 0;
        this._bufferSize = this._baseStream.Read((Span<byte>) this._buffer);
        if (this._bufferSize == 0)
          break;
      }
      fixed (byte* numPtr1 = this._buffer)
        fixed (byte* numPtr2 = &buffer.GetPinnableReference())
        {
          zstdOutBuffer = new ZSTD_outBuffer()
          {
            dst = (void*) numPtr2,
            pos = UIntPtr.Zero,
            size = (UIntPtr) buffer.Length
          };
          ZSTD_inBuffer zstdInBuffer = new ZSTD_inBuffer()
          {
            src = (void*) numPtr1,
            pos = (UIntPtr) this._bufferPos,
            size = (UIntPtr) this._bufferSize
          };
          UIntPtr code = Zstd.ZSTD_decompressStream(this._ctx, &zstdOutBuffer, &zstdInBuffer);
          this._bufferPos = (int) zstdInBuffer.pos;
          ZStdException.ThrowIfError(code);
          if (zstdOutBuffer.pos <= UIntPtr.Zero)
          {
            // ISSUE: __unpin statement
            __unpin(numPtr2);
            // ISSUE: __unpin statement
            __unpin(numPtr1);
          }
          else
            goto label_5;
        }
    }
    return 0;
label_5:
    return (int) zstdOutBuffer.pos;
  }

  public override unsafe async ValueTask<int> ReadAsync(
    Memory<byte> buffer,
    CancellationToken cancellationToken = default (CancellationToken))
  {
    ZStdDecompressStream stream = this;
    stream.ThrowIfDisposed();
    UIntPtr num1;
    do
    {
      if (stream._bufferSize == 0 || stream._bufferPos == stream._bufferSize)
      {
        stream._bufferPos = 0;
        int num2 = await stream._baseStream.ReadAsync((Memory<byte>) stream._buffer, cancellationToken);
        stream._bufferSize = num2;
        if (stream._bufferSize == 0)
          return 0;
      }
      num1 = DecompressChunk(stream, buffer.Span);
    }
    while (num1 <= UIntPtr.Zero);
    return (int) num1;

    static unsafe UIntPtr DecompressChunk(ZStdDecompressStream stream, Span<byte> buffer)
    {
      fixed (byte* numPtr1 = stream._buffer)
        fixed (byte* numPtr2 = &buffer.GetPinnableReference())
        {
          ZSTD_outBuffer zstdOutBuffer = new ZSTD_outBuffer();
          zstdOutBuffer.dst = (void*) numPtr2;
          zstdOutBuffer.pos = UIntPtr.Zero;
          zstdOutBuffer.size = (UIntPtr) buffer.Length;
          ZSTD_inBuffer zstdInBuffer = new ZSTD_inBuffer();
          zstdInBuffer.src = (void*) numPtr1;
          zstdInBuffer.pos = (UIntPtr) stream._bufferPos;
          zstdInBuffer.size = (UIntPtr) stream._bufferSize;
          UIntPtr code = Zstd.ZSTD_decompressStream(stream._ctx, &zstdOutBuffer, &zstdInBuffer);
          stream._bufferPos = (int) zstdInBuffer.pos;
          ZStdException.ThrowIfError(code);
          return zstdOutBuffer.pos;
        }
    }
  }

  public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

  public override void SetLength(long value) => throw new NotSupportedException();

  public override void Write(byte[] buffer, int offset, int count)
  {
    throw new NotSupportedException();
  }

  public override bool CanRead => true;

  public override bool CanSeek => false;

  public override bool CanWrite => false;

  public override long Length => throw new NotSupportedException();

  public override long Position
  {
    get => throw new NotSupportedException();
    set => throw new NotSupportedException();
  }

  private void ThrowIfDisposed()
  {
    if (this._disposed)
      throw new ObjectDisposedException(nameof (ZStdDecompressStream));
  }
}
