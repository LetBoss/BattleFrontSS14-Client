using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Components;

[Serializable]
[NetSerializable]
public struct AtmosAlertsDeviceNavMapData(NetEntity netEntity, NetCoordinates netCoordinates, AtmosAlertsComputerGroup group)
{
	public NetEntity NetEntity = netEntity;

	public NetCoordinates NetCoordinates = netCoordinates;

	public AtmosAlertsComputerGroup Group = group;
}
