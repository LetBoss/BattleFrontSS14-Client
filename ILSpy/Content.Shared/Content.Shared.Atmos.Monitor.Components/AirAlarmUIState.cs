using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Monitor.Components;

[Serializable]
[NetSerializable]
public sealed class AirAlarmUIState : BoundUserInterfaceState
{
	public string Address { get; }

	public int DeviceCount { get; }

	public float PressureAverage { get; }

	public float TemperatureAverage { get; }

	public List<(string, IAtmosDeviceData)> DeviceData { get; }

	public AirAlarmMode Mode { get; }

	public AtmosAlarmType AlarmType { get; }

	public bool AutoMode { get; }

	public bool PanicWireCut { get; }

	public AirAlarmUIState(string address, int deviceCount, float pressureAverage, float temperatureAverage, List<(string, IAtmosDeviceData)> deviceData, AirAlarmMode mode, AtmosAlarmType alarmType, bool autoMode, bool panicWireCut)
	{
		Address = address;
		DeviceCount = deviceCount;
		PressureAverage = pressureAverage;
		TemperatureAverage = temperatureAverage;
		DeviceData = deviceData;
		Mode = mode;
		AlarmType = alarmType;
		AutoMode = autoMode;
		PanicWireCut = panicWireCut;
	}
}
