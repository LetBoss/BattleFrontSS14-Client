using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderSetSquadOrderRequestEvent : EntityEventArgs
{
	public int SquadId { get; }

	public CivCommanderOrderType Order { get; }

	public MapId MapId { get; }

	public Vector2 Position { get; }

	public CivCommanderSetSquadOrderRequestEvent(int squadId, CivCommanderOrderType order, MapId mapId, Vector2 position)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		SquadId = squadId;
		Order = order;
		MapId = mapId;
		Position = position;
	}
}
