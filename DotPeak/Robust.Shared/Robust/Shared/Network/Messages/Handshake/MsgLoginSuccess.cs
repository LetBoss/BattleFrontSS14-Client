// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.Handshake.MsgLoginSuccess
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Robust.Shared.Network.Messages.Handshake;

internal sealed class MsgLoginSuccess : NetMessage
{
  public NetUserData UserData;
  public LoginType Type;

  public override string MsgName => string.Empty;

  public override MsgGroups MsgGroup => MsgGroups.Core;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    string userName = ((NetBuffer) buffer).ReadString();
    Guid userId = buffer.ReadGuid();
    string str = ((NetBuffer) buffer).ReadString();
    if (str.Length == 0)
      str = (string) null;
    this.UserData = new NetUserData(new NetUserId(userId), userName)
    {
      PatronTier = str
    };
    this.Type = (LoginType) ((NetBuffer) buffer).ReadByte();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.UserData.UserName);
    buffer.Write((Guid) this.UserData.UserId);
    ((NetBuffer) buffer).Write(this.UserData.PatronTier);
    ((NetBuffer) buffer).Write((byte) this.Type);
    ((NetBuffer) buffer).Write(new byte[100]);
  }
}
