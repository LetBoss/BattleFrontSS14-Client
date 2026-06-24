using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Monitor;

[Serializable]
[NetSerializable]
public enum AtmosAlarmType : sbyte
{
	Invalid,
	Normal,
	Warning,
	Danger,
	Emagged
}
