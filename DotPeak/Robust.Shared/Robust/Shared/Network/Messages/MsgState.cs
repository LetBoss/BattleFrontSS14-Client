// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgState
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using System;
using System.Buffers;
using System.IO;

#nullable disable
namespace Robust.Shared.Network.Messages;

public sealed class MsgState : NetMessage
{
  public const int ReliableThreshold = 488;
  public const int CompressionThreshold = 256 /*0x0100*/;
  public GameState State;
  public MemoryStream StateStream;
  public ZStdCompressionContext CompressionContext;
  internal bool HasWritten;
  internal bool ForceSendReliably;

  public override MsgGroups MsgGroup => MsgGroups.Entity;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.MsgSize = ((NetBuffer) buffer).LengthBytes;
    int num = ((NetBuffer) buffer).ReadVariableInt32();
    int length = ((NetBuffer) buffer).ReadVariableInt32();
    MemoryStream memoryStream1;
    if (length > 0)
    {
      MemoryStream memoryStream2 = RobustMemoryManager.GetMemoryStream(length);
      buffer.ReadAlignedMemory(memoryStream2, length);
      using (ZStdDecompressStream decompressStream = new ZStdDecompressStream((Stream) memoryStream2))
      {
        memoryStream1 = RobustMemoryManager.GetMemoryStream(num);
        memoryStream1.SetLength((long) num);
        decompressStream.CopyTo((Stream) memoryStream1, num);
        memoryStream1.Position = 0L;
      }
    }
    else
    {
      memoryStream1 = RobustMemoryManager.GetMemoryStream(num);
      buffer.ReadAlignedMemory(memoryStream1, num);
    }
    try
    {
      serializer.DeserializeDirect<GameState>((Stream) memoryStream1, out this.State);
    }
    finally
    {
      memoryStream1.Dispose();
    }
    this.State.PayloadSize = num;
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).WriteVariableInt32((int) this.StateStream.Length);
    if (this.StateStream.Length > 256L /*0x0100*/)
    {
      this.StateStream.Position = 0L;
      byte[] numArray = ArrayPool<byte>.Shared.Rent(ZStd.CompressBound((int) this.StateStream.Length));
      int length = this.CompressionContext.Compress2((Span<byte>) numArray, (ReadOnlySpan<byte>) this.StateStream.AsSpan());
      ((NetBuffer) buffer).WriteVariableInt32(length);
      ((NetBuffer) buffer).Write((ReadOnlySpan<byte>) numArray.AsSpan<byte>(0, length));
      ArrayPool<byte>.Shared.Return(numArray);
    }
    else
    {
      ((NetBuffer) buffer).WriteVariableInt32(0);
      ((NetBuffer) buffer).Write((ReadOnlySpan<byte>) this.StateStream.AsSpan());
    }
    this.HasWritten = true;
    this.MsgSize = ((NetBuffer) buffer).LengthBytes;
  }

  public bool ShouldSendReliably() => this.ForceSendReliably || this.MsgSize > 488;

  public override NetDeliveryMethod DeliveryMethod
  {
    get => !this.ShouldSendReliably() ? base.DeliveryMethod : (NetDeliveryMethod) 34;
  }
}
