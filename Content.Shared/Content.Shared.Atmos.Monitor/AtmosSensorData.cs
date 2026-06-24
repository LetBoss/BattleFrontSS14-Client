using System;
using System.Collections.Generic;
using Content.Shared.Atmos.Monitor.Components;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Monitor;

[Serializable]
[NetSerializable]
public sealed class AtmosSensorData : IAtmosDeviceData
{
	public bool Enabled { get; set; }

	public bool Dirty { get; set; }

	public bool IgnoreAlarms { get; set; }

	public float Pressure { get; }

	public float Temperature { get; }

	public float TotalMoles { get; }

	public AtmosAlarmType AlarmState { get; }

	public Dictionary<Gas, float> Gases { get; }

	public AtmosAlarmThreshold PressureThreshold { get; }

	public AtmosAlarmThreshold TemperatureThreshold { get; }

	public Dictionary<Gas, AtmosAlarmThreshold> GasThresholds { get; }

	public AtmosSensorData(float pressure, float temperature, float totalMoles, AtmosAlarmType alarmState, Dictionary<Gas, float> gases, AtmosAlarmThreshold pressureThreshold, AtmosAlarmThreshold temperatureThreshold, Dictionary<Gas, AtmosAlarmThreshold> gasThresholds)
	{
		Pressure = pressure;
		Temperature = temperature;
		TotalMoles = totalMoles;
		AlarmState = alarmState;
		Gases = gases;
		PressureThreshold = pressureThreshold;
		TemperatureThreshold = temperatureThreshold;
		GasThresholds = gasThresholds;
	}
}
