// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.Logs.AdminLogsEuiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Eui;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Administration.Logs;

[NetSerializable]
[Serializable]
public sealed class AdminLogsEuiState : EuiStateBase
{
  public AdminLogsEuiState(int roundId, Dictionary<Guid, string> players, int roundLogs)
  {
    this.RoundId = roundId;
    this.Players = players;
    this.RoundLogs = roundLogs;
  }

  public bool IsLoading { get; set; }

  public int RoundId { get; }

  public Dictionary<Guid, string> Players { get; }

  public int RoundLogs { get; }
}
