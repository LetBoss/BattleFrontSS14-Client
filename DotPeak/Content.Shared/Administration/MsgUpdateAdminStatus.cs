// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.MsgUpdateAdminStatus
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Administration;

public sealed class MsgUpdateAdminStatus : NetMessage
{
  public AdminData? Admin;
  public string[] AvailableCommands = Array.Empty<string>();

  public virtual MsgGroups MsgGroup => (MsgGroups) 4;

  public virtual void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    int length = ((NetBuffer) buffer).ReadVariableInt32();
    this.AvailableCommands = new string[length];
    for (int index = 0; index < length; ++index)
      this.AvailableCommands[index] = ((NetBuffer) buffer).ReadString();
    if (!((NetBuffer) buffer).ReadBoolean())
      return;
    bool flag = ((NetBuffer) buffer).ReadBoolean();
    ((NetBuffer) buffer).ReadPadBits();
    AdminFlags adminFlags = (AdminFlags) ((NetBuffer) buffer).ReadUInt32();
    string str = ((NetBuffer) buffer).ReadString();
    this.Admin = new AdminData()
    {
      Active = flag,
      Title = str,
      Flags = adminFlags
    };
  }

  public virtual void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).WriteVariableInt32(this.AvailableCommands.Length);
    foreach (string availableCommand in this.AvailableCommands)
      ((NetBuffer) buffer).Write(availableCommand);
    ((NetBuffer) buffer).Write(this.Admin != null);
    if (this.Admin == null)
      return;
    ((NetBuffer) buffer).Write(this.Admin.Active);
    ((NetBuffer) buffer).WritePadBits();
    ((NetBuffer) buffer).Write((uint) this.Admin.Flags);
    ((NetBuffer) buffer).Write(this.Admin.Title);
  }

  public virtual NetDeliveryMethod DeliveryMethod => (NetDeliveryMethod) 67;
}
