using System;
using System.Collections.Generic;
using Content.Shared.Atmos.Monitor;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Components;

[Serializable]
[NetSerializable]
public struct AtmosAlertsFocusDeviceData(NetEntity netEntity, (float, AtmosAlarmType) temperatureData, (float, AtmosAlarmType) pressureData, Dictionary<Gas, (float, float, AtmosAlarmType)> gasData)
{
	public NetEntity NetEntity = netEntity;

	public (float, AtmosAlarmType) TemperatureData = temperatureData;

	public (float, AtmosAlarmType) PressureData = pressureData;

	public Dictionary<Gas, (float, float, AtmosAlarmType)> GasData = gasData;
}
