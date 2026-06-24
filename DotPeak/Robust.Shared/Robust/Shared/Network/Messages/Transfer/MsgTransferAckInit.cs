// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.Transfer.MsgTransferAckInit
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Serialization;

#nullable enable
namespace Robust.Shared.Network.Messages.Transfer;

internal sealed class MsgTransferAckInit : NetMessage
{
  public override NetDeliveryMethod DeliveryMethod => (NetDeliveryMethod) 67;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
  }
}
