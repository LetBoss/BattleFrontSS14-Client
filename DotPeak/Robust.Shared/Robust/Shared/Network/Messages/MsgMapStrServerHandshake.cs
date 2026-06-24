// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgMapStrServerHandshake
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Robust.Shared.Network.Messages;

internal sealed class MsgMapStrServerHandshake : NetMessage
{
  public override MsgGroups MsgGroup => MsgGroups.String;

  public byte[]? Hash { get; set; }

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    int length = ((NetBuffer) buffer).ReadVariableInt32();
    if (length > 64 /*0x40*/)
      throw new InvalidOperationException("Hash too long.");
    ((NetBuffer) buffer).ReadBytes((Span<byte>) (this.Hash = new byte[length]));
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    if (this.Hash == null)
      throw new InvalidOperationException("Package has not been specified.");
    ((NetBuffer) buffer).WriteVariableInt32(this.Hash.Length);
    ((NetBuffer) buffer).Write(this.Hash);
  }
}
