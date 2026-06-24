using System;
using System.Numerics;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.GlobalMap;

[Serializable]
[NetSerializable]
public sealed class CivGlobalMapPlayerState
{
	public string Name { get; }

	public MapId MapId { get; }

	public Vector2 Position { get; }

	public int TeamId { get; }

	public int SquadId { get; }

	public bool IsSquadLeader { get; }

	public bool IsSelf { get; }

	public CivGlobalMapPlayerState(string name, MapId mapId, Vector2 position, int teamId, int squadId, bool isSquadLeader, bool isSelf)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Name = name;
		MapId = mapId;
		Position = position;
		TeamId = teamId;
		SquadId = squadId;
		IsSquadLeader = isSquadLeader;
		IsSelf = isSelf;
	}
}
