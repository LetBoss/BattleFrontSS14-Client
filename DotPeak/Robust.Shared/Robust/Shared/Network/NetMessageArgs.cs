// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.NetMessageArgs
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using System;

#nullable enable
namespace Robust.Shared.Network;

public sealed class NetMessageArgs : EventArgs
{
  public NetMessage Message { get; }

  public NetIncomingMessage RawMessage { get; }

  public NetMessageArgs(NetMessage message, NetIncomingMessage rawMessage)
  {
    this.Message = message;
    this.RawMessage = rawMessage;
  }
}
