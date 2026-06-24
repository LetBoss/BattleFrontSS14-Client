// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.GlobalMap.CivGlobalMapStateEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._CIV14merka.Capture;
using Content.Shared._CIV14merka.Commander;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared._CIV14merka.GlobalMap;

[NetSerializable]
[Serializable]
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

  public CivGlobalMapStateEvent(
    MapId mapId,
    bool hasBounds,
    Vector2 boundsMin,
    Vector2 boundsMax,
    int teamId,
    int squadId,
    bool isSquadLeader,
    bool isCommander,
    bool showAllTeamMatesOnMaps,
    string statusLabel,
    float roundTimeLeftSeconds,
    int team1AliveCount,
    int team2AliveCount,
    int team1Score,
    int team2Score,
    float airstrikeCooldown,
    float artilleryCooldown,
    float smokeSupportCooldown,
    IEnumerable<CivGlobalMapMarkerState> markers,
    IEnumerable<CivGlobalMapPlayerState> players,
    IEnumerable<CivPointCapturePointState> points,
    IEnumerable<CivCommanderOrderState> orders,
    CivCommanderState? commanderState,
    IEnumerable<CivGlobalMapDeathState> deaths,
    IEnumerable<CivFobMarkerState> fobs)
  {
    this.MapId = mapId;
    this.HasBounds = hasBounds;
    this.BoundsMin = boundsMin;
    this.BoundsMax = boundsMax;
    this.TeamId = teamId;
    this.SquadId = squadId;
    this.IsSquadLeader = isSquadLeader;
    this.IsCommander = isCommander;
    this.ShowAllTeamMatesOnMaps = showAllTeamMatesOnMaps;
    this.StatusLabel = statusLabel;
    this.RoundTimeLeftSeconds = roundTimeLeftSeconds;
    this.Team1AliveCount = team1AliveCount;
    this.Team2AliveCount = team2AliveCount;
    this.Team1Score = team1Score;
    this.Team2Score = team2Score;
    this.AirstrikeCooldown = airstrikeCooldown;
    this.ArtilleryCooldown = artilleryCooldown;
    this.SmokeSupportCooldown = smokeSupportCooldown;
    this.Markers = markers.ToList<CivGlobalMapMarkerState>();
    this.Players = players.ToList<CivGlobalMapPlayerState>();
    this.Points = points.ToList<CivPointCapturePointState>();
    this.Orders = orders.ToList<CivCommanderOrderState>();
    this.Deaths = deaths.ToList<CivGlobalMapDeathState>();
    this.Fobs = fobs.ToList<CivFobMarkerState>();
    this.CommanderState = commanderState;
  }
}
