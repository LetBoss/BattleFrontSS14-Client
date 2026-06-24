// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.INetManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Network;

[NotContentImplementable]
public interface INetManager
{
  bool IsServer { get; }

  bool IsClient { get; }

  bool IsRunning { get; }

  bool IsConnected { get; }

  NetworkStats Statistics { get; }

  IEnumerable<INetChannel> Channels { get; }

  int ChannelCount { get; }

  int Port { get; }

  IReadOnlyDictionary<Type, long> MessageBandwidthUsage { get; }

  void ResetBandwidthMetrics();

  void Initialize(bool isServer);

  void StartServer();

  void Shutdown(string reason);

  void ProcessPackets();

  void ServerSendToAll(NetMessage message);

  void ServerSendMessage(NetMessage message, INetChannel recipient);

  void ServerSendToMany(NetMessage message, List<INetChannel> recipients);

  void ClientSendMessage(NetMessage message);

  event Func<NetConnectingArgs, Task> Connecting;

  event EventHandler<NetChannelArgs> Connected;

  event EventHandler<NetDisconnectedArgs> Disconnect;

  void RegisterNetMessage<T>(ProcessMessage<T>? rxCallback = null, NetMessageAccept accept = NetMessageAccept.Both) where T : NetMessage, new();

  [Obsolete("Just new NetMessage directly")]
  T CreateNetMessage<T>() where T : NetMessage, new();
}
