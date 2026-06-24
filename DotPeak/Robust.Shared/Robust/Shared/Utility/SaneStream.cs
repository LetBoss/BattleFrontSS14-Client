// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.SaneStream
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.IO;

#nullable enable
namespace Robust.Shared.Utility;

internal abstract class SaneStream : Stream
{
  public override void Flush() => throw new NotSupportedException();

  public override int Read(byte[] buffer, int offset, int count)
  {
    return this.Read(buffer.AsSpan<byte>(offset, count));
  }

  public override int Read(Span<byte> buffer) => throw new NotSupportedException();

  public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

  public override void SetLength(long value) => throw new NotSupportedException();

  public override void Write(byte[] buffer, int offset, int count)
  {
    this.Write((ReadOnlySpan<byte>) buffer.AsSpan<byte>(offset, count));
  }

  public override void Write(ReadOnlySpan<byte> buffer) => throw new NotSupportedException();

  public override bool CanRead => false;

  public override bool CanSeek => false;

  public override bool CanWrite => false;

  public override long Length => throw new NotSupportedException();

  public override long Position
  {
    get => throw new NotSupportedException();
    set => throw new NotSupportedException();
  }
}
