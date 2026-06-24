using System;
using System.Numerics;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderOrderState
{
	public int TeamId { get; }

	public int SquadId { get; }

	public CivCommanderOrderType Order { get; }

	public MapId MapId { get; }

	public Vector2 Position { get; }

	public string SquadLabel { get; }

	public CivCommanderOrderState(int teamId, int squadId, CivCommanderOrderType order, MapId mapId, Vector2 position, string squadLabel)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		TeamId = teamId;
		SquadId = squadId;
		Order = order;
		MapId = mapId;
		Position = position;
		SquadLabel = squadLabel;
	}
}
