using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.NetProbe;

[Serializable]
[NetSerializable]
public sealed class PubgNetProbeUploadEvent : EntityEventArgs
{
	public int RequestId { get; }

	public int Kb { get; }

	public byte[] Payload { get; }

	public PubgNetProbeUploadEvent(int requestId, int kb, byte[] payload)
	{
		RequestId = requestId;
		Kb = kb;
		Payload = payload;
	}
}
