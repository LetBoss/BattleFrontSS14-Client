// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.LinkAccount.RMCChangeXenoShoutoutMsg
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

#nullable enable
namespace Content.Shared._RMC14.LinkAccount;

public sealed class RMCChangeXenoShoutoutMsg : NetMessage
{
  public string? Name;

  public override MsgGroups MsgGroup => MsgGroups.Command;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    if (((NetBuffer) buffer).PeekStringSize() > 10000)
      return;
    this.Name = ((NetBuffer) buffer).ReadString();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.Name);
  }
}
