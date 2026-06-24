using System;
using Lidgren.Network;

namespace Robust.Shared.Network;

public sealed class NetMessageArgs : EventArgs
{
	public NetMessage Message { get; }

	public NetIncomingMessage RawMessage { get; }

	public NetMessageArgs(NetMessage message, NetIncomingMessage rawMessage)
	{
		Message = message;
		RawMessage = rawMessage;
	}
}
