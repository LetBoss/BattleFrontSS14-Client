using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Network;

[Virtual]
public class NetChannelArgs : EventArgs
{
	public readonly INetChannel Channel;

	public NetChannelArgs(INetChannel channel)
	{
		Channel = channel;
	}
}
