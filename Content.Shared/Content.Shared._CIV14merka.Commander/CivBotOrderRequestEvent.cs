using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivBotOrderRequestEvent : EntityEventArgs
{
	public List<NetEntity> Bots { get; }

	public CivBotOrderType Order { get; }

	public MapId MapId { get; }

	public Vector2 Position { get; }

	public NetEntity? FollowTarget { get; }

	public CivBotOrderRequestEvent(IEnumerable<NetEntity> bots, CivBotOrderType order, MapId mapId, Vector2 position, NetEntity? followTarget = null)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Bots = bots.ToList();
		Order = order;
		MapId = mapId;
		Position = position;
		FollowTarget = followTarget;
	}
}
