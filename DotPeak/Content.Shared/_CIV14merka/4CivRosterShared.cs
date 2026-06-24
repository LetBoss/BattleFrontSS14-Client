// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.CivRosterStateEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._CIV14merka;

[NetSerializable]
[Serializable]
public sealed class CivRosterStateEvent : EntityEventArgs
{
  public bool Enabled { get; }

  public bool RoundInProgress { get; }

  public bool LateJoinActive { get; }

  public bool CanEnterRound { get; }

  public string? EnterRoundUnavailableReason { get; }

  public bool IsJoinedRound { get; }

  public bool HasParticipatedInCurrentRound { get; }

  public bool RejoinBlockedForCurrentRound { get; }

  public int? SelectedTeamId { get; }

  public int? SelectedSquadId { get; }

  public List<CivRosterTeamEntry> Teams { get; }

  public List<CivRosterPlayerEntry> Players { get; }

  public Civ14RoundMode RoundMode { get; }

  public bool AllowAutoLeader { get; }

  public int SelfPlaytimeMinutes { get; }

  public CivRosterStateEvent(
    bool enabled,
    bool roundInProgress,
    bool lateJoinActive,
    bool canEnterRound,
    string? enterRoundUnavailableReason,
    bool isJoinedRound,
    bool hasParticipatedInCurrentRound,
    bool rejoinBlockedForCurrentRound,
    int? selectedTeamId,
    int? selectedSquadId,
    List<CivRosterTeamEntry> teams,
    List<CivRosterPlayerEntry> players,
    Civ14RoundMode roundMode = Civ14RoundMode.BaseCapture,
    bool allowAutoLeader = false,
    int selfPlaytimeMinutes = 0)
  {
    this.Enabled = enabled;
    this.RoundInProgress = roundInProgress;
    this.LateJoinActive = lateJoinActive;
    this.CanEnterRound = canEnterRound;
    this.EnterRoundUnavailableReason = enterRoundUnavailableReason;
    this.IsJoinedRound = isJoinedRound;
    this.HasParticipatedInCurrentRound = hasParticipatedInCurrentRound;
    this.RejoinBlockedForCurrentRound = rejoinBlockedForCurrentRound;
    this.SelectedTeamId = selectedTeamId;
    this.SelectedSquadId = selectedSquadId;
    this.Teams = teams;
    this.Players = players;
    this.RoundMode = roundMode;
    this.AllowAutoLeader = allowAutoLeader;
    this.SelfPlaytimeMinutes = selfPlaytimeMinutes;
  }
}
