// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.SequenceMemoryStream
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Buffers;
using System.IO;

#nullable enable
namespace Robust.Shared.Utility;

internal sealed class SequenceMemoryStream : Stream
{
  private readonly int _segmentLength;
  private readonly SequenceMemoryStream.SequenceSegment _startSegment;
  private SequenceMemoryStream.SequenceSegment _curSegment;
  private int _curSegmentWritten;

  public SequenceMemoryStream(int segmentLength = 524288 /*0x080000*/)
  {
    this._segmentLength = segmentLength;
    this._startSegment = this._curSegment = new SequenceMemoryStream.SequenceSegment(segmentLength, 0L);
  }

  public ReadOnlySequence<byte> AsSequence
  {
    get
    {
      return new ReadOnlySequence<byte>((ReadOnlySequenceSegment<byte>) this._startSegment, 0, (ReadOnlySequenceSegment<byte>) this._curSegment, this._curSegmentWritten);
    }
  }

  public override void Flush()
  {
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

  public override void Write(ReadOnlySpan<byte> buffer)
  {
    Span<byte> destination;
    while (true)
    {
      destination = this._curSegment.Buffer.AsSpan<byte>(this._curSegmentWritten);
      if (destination.Length < buffer.Length)
      {
        buffer.Slice(0, destination.Length).CopyTo(destination);
        ref ReadOnlySpan<byte> local = ref buffer;
        int length = destination.Length;
        buffer = local.Slice(length, local.Length - length);
        SequenceMemoryStream.SequenceSegment segment = new SequenceMemoryStream.SequenceSegment(this._segmentLength, this._curSegment.RunningIndex + (long) this._segmentLength);
        this._curSegment.Append(segment);
        this._curSegment = segment;
        this._curSegmentWritten = 0;
      }
      else
        break;
    }
    buffer.CopyTo(destination);
    this._curSegmentWritten += buffer.Length;
  }

  public override void WriteByte(byte value)
  {
    Span<byte> buffer = stackalloc byte[1];
    buffer[0] = value;
    this.Write((ReadOnlySpan<byte>) buffer);
  }

  public override bool CanRead => false;

  public override bool CanSeek => false;

  public override bool CanWrite => true;

  public override long Length => this._curSegment.RunningIndex + (long) this._curSegmentWritten;

  public override long Position
  {
    get => this.Length;
    set => throw new NotSupportedException();
  }

  private sealed class SequenceSegment : ReadOnlySequenceSegment<byte>
  {
    public readonly byte[] Buffer;

    public SequenceSegment(int segmentLength, long runningIndex)
    {
      this.Buffer = GC.AllocateUninitializedArray<byte>(segmentLength);
      this.RunningIndex = runningIndex;
      this.Memory = (ReadOnlyMemory<byte>) this.Buffer;
    }

    public void Append(SequenceMemoryStream.SequenceSegment segment)
    {
      this.Next = (ReadOnlySequenceSegment<byte>) segment;
    }
  }
}
