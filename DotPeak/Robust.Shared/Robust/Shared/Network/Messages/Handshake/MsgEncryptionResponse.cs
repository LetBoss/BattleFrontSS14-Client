// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.Handshake.MsgEncryptionResponse
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Robust.Shared.Network.Messages.Handshake;

internal sealed class MsgEncryptionResponse : NetMessage
{
  public Guid UserId;
  public byte[] SealedData;
  public byte[] LegacyHwid;

  public override string MsgName => string.Empty;

  public override MsgGroups MsgGroup => MsgGroups.Core;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.UserId = buffer.ReadGuid();
    int num1 = ((NetBuffer) buffer).ReadVariableInt32();
    this.SealedData = ((NetBuffer) buffer).ReadBytes(num1);
    int num2 = ((NetBuffer) buffer).ReadVariableInt32();
    this.LegacyHwid = ((NetBuffer) buffer).ReadBytes(num2);
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    buffer.Write(this.UserId);
    ((NetBuffer) buffer).WriteVariableInt32(this.SealedData.Length);
    ((NetBuffer) buffer).Write(this.SealedData);
    ((NetBuffer) buffer).WriteVariableInt32(this.LegacyHwid.Length);
    ((NetBuffer) buffer).Write(this.LegacyHwid);
  }
}
