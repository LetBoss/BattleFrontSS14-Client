using System;

namespace Content.Shared.Power;

[Flags]
public enum PowerMonitoringFlags : byte
{
	None = 0,
	RoguePowerConsumer = 1,
	PowerNetAbnormalities = 2
}
