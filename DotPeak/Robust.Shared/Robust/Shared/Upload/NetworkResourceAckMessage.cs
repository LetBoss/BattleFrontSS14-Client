// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Upload.NetworkResourceAckMessage
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

#nullable enable
namespace Robust.Shared.Upload;

internal sealed class NetworkResourceAckMessage : NetMessage
{
  public int Key;

  public override MsgGroups MsgGroup => MsgGroups.String;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.Key = ((NetBuffer) buffer).ReadInt32();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.Key);
  }
}
