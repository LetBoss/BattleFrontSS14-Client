// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.StreamExt
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.IO;

#nullable enable
namespace Robust.Shared.Utility;

public static class StreamExt
{
  public static byte[] CopyToArray(this Stream stream)
  {
    using (MemoryStream destination = new MemoryStream())
    {
      stream.CopyTo((Stream) destination);
      return destination.ToArray();
    }
  }

  internal static byte[] CopyToPinnedArray(this Stream stream)
  {
    MemoryStream destination1 = new MemoryStream();
    stream.CopyTo((Stream) destination1);
    int length = (int) destination1.Length;
    byte[] destination2 = GC.AllocateUninitializedArray<byte>(length, true);
    destination1.GetBuffer().AsSpan<byte>(0, length).CopyTo((Span<byte>) destination2);
    return destination2;
  }

  internal static MemoryStream ConsumeToMemoryStream(this Stream stream)
  {
    MemoryStream memoryStream = stream.CopyToMemoryStream();
    stream.Dispose();
    return memoryStream;
  }

  internal static MemoryStream CopyToMemoryStream(this Stream stream)
  {
    MemoryStream destination = new MemoryStream();
    stream.CopyTo((Stream) destination);
    destination.Seek(0L, SeekOrigin.Begin);
    return destination;
  }

  public static byte[] ReadExact(this Stream stream, int amount)
  {
    byte[] buffer = new byte[amount];
    int num;
    for (int offset = 0; offset < amount; offset += num)
    {
      num = stream.Read(buffer, offset, amount - offset);
      if (num == 0)
        throw new EndOfStreamException();
    }
    return buffer;
  }

  public static void ReadExact(this Stream stream, Span<byte> buffer)
  {
    // ISSUE: variable of a reference type
    Span<byte>& local;
    int start;
    for (; buffer.Length > 0; buffer = local.Slice(start, local.Length - start))
    {
      int num = stream.Read(buffer);
      if (num == 0)
        throw new EndOfStreamException();
      local = ref buffer;
      start = num;
    }
  }

  public static int ReadToEnd(this Stream stream, Span<byte> buffer)
  {
    int end = 0;
    while (true)
    {
      int num = stream.Read(buffer);
      end += num;
      if (num != 0)
      {
        ref Span<byte> local = ref buffer;
        int start = num;
        buffer = local.Slice(start, local.Length - start);
      }
      else
        break;
    }
    return end;
  }

  public static int ReadToEnd(this Stream stream, byte[] buffer)
  {
    int offset = 0;
    int num;
    do
    {
      num = stream.Read(buffer, offset, buffer.Length - offset);
      offset += num;
    }
    while (num != 0);
    return offset;
  }

  public static Span<byte> AsSpan(this MemoryStream ms)
  {
    return ms.GetBuffer().AsSpan<byte>(0, (int) ms.Length);
  }

  public static Memory<byte> AsMemory(this MemoryStream ms)
  {
    return ms.GetBuffer().AsMemory<byte>(0, (int) ms.Length);
  }
}
