using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Trigger;

[Serializable]
[NetSerializable]
public enum TriggerVisualState : byte
{
	Primed,
	Unprimed
}
