// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgStateRequestFull
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

#nullable disable
namespace Robust.Shared.Network.Messages;

public sealed class MsgStateRequestFull : NetMessage
{
  public GameTick Tick;
  public NetEntity MissingEntity;

  public override MsgGroups MsgGroup => MsgGroups.Entity;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.Tick = buffer.ReadGameTick();
    this.MissingEntity = buffer.ReadNetEntity();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    buffer.Write(this.Tick);
    buffer.Write(this.MissingEntity);
  }

  public override NetDeliveryMethod DeliveryMethod => (NetDeliveryMethod) 34;
}
