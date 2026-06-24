// Decompiled with JetBrains decompiler
// Type: Content.Shared.Eui.MsgEuiCtl
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

#nullable enable
namespace Content.Shared.Eui;

public sealed class MsgEuiCtl : NetMessage
{
  public MsgEuiCtl.CtlType Type;
  public string OpenType = string.Empty;
  public uint Id;

  public override MsgGroups MsgGroup => MsgGroups.Command;

  public override NetDeliveryMethod DeliveryMethod => (NetDeliveryMethod) 67;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.Id = ((NetBuffer) buffer).ReadUInt32();
    this.Type = (MsgEuiCtl.CtlType) ((NetBuffer) buffer).ReadByte();
    if (this.Type != MsgEuiCtl.CtlType.Open)
      return;
    this.OpenType = ((NetBuffer) buffer).ReadString();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.Id);
    ((NetBuffer) buffer).Write((byte) this.Type);
    if (this.Type != MsgEuiCtl.CtlType.Open)
      return;
    ((NetBuffer) buffer).Write(this.OpenType);
  }

  public enum CtlType : byte
  {
    Open,
    Close,
  }
}
