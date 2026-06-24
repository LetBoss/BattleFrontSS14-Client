// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgMapStrClientHandshake
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Serialization;

#nullable enable
namespace Robust.Shared.Network.Messages;

internal sealed class MsgMapStrClientHandshake : NetMessage
{
  public override MsgGroups MsgGroup => MsgGroups.String;

  public bool NeedsStrings { get; set; }

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.NeedsStrings = ((NetBuffer) buffer).ReadBoolean();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.NeedsStrings);
  }
}
