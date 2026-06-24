using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.NetProbe;

[Serializable]
[NetSerializable]
public sealed class PubgNetProbeAckEvent : EntityEventArgs
{
	public int RequestId { get; }

	public PubgNetProbeAckStatus Status { get; }

	public string LocKey { get; }

	public int Kb { get; }

	public int ReceivedBytes { get; }

	public int ExpectedBytes { get; }

	public PubgNetProbeAckEvent(int requestId, PubgNetProbeAckStatus status, string locKey, int kb, int receivedBytes, int expectedBytes)
	{
		RequestId = requestId;
		Status = status;
		LocKey = locKey;
		Kb = kb;
		ReceivedBytes = receivedBytes;
		ExpectedBytes = expectedBytes;
	}
}
