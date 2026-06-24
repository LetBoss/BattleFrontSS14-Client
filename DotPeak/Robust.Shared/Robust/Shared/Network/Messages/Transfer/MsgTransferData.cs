// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.Transfer.MsgTransferData
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Serialization;
using System;
using System.Buffers;

#nullable enable
namespace Robust.Shared.Network.Messages.Transfer;

internal sealed class MsgTransferData : NetMessage
{
  internal const NetDeliveryMethod Method = (NetDeliveryMethod) 67;
  internal const int Channel = 16 /*0x10*/;
  public ArraySegment<byte> Data;

  public override NetDeliveryMethod DeliveryMethod => (NetDeliveryMethod) 67;

  public override int SequenceChannel => 16 /*0x10*/;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    int num = ((NetBuffer) buffer).ReadVariableInt32();
    byte[] array = num <= 16384 /*0x4000*/ ? ArrayPool<byte>.Shared.Rent(num) : throw new Exception("Buffer size is too large");
    ((NetBuffer) buffer).ReadBytes(array, 0, num);
    this.Data = new ArraySegment<byte>(array, 0, num);
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).WriteVariableInt32(this.Data.Count);
    ((NetBuffer) buffer).Write((ReadOnlySpan<byte>) this.Data.AsSpan<byte>());
  }
}
