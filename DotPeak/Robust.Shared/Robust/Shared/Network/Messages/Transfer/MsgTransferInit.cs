// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.Transfer.MsgTransferInit
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Serialization;

#nullable enable
namespace Robust.Shared.Network.Messages.Transfer;

internal sealed class MsgTransferInit : NetMessage
{
  public (string EndpointUrl, byte[] Key)? HttpInfo;

  public override NetDeliveryMethod DeliveryMethod => (NetDeliveryMethod) 67;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    if (!((NetBuffer) buffer).ReadBoolean())
    {
      this.HttpInfo = new (string, byte[])?();
    }
    else
    {
      ((NetBuffer) buffer).SkipPadBits();
      this.HttpInfo = new (string, byte[])?((((NetBuffer) buffer).ReadString(), ((NetBuffer) buffer).ReadBytes(32 /*0x20*/)));
    }
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    if (!this.HttpInfo.HasValue)
    {
      ((NetBuffer) buffer).Write(false);
    }
    else
    {
      ((NetBuffer) buffer).Write(true);
      ((NetBuffer) buffer).WritePadBits();
      (string EndpointUrl, byte[] Key) = this.HttpInfo.Value;
      ((NetBuffer) buffer).Write(EndpointUrl);
      ((NetBuffer) buffer).Write(Key);
    }
  }
}
