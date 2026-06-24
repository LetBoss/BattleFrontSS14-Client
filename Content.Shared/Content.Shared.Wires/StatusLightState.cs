using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Wires;

[Serializable]
[NetSerializable]
public enum StatusLightState : byte
{
	Off,
	On,
	BlinkingFast,
	BlinkingSlow
}
