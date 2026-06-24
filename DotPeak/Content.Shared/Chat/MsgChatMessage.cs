// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chat.MsgChatMessage
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
namespace Content.Shared.Chat;

public sealed class MsgChatMessage : NetMessage
{
  public ChatMessage Message;

  public virtual MsgGroups MsgGroup => (MsgGroups) 4;

  public virtual void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    int capacity = ((NetBuffer) buffer).ReadVariableInt32();
    using (MemoryStream memoryStream = new MemoryStream(capacity))
    {
      NetMessageExt.ReadAlignedMemory(buffer, memoryStream, capacity);
      serializer.DeserializeDirect<ChatMessage>((Stream) memoryStream, ref this.Message);
    }
  }

  public virtual void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    MemoryStream memoryStream = new MemoryStream();
    serializer.SerializeDirect<ChatMessage>((Stream) memoryStream, this.Message);
    ((NetBuffer) buffer).WriteVariableInt32((int) memoryStream.Length);
    ((NetBuffer) buffer).Write((ReadOnlySpan<byte>) StreamExt.AsSpan(memoryStream));
  }
}
