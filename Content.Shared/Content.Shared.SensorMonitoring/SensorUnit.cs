using System;
using Robust.Shared.Serialization;

namespace Content.Shared.SensorMonitoring;

[Serializable]
[NetSerializable]
public enum SensorUnit : byte
{
	Undetermined,
	PressureKpa,
	TemperatureK,
	Moles,
	Ratio,
	PowerW,
	EnergyJ
}
