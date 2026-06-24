// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgConCmdReg
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Serialization;
using System.Collections.Generic;

#nullable disable
namespace Robust.Shared.Network.Messages;

public sealed class MsgConCmdReg : NetMessage
{
  public override MsgGroups MsgGroup => MsgGroups.String;

  public List<MsgConCmdReg.Command> Commands { get; set; }

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    ushort capacity = ((NetBuffer) buffer).ReadUInt16();
    this.Commands = new List<MsgConCmdReg.Command>((int) capacity);
    for (int index = 0; index < (int) capacity; ++index)
      this.Commands.Add(new MsgConCmdReg.Command()
      {
        Name = ((NetBuffer) buffer).ReadString(),
        Description = ((NetBuffer) buffer).ReadString(),
        Help = ((NetBuffer) buffer).ReadString()
      });
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    if (this.Commands == null)
    {
      ((NetBuffer) buffer).Write((ushort) 0);
    }
    else
    {
      ((NetBuffer) buffer).Write((ushort) this.Commands.Count);
      foreach (MsgConCmdReg.Command command in this.Commands)
      {
        ((NetBuffer) buffer).Write(command.Name);
        ((NetBuffer) buffer).Write(command.Description);
        ((NetBuffer) buffer).Write(command.Help);
      }
    }
  }

  public sealed class Command
  {
    public string Name { get; set; }

    public string Description { get; set; }

    public string Help { get; set; }
  }
}
