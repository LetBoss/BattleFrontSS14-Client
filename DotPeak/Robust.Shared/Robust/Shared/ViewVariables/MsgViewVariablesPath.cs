// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.MsgViewVariablesPath
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

#nullable enable
namespace Robust.Shared.ViewVariables;

internal abstract class MsgViewVariablesPath : NetMessage
{
  public override MsgGroups MsgGroup => MsgGroups.Command;

  public uint RequestId { get; set; }

  public string Path { get; set; } = string.Empty;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.RequestId = ((NetBuffer) buffer).ReadUInt32();
    this.Path = ((NetBuffer) buffer).ReadString();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.RequestId);
    ((NetBuffer) buffer).Write(this.Path);
  }
}
