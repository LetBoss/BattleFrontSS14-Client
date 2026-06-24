using System.IO;

namespace Robust.Shared.Network.Transfer;

public sealed class TransferReceivedEvent
{
	public readonly string Key;

	public readonly Stream DataStream;

	public readonly INetChannel Channel;

	internal TransferReceivedEvent(string key, INetChannel channel, Stream stream)
	{
		Key = key;
		DataStream = stream;
		Channel = channel;
	}
}
