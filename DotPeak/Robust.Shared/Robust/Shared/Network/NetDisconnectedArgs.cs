// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.NetDisconnectedArgs
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable enable
namespace Robust.Shared.Network;

public sealed class NetDisconnectedArgs : NetChannelArgs, INetStructuredReason
{
  public NetDisconnectedArgs(INetChannel channel, string reason)
    : this(channel, NetDisconnectMessage.Decode(reason))
  {
  }

  internal NetDisconnectedArgs(INetChannel channel, NetDisconnectMessage reason)
    : base(channel)
  {
    this.Message = reason;
  }

  public NetDisconnectMessage Message { get; }

  public string Reason => this.Message.Reason;

  public bool RedialFlag => this.Message.RedialFlag;
}
