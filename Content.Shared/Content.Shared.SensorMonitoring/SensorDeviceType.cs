using System;
using Robust.Shared.Serialization;

namespace Content.Shared.SensorMonitoring;

[Serializable]
[NetSerializable]
public enum SensorDeviceType
{
	Unknown,
	Teg,
	AtmosSensor,
	ThermoMachine,
	VolumePump,
	Battery
}
