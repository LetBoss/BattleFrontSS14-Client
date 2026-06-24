// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Mentor.MentorHelpClientTypingUpdatedMsg
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._RMC14.Mentor;

public sealed class MentorHelpClientTypingUpdatedMsg : NetMessage
{
  public Guid Destination;
  public bool Typing;

  public override MsgGroups MsgGroup => MsgGroups.Core;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.Destination = buffer.ReadGuid();
    this.Typing = ((NetBuffer) buffer).ReadBoolean();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    buffer.Write(this.Destination);
    ((NetBuffer) buffer).Write(this.Typing);
  }
}
