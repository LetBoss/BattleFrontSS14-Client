using System;
using Robust.Shared.Analyzers;

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
