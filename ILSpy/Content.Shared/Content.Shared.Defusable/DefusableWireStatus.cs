using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Defusable;

[Serializable]
[NetSerializable]
public enum DefusableWireStatus
{
	LiveIndicator,
	BoltIndicator,
	BoomIndicator,
	DelayIndicator,
	ProceedIndicator
}
