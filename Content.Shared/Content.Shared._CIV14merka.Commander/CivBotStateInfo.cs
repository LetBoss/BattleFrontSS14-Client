using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivBotStateInfo
{
	public NetEntity Bot { get; }

	public CivBotOrderType Order { get; }

	public Vector2 Position { get; }

	public MapId MapId { get; }

	public bool HasTarget { get; }

	public float Health { get; }

	public int SquadId { get; }

	public CivBotRole Role { get; }

	public bool IsLeader { get; }

	public CivBotStateInfo(NetEntity bot, CivBotOrderType order, Vector2 position, MapId mapId, bool hasTarget, float health, int squadId, CivBotRole role, bool isLeader)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Bot = bot;
		Order = order;
		Position = position;
		MapId = mapId;
		HasTarget = hasTarget;
		Health = health;
		SquadId = squadId;
		Role = role;
		IsLeader = isLeader;
	}
}
