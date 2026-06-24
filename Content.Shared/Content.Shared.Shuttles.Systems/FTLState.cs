using System;

namespace Content.Shared.Shuttles.Systems;

[Flags]
public enum FTLState : byte
{
	Invalid = 0,
	Available = 1,
	Starting = 2,
	Travelling = 4,
	Arriving = 8,
	Cooldown = 0x10
}
