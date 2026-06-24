// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.INetChannel
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Analyzers;
using System;
using System.Net;

#nullable enable
namespace Robust.Shared.Network;

[NotContentImplementable]
public interface INetChannel
{
  INetManager NetPeer { get; }

  long ConnectionId { get; }

  IPEndPoint RemoteEndPoint { get; }

  [Robust.Shared.ViewVariables.ViewVariables]
  NetUserId UserId { get; }

  [Robust.Shared.ViewVariables.ViewVariables]
  string UserName { get; }

  [Robust.Shared.ViewVariables.ViewVariables]
  LoginType AuthType { get; }

  TimeSpan RemoteTimeOffset { get; }

  TimeSpan RemoteTime { get; }

  [Robust.Shared.ViewVariables.ViewVariables]
  short Ping { get; }

  [Robust.Shared.ViewVariables.ViewVariables]
  bool IsConnected { get; }

  NetUserData UserData { get; }

  bool IsHandshakeComplete { get; }

  [Robust.Shared.ViewVariables.ViewVariables]
  int CurrentMtu { get; }

  [Obsolete("Just new NetMessage directly")]
  T CreateNetMessage<T>() where T : NetMessage, new();

  void SendMessage(NetMessage message);

  void Disconnect(string reason);

  void Disconnect(string reason, bool sendBye);

  bool CanSendImmediately(NetDeliveryMethod method, int sequenceChannel);
}
