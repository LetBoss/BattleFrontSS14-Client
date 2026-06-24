// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.CivHudStatusEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._CIV14merka.Capture;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._CIV14merka;

[NetSerializable]
[Serializable]
public sealed class CivHudStatusEvent : EntityEventArgs
{
  public bool Enabled { get; }

  public int ViewerTeamId { get; }

  public CivHudPhase Phase { get; }

  public bool ShowBriefingPanel { get; }

  public float PhaseTimeLeftSeconds { get; }

  public int Team1AliveCount { get; }

  public int Team2AliveCount { get; }

  public int Team1Score { get; }

  public int Team2Score { get; }

  public string ModeName { get; }

  public string ObjectiveText { get; }

  public string GuidanceText { get; }

  public bool IsSquadLeader { get; }

  public List<CivPointCapturePointState> PointStates { get; }

  public float WaveRespawnSecondsLeft { get; }

  public bool WaveConfirmActive { get; }

  public CivHudStatusEvent(
    bool enabled,
    int viewerTeamId,
    CivHudPhase phase,
    bool showBriefingPanel,
    float phaseTimeLeftSeconds,
    int team1AliveCount,
    int team2AliveCount,
    int team1Score,
    int team2Score,
    string modeName,
    string objectiveText,
    string guidanceText,
    bool isSquadLeader,
    IEnumerable<CivPointCapturePointState> pointStates,
    float waveRespawnSecondsLeft,
    bool waveConfirmActive)
  {
    this.Enabled = enabled;
    this.ViewerTeamId = viewerTeamId;
    this.Phase = phase;
    this.ShowBriefingPanel = showBriefingPanel;
    this.PhaseTimeLeftSeconds = phaseTimeLeftSeconds;
    this.Team1AliveCount = team1AliveCount;
    this.Team2AliveCount = team2AliveCount;
    this.Team1Score = team1Score;
    this.Team2Score = team2Score;
    this.ModeName = modeName;
    this.ObjectiveText = objectiveText;
    this.GuidanceText = guidanceText;
    this.IsSquadLeader = isSquadLeader;
    this.PointStates = pointStates.ToList<CivPointCapturePointState>();
    this.WaveRespawnSecondsLeft = waveRespawnSecondsLeft;
    this.WaveConfirmActive = waveConfirmActive;
  }
}
