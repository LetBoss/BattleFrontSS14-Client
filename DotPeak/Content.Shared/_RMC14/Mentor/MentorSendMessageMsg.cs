// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Mentor.MentorSendMessageMsg
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._RMC14.Mentor;

public sealed class MentorSendMessageMsg : NetMessage
{
  public string Message = string.Empty;
  public Guid To;

  public override MsgGroups MsgGroup => MsgGroups.Core;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.Message = ((NetBuffer) buffer).ReadString();
    this.To = buffer.ReadGuid();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.Message);
    buffer.Write(this.To);
  }
}
