using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Robust.Shared.Analyzers;

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

	event Func<NetConnectingArgs, Task> Connecting;

	event EventHandler<NetChannelArgs> Connected;

	event EventHandler<NetDisconnectedArgs> Disconnect;

	void ResetBandwidthMetrics();

	void Initialize(bool isServer);

	void StartServer();

	void Shutdown(string reason);

	void ProcessPackets();

	void ServerSendToAll(NetMessage message);

	void ServerSendMessage(NetMessage message, INetChannel recipient);

	void ServerSendToMany(NetMessage message, List<INetChannel> recipients);

	void ClientSendMessage(NetMessage message);

	void RegisterNetMessage<T>(ProcessMessage<T>? rxCallback = null, NetMessageAccept accept = NetMessageAccept.Both) where T : NetMessage, new();

	[Obsolete("Just new NetMessage directly")]
	T CreateNetMessage<T>() where T : NetMessage, new();
}
