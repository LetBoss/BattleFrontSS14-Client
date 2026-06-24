using System;
using System.Net;
using Lidgren.Network;
using Robust.Shared.Analyzers;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Network;

[NotContentImplementable]
public interface INetChannel
{
	INetManager NetPeer { get; }

	long ConnectionId { get; }

	IPEndPoint RemoteEndPoint { get; }

	[ViewVariables]
	NetUserId UserId { get; }

	[ViewVariables]
	string UserName { get; }

	[ViewVariables]
	LoginType AuthType { get; }

	TimeSpan RemoteTimeOffset { get; }

	TimeSpan RemoteTime { get; }

	[ViewVariables]
	short Ping { get; }

	[ViewVariables]
	bool IsConnected { get; }

	NetUserData UserData { get; }

	bool IsHandshakeComplete { get; }

	[ViewVariables]
	int CurrentMtu { get; }

	[Obsolete("Just new NetMessage directly")]
	T CreateNetMessage<T>() where T : NetMessage, new();

	void SendMessage(NetMessage message);

	void Disconnect(string reason);

	void Disconnect(string reason, bool sendBye);

	bool CanSendImmediately(NetDeliveryMethod method, int sequenceChannel);
}
