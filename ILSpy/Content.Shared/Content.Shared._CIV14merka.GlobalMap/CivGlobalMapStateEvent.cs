using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared._CIV14merka.Capture;
using Content.Shared._CIV14merka.Commander;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.GlobalMap;

[Serializable]
[NetSerializable]
public sealed class CivGlobalMapStateEvent : EntityEventArgs
{
	public MapId MapId { get; }

	public bool HasBounds { get; }

	public Vector2 BoundsMin { get; }

	public Vector2 BoundsMax { get; }

	public int TeamId { get; }

	public int SquadId { get; }

	public bool IsSquadLeader { get; }

	public bool IsCommander { get; }

	public bool ShowAllTeamMatesOnMaps { get; }

	public string StatusLabel { get; }

	public float RoundTimeLeftSeconds { get; }

	public int Team1AliveCount { get; }

	public int Team2AliveCount { get; }

	public int Team1Score { get; }

	public int Team2Score { get; }

	public float AirstrikeCooldown { get; }

	public float ArtilleryCooldown { get; }

	public float SmokeSupportCooldown { get; }

	public List<CivGlobalMapMarkerState> Markers { get; }

	public List<CivGlobalMapPlayerState> Players { get; }

	public List<CivPointCapturePointState> Points { get; }

	public List<CivCommanderOrderState> Orders { get; }

	public List<CivGlobalMapDeathState> Deaths { get; }

	public List<CivFobMarkerState> Fobs { get; }

	public CivCommanderState? CommanderState { get; }

	public CivGlobalMapStateEvent(MapId mapId, bool hasBounds, Vector2 boundsMin, Vector2 boundsMax, int teamId, int squadId, bool isSquadLeader, bool isCommander, bool showAllTeamMatesOnMaps, string statusLabel, float roundTimeLeftSeconds, int team1AliveCount, int team2AliveCount, int team1Score, int team2Score, float airstrikeCooldown, float artilleryCooldown, float smokeSupportCooldown, IEnumerable<CivGlobalMapMarkerState> markers, IEnumerable<CivGlobalMapPlayerState> players, IEnumerable<CivPointCapturePointState> points, IEnumerable<CivCommanderOrderState> orders, CivCommanderState? commanderState, IEnumerable<CivGlobalMapDeathState> deaths, IEnumerable<CivFobMarkerState> fobs)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		MapId = mapId;
		HasBounds = hasBounds;
		BoundsMin = boundsMin;
		BoundsMax = boundsMax;
		TeamId = teamId;
		SquadId = squadId;
		IsSquadLeader = isSquadLeader;
		IsCommander = isCommander;
		ShowAllTeamMatesOnMaps = showAllTeamMatesOnMaps;
		StatusLabel = statusLabel;
		RoundTimeLeftSeconds = roundTimeLeftSeconds;
		Team1AliveCount = team1AliveCount;
		Team2AliveCount = team2AliveCount;
		Team1Score = team1Score;
		Team2Score = team2Score;
		AirstrikeCooldown = airstrikeCooldown;
		ArtilleryCooldown = artilleryCooldown;
		SmokeSupportCooldown = smokeSupportCooldown;
		Markers = markers.ToList();
		Players = players.ToList();
		Points = points.ToList();
		Orders = orders.ToList();
		Deaths = deaths.ToList();
		Fobs = fobs.ToList();
		CommanderState = commanderState;
	}
}
