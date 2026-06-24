namespace Robust.Shared.Network;

public sealed class NetDisconnectedArgs : NetChannelArgs, INetStructuredReason
{
	public NetDisconnectMessage Message { get; }

	public string Reason => Message.Reason;

	public bool RedialFlag => Message.RedialFlag;

	public NetDisconnectedArgs(INetChannel channel, string reason)
		: this(channel, NetDisconnectMessage.Decode(reason))
	{
	}

	internal NetDisconnectedArgs(INetChannel channel, NetDisconnectMessage reason)
		: base(channel)
	{
		Message = reason;
	}
}
