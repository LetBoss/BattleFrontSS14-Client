// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.Handshake.MsgLoginStart
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Serialization;

#nullable disable
namespace Robust.Shared.Network.Messages.Handshake;

internal sealed class MsgLoginStart : NetMessage
{
  public string UserName;
  public bool CanAuth;
  public bool NeedPubKey;
  public bool Encrypt;

  public override string MsgName => string.Empty;

  public override MsgGroups MsgGroup => MsgGroups.Core;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.UserName = ((NetBuffer) buffer).ReadString();
    this.CanAuth = ((NetBuffer) buffer).ReadBoolean();
    this.NeedPubKey = ((NetBuffer) buffer).ReadBoolean();
    this.Encrypt = ((NetBuffer) buffer).ReadBoolean();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.UserName);
    ((NetBuffer) buffer).Write(this.CanAuth);
    ((NetBuffer) buffer).Write(this.NeedPubKey);
    ((NetBuffer) buffer).Write(this.Encrypt);
  }
}
