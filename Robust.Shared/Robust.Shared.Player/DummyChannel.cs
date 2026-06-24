using System;
using System.Net;
using Lidgren.Network;
using Robust.Shared.Network;

namespace Robust.Shared.Player;

internal sealed class DummyChannel(DummySession session) : INetChannel
{
	public readonly DummySession Session = session;

	public NetUserData UserData => Session.UserData;

	public short Ping => Session.Ping;

	public string UserName => Session.Name;

	public LoginType AuthType => Session.AuthType;

	public NetUserId UserId => Session.UserId;

	public int CurrentMtu { get; set; }

	public long ConnectionId { get; set; }

	public TimeSpan RemoteTimeOffset { get; set; }

	public TimeSpan RemoteTime { get; set; }

	public bool IsConnected { get; set; } = true;

	public bool IsHandshakeComplete { get; set; } = true;

	public IPEndPoint RemoteEndPoint { get; } = new IPEndPoint(IPAddress.Loopback, 1212);

	public NetEncryption? Encryption { get; set; }

	public INetManager NetPeer
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public T CreateNetMessage<T>() where T : NetMessage, new()
	{
		throw new NotImplementedException();
	}

	public void SendMessage(NetMessage message)
	{
		throw new NotImplementedException();
	}

	public void Disconnect(string reason)
	{
		throw new NotImplementedException();
	}

	public void Disconnect(string reason, bool sendBye)
	{
		throw new NotImplementedException();
	}

	public bool CanSendImmediately(NetDeliveryMethod method, int sequenceChannel)
	{
		return true;
	}
}
