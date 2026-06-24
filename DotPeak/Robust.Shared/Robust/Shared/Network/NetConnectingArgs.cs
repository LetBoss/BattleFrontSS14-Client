// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.NetConnectingArgs
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Net;

#nullable enable
namespace Robust.Shared.Network;

public sealed class NetConnectingArgs : EventArgs
{
  public bool IsDenied => this.DenyReasonData != (NetDenyReason) null;

  public string? DenyReason => this.DenyReasonData?.Text;

  public NetDenyReason? DenyReasonData { get; private set; }

  public NetUserData UserData { get; }

  public NetUserId UserId => this.UserData.UserId;

  public string UserName => this.UserData.UserName;

  public IPEndPoint IP { get; }

  public LoginType AuthType { get; }

  public void Deny(string reason) => this.Deny(new NetDenyReason(reason));

  public void Deny(NetDenyReason reason) => this.DenyReasonData = reason;

  public NetConnectingArgs(NetUserData data, IPEndPoint ip, LoginType authType)
  {
    this.UserData = data;
    this.IP = ip;
    this.AuthType = authType;
  }
}
