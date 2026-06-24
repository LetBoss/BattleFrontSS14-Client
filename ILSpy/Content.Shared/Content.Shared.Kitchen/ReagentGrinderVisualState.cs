using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen;

[Serializable]
[NetSerializable]
public enum ReagentGrinderVisualState : byte
{
	BeakerAttached
}
