using System;
using Robust.Shared.Serialization;

namespace Content.Shared.APC;

[Serializable]
[NetSerializable]
public enum ApcChargeState : sbyte
{
	Lack = 0,
	Charging = 1,
	Full = 2,
	Remote = 3,
	NumStates = 4,
	Emag = -1
}
