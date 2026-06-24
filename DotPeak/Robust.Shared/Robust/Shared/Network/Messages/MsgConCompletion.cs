// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgConCompletion
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Serialization;

#nullable disable
namespace Robust.Shared.Network.Messages;

public sealed class MsgConCompletion : NetMessage
{
  public override MsgGroups MsgGroup => MsgGroups.Command;

  public int Seq { get; set; }

  public string[] Args { get; set; }

  public string ArgString { get; set; }

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.Seq = ((NetBuffer) buffer).ReadInt32();
    int length = ((NetBuffer) buffer).ReadVariableInt32();
    this.Args = new string[length];
    for (int index = 0; index < length; ++index)
      this.Args[index] = ((NetBuffer) buffer).ReadString();
    this.ArgString = ((NetBuffer) buffer).ReadString();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.Seq);
    ((NetBuffer) buffer).WriteVariableInt32(this.Args.Length);
    foreach (string str in this.Args)
      ((NetBuffer) buffer).Write(str);
    ((NetBuffer) buffer).Write(this.ArgString);
  }
}
