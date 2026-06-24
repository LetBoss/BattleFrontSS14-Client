using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Revenant;

[Serializable]
[NetSerializable]
public enum RevenantVisuals : byte
{
	Corporeal,
	Stunned,
	Harvesting
}
