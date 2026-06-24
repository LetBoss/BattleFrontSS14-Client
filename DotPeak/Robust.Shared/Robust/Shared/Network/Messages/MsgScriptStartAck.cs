// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgScriptStartAck
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Serialization;

#nullable disable
namespace Robust.Shared.Network.Messages;

public sealed class MsgScriptStartAck : NetMessage
{
  public override MsgGroups MsgGroup => MsgGroups.Command;

  public bool WasAccepted { get; set; }

  public int ScriptSession { get; set; }

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.WasAccepted = ((NetBuffer) buffer).ReadBoolean();
    this.ScriptSession = ((NetBuffer) buffer).ReadInt32();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.WasAccepted);
    ((NetBuffer) buffer).Write(this.ScriptSession);
  }
}
