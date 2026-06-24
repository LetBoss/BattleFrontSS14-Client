using Lidgren.Network;

namespace Robust.Shared.Network;

public struct NetworkStats
{
	public readonly long SentBytes;

	public readonly long ReceivedBytes;

	public readonly long SentPackets;

	public readonly long ReceivedPackets;

	public NetworkStats(long sentBytes, long receivedBytes, long sentPackets, long receivedPackets)
	{
		SentBytes = sentBytes;
		ReceivedBytes = receivedBytes;
		SentPackets = sentPackets;
		ReceivedPackets = receivedPackets;
	}

	public NetworkStats(NetPeerStatistics statistics)
	{
		SentBytes = statistics.SentBytes;
		ReceivedBytes = statistics.ReceivedBytes;
		SentPackets = statistics.SentPackets;
		ReceivedPackets = statistics.ReceivedPackets;
	}
}
