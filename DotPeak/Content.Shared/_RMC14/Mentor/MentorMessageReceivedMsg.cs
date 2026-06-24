// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Mentor.MentorMessagesReceivedMsg
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Mentor;

public sealed class MentorMessagesReceivedMsg : NetMessage
{
  public List<MentorMessage> Messages = new List<MentorMessage>();

  public override MsgGroups MsgGroup => MsgGroups.Core;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    int capacity = ((NetBuffer) buffer).ReadInt32();
    this.Messages.EnsureCapacity(capacity);
    for (int index = 0; index < capacity; ++index)
    {
      NetUserId Destination = new NetUserId(buffer.ReadGuid());
      string DestinationName = ((NetBuffer) buffer).ReadString();
      int num = ((NetBuffer) buffer).ReadBoolean() ? 1 : 0;
      NetUserId? Author = new NetUserId?();
      if (num != 0)
        Author = new NetUserId?(new NetUserId(buffer.ReadGuid()));
      string AuthorName = ((NetBuffer) buffer).ReadString();
      string Text = ((NetBuffer) buffer).ReadString();
      DateTime Time = DateTime.FromBinary(((NetBuffer) buffer).ReadInt64());
      bool IsMentor = ((NetBuffer) buffer).ReadBoolean();
      bool IsAdmin = ((NetBuffer) buffer).ReadBoolean();
      bool Create = ((NetBuffer) buffer).ReadBoolean();
      this.Messages.Add(new MentorMessage(Destination, DestinationName, Author, AuthorName, Text, Time, IsMentor, IsAdmin, Create));
    }
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.Messages.Count);
    foreach (MentorMessage message in this.Messages)
    {
      buffer.Write(message.Destination.UserId);
      ((NetBuffer) buffer).Write(message.DestinationName);
      if (message.Author.HasValue)
      {
        ((NetBuffer) buffer).Write(true);
        buffer.Write((Guid) message.Author.Value);
      }
      else
        ((NetBuffer) buffer).Write(false);
      ((NetBuffer) buffer).Write(message.AuthorName);
      ((NetBuffer) buffer).Write(message.Text);
      ((NetBuffer) buffer).Write(message.Time.ToBinary());
      ((NetBuffer) buffer).Write(message.IsMentor);
      ((NetBuffer) buffer).Write(message.IsAdmin);
      ((NetBuffer) buffer).Write(message.Create);
    }
  }
}
