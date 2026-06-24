using System;
using Content.Shared.Atmos.Monitor;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Components;

[Serializable]
[NetSerializable]
public struct AtmosAlertsComputerEntry(NetEntity entity, NetCoordinates coordinates, AtmosAlertsComputerGroup group, AtmosAlarmType alarmState, string entityName, string address)
{
	public NetEntity NetEntity = entity;

	public NetCoordinates Coordinates = coordinates;

	public AtmosAlertsComputerGroup Group = group;

	public AtmosAlarmType AlarmState = alarmState;

	public string EntityName = entityName;

	public string Address = address;
}
