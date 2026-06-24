// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.NetworkStats
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;

#nullable enable
namespace Robust.Shared.Network;

public struct NetworkStats
{
  public readonly long SentBytes;
  public readonly long ReceivedBytes;
  public readonly long SentPackets;
  public readonly long ReceivedPackets;

  public NetworkStats(long sentBytes, long receivedBytes, long sentPackets, long receivedPackets)
  {
    this.SentBytes = sentBytes;
    this.ReceivedBytes = receivedBytes;
    this.SentPackets = sentPackets;
    this.ReceivedPackets = receivedPackets;
  }

  public NetworkStats(NetPeerStatistics statistics)
  {
    this.SentBytes = statistics.SentBytes;
    this.ReceivedBytes = statistics.ReceivedBytes;
    this.SentPackets = statistics.SentPackets;
    this.ReceivedPackets = statistics.ReceivedPackets;
  }
}
