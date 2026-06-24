using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.NetProbe;

[Serializable]
[NetSerializable]
public enum PubgNetProbeAckStatus
{
	Ok,
	Denied,
	TooBig,
	InvalidKb,
	Mismatch
}
