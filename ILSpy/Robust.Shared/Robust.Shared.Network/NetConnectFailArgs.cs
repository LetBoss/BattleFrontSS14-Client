using System;

namespace Robust.Shared.Network;

public sealed class NetConnectFailArgs : EventArgs, INetStructuredReason
{
	public NetDisconnectMessage Message { get; }

	public string Reason => Message.Reason;

	public bool RedialFlag => Message.RedialFlag;

	public NetConnectFailArgs(string reason)
		: this(NetDisconnectMessage.Decode(reason))
	{
	}

	internal NetConnectFailArgs(NetDisconnectMessage reason)
	{
		Message = reason;
	}
}
