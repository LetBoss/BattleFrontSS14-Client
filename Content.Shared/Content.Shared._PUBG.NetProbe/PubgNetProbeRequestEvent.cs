using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.NetProbe;

[Serializable]
[NetSerializable]
public sealed class PubgNetProbeRequestEvent : EntityEventArgs
{
	public int RequestId { get; }

	public int Kb { get; }

	public PubgNetProbeRequestEvent(int requestId, int kb)
	{
		RequestId = requestId;
		Kb = kb;
	}
}
