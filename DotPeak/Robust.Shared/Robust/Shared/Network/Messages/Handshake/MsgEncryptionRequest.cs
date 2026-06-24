// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.Handshake.MsgEncryptionRequest
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Serialization;

#nullable disable
namespace Robust.Shared.Network.Messages.Handshake;

internal sealed class MsgEncryptionRequest : NetMessage
{
  public byte[] VerifyToken;
  public byte[] PublicKey;
  public bool WantHwid;

  public override string MsgName => string.Empty;

  public override MsgGroups MsgGroup => MsgGroups.Core;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    int num1 = ((NetBuffer) buffer).ReadVariableInt32();
    this.VerifyToken = ((NetBuffer) buffer).ReadBytes(num1);
    int num2 = ((NetBuffer) buffer).ReadVariableInt32();
    this.PublicKey = ((NetBuffer) buffer).ReadBytes(num2);
    this.WantHwid = ((NetBuffer) buffer).ReadBoolean();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).WriteVariableInt32(this.VerifyToken.Length);
    ((NetBuffer) buffer).Write(this.VerifyToken);
    ((NetBuffer) buffer).WriteVariableInt32(this.PublicKey.Length);
    ((NetBuffer) buffer).Write(this.PublicKey);
    ((NetBuffer) buffer).Write(this.WantHwid);
  }
}
