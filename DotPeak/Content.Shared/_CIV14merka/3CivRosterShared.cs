// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.CivRosterTeamEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._CIV14merka;

[NetSerializable]
[Serializable]
public sealed class CivRosterTeamEntry
{
  public int TeamId { get; }

  public string SideId { get; }

  public string TeamName { get; }

  public string TeamRoleName { get; }

  public int PlayerCount { get; }

  public bool IsSelected { get; }

  public bool CanSelect { get; }

  public string? SelectUnavailableReason { get; }

  public bool CanCreateSquad { get; }

  public string? CreateSquadUnavailableReason { get; }

  public List<CivRosterSquadEntry> Squads { get; }

  public string? CommanderName { get; }

  public NetUserId? CommanderUserId { get; }

  public List<CivCommanderCandidateEntry> CommanderCandidates { get; }

  public bool CanNominate { get; }

  public string? NominateUnavailableReason { get; }

  public CivRosterTeamEntry(
    int teamId,
    string sideId,
    string teamName,
    string teamRoleName,
    int playerCount,
    bool isSelected,
    bool canSelect,
    string? selectUnavailableReason,
    bool canCreateSquad,
    string? createSquadUnavailableReason,
    List<CivRosterSquadEntry> squads,
    string? commanderName = null,
    NetUserId? commanderUserId = null,
    List<CivCommanderCandidateEntry>? commanderCandidates = null,
    bool canNominate = false,
    string? nominateUnavailableReason = null)
  {
    this.TeamId = teamId;
    this.SideId = sideId ?? string.Empty;
    this.TeamName = teamName;
    this.TeamRoleName = teamRoleName;
    this.PlayerCount = playerCount;
    this.IsSelected = isSelected;
    this.CanSelect = canSelect;
    this.SelectUnavailableReason = selectUnavailableReason;
    this.CanCreateSquad = canCreateSquad;
    this.CreateSquadUnavailableReason = createSquadUnavailableReason;
    this.Squads = squads;
    this.CommanderName = commanderName;
    this.CommanderUserId = commanderUserId;
    this.CommanderCandidates = commanderCandidates ?? new List<CivCommanderCandidateEntry>();
    this.CanNominate = canNominate;
    this.NominateUnavailableReason = nominateUnavailableReason;
  }
}
