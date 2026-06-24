using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Components;

[Serializable]
[NetSerializable]
public struct AtmosMonitoringConsoleEntry
{
	public NetEntity NetEntity;

	public NetCoordinates Coordinates;

	public int NetId;

	public string EntityName;

	public string Address;

	public float TemperatureData;

	public float PressureData;

	public float TotalMolData;

	public Dictionary<Gas, float> GasData;

	public Color Color;

	public bool IsPowered;

	public AtmosMonitoringConsoleEntry(NetEntity entity, NetCoordinates coordinates, int netId, string entityName, string address)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		TemperatureData = 0f;
		PressureData = 0f;
		TotalMolData = 0f;
		Color = default(Color);
		NetId = -1;
		GasData = new Dictionary<Gas, float>();
		IsPowered = true;
		NetEntity = entity;
		Coordinates = coordinates;
		NetId = netId;
		EntityName = entityName;
		Address = address;
	}
}
