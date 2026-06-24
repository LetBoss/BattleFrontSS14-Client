// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgPlayerList
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Enums;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using System.Collections.Generic;

#nullable disable
namespace Robust.Shared.Network.Messages;

public sealed class MsgPlayerList : NetMessage
{
  public override MsgGroups MsgGroup => MsgGroups.Core;

  public List<SessionState> Plyrs { get; set; }

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    int capacity = ((NetBuffer) buffer).ReadInt32();
    this.Plyrs = new List<SessionState>(capacity);
    for (int index = 0; index < capacity; ++index)
      this.Plyrs.Add(new SessionState()
      {
        UserId = new NetUserId(buffer.ReadGuid()),
        Name = ((NetBuffer) buffer).ReadString(),
        Status = (SessionStatus) ((NetBuffer) buffer).ReadByte()
      });
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.Plyrs.Count);
    foreach (SessionState plyr in this.Plyrs)
    {
      buffer.Write(plyr.UserId.UserId);
      ((NetBuffer) buffer).Write(plyr.Name);
      ((NetBuffer) buffer).Write((byte) plyr.Status);
    }
  }
}
