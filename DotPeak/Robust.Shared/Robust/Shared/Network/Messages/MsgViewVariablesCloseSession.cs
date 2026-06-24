// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgViewVariablesCloseSession
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Serialization;

#nullable disable
namespace Robust.Shared.Network.Messages;

public sealed class MsgViewVariablesCloseSession : NetMessage
{
  public override MsgGroups MsgGroup => MsgGroups.Command;

  public uint SessionId { get; set; }

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.SessionId = ((NetBuffer) buffer).ReadUInt32();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.SessionId);
  }
}
