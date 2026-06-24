// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mapping.MappingMapDataMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using System;
using System.IO;

#nullable enable
namespace Content.Shared.Mapping;

public sealed class MappingMapDataMessage : NetMessage
{
  public ZStdCompressionContext Context;
  public string Yml;

  public override MsgGroups MsgGroup => MsgGroups.Command;

  public override NetDeliveryMethod DeliveryMethod => (NetDeliveryMethod) 34;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.MsgSize = ((NetBuffer) buffer).LengthBytes;
    int num1 = ((NetBuffer) buffer).ReadVariableInt32();
    int num2 = ((NetBuffer) buffer).ReadVariableInt32();
    MemoryStream memoryStream = new MemoryStream(num2);
    buffer.ReadAlignedMemory(memoryStream, num2);
    using (ZStdDecompressStream decompressStream = new ZStdDecompressStream((Stream) memoryStream))
    {
      using (MemoryStream destination = new MemoryStream(num1))
      {
        decompressStream.CopyTo((Stream) destination, num1);
        destination.Position = 0L;
        serializer.DeserializeDirect<string>((Stream) destination, out this.Yml);
      }
    }
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    MemoryStream ms = new MemoryStream();
    serializer.SerializeDirect<string>((Stream) ms, this.Yml);
    ((NetBuffer) buffer).WriteVariableInt32((int) ms.Length);
    ms.Position = 0L;
    byte[] numArray = new byte[ZStd.CompressBound((int) ms.Length)];
    int length = this.Context.Compress2((Span<byte>) numArray, (ReadOnlySpan<byte>) ms.AsSpan());
    ((NetBuffer) buffer).WriteVariableInt32(length);
    ((NetBuffer) buffer).Write((ReadOnlySpan<byte>) numArray.AsSpan<byte>(0, length));
  }
}
