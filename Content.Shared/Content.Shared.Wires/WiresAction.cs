using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Wires;

[Serializable]
[NetSerializable]
public enum WiresAction : byte
{
	Mend,
	Cut,
	Pulse
}
