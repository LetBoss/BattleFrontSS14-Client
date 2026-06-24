using System;
using Content.Shared.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Components;

[Serializable]
[NetSerializable]
public struct AtmosDeviceNavMapData
{
	public NetEntity NetEntity;

	public NetCoordinates NetCoordinates;

	public int NetId;

	public ProtoId<NavMapBlipPrototype> NavMapBlip;

	public Direction Direction;

	public Color PipeColor;

	public AtmosPipeLayer PipeLayer;

	public AtmosDeviceNavMapData(NetEntity netEntity, NetCoordinates netCoordinates, int netId, ProtoId<NavMapBlipPrototype> navMapBlip, Direction direction, Color pipeColor, AtmosPipeLayer pipeLayer)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		NetId = -1;
		NetEntity = netEntity;
		NetCoordinates = netCoordinates;
		NetId = netId;
		NavMapBlip = navMapBlip;
		Direction = direction;
		PipeColor = pipeColor;
		PipeLayer = pipeLayer;
	}
}
