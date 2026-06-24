// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.IClientNetManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;

#nullable enable
namespace Robust.Shared.Network;

[NotContentImplementable]
public interface IClientNetManager : INetManager
{
  INetChannel? ServerChannel { get; }

  ClientConnectionState ClientConnectState { get; }

  event Action<ClientConnectionState> ClientConnectStateChanged;

  event EventHandler<NetConnectFailArgs> ConnectFailed;

  void ClientConnect(string host, int port, string userNameRequest);

  void ClientDisconnect(string reason);

  void Reset(string reason);

  void DispatchLocalNetMessage(NetMessage message);
}
