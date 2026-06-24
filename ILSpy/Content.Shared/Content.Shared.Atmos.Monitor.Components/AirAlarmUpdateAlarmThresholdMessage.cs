using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Monitor.Components;

[Serializable]
[NetSerializable]
public sealed class AirAlarmUpdateAlarmThresholdMessage : BoundUserInterfaceMessage
{
	public string Address { get; }

	public AtmosAlarmThreshold Threshold { get; }

	public AtmosMonitorThresholdType Type { get; }

	public Gas? Gas { get; }

	public AirAlarmUpdateAlarmThresholdMessage(string address, AtmosMonitorThresholdType type, AtmosAlarmThreshold threshold, Gas? gas = null)
	{
		Address = address;
		Threshold = threshold;
		Type = type;
		Gas = gas;
	}
}
