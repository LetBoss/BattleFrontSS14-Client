// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chat.MsgDeleteChatMessagesBy
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Lidgren.Network;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Chat;

public sealed class MsgDeleteChatMessagesBy : NetMessage
{
  public int Key;
  public HashSet<NetEntity> Entities;

  public virtual MsgGroups MsgGroup => (MsgGroups) 4;

  public virtual void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.Key = ((NetBuffer) buffer).ReadInt32();
    int capacity = ((NetBuffer) buffer).ReadInt32();
    this.Entities = new HashSet<NetEntity>(capacity);
    for (int index = 0; index < capacity; ++index)
      this.Entities.Add(NetMessageExt.ReadNetEntity(buffer));
  }

  public virtual void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.Key);
    ((NetBuffer) buffer).Write(this.Entities.Count);
    foreach (NetEntity entity in this.Entities)
      NetMessageExt.Write(buffer, entity);
  }
}
