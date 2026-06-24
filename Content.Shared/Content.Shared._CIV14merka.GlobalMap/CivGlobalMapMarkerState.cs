using System;
using System.Numerics;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.GlobalMap;

[Serializable]
[NetSerializable]
public sealed class CivGlobalMapMarkerState
{
	public int Id { get; }

	public bool IsObjective { get; }

	public CivGlobalMapMarkerType Type { get; }

	public MapId MapId { get; }

	public Vector2 Position { get; }

	public int TeamId { get; }

	public int SquadId { get; }

	public bool PlacedBySquadLeader { get; }

	public string PlacedByName { get; }

	public CivGlobalMapMarkerState(int id, bool isObjective, CivGlobalMapMarkerType type, MapId mapId, Vector2 position, int teamId, int squadId, bool placedBySquadLeader, string placedByName)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Id = id;
		IsObjective = isObjective;
		Type = type;
		MapId = mapId;
		Position = position;
		TeamId = teamId;
		SquadId = squadId;
		PlacedBySquadLeader = placedBySquadLeader;
		PlacedByName = placedByName;
	}
}
