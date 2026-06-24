// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ContentPack.NonSeekableStream
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.ContentPack;

internal sealed class NonSeekableStream : Stream
{
  private readonly Stream _baseStream;

  public NonSeekableStream(Stream baseStream) => this._baseStream = baseStream;

  protected override void Dispose(bool disposing)
  {
    if (!disposing)
      return;
    this._baseStream.Dispose();
  }

  public override ValueTask DisposeAsync() => this._baseStream.DisposeAsync();

  public override void Flush() => this._baseStream.Flush();

  public override int Read(byte[] buffer, int offset, int count)
  {
    return this._baseStream.Read(buffer, offset, count);
  }

  public override int Read(Span<byte> buffer) => this._baseStream.Read(buffer);

  public override int ReadByte() => this._baseStream.ReadByte();

  public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default (CancellationToken))
  {
    return this._baseStream.ReadAsync(buffer, cancellationToken);
  }

  public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

  public override void SetLength(long value) => throw new NotSupportedException();

  public override void Write(byte[] buffer, int offset, int count)
  {
    this._baseStream.Write(buffer, offset, count);
  }

  public override void Write(ReadOnlySpan<byte> buffer) => this._baseStream.Write(buffer);

  public override void WriteByte(byte value) => this._baseStream.WriteByte(value);

  public override ValueTask WriteAsync(
    ReadOnlyMemory<byte> buffer,
    CancellationToken cancellationToken = default (CancellationToken))
  {
    return this._baseStream.WriteAsync(buffer, cancellationToken);
  }

  public override bool CanRead => this._baseStream.CanRead;

  public override bool CanSeek => false;

  public override bool CanWrite => this._baseStream.CanWrite;

  public override long Length => throw new NotSupportedException();

  public override long Position
  {
    get => throw new NotSupportedException();
    set => throw new NotSupportedException();
  }
}
