// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.HasherStream
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Utility;

internal sealed class HasherStream : Stream
{
  private readonly Stream _wrapping;
  private readonly IncrementalHash _hash;
  private readonly bool _leaveOpen;

  public HasherStream(Stream wrapping, IncrementalHash hash, bool leaveOpen = false)
  {
    this._wrapping = wrapping;
    this._hash = hash;
    this._leaveOpen = leaveOpen;
  }

  protected override void Dispose(bool disposing)
  {
    if (this._leaveOpen)
      return;
    this._wrapping.Dispose();
  }

  public override ValueTask DisposeAsync()
  {
    return !this._leaveOpen ? this._wrapping.DisposeAsync() : new ValueTask();
  }

  public override void Flush() => this._wrapping.Flush();

  public override int Read(byte[] buffer, int offset, int count)
  {
    int count1 = this._wrapping.Read(buffer, offset, count);
    if (count1 > 0)
      this._hash.AppendData(buffer, offset, count1);
    return count1;
  }

  public override int Read(Span<byte> buffer)
  {
    int length = this._wrapping.Read(buffer);
    if (length > 0)
      this._hash.AppendData((ReadOnlySpan<byte>) buffer.Slice(0, length));
    return length;
  }

  public override int ReadByte()
  {
    Span<byte> buffer = stackalloc byte[1];
    return this.Read(buffer) != 0 ? (int) buffer[0] : -1;
  }

  public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

  public override void SetLength(long value) => throw new NotSupportedException();

  public override void Write(byte[] buffer, int offset, int count)
  {
    this._hash.AppendData(buffer, offset, count);
    this._wrapping.Write(buffer, offset, count);
  }

  public override void Write(ReadOnlySpan<byte> buffer)
  {
    this._hash.AppendData(buffer);
    this._wrapping.Write(buffer);
  }

  public override void WriteByte(byte value)
  {
    this.Write(stackalloc byte[1]
    {
      value
    });
  }

  public override bool CanRead => this._wrapping.CanRead;

  public override bool CanSeek => false;

  public override bool CanWrite => this._wrapping.CanWrite;

  public override long Length => this._wrapping.Length;

  public override long Position
  {
    get => this._wrapping.Position;
    set => throw new NotSupportedException();
  }
}
