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
public sealed class CivBotPatrolPointsRequestEvent : EntityEventArgs
{
	public List<NetEntity> Bots { get; }

	public List<Vector2> Points { get; }

	public MapId MapId { get; }

	public CivBotPatrolPointsRequestEvent(IEnumerable<NetEntity> bots, IEnumerable<Vector2> points, MapId mapId)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Bots = bots.ToList();
		Points = points.ToList();
		MapId = mapId;
	}
}
