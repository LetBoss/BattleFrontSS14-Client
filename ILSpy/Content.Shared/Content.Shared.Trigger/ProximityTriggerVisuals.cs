using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Trigger;

[Serializable]
[NetSerializable]
public enum ProximityTriggerVisuals : byte
{
	Off,
	Inactive,
	Active
}
