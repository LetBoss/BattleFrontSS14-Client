// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgStateLeavePvs
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System.Collections.Generic;

#nullable disable
namespace Robust.Shared.Network.Messages;

public sealed class MsgStateLeavePvs : NetMessage
{
  public List<NetEntity> Entities;
  public GameTick Tick;

  public override MsgGroups MsgGroup => MsgGroups.Entity;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.Tick = buffer.ReadGameTick();
    int capacity = ((NetBuffer) buffer).ReadInt32();
    this.Entities = new List<NetEntity>(capacity);
    for (int index = 0; index < capacity; ++index)
      this.Entities.Add(buffer.ReadNetEntity());
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    buffer.Write(this.Tick);
    ((NetBuffer) buffer).Write(this.Entities.Count);
    foreach (NetEntity entity in this.Entities)
      buffer.Write(entity);
  }

  public override NetDeliveryMethod DeliveryMethod => (NetDeliveryMethod) 34;
}
