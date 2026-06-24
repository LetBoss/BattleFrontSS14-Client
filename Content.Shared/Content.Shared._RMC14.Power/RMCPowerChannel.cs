using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Power;

[Serializable]
[NetSerializable]
public enum RMCPowerChannel
{
	Equipment,
	Lighting,
	Environment
}
