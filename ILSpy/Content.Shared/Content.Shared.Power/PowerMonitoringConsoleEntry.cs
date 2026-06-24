using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Power;

[Serializable]
[NetSerializable]
public struct PowerMonitoringConsoleEntry(NetEntity netEntity, PowerMonitoringConsoleGroup group, double powerValue = 0.0, float? batteryLevel = null)
{
	public NetEntity NetEntity = netEntity;

	public PowerMonitoringConsoleGroup Group = group;

	public double PowerValue = powerValue;

	public float? BatteryLevel = batteryLevel;

	[NonSerialized]
	public PowerMonitoringDeviceMetaData? MetaData = null;
}
