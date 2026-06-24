// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.NetMessage
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Robust.Shared.Network;

public abstract class NetMessage
{
  public virtual string MsgName { get; }

  public virtual MsgGroups MsgGroup { get; }

  public INetChannel MsgChannel { get; set; }

  public int MsgSize { get; set; }

  protected NetMessage() => this.MsgName = this.GetType().Name;

  public abstract void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer);

  public abstract void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer);

  public virtual NetDeliveryMethod DeliveryMethod
  {
    get
    {
      switch (this.MsgGroup)
      {
        case MsgGroups.Core:
        case MsgGroups.Command:
          return (NetDeliveryMethod) 34;
        case MsgGroups.Entity:
          return (NetDeliveryMethod) 1;
        case MsgGroups.String:
        case MsgGroups.EntityEvent:
          return (NetDeliveryMethod) 67;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }

  public virtual int SequenceChannel => 0;
}
