// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.NetConnectFailArgs
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Network;

public sealed class NetConnectFailArgs : EventArgs, INetStructuredReason
{
  public NetConnectFailArgs(string reason)
    : this(NetDisconnectMessage.Decode(reason))
  {
  }

  internal NetConnectFailArgs(NetDisconnectMessage reason) => this.Message = reason;

  public NetDisconnectMessage Message { get; }

  public string Reason => this.Message.Reason;

  public bool RedialFlag => this.Message.RedialFlag;
}
