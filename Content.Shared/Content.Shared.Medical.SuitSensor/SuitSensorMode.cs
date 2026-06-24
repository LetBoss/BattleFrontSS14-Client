using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Medical.SuitSensor;

[Serializable]
[NetSerializable]
public enum SuitSensorMode : byte
{
	SensorOff,
	SensorBinary,
	SensorVitals,
	SensorCords
}
