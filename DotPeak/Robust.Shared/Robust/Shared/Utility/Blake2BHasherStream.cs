// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.Blake2BHasherStream
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using SpaceWizards.Sodium;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Utility;

internal sealed class Blake2BHasherStream : Stream
{
  private readonly bool _reader;
  public readonly int OutputLength;
  public readonly Stream WrappingStream;
  public CryptoGenericHashBlake2B.State State;

  private Blake2BHasherStream(
    Stream wrapping,
    bool reader,
    ReadOnlySpan<byte> key,
    int outputLength)
  {
    this.OutputLength = outputLength;
    this.WrappingStream = wrapping;
    this._reader = reader;
    CryptoGenericHashBlake2B.Init(ref this.State, key, outputLength);
  }

  public byte[] Finish()
  {
    byte[] numArray = new byte[this.OutputLength];
    CryptoGenericHashBlake2B.Final(ref this.State, (Span<byte>) numArray);
    return numArray;
  }

  public static Blake2BHasherStream CreateReader(
    Stream wrapping,
    ReadOnlySpan<byte> key,
    int outputLength)
  {
    if (!wrapping.CanRead)
      throw new ArgumentException("Must pass readable stream.");
    return new Blake2BHasherStream(wrapping, true, key, outputLength);
  }

  public static Blake2BHasherStream CreateWriter(
    Stream wrapping,
    ReadOnlySpan<byte> key,
    int outputLength)
  {
    if (!wrapping.CanWrite)
      throw new ArgumentException("Must pass writeable stream.");
    return new Blake2BHasherStream(wrapping, false, key, outputLength);
  }

  public override void Flush()
  {
    if (!this.CanWrite)
      throw new InvalidOperationException();
    this.WrappingStream.Flush();
  }

  public override int Read(byte[] buffer, int offset, int count)
  {
    if (!this.CanRead)
      throw new InvalidOperationException();
    int length = this.WrappingStream.Read(buffer, offset, count);
    if (length > 0)
      CryptoGenericHashBlake2B.Update(ref this.State, (ReadOnlySpan<byte>) buffer.AsSpan<byte>(offset, length));
    return length;
  }

  public override int Read(Span<byte> buffer)
  {
    if (!this.CanRead)
      throw new InvalidOperationException();
    int length = this.WrappingStream.Read(buffer);
    if (length > 0)
      CryptoGenericHashBlake2B.Update(ref this.State, (ReadOnlySpan<byte>) buffer.Slice(0, length));
    return length;
  }

  public override async ValueTask<int> ReadAsync(
    Memory<byte> buffer,
    CancellationToken cancellationToken = default (CancellationToken))
  {
    Blake2BHasherStream blake2BhasherStream = this;
    if (!blake2BhasherStream.CanRead)
      throw new InvalidOperationException();
    int length = await blake2BhasherStream.WrappingStream.ReadAsync(buffer, cancellationToken);
    if (length > 0)
      CryptoGenericHashBlake2B.Update(ref blake2BhasherStream.State, (ReadOnlySpan<byte>) buffer.Slice(0, length).Span);
    return length;
  }

  public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

  public override void SetLength(long value) => throw new NotSupportedException();

  public override void Write(byte[] buffer, int offset, int count)
  {
    if (!this.CanWrite)
      throw new InvalidOperationException();
    this.WrappingStream.Write(buffer, offset, count);
    CryptoGenericHashBlake2B.Update(ref this.State, (ReadOnlySpan<byte>) buffer.AsSpan<byte>(offset, count));
  }

  public override void Write(ReadOnlySpan<byte> buffer)
  {
    if (!this.CanWrite)
      throw new InvalidOperationException();
    this.WrappingStream.Write(buffer);
    CryptoGenericHashBlake2B.Update(ref this.State, buffer);
  }

  public override void WriteByte(byte value)
  {
    Span<byte> buffer = stackalloc byte[1];
    buffer[0] = value;
    this.Write((ReadOnlySpan<byte>) buffer);
  }

  public override bool CanRead => this._reader;

  public override bool CanSeek => false;

  public override bool CanWrite => !this._reader;

  public override long Length => throw new NotSupportedException();

  public override long Position
  {
    get => throw new NotSupportedException();
    set => throw new NotSupportedException();
  }
}
